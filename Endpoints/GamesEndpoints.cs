using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
	const string GetGameEndpointName = "GetGame";

	public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("games")
					.WithParameterValidation();

		// GET /games
		group.MapGet("/", async (GameStoreContext dbContext) =>
			await dbContext.Games
					.Include(game => game.Genre)
					.Select(game => game.ToGameSummaryDto())
					.AsNoTracking()
					.ToListAsync());

		// GET /games/1
		group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
		{
			Game? game = await dbContext.Games.FindAsync(id);

			return game is null ?
				Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
		})
		.WithName(GetGameEndpointName);

		// POST /games
		group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
			{
				try
				{
					Game game = newGame.ToEntity();
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
		group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
			{
				try
				{
					Game? existingGame = await dbContext.Games.FindAsync(id);

					if (existingGame is null)
					{
						return Results.NotFound("Game not found.");
					}

					dbContext.Entry(existingGame)
							.CurrentValues
							.SetValues(updatedGame.ToEntity(id));

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
		group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
			{
				try
				{
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
