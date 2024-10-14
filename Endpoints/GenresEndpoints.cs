// using GameStore.Api.Data;
// using GameStore.Api.Entities;
// using GameStore.Api.Mapping;
// using Microsoft.EntityFrameworkCore;

// namespace GameStore.Api.Endpoints;

// public static class GenresEndpoints
// {
//     public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
//     {
//         var group = app.MapGroup("genres");

//         group.MapGet("/", async (GameStoreContext dbContext) =>
//             await dbContext.Genres
//                         .Select(genre => genre.ToDto())
//                         .AsNoTracking()
//                         .ToListAsync());
//         return group;


//     }
// }


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
					.WithParameterValidation();

		// GET /genres
		group.MapGet("/", async (GameStoreContext dbContext) =>
			await dbContext.Genres
						.AsNoTracking()
						.ToListAsync());

		// GET /genres/1
		group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
		{
			Genre? genre = await dbContext.Genres.FindAsync(id);

			return genre is null ?
				Results.NotFound() : Results.Ok(genre.ToGenreDetailsDto());
		})
		.WithName(GetGenreEndpointName);

		// POST /genres
		group.MapPost("/", async (CreateGenreDto newGenre, GameStoreContext dbContext) =>
		{
			try
			{
				Genre genre = newGenre.ToEntity();
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
		group.MapPut("/{id}", async (int id, UpdateGenreDto updatedGenre, GameStoreContext dbContext) =>
		{
			try
			{
				Genre? existingGenre = await dbContext.Genres.FindAsync(id);

				if (existingGenre is null)
				{
					return Results.NotFound("Genre not found.");
				}

				dbContext.Entry(existingGenre)
						.CurrentValues
						.SetValues(updatedGenre.ToEntity(id));

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
		group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
		{
			try
			{
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