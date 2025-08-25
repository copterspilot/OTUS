using System.IdentityModel.Tokens.Jwt;
using Keycloak.Web;
using Keycloak.Web.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpContextAccessor()
                .AddTransient<AuthorizationHandler>();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
    })
    .AddHttpMessageHandler<AuthorizationHandler>();

var oidcScheme = OpenIdConnectDefaults.AuthenticationScheme;

// Добавляем службу аутентификации в DI-контейнер и указываем схему по умолчанию для входа.
// `oidcScheme` — это строка (например, "oidc"), определяющая, какая схема будет использоваться по умолчанию.
builder.Services.AddAuthentication(oidcScheme)
    // Добавляем поддержку OpenID Connect с Keycloak как провайдером.
    // Настраиваем схему с именем "keycloak", указываем realm "WeatherShop".
    // Используем ранее определённую OIDC-схему (например, "oidc").
    .AddKeycloakOpenIdConnect("keycloak", realm: "WeatherShop", oidcScheme, options =>
    {
        // Указываем Client ID — идентификатор клиентского приложения в Keycloak.
        // Должен соответствовать клиенту, зарегистрированному в realm "WeatherShop".
        options.ClientId = "WeatherWeb";

        // Указываем тип потока аутентификации: Authorization Code Flow.
        // Это наиболее безопасный поток для веб-приложений (не используется в SPA).
        options.ResponseType = OpenIdConnectResponseType.Code;

        // Добавляем кастомную область (scope) "weather:all" к запросу токена.
        // Это позволяет запрашивать доступ к ресурсам, защищённым этим scope (например, API погоды).
        options.Scope.Add("weather:all");

        // Отключаем требование HTTPS для метаданных (например, .well-known/openid-configuration).
        // ⚠️ Только для разработки! В продакшене всегда должно быть true.
        options.RequireHttpsMetadata = false;

        // Указываем, какой claim в JWT будет использоваться как "имя пользователя" в ASP.NET Core.
        // По умолчанию ASP.NET использует "name", но в JWT это может быть "given_name", "preferred_username" и др.
        // Здесь явно указываем, что имя берётся из claim'а "name" (соответствует JwtRegisteredClaimNames.Name).
        options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;

        // Сохраняем полученные токены (ID token, access token) в AuthenticationProperties.
        // Это позволяет в дальнейшем использовать их для вызова API от имени пользователя.
        options.SaveTokens = true;

        // Указываем, какая схема аутентификации будет использоваться для локального входа после OIDC.
        // После успешного входа через Keycloak, пользователь будет аутентифицирован локально через куки.
        // Это стандартная практика: OIDC для входа, куки — для поддержания сессии.
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    // Добавляем схему аутентификации по куки.
    // Она используется для хранения сессии пользователя после входа через Keycloak.
    // Без этого ASP.NET Core не сможет поддерживать аутентифицированную сессию.
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

// Добавляет сервис, который "каскадно" (каскадом) передаёт состояние аутентификации
// из серверного контекста (например, HttpContext.User) в дерево компонентов Blazor.
//
// Это позволяет использовать:
//   - @context.User в <AuthorizeView>
//   - [CascadingParameter] private Task<AuthenticationState> authenticationState
//   - AuthorizeAttribute в компонентах
//
// Без этого — Blazor не сможет определить, кто пользователь, даже если он залогинен.
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();
app.MapLoginAndLogout();

await app.RunAsync().ConfigureAwait(false);
