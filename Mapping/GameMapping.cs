using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping;

public static class GameMapping
{
	public static Game ToEntity(this UpdateGameDto game, int id)
	{
		return new Game()
		{
			Id = id,
			Name = game.Name,
			GenreId = game.GenreId,
			Price = game.Price,
			ReleaseDate = game.ReleaseDate,
			CreatedBy = game.CreatedBy,
			UpdatedAt = game.UpdatedAt
		};
	}

	public static Game ToEntity(this CreateGameDto game)
	{
		return new Game()
		{
			Name = game.Name,
			GenreId = game.GenreId,
			Price = game.Price,
			ReleaseDate = game.ReleaseDate,
			CreatedBy = game.CreatedBy,
			CreatedAt = game.CreatedAt,
			UpdatedAt = game.UpdatedAt
		};
	}

	public static GameSummaryDto ToGameSummaryDto(this Game game)
	{
		return new(
			game.Id,
			game.Name,
			game.Genre!.Name,
			game.Price,
			game.ReleaseDate,
			game.CreatedBy,
			game.CreatedAt,
			game.UpdatedAt
		);
	}

	public static GameDetailsDto ToGameDetailsDto(this Game game)
	{
		return new(
			game.Id,
			game.Name,
			game.GenreId,
			game.Price,
			game.ReleaseDate,
			game.CreatedBy,
			game.CreatedAt,
			game.UpdatedAt
		);
	}


}
