namespace GameStore.Api.Entities
{
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int Age { get; set; }
		public string Gender { get; set; } = string.Empty;
		public string PhoneNumber { get; set; } = string.Empty;
		public string? Email { get; set; }
		public string Password { get; set; } = string.Empty;


		// You may want to add other properties as needed for your application 
	}
}