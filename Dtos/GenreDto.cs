namespace GameStore.Api.Dtos;

public record class GenreDto(int Id, string Name, int CreatedBy, DateTime? CreatedAt, DateTime? UpdatedAt)
{

}
