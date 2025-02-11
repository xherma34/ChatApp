using System;
using System.Security.Claims;
using ChatAppBackend.DTOs;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using ChatAppBackend.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ChatAppBackend.Services.Implementations;

public class UserService : IUserService
{
	private readonly IUserRepository _userRepository;
	private readonly IPasswordHasher<User> _passwordHasher;

	public UserService(
		IUserRepository userRep,
		IPasswordHasher<User> pswdHasher
		)
	{
		_userRepository = userRep;
		_passwordHasher = pswdHasher;
	}


	public async Task AddAsync(UserDto userDto)
	{
		// Check all properties that needs to be filed:
		// Password present
		// Pressence of required fields: password, mail, nickname
		// if (string.IsNullOrEmpty(userDto.Password) ||
		// 	string.IsNullOrEmpty(userDto.MailAddress) ||
		// 	string.IsNullOrEmpty(userDto.Nickname))
		// 	throw new ArgumentException("Required information are missing for user creation");

		// // Mail present, not redundant
		// var mailUser = _userRepository.GetByMailAddressAsync(userDto.MailAddress);
		// if (mailUser != null)
		// 	throw new ArgumentException("Account with this mail address already exists");

		// // Check email valid format
		// if (!new EmailAddressAttribute().IsValid(userDto.MailAddress))
		// 	throw new ArgumentException("Passed email address is not in a correct format");

		// TODO: redundant call, find out how to not get the warning.. this is checked in the auth serv
		if (string.IsNullOrEmpty(userDto.MailAddress))
			throw new ArgumentException("");

		User user = new User
		{
			Nickname = userDto.Nickname,
			MailAddress = userDto.MailAddress,
			JoinDate = DateTime.Today,
			IsBanned = false,
			Role = userDto.Role
		};

		user.PasswordHash = _passwordHasher.HashPassword(user, userDto.Password);

		await _userRepository.AddAsync(user);

	}

	public async Task<IEnumerable<UserDto>> GetAllAsync(bool isAdmin)
	{

		if (!isAdmin)
			throw new UnauthorizedAccessException("Permission denied: Only admins can get all user objects");


		// Get the list
		var users = await _userRepository.GetAllAsync();

		if (users == null || users.Count() == 0) throw new KeyNotFoundException("No users found");

		// Return
		return users.Select(user => new UserDto
		{
			Id = user.Id,
			Nickname = user.Nickname,
			MailAddress = user.MailAddress,
			JoinDate = user.JoinDate,
			IsBanned = user.IsBanned
		});
	}

	public async Task<UserDto> GetByIdAsync(int userId, int requestorId, bool isAdmin)
	{
		// Check authority permission
		if (!isAdmin && requestorId != userId)
			throw new UnauthorizedAccessException("Permission denied: user cannot get other user objects");

		var user = await _userRepository.GetByIdAsync(userId);

		// Check existence of user
		if (user == null)
			throw new KeyNotFoundException($"No user with id {userId} found");

		// Return new object
		return new UserDto
		{
			Id = user.Id,
			Nickname = user.Nickname,
			MailAddress = user.MailAddress,
			JoinDate = user.JoinDate,
			IsBanned = user.IsBanned
		};
	}

	public async Task RemoveByIdAsync(int userId, int requestorId)
	{
		if (userId != requestorId)
			throw new UnauthorizedAccessException("Permission denied: users cannot delete other users");

		// Check that the user with this id exists, if not throw exception

		await _userRepository.RemoveAsync(userId);
	}

	public async Task UpdateAsync(UserDto userDto, int userId, int requestorId, bool isAdmin)
	{
		// Authorization check
		if (!isAdmin && userId != requestorId)
			throw new UnauthorizedAccessException("Permission denied, cannot change information of other users");

		// Fetch user
		var user = await _userRepository.GetByIdAsync(userId);
		if (user == null)
			throw new KeyNotFoundException($"User with id {userId} not found");

		// Change status of isbanned if different
		user.IsBanned = userDto.IsBanned != user.IsBanned ? !user.IsBanned : user.IsBanned;

		// Change nickname if passed
		user.Nickname = string.IsNullOrEmpty(userDto.Nickname) ? user.Nickname : userDto.Nickname;

		await _userRepository.UpdateAsync(user);
	}

	// TODO: Rewrite, dont put the password logic here, use authentication service and verifyPassword
	public async Task UpdateMailAddressAsync(int userId, string mail, string password, bool isAdmin, int requestorId)
	{
		// Authorization check
		if (!isAdmin && userId != requestorId)
			throw new UnauthorizedAccessException("Permission denied: unauthorized change operation");

		// fetch user
		var user = await _userRepository.GetByIdAsync(userId);
		if (user == null)
			throw new KeyNotFoundException($"User with id {userId} not found");

		// Check that user entered the right password
		// User passed correct password
		if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Success)
			throw new UnauthorizedAccessException("Permission denied: wrong password");

		// Validate email format
		if (string.IsNullOrWhiteSpace(mail) || !new EmailAddressAttribute().IsValid(mail))
			throw new ArgumentException("Invalid mail format");

		// Ensure uniquenes of email
		var mailUser = await _userRepository.GetByMailAddressAsync(mail);
		if (mailUser != null)
			throw new ArgumentException($"Account with this email address: {mail} already exists");

		// Update
		user.MailAddress = mail;
		await _userRepository.UpdateAsync(user);
	}

	// TODO: Rewrite, dont put the password logic here, use authentication service and verifyPassword
	public async Task UpdatePasswordAsync(string password, string oldPassword, int userId, bool isAdmin, int requestorId)
	{
		// authorization check
		if (!isAdmin && requestorId != userId)
			throw new UnauthorizedAccessException("Permission denied: unauthorized change operation");

		// fetch user
		var user = await _userRepository.GetByIdAsync(userId);
		if (user == null)
			throw new KeyNotFoundException($"User with id {userId} not found");

		// User passed correct password
		if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword) != PasswordVerificationResult.Success)
			throw new UnauthorizedAccessException("Permission denied: wrong password");

		// Check hash of password != oldPassword
		if (password == oldPassword)
			throw new ArgumentException("New password cannot be same as the old password");

		// update
		user.PasswordHash = _passwordHasher.HashPassword(user, password);
		await _userRepository.UpdateAsync(user);
	}

	// TODO: Implement
	public Task UpdateRefreshTokenAsync(string token)
	{
		throw new NotImplementedException();
	}
}
