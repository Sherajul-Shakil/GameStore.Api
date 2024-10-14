using GameStore.Api.Data;
using GameStore.Api.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		// Get secret key from appsettings.json
		var secretKey = builder.Configuration["JwtSettings:SecretKey"];
		if (string.IsNullOrEmpty(secretKey))
		{
			throw new Exception("JWT secret key is missing. Please configure the 'Jwt:Key' setting in appsettings.json.");
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

app.MapGamesEndpoints();
app.MapGenresEndpoints();
app.MapAuthEndpoints();

app.MapGet("/", () => "Hello, World!");

app.UseAuthentication();
app.UseAuthorization();

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