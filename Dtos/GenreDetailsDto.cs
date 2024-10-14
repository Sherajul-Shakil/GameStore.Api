namespace GameStore.Api.Dtos;

public record class GenreDetailsDto(
	int Id,
	string Name,
	int CreatedBy,
	DateTime? CreatedAt,
	DateTime? UpdatedAt
	);
