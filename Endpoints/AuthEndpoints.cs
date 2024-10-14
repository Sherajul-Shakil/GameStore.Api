using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace GameStore.Api.Endpoints;

public static class AuthEndpoints
{
	public static RouteGroupBuilder MapAuthEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("auth");

		// Configuration for JWT
		var jwtSettings = app.Configuration.GetSection("JwtSettings");
		var secretKey = jwtSettings.GetValue<string>("SecretKey");
		var issuer = jwtSettings.GetValue<string>("Issuer");
		var audience = jwtSettings.GetValue<string>("Audience");

		if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
		{
			throw new Exception("JWT configuration is missing. Please configure the 'Jwt:Key', 'Jwt:Issuer', and 'Jwt:Audience' settings.");
		}

		// Register
		group.MapPost("register", async (RegisterDto request, GameStoreContext dbContext) =>
		{
			try
			{
				var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
				if (existingUser != null)
				{
					return Results.BadRequest("User with this phone number already exists.");
				}

				var user = request.ToEntity();
				user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
				dbContext.Users.Add(user);
				await dbContext.SaveChangesAsync();

				return Results.Ok("User registered successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error registering user: {ex.Message}");
				return Results.BadRequest("Error registering user.");
			}
		});

		// Login
		group.MapPost("login", async (LoginDto request, GameStoreContext dbContext) =>
		{
			try
			{
				var user = await dbContext.Users
					.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);

				if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
				{
					return Results.Unauthorized();
				}

				var token = GenerateJwtToken(user, secretKey, issuer, audience);
				return Results.Ok(new { token = token });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error logging in: {ex.Message}");
				return Results.BadRequest("Error logging in.");
			}
		});

		// Protected endpoint (requires authentication)
		group.MapGet("me", [Authorize] async (HttpContext httpContext, GameStoreContext dbContext) =>
			{
				try
				{
					var userIdString = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
					if (int.TryParse(userIdString, out var userId))
					{
						User? user = await dbContext.Users.FindAsync(userId);

						if (user == null)
						{
							return Results.NotFound();
						}

						var userDto = user.ToUserDto();
						return Results.Ok(userDto);
					}
					else
					{
						return Results.BadRequest("Invalid user ID in the token.");
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error getting user data: {ex.Message}");
					return Results.BadRequest("Error getting user data.");
				}
			});

		return group;
	}

	private static string GenerateJwtToken(User user, string secretKey, string issuer, string audience)
	{
		if (string.IsNullOrEmpty(secretKey))
		{
			throw new Exception("JWT secret key is missing. Please configure the 'Jwt:Key' setting in appsettings.json.");
		}

		var claims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.Name),
			new Claim(ClaimTypes.Email, user.Email ?? "")
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var tokenDescriptor = new JwtSecurityToken(
			issuer: issuer,
			audience: audience,
			claims: claims,
			expires: DateTime.Now.AddHours(6),
			signingCredentials: creds);

		return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
	}
}