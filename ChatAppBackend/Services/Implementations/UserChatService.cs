using System;
using ChatAppBackend.Controllers.DTOs;
using ChatAppBackend.DTOs;
using ChatAppBackend.Enums;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using ChatAppBackend.Services.Interfaces;

namespace ChatAppBackend.Services.Implementations;

public class UserChatService : IUserChatService
{
	private readonly IUserRepository _userRepository;
	private readonly IChatRepository _chatRepository;
	private readonly IUserChatRepository _userChatRepository;

	public UserChatService(
		IUserRepository userRepo,
		IChatRepository chatRepo,
		IUserChatRepository userChatRepo
		)
	{
		_userRepository = userRepo;
		_chatRepository = chatRepo;
		_userChatRepository = userChatRepo;

	}

	public async Task AddUserToChatAsync(UserChatRequest ucReq)
	{
		// TODO: Add functionality that users can be banned from channels -> here you check if they are banned

		if (ucReq.UserId == null || ucReq.ChatId == null || ucReq.RequestorId == null)
			throw new ArgumentException($"Missing required data to fetch add a user");

		// IsSameId
		if (ucReq.RequestorId.Value != ucReq.UserId.Value)
			throw new UnauthorizedAccessException("Permission denied: unauthorized call of add user to chat");

		// User not null
		var user = await _userRepository.GetByIdAsync(ucReq.UserId.Value);
		if (user == null)
			throw new KeyNotFoundException($"User with id {ucReq.UserId.Value} doesn't exist");

		// Chat not null
		var chat = await _chatRepository.GetByIdAsync(ucReq.ChatId.Value);
		if (chat == null)
			throw new KeyNotFoundException($"Chat with id {ucReq.ChatId.Value} doesn't exist");

		// Create
		var userChat = new UserChat
		{
			UserId = ucReq.UserId.Value,
			ChatId = ucReq.ChatId.Value,
			UserRole = UserChatRole.Regular
		};

		await _userChatRepository.AddUserToChatAsync(userChat);
	}

	public async Task<IEnumerable<ChatDto>> GetAllChatsOfUserAsync(UserChatRequest ucReq)
	{
		if (ucReq.UserId == null || ucReq.IsAdmin == null || ucReq.RequestorId == null)
			throw new ArgumentException($"Missing required data to fetch chats of user");

		// IsAdmin || IsSameId
		if (!(bool)ucReq.IsAdmin && (int)ucReq.RequestorId != (int)ucReq.UserId)
			throw new UnauthorizedAccessException("Permission denied: unauthorized call of get all user's chats");

		// User exists
		var exists = await _userRepository.UserExists((int)ucReq.UserId);
		if (!exists)
			throw new KeyNotFoundException($"User with id {(int)ucReq.UserId} doesn't exist");

		// Get list of chats
		var chats = await _userChatRepository.GetAllChatsOfUserAsync((int)ucReq.UserId);
		// Check they are not null or empty
		if (chats == null || chats.Count() == 0)
			throw new ArgumentException($"User {(int)ucReq.UserId} isn't part of any chats");

		// return list of dto's
		return chats.Select(c => new ChatDto
		{
			Id = c.Id,
			Name = c.Name
		});
	}

	public async Task<IEnumerable<UserDto>> GetAllUsersInChatAsync(UserChatRequest data)
	{
		// Check data
		if (data.ChatId == null || data.RequestorId == null || data.IsAdmin == null)
			throw new ArgumentException($"Missing required data to fetch users of chat");

		// Get any occurence of: UserChat (chatId, requestorID)
		var isInChat = _userChatRepository.IsUserInChat(data.RequestorId.Value, data.ChatId.Value);
		// authority: IsPartOfChat || IsAdmin
		if (!isInChat && !(bool)data.IsAdmin)
			throw new UnauthorizedAccessException($"Permission denied: unauthorized call of get all users in chat");

		// Get all users chats
		var users = await _userChatRepository.GetAllUsersInChatAsync((int)data.ChatId);
		// check not null
		// if (users == null)
		// throw new ArgumentException($"No records of users in chat ${(int)data.ChatId}");
		if (users.Count() == 0)
			throw new ArgumentException($"No records of users in chat ${(int)data.ChatId}");

		// return dto
		return users.Select(u => new UserDto
		{
			Id = u.Id,
			Nickname = u.Nickname,
			IsBanned = u.IsBanned
		});
	}

	public async Task<UserChatDto> GetByIdAsync(int userId, int chatId)
	{
		var userChat = await _userChatRepository.GetByIdAsync(userId, chatId);

		if (userChat == null)
			throw new ArgumentException($"User {userId} is not in chat {chatId}");

		return new UserChatDto
		{
			UserId = userChat.UserId,
			ChatId = userChat.ChatId,
			UserRole = userChat.UserRole
		};
	}

	public async Task RemoveUserFromChatAsync(UserChatRequest ucReq)
	{
		if (ucReq.UserId == null || ucReq.ChatId == null || ucReq.RequestorId == null)
			throw new ArgumentException($"Missing required data to remove user from chat");

		// Record of UserChat exists
		var userChat = await _userChatRepository.GetByIdAsync((int)ucReq.UserId, (int)ucReq.ChatId);
		if (userChat == null)
			throw new KeyNotFoundException($"User {ucReq.UserId} is not in chat {ucReq.ChatId}: cannot remove");

		// IsModerator || IsSameUser
		if ((int)ucReq.RequestorId != (int)ucReq.UserId && userChat.UserRole != UserChatRole.Moderator)
			throw new UnauthorizedAccessException("Permission denied: unauthorized call of remove user from chat");

		// call repository
		await _userChatRepository.RemoveUserFromChatAsync((int)ucReq.UserId, (int)ucReq.ChatId);
	}

	public async Task UpdateUserChatStatus(UserChatRequest req, UserChatRole role)
	{

		if (req.ChatId == null || req.RequestorId == null || req.UserId == null || req.IsAdmin == null)
			throw new ArgumentException("Error: missing a required property to update user's status in chat");

		// IsModerator of chat -> THE REQUESTOR
		var requestorChat = await _userChatRepository.GetByIdAsync(req.RequestorId.Value, req.ChatId.Value);
		if (requestorChat == null)
			throw new KeyNotFoundException($"User {req.RequestorId} is not a part of chat {req.ChatId}");

		// Is the moderator of chat || admin
		if (requestorChat.UserRole != UserChatRole.Moderator && !req.IsAdmin.Value)
			throw new UnauthorizedAccessException("Permission denied: unauthorized call of change user chat status");

		// Get the userChat of userId
		var userChat = await _userChatRepository.GetByIdAsync(req.UserId.Value, req.ChatId.Value);

		// Check its not null
		if (userChat == null)
			throw new KeyNotFoundException($"User {req.UserId} is not a part of chat {req.ChatId}");

		// Change status
		await _userChatRepository.UpdateUserChatRoleAsync(
			new UserChat
			{
				UserId = req.UserId.Value,
				ChatId = req.ChatId.Value,
				UserRole = role
			});

	}
}
