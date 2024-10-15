namespace GameStore.Api.Entities;

public class Genre
{
	public int Id { get; set; }

	public required string Name { get; set; }
	public int CreatedBy { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }

	public Genre CopyWith(string? name = null, DateTime? updatedAt = null)
	{
		return new Genre
		{
			Id = this.Id,
			Name = name ?? this.Name,
			CreatedBy = this.CreatedBy,
			CreatedAt = this.CreatedAt,
			UpdatedAt = updatedAt ?? this.UpdatedAt
		};
	}
}