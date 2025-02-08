using System;
using ChatAppBackend.DTOs;
using ChatAppBackend.Enums;
using ChatAppBackend.Models;

namespace ChatAppBackend.Services.Interfaces;

public interface IUserChatService
{
	/// <summary>
	/// Tries to get UserChat record via userId and chatId
	/// </summary>
	/// <returns>UserChatDto or null if not found</returns>
	Task<UserChatDto> GetById(int userId, int chatId);

	/// <summary>
	/// Tries to get list of all users within a chat
	/// </summary>
	/// <returns>List of users or null if chatId doesn't exist</returns>
	Task<IEnumerable<UserDto>> GetAllUsersInChat(int chatId, int userId);

	/// <summary>
	/// Returns
	/// </summary>
	/// <param name="userId"></param>
	/// <returns></returns>
	Task<IEnumerable<ChatDto>> GetAllChatsOfUser(int userId);

	/// <summary>
	/// Adds user to chat
	/// </summary>
	/// <param name="userId">Id of user toi be added</param>
	/// <param name="chatId">Id of chat that the user is being added to</param>
	Task AddUserToChat(int userId, int chatId);

	/// <summary>
	/// Removes record of UserChat with userId and chatId
	/// </summary>
	Task RemoveUserFromChat(int userId, int chatId);

	/// <summary>
	/// Updates user with userId status within a chat of chatId
	/// </summary>
	/// <param name="userStatus"></param>
	Task UpdateUserChatStatus(int rquestorId, UserChatDto ucDto);


}
