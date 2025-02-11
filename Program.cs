using GameStore.Api.Data;
using GameStore.Api.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "GameStore API",
		Version = "v1",
		Description = "A REST API for managing a game store",
	});

	// Configure JWT Authentication in Swagger
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Enter 'Bearer {your_token_here}'"
	});

	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		var secretKey = builder.Configuration["JwtSettings:SecretKey"];
		if (string.IsNullOrEmpty(secretKey))
		{
			throw new Exception("JWT secret key is missing. Please configure the 'JwtSettings:SecretKey' setting in appsettings.json.");
		}

		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
			ValidateIssuer = false,
			ValidateAudience = false
		};
	});

builder.Services.AddAuthorization();

var app = builder.Build();

// Enable Swagger UI
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "GameStore API v1");
		options.RoutePrefix = string.Empty;
		// options.RoutePrefix = "swagger";

	});
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGamesEndpoints();
app.MapGenresEndpoints();
app.MapAuthEndpoints();
app.MapExternalApiEndpoints();
// app.MapGet("/", () => "Hello, World!");

await app.MigrateDbAsync();
app.Run();


// dotnet run
// dotnet watch run
// dotnet clean
// dotnet restore
// dotnet build
// dotnet ef migrations add InitialCreate --output-dir Data\Migrations
// dotnet ef database update
// git rm -r --cached obj
// http://localhost:5074/swagger/index.html