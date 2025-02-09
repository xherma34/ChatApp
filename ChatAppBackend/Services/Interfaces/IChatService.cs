using System;
using ChatAppBackend.DTOs;
using ChatAppBackend.Models;

namespace ChatAppBackend.Services.Interfaces;

public interface IChatService
{
	/// <summary>
	/// Tries to return chat with ID chatId
	/// </summary>
	Task<ChatDto> GetByIdAsync(int requestorId, int chatId);

	/// <summary>
	/// Tries to return a list of all existing chats
	/// </summary>
	Task<IEnumerable<ChatDto>> GetAllAsync(int requestorId, bool isAdmin);

	/// <summary>
	/// Adds a new chat 
	/// </summary>
	Task AddAsync(int requestorId, ChatDto chatDto);

	/// <summary>
	/// Updates an existing chat
	/// </summary>
	Task UpdateNameAsync(int requestorId, ChatDto chat, bool isAdmin);

	/// <summary>
	/// Removes a chat with ID chatId
	/// </summary>
	Task RemoveAsync(int requestorId, int chatId, bool isAdmin);
}
