namespace GameStore.Api.Entities
{
	public class Game
	{
		public int Id { get; set; }

		public required string Name { get; set; }

		public int GenreId { get; set; }

		public Genre? Genre { get; set; }

		public decimal Price { get; set; }

		public DateOnly ReleaseDate { get; set; }

		public int CreatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		public Game CopyWith(string? name = null, decimal? price = null, DateOnly? releaseDate = null, DateTime? updatedAt = null)
		{
			return new Game
			{
				Id = this.Id,
				Name = name ?? this.Name,
				GenreId = this.GenreId,
				Genre = this.Genre,
				Price = price ?? this.Price,
				ReleaseDate = releaseDate ?? this.ReleaseDate,
				CreatedBy = this.CreatedBy,
				CreatedAt = this.CreatedAt,
				UpdatedAt = updatedAt ?? this.UpdatedAt
			};
		}
	}
}