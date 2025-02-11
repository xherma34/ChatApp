using System;
using ChatAppBackend.DTOs;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using ChatAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ChatAppBackend.Services.Implementations;

public class ChatService : IChatService
{

	private readonly IChatRepository _chatRepository;
	private readonly IUserChatRepository _userChatRepository;
	private readonly IUserRepository _userRepository;

	public ChatService(
		IChatRepository chatRepo,
		IUserChatRepository ucRepo,
		IUserRepository userRepo
		)
	{
		_chatRepository = chatRepo;
		_userChatRepository = ucRepo;
		_userRepository = userRepo;
	}

	public async Task AddAsync(int requestorId, ChatDto chatDto)
	{
		// requestorId exists
		var exists = await _userRepository.UserExists(requestorId);
		if (!exists)
			throw new UnauthorizedAccessException("Permission denied: only registered users can create chats");

		// Check required fields when adding
		if (string.IsNullOrWhiteSpace(chatDto.Name))
			throw new ArgumentException("Name is a required field when creating chat");

		// Create new chat
		var newChat = new Chat { Name = chatDto.Name };
		// Add him
		await _chatRepository.AddAsync(newChat);

		// Set the user-chat relationship -> creator of chat is also a moderator
		var userChat = new UserChat
		{
			UserId = requestorId,
			ChatId = newChat.Id,
			UserRole = Enums.UserChatRole.Moderator
		};

		await _userChatRepository.AddUserToChatAsync(userChat);
	}

	public async Task<IEnumerable<ChatDto>> GetAllAsync(int requestorId, bool isAdmin)
	{

		var exists = _userRepository.UserExists(requestorId);
		// All registered users can access chat rooms
		if (!isAdmin)
			throw new UnauthorizedAccessException("Permission denied: unauthorized access of Get all chats");

		var chats = await _chatRepository.GetAllAsync();
		if (chats == null)
			throw new KeyNotFoundException("There are no chats to return");


		return chats.Select(c => new ChatDto
		{
			Id = c.Id,
			Name = c.Name
		});

	}

	public async Task<ChatDto> GetByIdAsync(int requestorId, int chatId)
	{
		var exists = await _userRepository.UserExists(requestorId);
		if (!exists)
			throw new UnauthorizedAccessException("Permission denied: only registered users can view chat rooms");

		var chat = await _chatRepository.GetByIdAsync(chatId);

		return new ChatDto
		{
			Id = chat.Id,
			Name = chat.Name
		};
	}

	public async Task RemoveAsync(int requestorId, int chatId, bool isAdmin)
	{
		// Check if admin -yes-> remove
		if (isAdmin)
		{
			await _chatRepository.RemoveAsync(chatId);
			return;
		}

		var uc = await _userChatRepository.GetByIdAsync(requestorId, chatId);
		if (uc == null || uc.UserRole != Enums.UserChatRole.Moderator)
			throw new UnauthorizedAccessException("Permission denied: only moderators and admins can remove chat rooms");

		await _chatRepository.RemoveAsync(chatId);

	}

	public async Task UpdateNameAsync(int requestorId, ChatDto chatDto, bool isAdmin)
	{
		// Check name field
		if (string.IsNullOrWhiteSpace(chatDto.Name))
			throw new ArgumentException("Missing required field name for update chat name");

		// Check that ID exists
		if (chatDto.Id == null)
			throw new ArgumentException("Chat ID cannot be null for update method");

		// Check authority
		var uc = await _userChatRepository.GetByIdAsync(requestorId, (int)chatDto.Id);

		if (!isAdmin && uc.UserRole != Enums.UserChatRole.Moderator)
			throw new UnauthorizedAccessException("Permission denied: only moderators or admins can update chat info");

		await _chatRepository.UpdateAsync(new Chat { Name = chatDto.Name });
	}
}
