using System;
using ChatAppBackend.DTOs;

namespace ChatAppBackend.Services.Interfaces;

public interface ITokenService
{
	/// <summary>
	/// Method generates JWT token and returns it
	/// </summary>
	public string GenerateJwtToken(UserDto userDto);

	/// <summary>
	/// Method takes in a JWT token and refreshes it's ttl
	/// </summary>
	Task<string> GenerateRefreshJwtToken(UserDto userDto);
}
