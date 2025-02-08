using System;
using ChatAppBackend.DTOs;
using ChatAppBackend.Enums;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using ChatAppBackend.Services.Interfaces;

namespace ChatAppBackend.Services.Implementations;

public class UserChatService : BaseService, IUserChatService
{
	private readonly IUserRepository _userRepository;
	private readonly IChatRepository _chatRepository;
	private readonly IUserChatRepository _userChatRepository;

	public UserChatService(
		IHttpContextAccessor httpContextAccessor,
		IUserRepository userRepo,
		IChatRepository chatRepo,
		IUserChatRepository userChatRepo
		) : base(httpContextAccessor)
	{
		_userRepository = userRepo;
		_chatRepository = chatRepo;
		_userChatRepository = userChatRepo;

	}

	public async Task AddUserToChat(int userId, int chatId)
	{
		// TODO: Add functionality that users can be banned from channels -> here you check if they are banned
		// IsSameId
		if (!IsRequestorSameUser(userId))
			throw new UnauthorizedAccessException("Permission denied: unauthorized call of add user to chat");

		// User not null
		var user = await _userRepository.GetByIdAsync(userId);
		if (user == null)
			throw new UnauthorizedAccessException($"User with id {userId} doesn't exist");

		// Chat not null
		var chat = await _chatRepository.GetByIdAsync(chatId);
		if (chat == null)
			throw new UnauthorizedAccessException($"User with id {chatId} doesn't exist");

		// Create
		var userChat = new UserChat
		{
			UserId = userId,
			ChatId = chatId,
			UserStatus = UserStatus.Regular
		};

		await _userChatRepository.AddUserToChatAsync(userChat);
	}

	public async Task<IEnumerable<ChatDto>> GetAllChatsOfUser(int userId)
	{
		// IsAdmin || IsSameId
		if (!IsRequestorAdmin() && !IsRequestorSameUser(userId))
			throw new UnauthorizedAccessException("Permission denied: unauthorized call of get all user's chats");

		// User exists
		var exists = await _userRepository.UserExists(userId);
		if (!exists)
			throw new KeyNotFoundException($"User with id {userId} doesn't exist");

		// Get list of chats
		var chats = await _userChatRepository.GetAllChatsOfUserAsync(userId);
		// Check they are not null or empty
		if (chats == null || chats.Count() == 0)
			throw new ArgumentException($"User {userId} isn't part of any chats");

		// return list of dto's
		return chats.Select(c => new ChatDto
		{
			Id = c.Id,
			Name = c.Name
		});
	}

	public async Task<IEnumerable<UserDto>> GetAllUsersInChat(int chatId, int userId)
	{
		// Get any occurence of: UserChat (chatId, requestorID)
		var isInChat = _userChatRepository.IsUserInChat(userId, chatId);
		// authority: IsPartOfChat || IsAdmin
		if (!isInChat && IsRequestorAdmin())
			throw new UnauthorizedAccessException($"Permission denied: unauthorized call of get all users in chat");

		// Get all users chats
		var users = await _userChatRepository.GetAllUsersInChatAsync(chatId);
		// check not null
		if (users == null)
			throw new ArgumentException($"No records of users in chat ${chatId}");

		// return dto
		return users.Select(u => new UserDto
		{
			Id = u.Id,
			Nickname = u.Nickname,
			IsBanned = u.IsBanned
		});
	}

	public async Task<UserChatDto> GetById(int userId, int chatId)
	{
		var userChat = await _userChatRepository.GetByIdAsync(userId, chatId);

		if (userChat == null)
			throw new ArgumentException($"User {userId} is not in chat {chatId}");

		return new UserChatDto
		{
			UserId = userChat.UserId,
			ChatId = userChat.ChatId,
			UserStatus = userChat.UserStatus
		};
	}

	public async Task RemoveUserFromChat(int userId, int chatId)
	{
		// Record of UserChat exists
		var userChat = await _userChatRepository.GetByIdAsync(userId, chatId);
		if (userChat == null)
			throw new ArgumentException($"User {userId} is not in chat {chatId}: cannot remove");

		// IsModerator || IsSameUser
		if (!IsRequestorSameUser(userId) && userChat.UserStatus != UserStatus.Moderator)
			throw new UnauthorizedAccessException("Permission denied: unauthorized call of remove user from chat");

		// call repository
		await _userChatRepository.RemoveUserFromChatAsync(userId, chatId);
	}

	public async Task UpdateUserChatStatus(int requestorId, UserChatDto ucDto)
	{
		// IsModerator of chat -> THE REQUESTOR
		var requestorChat = await _userChatRepository.GetByIdAsync(requestorId, ucDto.ChatId);
		if (requestorChat == null)
			throw new ArgumentException($"User {requestorId} is not a part of chat {ucDto.ChatId}");

		// Is the moderator of chat || admin
		if (requestorChat.UserStatus != UserStatus.Moderator && IsRequestorAdmin())
			throw new UnauthorizedAccessException("Permission denied: unauthorized call of change user chat status");

		// Get the userChat of userId
		var userChat = await _userChatRepository.GetByIdAsync(ucDto.UserId, ucDto.ChatId);

		// Check its not null
		if (userChat == null)
			throw new ArgumentException($"User {requestorId} is not a part of chat {ucDto.ChatId}");

		// Change status
		await _userChatRepository.UpdateUserChatStatusAsync(
			new UserChat
			{
				UserId = ucDto.UserId,
				ChatId = ucDto.ChatId,
				UserStatus = ucDto.UserStatus
			});

	}
}
