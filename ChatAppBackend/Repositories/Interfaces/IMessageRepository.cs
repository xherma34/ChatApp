using System;
using ChatAppBackend.Models;

namespace ChatAppBackend.Repositories.Interfaces;

public interface IMessageRepository
{
	// ----------------------- GET METHODS -----------------------
	/// <summary>
	/// Returns all messages sent within one chat
	/// </summary>
	/// <param name="chatId">Id of the chat</param>
	Task<IEnumerable<Message>> GetAllByChatIdAsync(int chatId);

	/// <summary>
	/// Get singular message with its ID
	/// </summary>
	/// <param name="id">Message Id</param>
	Task GetByIdAsync(int id);

	/// <summary>
	/// Get all messages within a chat of chatId that were sent by user of userId
	/// </summary>
	/// <param name="chatId">Id of chat</param>
	/// <param name="userId">Id of sender (user)</param>
	Task<IEnumerable<Message>> GetAllByChatIdUserIdAsync(int chatId, int userId);

	// ----------------------- ADD METHODS -----------------------
	/// <summary>
	/// Add method
	/// </summary>
	/// <param name="message">New message</param>
	Task AddAsync(Message message);

	// ----------------------- UPDATE METHODS -----------------------
	/// <summary>
	/// Update method
	/// </summary>
	/// <param name="message">New message information</param>
	Task UpdateAsync(Message message);

	// ----------------------- REMOVE METHODS -----------------------
	/// <summary>
	/// Removes message of ID id
	/// </summary>
	/// <param name="id">message id</param>
	Task RemoveAsync(int id);

}
