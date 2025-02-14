using System;
using ChatAppBackend.Controllers.DTOs;
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
	Task<UserChatDto> GetByIdAsync(int userId, int chatId);

	/// <summary>
	/// Tries to get list of all users within a chat
	/// </summary>
	/// <returns>List of users or null if chatId doesn't exist</returns>
	Task<IEnumerable<UserDto>> GetAllUsersInChat(UserChatRequest ucReq);

	/// <summary>
	/// Returns
	/// </summary>
	/// <param name="userId"></param>
	/// <returns></returns>
	Task<IEnumerable<ChatDto>> GetAllChatsOfUser(UserChatRequest ucReq);

	/// <summary>
	/// Adds user to chat
	/// </summary>
	/// <param name="userId">Id of user toi be added</param>
	/// <param name="chatId">Id of chat that the user is being added to</param>
	Task AddUserToChat(int requestorId, int userId, int chatId);

	/// <summary>
	/// Removes record of UserChat with userId and chatId
	/// </summary>
	Task RemoveUserFromChat(UserChatRequest ucReq);

	/// <summary>
	/// Updates user with userId status within a chat of chatId
	/// </summary>
	/// <param name="userStatus"></param>
	Task UpdateUserChatStatus(int requestorId, bool isAdmin, UserChatDto ucDto);


}
