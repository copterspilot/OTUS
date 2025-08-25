var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Добавляем службу аутентификации в DI-контейнер.
// Это основа для всех механизмов проверки подлинности (Authentication) в ASP.NET Core.
builder.Services.AddAuthentication()

    // Добавляем схему аутентификации на основе JWT Bearer токенов, получаемых от Keycloak.
    // "keycloak" — имя схемы (может быть любым, но должно совпадать с тем, что указано в Authorization).
    // realm: "WeatherShop" — указывает, из какого realm Keycloak будут приходить токены.
    .AddKeycloakJwtBearer("keycloak", realm: "WeatherShop", options =>
        {
            // Отключаем требование HTTPS для получения метаданных (например, JWKS).
            // ⚠️ Только для разработки! В продакшене должно быть true.
            // Позволяет работать с Keycloak по HTTP (например, локально).
            options.RequireHttpsMetadata = false;

            // Указываем, какую аудиторию (audience) ожидаем в JWT-токене.
            // Токен будет считаться валидным, только если в claim "aud" будет значение "weather.api".
            // Это обеспечивает, что токен предназначен именно для этого API.
            options.Audience = "weather.api";
        }); // Завершение настройки AddKeycloakJwtBearer

// Добавляем службу авторизации и включаем возможность настройки политик авторизации.
// Это позволяет использовать [Authorize] атрибуты и Policy-based авторизацию.
builder.Services.AddAuthorizationBuilder();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.RequireAuthorization();
//.WithName("GetWeatherForecast");

app.MapDefaultEndpoints();

await app.RunAsync().ConfigureAwait(false);

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
