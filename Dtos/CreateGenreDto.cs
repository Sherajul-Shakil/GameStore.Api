using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class CreateGenreDto(
	[Required][StringLength(50)] string Name,
	[Required] int CreatedBy,
	DateTime CreatedAt,
	DateTime UpdatedAt

);