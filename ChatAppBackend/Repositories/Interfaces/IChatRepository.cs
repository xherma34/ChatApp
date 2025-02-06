using System;
using ChatAppBackend.Models;

namespace ChatAppBackend.Repositories.Interfaces;

public interface IChatRepository
{

	// ----------------------- GET METHODS -----------------------
	/// <summary>
	/// Tries to get chat by passed ID
	/// </summary>
	/// <param name="id">chat's id</param>
	/// <returns>Chat info from DB</returns>
	/// <exception cref="KeyNotFoundException">Chat with passed ID isn't in the DB</exception>
	Task<Chat> GetChatByIdAsync(int id);

	/// <summary>
	/// Method gets all chats from DB
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<Chat>> GetAllAsync();


	// ----------------------- ADD METHODS -----------------------
	/// <summary>
	/// Add method
	/// </summary>
	/// <param name="chat">Chat info</param>
	Task AddAsync(Chat chat);

	// ----------------------- UPDATE METHODS -----------------------
	/// <summary>
	/// Update method
	/// </summary>
	/// <param name="chat">Chat to be updated</param>
	Task UpdateAsync(Chat chat);

	// ----------------------- REMOVE METHODS -----------------------
	/// <summary>
	/// Remove method
	/// </summary>
	/// <param name="id">ID of a chat to delete</param>
	Task RemoveAsync(int id);

}
