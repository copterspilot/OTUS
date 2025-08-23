// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using SixLabors.ImageSharp.Processing;

namespace CleanAspire.Application.Common.Interfaces;

public interface IUploadService
{
    Task<string> UploadAsync(UploadRequest request);
    Task RemoveAsync(string filename);
}
public class UploadRequest
{
    public UploadRequest(string fileName, UploadType uploadType, byte[] data, bool overwrite = false, string? folder = null, ResizeOptions? resizeOptions = null)
    {
        FileName = fileName;
        UploadType = uploadType;
        Data = data;
        Overwrite = overwrite;
        Folder = folder;
        ResizeOptions = resizeOptions;
    }
    public string FileName { get; set; }
    public string? Extension { get; set; }
    public UploadType UploadType { get; set; }
    public bool Overwrite { get; set; }
    public byte[] Data { get; set; }
    public string? Folder { get; set; }
    public ResizeOptions? ResizeOptions { get; set; }
}
public enum UploadType : byte
{
    [Description(@"Products")] Product,
    [Description(@"Images")] Images,
    [Description(@"ProfilePictures")] ProfilePicture,
    [Description(@"Documents")] Document
}
