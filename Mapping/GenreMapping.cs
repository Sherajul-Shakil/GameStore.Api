using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping;

public static class GenreMapping
{
	public static GenreDto ToDto(this Genre genre)
	{
		return new GenreDto(genre.Id, genre.Name, genre.CreatedBy, genre.CreatedAt, genre.UpdatedAt);
	}

	public static GenreDetailsDto ToGenreDetailsDto(this Genre genre)
	{
		return new(
			genre.Id,
			genre.Name,
			genre.CreatedBy,
			genre.CreatedAt,
			genre.UpdatedAt
		);
	}



	public static Genre ToEntity(this CreateGenreDto genre)
	{
		return new Genre()
		{
			Name = genre.Name,
			CreatedBy = genre.CreatedBy,
			CreatedAt = genre.CreatedAt,
			UpdatedAt = genre.UpdatedAt
		};
	}
	// public static Genre ToEntity(this UpdateGenreDto genre, int id)
	// {
	// 	return new Genre()
	// 	{
	// 		Id = id,
	// 		Name = genre.Name,
	// 		CreatedBy = genre.CreatedBy,
	// 		UpdatedAt = genre.UpdatedAt,
	// 	};
	// }
}
