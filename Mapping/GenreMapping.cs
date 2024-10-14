using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping;

public static class GenreMapping
{
	public static GenreDto ToDto(this Genre genre)
	{
		return new GenreDto(genre.Id, genre.Name);
	}

	public static GenreDetailsDto ToGenreDetailsDto(this Genre genre)
	{
		return new(
			genre.Id,
			genre.Name
		);
	}

	public static Genre ToEntity(this UpdateGenreDto game, int id)
	{
		return new Genre()
		{
			Id = id,
			Name = game.Name
		};
	}
}
