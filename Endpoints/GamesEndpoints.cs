using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
	const string GetGameEndpointName = "GetGame";

	public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("games")
					.WithParameterValidation()
					.RequireAuthorization();

		// GET /games
		group.MapGet("/", async (GameStoreContext dbContext, HttpContext httpContext) =>
		{
			try
			{
				var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
				if (userId == 0)
				{
					return Results.Unauthorized();
				}
				var games = await dbContext.Games
					.Include(game => game.Genre)
					.Where(g => g.CreatedBy == userId)
					.Select(game => game.ToGameSummaryDto())
					.AsNoTracking()
					.ToListAsync();

				return Results.Ok(games);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error getting games: {ex.Message}");
				return Results.BadRequest("Error getting games.");
			}
		});

		// GET /games/1
		group.MapGet("/{id}", async (int id, GameStoreContext dbContext, HttpContext httpContext) =>
		{
			try
			{
				var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
				if (userId == 0)
				{
					return Results.Unauthorized();
				}
				Game? game = await dbContext.Games
					.Include(game => game.Genre)
					.Where(g => g.CreatedBy == userId)
					.FirstOrDefaultAsync(g => g.Id == id);

				return game is null ?
					Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error getting game: {ex.Message}");
				return Results.BadRequest("Error getting game.");
			}
		})
		.WithName(GetGameEndpointName);

		// POST /games
		group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext, HttpContext httpContext) =>
		{
			try
			{
				var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
				if (userId == 0)
				{
					return Results.Unauthorized();
				}
				Game game = newGame.ToEntity();
				game.CreatedBy = userId;
				game.CreatedAt = DateTime.UtcNow;
				game.UpdatedAt = DateTime.UtcNow;

				dbContext.Games.Add(game);
				await dbContext.SaveChangesAsync();

				return Results.CreatedAtRoute(
					GetGameEndpointName,
					new { id = game.Id },
					game.ToGameDetailsDto());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error creating game: {ex.Message}");
				return Results.BadRequest("Error creating game.");
			}
		});

		// PUT /games
		group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext, HttpContext httpContext) =>
			{
				try
				{

					var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
					if (userId == 0)
					{
						return Results.Unauthorized();
					}
					Game? existingGame = await dbContext.Games.FindAsync(id);

					if (existingGame is null)
					{
						return Results.NotFound("Game not found.");
					}

					dbContext.Entry(existingGame)
							.CurrentValues
							.SetValues(updatedGame.ToEntity(id));
					existingGame.CreatedBy = userId;
					existingGame.CreatedAt = existingGame.CreatedAt;
					existingGame.UpdatedAt = DateTime.UtcNow;

					await dbContext.SaveChangesAsync();
					existingGame = await dbContext.Games.FindAsync(id);

					return Results.Ok(existingGame);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error updating game: {ex.Message}");
					return Results.BadRequest("Error updating game.");
				}
			});

		// DELETE /games/1
		group.MapDelete("/{id}", async (int id, GameStoreContext dbContext, HttpContext httpContext) =>
			{
				try
				{

					var userId = int.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
					if (userId == 0)
					{
						return Results.Unauthorized();
					}

					var rowsAffected = await dbContext.Games
						.Where(game => game.Id == id)
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
					Console.WriteLine($"Error deleting game: {ex.Message}");
					return Results.BadRequest("Error deleting game.");
				}
			});

		return group;
	}
}
