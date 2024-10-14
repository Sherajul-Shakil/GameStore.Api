namespace GameStore.Api.Dtos;

public record class GameSummaryDto(
	int Id,
	string Name,
	string Genre,
	decimal Price,
	DateOnly ReleaseDate,
	int CreatedBy,
	DateTime CreatedAt,
	DateTime UdatedAt
	);
