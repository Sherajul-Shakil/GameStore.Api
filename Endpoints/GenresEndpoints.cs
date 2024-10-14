using System.Security.Claims;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenresEndpoints
{
	const string GetGenreEndpointName = "GetGenre";

	public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("genres")
					.WithParameterValidation()
					.RequireAuthorization();

		// GET /genres
		group.MapGet("/", async (GameStoreContext dbContext, HttpContext httpContext) =>
			{
				try
				{
					var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
					if (userId == 0)
					{
						return Results.Unauthorized();
					}

					var genres = await dbContext.Genres
						.AsNoTracking()
						.Where(g => g.CreatedBy == userId)
						.ToListAsync();

					return Results.Ok(genres);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error getting genres: {ex.Message}");
					return Results.BadRequest("Error getting genres.");
				}
			});

		// GET /genres/1
		group.MapGet("/{id}", async (int id, GameStoreContext dbContext, HttpContext httpContext) =>
		{
			var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
			if (userId == 0)
			{
				return Results.Unauthorized();
			}
			Genre? genre = await dbContext.Genres.FindAsync(id);

			return genre is null ?
				Results.NotFound() : Results.Ok(genre.ToGenreDetailsDto());
		})
		.WithName(GetGenreEndpointName);

		// POST /genres
		group.MapPost("/", async (CreateGenreDto newGenre, GameStoreContext dbContext, HttpContext httpContext) =>
		{
			try
			{
				// Get the user ID from the JWT token
				var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
				if (userId == 0)
				{
					return Results.Unauthorized();
				}

				Genre genre = newGenre.ToEntity();
				genre.CreatedBy = userId;
				genre.CreatedAt = DateTime.UtcNow;
				genre.UpdatedAt = DateTime.UtcNow;

				dbContext.Genres.Add(genre);
				await dbContext.SaveChangesAsync();

				return Results.CreatedAtRoute(
					GetGenreEndpointName,
					new { id = genre.Id },
					genre.ToGenreDetailsDto());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error creating genre: {ex.Message}");
				return Results.BadRequest("Error creating genre.");
			}
		});

		// PUT /genres/1
		group.MapPut("/{id}", async (int id, UpdateGenreDto updatedGenre, GameStoreContext dbContext, HttpContext httpContext) =>
			{
				try
				{
					Genre? existingGenre = await dbContext.Genres.FindAsync(id);

					if (existingGenre is null)
					{
						return Results.NotFound("Genre not found.");
					}

					var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
					if (userId == 0)
					{
						return Results.Unauthorized();
					}

					dbContext.Entry(existingGenre)
							.CurrentValues
							.SetValues(updatedGenre.ToEntity(id));

					existingGenre.CreatedBy = userId;
					existingGenre.CreatedAt = existingGenre.CreatedAt;
					existingGenre.UpdatedAt = DateTime.UtcNow;


					await dbContext.SaveChangesAsync();
					existingGenre = await dbContext.Genres.FindAsync(id);

					return Results.Ok(existingGenre);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error updating genre: {ex.Message}");
					return Results.BadRequest("Error updating genre.");
				}
			});

		// DELETE /genres/1
		group.MapDelete("/{id}", async (int id, GameStoreContext dbContext, HttpContext httpContext) =>
		{
			try
			{
				var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
				if (userId == 0)
				{
					return Results.Unauthorized();
				}
				var rowsAffected = await dbContext.Genres
					.Where(genre => genre.Id == id)
					.ExecuteDeleteAsync();

				if (rowsAffected > 0)
				{
					return Results.Ok(new { status = "success", message = "Genre deleted successfully" });
				}
				else
				{
					return Results.NotFound();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error deleting genre: {ex.Message}");
				return Results.BadRequest("Error deleting genre.");
			}
		});

		return group;
	}
}