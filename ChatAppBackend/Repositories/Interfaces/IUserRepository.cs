using System;
using ChatAppBackend.Models;

namespace ChatAppBackend.Repositories.Interfaces;

public interface IUserRepository
{


	// ----------------------- GET METHODS -----------------------
	/// <summary>
	/// Tries to get user by passed ID
	/// </summary>
	/// <param name="id">user's id</param>
	/// <returns>User info from DB</returns>
	/// <exception cref="KeyNotFoundException">User with passed ID isn't in the DB</exception>
	Task<User> GetByIdAsync(int id);

	/// <summary>
	/// Method gets all users from DB
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<User>> GetAllAsync();

	/// <summary>
	/// Returns all chats that user is a part of
	/// </summary>
	/// <param name="userId">ID of user</param>
	Task<IEnumerable<Chat>> GetChatsByUserIdAsync(int userId);

	// ----------------------- ADD METHODS -----------------------
	/// <summary>
	/// Add method
	/// </summary>
	/// <param name="user">User info</param>
	Task AddAsync(User user);

	/// <summary>
	/// Creates the record for UserChat joint table
	/// </summary>
	Task AddUserToChatAsync(UserChat userChat);

	// ----------------------- UPDATE METHODS -----------------------
	/// <summary>
	/// Update method
	/// </summary>
	/// <param name="user">User to be updated</param>
	Task UpdateAsync(User user);

	// ----------------------- REMOVE METHODS -----------------------
	/// <summary>
	/// Remove method
	/// </summary>
	/// <param name="id">ID of a user to delete</param>
	Task RemoveAsync(int id);

	/// <summary>
	/// Removes the record from UserChat joint table
	/// </summary>
	/// <param name="userId">User to be removed</param>
	/// <param name="chatId">Chat id</param>
	/// <returns></returns>
	Task RemoveUserFromChatAsync(int userId, int chatId);
}
