using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos
{
	public class RegisterDto
	{
		[Required]
		public string Name { get; set; } = string.Empty;

		[Required]
		public int Age { get; set; }

		[Required]
		public string Gender { get; set; } = string.Empty;

		[Required]
		public string PhoneNumber { get; set; } = string.Empty;

		[Required]
		public string Password { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;

		// Optional: Add validation methods or attributes as needed
		// public bool IsValid() { ... } 
	}
}