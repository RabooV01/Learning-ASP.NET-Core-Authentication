using System.Text;
using BearerAuthentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var JwtConfig = builder.Configuration.GetSection("JWT") // bring jwt option section as an object
    .Get<JWTConfig>()!; // bind it to class object

builder.Services.AddAuthentication() // add authentication to the builder
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt => { // add authentication option (JWT Bearer)
        opt.SaveToken = true;
        opt.TokenValidationParameters = new () { // setting up validation params
            ValidateIssuer = true, // Ensures that the issuer of the token matches the expected issuer
            ValidIssuer = JwtConfig.Issuer, 
            ValidateAudience = true,
            ValidAudience = JwtConfig.Audience,
            ValidateIssuerSigningKey = true, //  Validates that the signing key used to sign the token matches our signing key
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.SigningKey!)),
            ValidateLifetime = true
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
