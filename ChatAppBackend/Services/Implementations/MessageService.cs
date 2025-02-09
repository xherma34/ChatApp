using System;
using ChatAppBackend.DTOs;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using ChatAppBackend.Services.Interfaces;

namespace ChatAppBackend.Services.Implementations;

public class MessageService : IMessageService
{
	private readonly IUserRepository _userRepository;
	private readonly IChatRepository _chatRepository;
	private readonly IMessageRepository _msgRepository;
	private readonly IUserChatRepository _userChatRepository;

	public MessageService(
		IUserRepository userRep,
		IMessageRepository msgRep,
		IChatRepository chatRep,
		IUserChatRepository userChatRep
	)
	{
		_userRepository = userRep;
		_msgRepository = msgRep;
		_chatRepository = chatRep;
		_userChatRepository = userChatRep;
	}

	public async Task AddMessage(MessageDto msgDto)
	{
		if (msgDto.RequestorId == null)
			throw new ArgumentException("Missing required arguments (RequestorId) to add message");

		// Authority check -> IsSameUser
		if (msgDto.UserId != (int)msgDto.RequestorId)
			throw new UnauthorizedAccessException("Permission denied");

		// Authority check -> user is within the chat
		if (_userChatRepository.IsUserInChat(msgDto.UserId, msgDto.ChatId))
			throw new UnauthorizedAccessException("Permission denied: user must be a part of chat to send messages");

		// Get user - check null
		var user = await _userRepository.GetByIdAsync(msgDto.UserId);
		if (user == null)
			throw new ArgumentException($"User with id {msgDto.UserId} doesn't exist");

		// Get chat - check null
		var chat = await _chatRepository.GetByIdAsync(msgDto.ChatId);
		if (chat == null)
			throw new ArgumentException($"Chat with id {msgDto.ChatId} doesn't exist");

		Message msg = new Message
		{
			Content = msgDto.Content,
			TimeStamp = DateTime.Today,
			UserId = msgDto.UserId,
			ChatId = msgDto.ChatId,
			User = user,
			Chat = chat
		};

		await _msgRepository.AddAsync(msg);

	}

	public async Task<IEnumerable<MessageDto>> GetAllUserMessages(MessageDto msgDto)
	{
		// Check authority: SameId OR admin
		if (msgDto.IsAdmin == null || msgDto.RequestorId == null)
			throw new ArgumentException("Missing required argument (IsAdmin or RequestorId)");

		if (!(bool)msgDto.IsAdmin && (int)msgDto.RequestorId != msgDto.UserId)
			throw new UnauthorizedAccessException("Permission denied: unauthorized access");

		var exists = await _userRepository.UserExists(msgDto.UserId);
		if (!exists)
			throw new ArgumentException($"User with id {msgDto.UserId} doesn't exist");

		var messages = await _msgRepository.GetAllByUserIdAsync(msgDto.UserId);

		return messages.Select(m => new MessageDto
		{
			Id = m.Id,
			Content = m.Content,
			TimeStamp = m.TimeStamp,
			DeleteDate = m.DeleteDate,
			UserId = msgDto.UserId,
			ChatId = m.ChatId
		});
	}

	public async Task<MessageDto> GetById(MessageDto msgDto)
	{
		if (msgDto.IsAdmin == null || msgDto.RequestorId == null)
			throw new ArgumentException("Missing required argument (IsAdmin or RequestorId)");
		// IsSameUser || Admin
		if (!(bool)msgDto.IsAdmin && (int)msgDto.RequestorId != msgDto.UserId)
			throw new UnauthorizedAccessException("Permission denied: unauthorized access");

		// var message = await _msgRepository.GetByIdAsync(messageId);
		var msg = await _msgRepository.GetByIdAsync(msgDto.Id);

		if (msg == null)
			throw new ArgumentException($"Message with id {msgDto.Id} doesn't exist");

		return new MessageDto
		{
			Id = msgDto.Id,
			Content = msg.Content,
			TimeStamp = msg.TimeStamp,
			UserId = msg.UserId,
			ChatId = msg.ChatId
		};

	}

	public async Task RemoveMessage(MessageDto msgDto)
	{
		// Get the userChat record for userId, chatId
		var userChat = await _userChatRepository.GetByIdAsync(msgDto.UserId, msgDto.ChatId);

		// Check if null
		if (userChat == null)
			throw new ArgumentException($"UserChat record with user id {msgDto.UserId} and chat id {msgDto.ChatId} doesnt exist");

		// Authority: isSameUser/isAdmin/moderator
		// Same user -> User is in the chat of MSG => userChat != null && IsRequesterSameUser()
		// Moderator -> message is in the chat where the requestor is Moderator => UserChat != null && UserChat.Status == moderator
		// Is admin -> whenever

		if (msgDto.IsAdmin == null || msgDto.RequestorId == null)
			throw new ArgumentException("Missing required argument (IsAdmin or RequestorId)");
		// IsSameUser || Admin
		if (!(bool)msgDto.IsAdmin && (int)msgDto.RequestorId != msgDto.UserId)
			throw new UnauthorizedAccessException("Permission denied: unauthorized access");

		// Get message
		var msg = await _msgRepository.GetByIdAsync(msgDto.Id);
		// Check for null
		if (msg == null)
			throw new ArgumentException($"Message with id {msgDto.Id} doesn't exist");

		await _msgRepository.RemoveAsync(msgDto.Id);
	}

	public async Task UpdateMessage(MessageDto msgDto)
	{
		if (msgDto.RequestorId == null)
			throw new ArgumentException("Missing required argument (requestorId)");

		// Get message
		var msg = await _msgRepository.GetByIdAsync(msgDto.Id);
		// Check for null
		if (msg == null)
			throw new ArgumentException($"Message with id {msgDto.Id} doesn't exist");

		// Authority
		if (msgDto.RequestorId != msgDto.UserId)
			throw new UnauthorizedAccessException("Permission denied: unauthorized call of update message");

		// Update msg content
		msg.Content = msgDto.Content;

		// Call update
		await _msgRepository.UpdateAsync(msg);
	}
}
