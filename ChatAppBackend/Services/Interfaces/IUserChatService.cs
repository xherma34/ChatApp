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
	Task<IEnumerable<UserDto>> GetAllUsersInChatAsync(UserChatRequest ucReq);

	/// <summary>
	/// Returns all chats of passed ucReq.UserId
	/// </summary>
	/// <param name="ucReq">Data object with request parameters</param>
	Task<IEnumerable<ChatDto>> GetAllChatsOfUserAsync(UserChatRequest ucReq);

	/// <summary>
	/// Adds user to chat
	/// </summary>
	/// <param name="ucReq">Data object with request parameters</param>
	Task AddUserToChatAsync(UserChatRequest ucReq);

	/// <summary>
	/// Removes record of UserChat with userId and chatId
	/// </summary>
	/// <param name="ucReq">Data object with request parameters</param>
	Task RemoveUserFromChatAsync(UserChatRequest ucReq);

	/// <summary>
	/// Updates user with userId status within a chat of chatId
	/// </summary>
	/// <param name="requestorId"></param>
	/// <param name="isAdmin"></param>
	/// <param name="ucDto"></param>
	/// <returns></returns>
	Task UpdateUserChatStatus(UserChatRequest request, UserChatRole role);


}
