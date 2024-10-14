// GameMapping.cs 
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping
{
	public static class AuthMapping
	{
		// ... (your existing mapping methods for games and genres) ...

		// New mapping method for RegisterDto to User
		public static User ToEntity(this RegisterDto registerDto)
		{
			return new User
			{
				Name = registerDto.Name,
				Age = registerDto.Age,
				Gender = registerDto.Gender,
				PhoneNumber = registerDto.PhoneNumber,
				Email = registerDto.Email,
				Password = registerDto.Password

			};
		}

		public static UserDto ToUserDto(this User user)
		{
			return new UserDto
			{
				Name = user.Name,
				Age = user.Age,
				Gender = user.Gender,
				PhoneNumber = user.PhoneNumber,
				Email = user.Email ?? string.Empty,
			};
		}
	}
}