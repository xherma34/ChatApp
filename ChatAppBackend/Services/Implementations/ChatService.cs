using System;
using ChatAppBackend.DTOs;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using ChatAppBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ChatAppBackend.Services.Implementations;

public class ChatService : BaseService, IChatService
{

	private readonly IChatRepository _chatRepository;
	private readonly IUserChatRepository _userChatRepository;
	private readonly IUserRepository _userRepository;

	public ChatService(
		IHttpContextAccessor httpContextAccessor,
		IChatRepository chatRepo,
		IUserChatRepository ucRepo,
		IUserRepository userRepo
		) : base(httpContextAccessor)
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

		// Add
		await _chatRepository.AddAsync(new Chat { Name = chatDto.Name });

	}

	public async Task<IEnumerable<ChatDto>> GetAllAsync(int requestorId)
	{

		var exists = _userRepository.UserExists(requestorId);
		// All registered users can access chat rooms
		if (!IsRequestorAdmin())
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

	public async Task RemoveAsync(int requestorId, int chatId)
	{
		// Check if admin -yes-> remove
		if (IsRequestorAdmin())
		{
			await _chatRepository.RemoveAsync(chatId);
			return;
		}

		var cu = await _userChatRepository.GetByIdAsync(requestorId, chatId);
		if (cu == null || cu.UserStatus != Enums.UserStatus.Moderator)
			throw new UnauthorizedAccessException("Permission denied: only moderators and admins can remove chat rooms");

		await _chatRepository.RemoveAsync(chatId);

	}

	public async Task UpdateNameAsync(int requestorId, ChatDto chatDto)
	{
		if (string.IsNullOrWhiteSpace(chatDto.Name))
			throw new ArgumentException("Missing required field name for update chat name");

		await _chatRepository.UpdateAsync(new Chat { Name = chatDto.Name });
	}
}
