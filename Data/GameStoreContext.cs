using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public class GameStoreContext(DbContextOptions<GameStoreContext> options)
	: DbContext(options)
{

	public DbSet<User> Users { get; set; }
	public DbSet<Game> Games => Set<Game>();
	public DbSet<Genre> Genres => Set<Genre>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// modelBuilder.Entity<Genre>().HasData(
		// 	new { Id = 1, Name = "Fighting" },
		// 	new { Id = 2, Name = "Roleplaying" },
		// 	new { Id = 3, Name = "Sports" },
		// 	new { Id = 4, Name = "Racing" },
		// 	new { Id = 5, Name = "Kids and Family" }
		// );

		// Define the relationship between Game and Genre
		modelBuilder.Entity<Game>()
			.HasOne(g => g.Genre)
			.WithMany()
			.HasForeignKey(g => g.GenreId);
	}
}