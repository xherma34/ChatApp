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
	/// Returns a list of all users that are a part of chat with ID chatId
	/// </summary>
	/// <param name="chatId">Id of chat room</param>
	Task<IEnumerable<User>> GetAllUsersInChatAsync(int chatId);

	/// <summary>
	/// Returns a list of all chats that the user is a part of
	/// </summary>
	/// <param name="userId">Id of user</param>
	Task<IEnumerable<Chat>> GetAllChatsOfUserAsync(int userId);

}
