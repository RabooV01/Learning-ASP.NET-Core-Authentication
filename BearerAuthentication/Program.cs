using System.Text;
using BearerAuthentication;
using BearerAuthentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.IdentityModel.Tokens;

// considered as user repository, will be injected into JWTAuthService
List<User> users = [];

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var JwtConfig = builder.Configuration.GetSection("JWT") // bring jwt option section from appsettings.json as an object
    .Get<JWTOptions>()!; // bind it to class object (more specificity)

builder.Services.AddSingleton<JWTAuthService>();
builder.Services.AddSingleton(users); // represents user repository
builder.Services.AddSingleton(JwtConfig);

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
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1) // allowing only 1min difference
        };
    });

/*must add this line because minimal API does not assume any default authorization implementations*/
builder.Services.AddAuthorization(); 


 // instead of injecting IConfiguration and getting jwt section


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};



app.MapPost("/api/Register", ([FromBody] User user) => 
{  
    try
    {
        // consider that logic done on service model :)
        if(user.username.Length < 8 || user.password.Length < 8)
            throw new Exception("username and password must be longer than 8 characters");
        
        if(users.Any(exist => exist.username == user.username))
            throw new Exception("username is already exists");
        
        // valid username and password
        user.Id = Guid.NewGuid(); // assign the generated Id
        
        users.Add(user); // add to my database

        return Results.NoContent();
    }
    catch (Exception e)
    {
        return Results.BadRequest(e.Message);
    }
});

app.MapPost("/api/Auth", (JWTAuthService authSerivce, Login login) => 
{
    try
    {
        var AccessToken = authSerivce.AuthenticateUser(login);
        return Results.Ok(AccessToken);
    }
    catch (Exception e)
    {
        return Results.BadRequest(e.Message);
    }

});


app.MapGet("/weatherforecast", (HttpContext ctx) =>
{
    // able to get user info because of adding UseAuthentication middleware to the pipeline
    var user = ctx.User.Identity?.Name; 
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return Results.Ok(new { forecast, user});
})
.RequireAuthorization()
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
