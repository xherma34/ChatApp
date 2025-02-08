using System;
using System.Security.Claims;
using ChatAppBackend.DTOs;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using ChatAppBackend.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ChatAppBackend.Services.Implementations;

public class UserService : BaseService, IUserService
{
	private readonly IUserRepository _userRepository;
	private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

	public UserService(
		IUserRepository userRep,
		IHttpContextAccessor httpAcc) : base(httpAcc)
	{
		_userRepository = userRep;
	}


	public async Task AddAsync(UserDto userDto)
	{
		// Check all properties that needs to be filed:
		// Password present
		// Pressence of required fields: password, mail, nickname
		if (userDto.Password == string.Empty || userDto.MailAddress == null || userDto.Nickname == string.Empty)
			throw new ArgumentException("Required information are missing for user creation");

		// Mail present, not redundant
		var mailUser = _userRepository.GetByMailAddressAsync(userDto.MailAddress);
		if (mailUser != null)
			throw new ArgumentException("Account with this mail address already exists");

		// Nickname - CAN BE REDUNDANT with Id combination it makes a unique nickname

		if (!new EmailAddressAttribute().IsValid(userDto.MailAddress))
			throw new ArgumentException("Passed email address is not in a correct format");

		User user = new User
		{
			Nickname = userDto.Nickname,
			MailAddress = userDto.MailAddress,
			JoinDate = DateTime.Today,
			IsBanned = false
		};

		user.PasswordHash = _passwordHasher.HashPassword(user, userDto.Password);

		await _userRepository.AddAsync(user);

	}

	public async Task<IEnumerable<UserDto>> GetAllAsync()
	{

		if (!IsRequestorAdmin())
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

	public async Task<UserDto> GetByIdAsync(int id)
	{
		// Check authority permission
		if (!IsRequestorAdmin() && !IsRequestorSameUser(id))
			throw new UnauthorizedAccessException("Permission denied: user cannot get other user objects");

		var user = await _userRepository.GetByIdAsync(id);

		// Check existence of user
		if (user == null)
			throw new KeyNotFoundException($"No user with id {id} found");

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

	public async Task RemoveByIdAsync(int id)
	{
		if (!IsRequestorSameUser(id))
			throw new UnauthorizedAccessException("Permission denied: users cannot delete other users");

		// Check that the user with this id exists, if not throw exception

		await _userRepository.RemoveAsync(id);
	}

	public async Task UpdateAsync(UserDto userDto, int userId)
	{
		// Authorization check
		if (!IsRequestorAdmin() && !IsRequestorSameUser(userDto.Id))
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

	public async Task UpdateMailAddressAsync(int userId, string mail, string password)
	{
		// Authorization check
		if (!IsRequestorAdmin() && !IsRequestorSameUser(userId))
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

	public async Task UpdateNickNameAsync(int userId, string nickname)
	{
		// authorization check
		if (!IsRequestorAdmin() && !IsRequestorSameUser(userId))
			throw new UnauthorizedAccessException("Permission denied: unauthorized change operation");

		// fetch user
		var user = await _userRepository.GetByIdAsync(userId);
		if (user == null)
			throw new KeyNotFoundException($"User with id {userId} not found");

		// update
		user.Nickname = nickname;
		await _userRepository.UpdateAsync(user);
	}

	public async Task UpdatePasswordAsync(string password, string oldPassword, int userId)
	{
		// authorization check
		if (!IsRequestorAdmin() && !IsRequestorSameUser(userId))
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

	// public async Task<IEnumerable<UserDto>> GetAllByChatIdAsync(int chatId)
	// {
	// 	// Extract user from request
	// 	var userClaims = _httpContextAccessor.HttpContext?.User;

	// 	if (userClaims == null) throw new UnauthorizedAccessException("No user context available");

	// 	// Get the requester's ID safely
	// 	var requesterClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
	// 	if (string.IsNullOrEmpty(requesterClaim))
	// 		throw new UnauthorizedAccessException("User ID claim is missing.");

	// 	var requesterId = int.Parse(requesterClaim);


	// 	// Store requestors role == admin
	// 	var isAdmin = userClaims.IsInRole("Admin");

	// 	// Check if he is in the chat group with the chatId 

	// 	// Get all users within the chat
	// 	var chatUsers = await _chatRepository.GetUsersByChatId(chatId);

	// 	// Check if user has authority to get the users within the chat
	// 	if (!isAdmin || !chatUsers.Any(u => u.Id == requesterId))
	// 		throw new UnauthorizedAccessException("Only admins or participants of chats can see all users within a chat");

	// 	return chatUsers.Select(user => new UserDto
	// 	{
	// 		Nickname = user.Nickname,
	// 		JoinDate = user.JoinDate,
	// 		IsBanned = user.IsBanned
	// 	});
	// }

	// public Task<ChatDto> GetAllChatsByIdAsync(int id)
	// {
	// 	// Extract user from request
	// 	var userClaims = _httpContextAccessor.HttpContext?.User;

	// 	if (userClaims == null) throw new UnauthorizedAccessException("No user context available");

	// 	// Get the requester's ID safely
	// 	var requesterClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
	// 	if (string.IsNullOrEmpty(requesterClaim))
	// 		throw new UnauthorizedAccessException("User ID claim is missing.");

	// 	var requesterId = int.Parse(requesterClaim);
	// }
}
