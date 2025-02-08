using System;
using ChatAppBackend.DTOs;

namespace ChatAppBackend.Services.Interfaces;

public interface IMessageService
{
	/// <summary>
	/// Returns a message of ID
	/// </summary>
	Task<MessageDto> GetById(int messageId, int userId); // admin/sender

	/// <summary>
	/// Returns all messages sent by user
	/// </summary>
	Task<IEnumerable<MessageDto>> GetAllUserMessages(int userId);

	/// <summary>
	/// Adds a new message
	/// </summary>
	/// <param name="messageDto">Message data</param>
	Task AddMessage(MessageDto messageDto); // IsSameUser && IsInChat

	/// <summary>
	/// Updates the message field content
	/// </summary>
	/// <param name="messageId">Id of message</param>
	/// <param name="newContent">Changed content of the message</param>
	Task UpdateMessage(int messageId, int userId, string newContent); // IsSameUser

	/// <summary>
	/// Removes message with id
	/// </summary>
	Task RemoveMessage(int messageId, int chatId, int userId);

	// Consider moving these to IChatService
	// IEnumerable<MessageDto> GetAllMessagesWithinChat(int chatId); // Participants/Admins
	// IEnumerable<MessageDto> GetAllUserMessagesWithinChat(int chatId, int userId); // Admin OR RequestingUser
}
