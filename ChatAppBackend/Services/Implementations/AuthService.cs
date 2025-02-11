using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ChatAppBackend.DTOs;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using ChatAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ChatAppBackend.Services.Implementations;

public class AuthService : IAuthService
{

	private readonly IUserRepository _userRepository;
	private readonly IUserService _userService;
	private readonly ITokenService _tokenService;
	private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

	public AuthService(IUserRepository uRepo,
		ITokenService tokenServ,
		IUserService userServ)
	{
		_userRepository = uRepo;
		_tokenService = tokenServ;
		_userService = userServ;
	}

	public async Task<string> AuthenticateUserAsync(UserDto userDto)
	{

		if (string.IsNullOrEmpty(userDto.Password) || string.IsNullOrEmpty(userDto.MailAddress))
			throw new ArgumentException("Missing required field password or mail address");

		// find user with the passed mail
		var user = await _userRepository.GetByMailAddressAsync(userDto.MailAddress);

		// Check if not null
		if (user == null)
			throw new KeyNotFoundException($"No user is registered with email {userDto.MailAddress}");


		if (!VerifyPassword(userDto.Password, user))
			throw new UnauthorizedAccessException("Acces denied: wrong password");

		// Add user's nickname so it can be stored inside of the token
		userDto.Nickname = user.Nickname;

		// Generate JWT token
		string token = _tokenService.GenerateJwtToken(userDto);

		// Geneerate Refresh Token
		string refreshToken = await _tokenService.GenerateRefreshJwtToken(userDto);

		// TODO: Handle refresh token -> call userUpdateRefreshToken()

		// Return JWT token
		return token;
	}

	public async Task RegisterUserAsync(UserDto userDto)
	{
		// Check password,mail,nickame is not null
		if (string.IsNullOrEmpty(userDto.Nickname) || string.IsNullOrEmpty(userDto.Password) || string.IsNullOrEmpty(userDto.MailAddress))
			throw new ArgumentException("Missing required field for registration");

		// Check mail address is a correct format
		if (!new EmailAddressAttribute().IsValid(userDto.MailAddress))
			throw new ArgumentException("Email addres is not in a correct format");

		// Check that user isn't already registered with this email address
		var user = await _userRepository.GetByMailAddressAsync(userDto.MailAddress);
		if (user != null)
			throw new ArgumentException($"Account with this email address {userDto.MailAddress} already exists");

		// Call Add user in user service
		await _userService.AddAsync(userDto);

		// 

		throw new NotImplementedException();
	}

	public bool VerifyPassword(string enteredPassword, User user)
	{
		return _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, enteredPassword) == PasswordVerificationResult.Success;

	}
}