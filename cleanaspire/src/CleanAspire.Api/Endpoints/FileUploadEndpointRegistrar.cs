// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CleanAspire.Application.Common.Interfaces;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace CleanAspire.Api.Endpoints;

public class FileUploadEndpointRegistrar : IEndpointRegistrar
{
    public void RegisterRoutes(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/fileManagement").WithTags("File Upload").RequireAuthorization();

        group.MapGet("/antiforgeryToken", (HttpContext context, [FromServices] IAntiforgery antiforgery) =>
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            return TypedResults.Ok(new AntiforgeryTokenResponse(tokens.CookieToken, tokens.RequestToken, tokens.HeaderName));
        }).Produces<AntiforgeryTokenResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithSummary("Get Antiforgery Token")
        .WithDescription("Retrieves a new antiforgery token to be used for validating subsequent requests.");

        group.MapPost("/upload", async ([FromForm] FileUploadRequest request, HttpContext context) =>
        {
            var response = new List<FileUploadResponse>();
            var uploadService = context.RequestServices.GetRequiredService<IUploadService>();
            // Construct the URL to access the file
            var requestScheme = context.Request.Scheme; // 'http' or 'https'
            var requestHost = context.Request.Host.Value; // 'host:port'
            foreach (var file in request.Files)
            {
                var filestream = file.OpenReadStream();
                var stream = new MemoryStream();
                await filestream.CopyToAsync(stream);
                stream.Position = 0;
                var size = stream.Length;
                var uploadRequest = new UploadRequest(
                    file.FileName,
                    UploadType.Document,
                    stream.ToArray(),
                    request.Overwrite,
                    request.Folder
                );
                var result = await uploadService.UploadAsync(uploadRequest);
                var fileUrl = result.StartsWith("https://") ? result : $"{requestScheme}://{requestHost}/{result.Replace("\\", "/")}";
                response.Add(new FileUploadResponse { Path = result, Url = fileUrl, Size = size });
            }
            return TypedResults.Ok(response);
        }).Accepts<FileUploadRequest>("multipart/form-data")
        .Produces<IEnumerable<FileUploadResponse>>()
        .WithMetadata(new ConsumesAttribute("multipart/form-data"))
        .WithSummary("Upload files to the server")
        .WithDescription("Allows uploading multiple files to a specific folder on the server.");

        group.MapPost("/image", async ([FromForm] ImageUploadRequest request, HttpContext context) =>
        {
            var response = new List<FileUploadResponse>();
            var uploadService = context.RequestServices.GetRequiredService<IUploadService>();
            // Construct the URL to access the file
            var requestScheme = context.Request.Scheme; // 'http' or 'https'
            var requestHost = context.Request.Host.Value; // 'host:port'
            foreach (var file in request.Files)
            {
                var filestream = file.OpenReadStream();
                var imgstream = new MemoryStream();
                await filestream.CopyToAsync(imgstream);
                imgstream.Position = 0;
                if (request.CropSize_Width != null && request.CropSize_Height != null)
                {
                    using (var outStream = new MemoryStream())
                    {
                        using (var image = SixLabors.ImageSharp.Image.Load(imgstream))
                        {
                            image.Mutate(i => i.Resize(new ResizeOptions { Mode = ResizeMode.Crop, Size = new Size((int)request.CropSize_Width, (int)request.CropSize_Height) }));
                            image.Save(outStream, PngFormat.Instance);
                            var result = await uploadService.UploadAsync(new UploadRequest(
                                file.FileName,
                                UploadType.Images,
                                outStream.ToArray(),
                                request.Overwrite,
                                request.Folder
                            ));
                            var fileUrl = result.StartsWith("https://") ? result : $"{requestScheme}://{requestHost}/{result.Replace("\\", "/")}";
                            response.Add(new FileUploadResponse { Url = fileUrl, Path = result, Size = imgstream.Length });
                        }
                    }
                }
                else
                {
                    var uploadRequest = new UploadRequest(
                        file.FileName,
                        UploadType.Images,
                        imgstream.ToArray(),
                        request.Overwrite,
                        request.Folder
                    );
                    var result = await uploadService.UploadAsync(uploadRequest);
                    string fileUrl = result.StartsWith("https://") ? result : $"{requestScheme}://{requestHost}/{result.Replace("\\", "/")}";
                    response.Add(new FileUploadResponse { Url = fileUrl, Path = result, Size = imgstream.Length });
                }
            }
            return TypedResults.Ok(response);
        }).Accepts<ImageUploadRequest>("multipart/form-data")
        .Produces<IEnumerable<FileUploadResponse>>()
        .WithMetadata(new ConsumesAttribute("multipart/form-data"))
        .WithSummary("Upload images to the server with cropping options")
        .WithDescription("Allows uploading multiple image files with optional cropping options to a specific folder on the server.");

        group.MapGet("/", async Task<Results<FileStreamHttpResult, ValidationProblem, NotFound<string>>> (
            [FromQuery] string path,
            HttpContext context) =>
        {
            if (path.Contains("..") || path.StartsWith("/") || path.StartsWith("\\"))
            {
                var validationProblem = TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "path", new[] { "Invalid file path." } }
                });
                return validationProblem;
            }

            var baseDirectory = Path.GetFullPath(Directory.GetCurrentDirectory());
            var filePath = Path.GetFullPath(Path.Combine(baseDirectory, path));
            if (!filePath.StartsWith(baseDirectory))
            {
                var validationProblem = TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "path", new[] { "Invalid file path." } }
                });
                return validationProblem;
            }
            if (!File.Exists(filePath))
            {
                return TypedResults.NotFound($"File '{filePath}' does not exist.");
            }
            FileStream fileStream;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            }
            catch (Exception ex)
            {
                var validationProblem = TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "file", new[] { "An error occurred while accessing the file.", ex.Message } }
                });
                return validationProblem;
            }
            var contentType = GetContentType(filePath);
            var fileName = Path.GetFileName(filePath);
            return TypedResults.File(fileStream, contentType, fileName);
        })
        .WithSummary("Download or preview a file from the server")
        .WithDescription("Allows clients to download or preview a file by specifying the folder and file name.");

        group.MapDelete("/", async Task<Results<NoContent, ValidationProblem, NotFound<string>>> (
            [FromQuery] string path,
            HttpContext context) =>
        {
            if (path.Contains("..") || path.StartsWith("/") || path.StartsWith("\\"))
            {
                var validationProblem = TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "path", new[] { "Invalid file path." } }
                });
                return validationProblem;
            }
            var baseDirectory = Path.GetFullPath(Directory.GetCurrentDirectory());
            var filePath = Path.GetFullPath(Path.Combine(baseDirectory, path));
            if (!filePath.StartsWith(baseDirectory))
            {
                var validationProblem = TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "path", new[] { "Invalid file path." } }
                });
                return validationProblem;
            }
            if (!File.Exists(filePath))
            {
                return TypedResults.NotFound($"File '{path}' does not exist.");
            }
            try
            {
                File.Delete(filePath);
                return TypedResults.NoContent();
            }
            catch (Exception ex)
            {
                var validationProblem = TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "file", new[] { "An error occurred while deleting the file.", ex.Message } }
                });
                return validationProblem;
            }
        })
        .WithSummary("Delete a file from the server")
        .WithDescription("Allows clients to delete a file by specifying the folder and file name.");
    }

    private static string GetContentType(string path)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(path, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }
    public class FileUploadRequest
    {
        /// <summary>
        /// Represents a request for uploading files to a specific folder.
        /// </summary>
        [Required]
        [StringLength(255, ErrorMessage = "Folder name cannot exceed 255 characters.")]
        [Description("The folder path where the files should be uploaded.")]
        public string Folder { get; set; } = string.Empty;

        [Description("Indicates whether to overwrite existing files.")]
        public bool Overwrite { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one file must be uploaded.")]
        [Description("The list of files to be uploaded.")]
        public IFormFileCollection Files { get; set; } = new FormFileCollection();
    }
    public class FileUploadResponse
    {
        /// <summary>
        /// The full URL to access the uploaded file.
        /// </summary>
        [Description("The full URL to access the uploaded file.")]
        public string Url { get; set; } = string.Empty;
        /// <summary>
        /// The path where the uploaded file is saved.
        /// </summary>  
        [Description("The path where the uploaded file is saved.")]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// The size of the uploaded file in bytes.
        /// </summary>
        [Description("The size of the uploaded file in bytes.")]
        public long Size { get; set; }
    }


    public class ImageUploadRequest
    {
        /// <summary>
        /// Represents a request for uploading image files to a specific folder, with an option to crop.
        /// </summary>
        [Required]
        [StringLength(255, ErrorMessage = "Folder name cannot exceed 255 characters.")]
        [Description("The folder path where the files should be uploaded.")]
        public string Folder { get; set; } = string.Empty;

        [Description("Indicates whether to overwrite existing files.")]
        public bool Overwrite { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one file must be uploaded.")]
        [Description("The list of files to be uploaded.")]
        public IFormFileCollection Files { get; set; } = new FormFileCollection();

        [Description("The desired crop size for width the uploaded image.")]
        public int? CropSize_Width { get; set; }
        [Description("The desired crop size for height the uploaded image.")]
        public int? CropSize_Height { get; set; }
    }
    public record AntiforgeryTokenResponse(string? CookieToken, string? RequestToken, string? HeaderName);
    public class CropSize
    {
        /// <summary>
        /// Represents the desired size for cropping an image.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Width must be greater than 0.")]
        [Description("The width of the cropped image.")]
        public int Width { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Height must be greater than 0.")]
        [Description("The height of the cropped image.")]
        public int Height { get; set; }
    }

}



