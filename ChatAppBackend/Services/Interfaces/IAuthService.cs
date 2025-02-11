using System;
using ChatAppBackend.DTOs;
using ChatAppBackend.Models;

namespace ChatAppBackend.Services.Interfaces;

public interface IAuthService
{
	// Register
	Task RegisterUserAsync(UserDto userDto);

	// Login
	Task<string> AuthenticateUserAsync(UserDto userDto);

	// Refresh token


	// Verify password
	public bool VerifyPassword(string enteredPassword, User user);

}
