using System;
using ChatAppBackend.Models;

namespace ChatAppBackend.Repositories.Interfaces;

public interface IUserChatRepository
{

	/// <summary>
	/// Removes the record from UserChat joint table
	/// </summary>
	/// <param name="userId">User to be removed</param>
	/// <param name="chatId">Chat id</param>
	/// <returns></returns>
	Task RemoveUserFromChatAsync(int userId, int chatId);


	/// <summary>
	/// Creates the record for UserChat joint table
	/// </summary>
	Task AddUserToChatAsync(UserChat userChat);

	/// <summary>
	/// Updates the user with id userChat.UserId status in the chat of userChat.ChatId
	/// </summary>
	Task UpdateUserChatRoleAsync(UserChat userChat);

	/// <summary>
	/// Returns a record of UserChat join table of passed ids
	/// </summary>
	Task<UserChat?> GetByIdAsync(int userId, int chatId);

	/// <summary>
	/// Returns a list of all users that are a part of chat with ID chatId
	/// </summary>
	/// <param name="chatId">Id of chat room</param>
	Task<IEnumerable<User>> GetAllUsersInChatAsync(int chatId);

	/// <summary>
	/// Returns a list of all chats that the user is a part of
	/// </summary>
	/// <param name="userId">Id of user</param>
	Task<IEnumerable<Chat>> GetAllChatsOfUserAsync(int userId);

	// OTHERS
	/// <summary>
	/// Returns true if user with userId is a part of the chat of chatId
	/// </summary>
	public bool IsUserInChat(int userId, int chatId);

	// /// <summary>
	// /// Returns true if user with id has a status of moderator in chat chatId
	// /// </summary>
	// /// <param name="userId"></param>
	// /// <param name="chatId"></param>
	// /// <returns></returns>
	// Task<bool> IsUserChatModerator(int userId, int chatId);
}
