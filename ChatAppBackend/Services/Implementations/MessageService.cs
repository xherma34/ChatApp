using System;
using ChatAppBackend.DTOs;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using ChatAppBackend.Services.Interfaces;

namespace ChatAppBackend.Services.Implementations;

public class MessageService : BaseService, IMessageService
{
	private readonly IUserRepository _userRepository;
	private readonly IChatRepository _chatRepository;
	private readonly IMessageRepository _msgRepository;
	private readonly IUserChatRepository _userChatRepository;

	public MessageService(
		IHttpContextAccessor httpContextAccessor,
		IUserRepository userRep,
		IMessageRepository msgRep,
		IChatRepository chatRep,
		IUserChatRepository userChatRep
	) : base(httpContextAccessor)
	{
		_userRepository = userRep;
		_msgRepository = msgRep;
		_chatRepository = chatRep;
		_userChatRepository = userChatRep;
	}

	public async Task AddMessage(MessageDto msgDto)
	{
		// Authority check -> IsSameUser
		if (!IsRequesterSameUser(msgDto.UserId))
			throw new UnauthorizedAccessException("Permission denied");

		// Authority check -> user is within the chat
		if (_userChatRepository.IsUserInChat(msgDto.UserId, msgDto.ChatId))
			throw new UnauthorizedAccessException("Permission denied: user must be a part of chat to send messages");

		// Get user - check null
		var user = await _userRepository.GetByIdAsync(msgDto.UserId);

		// Get chat - check null
		var chat = await _chatRepository.GetByIdAsync(msgDto.ChatId);

		Message msg = new Message
		{
			Id = msgDto.Id,
			Content = msgDto.Content,
			TimeStamp = DateTime.Today,
			UserId = msgDto.UserId,
			ChatId = msgDto.ChatId,
			User = user,
			Chat = chat
		};

		await _msgRepository.AddAsync(msg);

	}

	public async Task<IEnumerable<MessageDto>> GetAllUserMessages(int userId)
	{
		// Check authority: SameId OR admin
		if (!IsRequesterAdmin() && !IsRequesterSameUser(userId))
			throw new UnauthorizedAccessException("Permission denied: unauthorized access");

		var messages = await _msgRepository.GetAllByUserIdAsync(userId);

		return messages.Select(m => new MessageDto
		{
			Id = m.Id,
			Content = m.Content,
			TimeStamp = m.TimeStamp,
			DeleteDate = m.DeleteDate,
			UserId = userId,
			ChatId = m.ChatId
		});
	}

	public async Task<MessageDto> GetById(int messageId, int userId)
	{
		// IsSameUser || Admin
		if (!IsRequesterAdmin() && !IsRequesterSameUser(userId))
			throw new UnauthorizedAccessException("Permission denied: unauthorized access");

		// var message = await _msgRepository.GetByIdAsync(messageId);
		var msg = await _msgRepository.GetByIdAsync(messageId);

		if (msg == null)
			throw new ArgumentException($"Message with id {messageId} doesn't exist");

		return new MessageDto
		{
			Id = messageId,
			Content = msg.Content,
			TimeStamp = msg.TimeStamp,
			UserId = msg.UserId,
			ChatId = msg.ChatId
		};

	}

	public async Task RemoveMessage(int messageId, int chatId, int userId)
	{
		// Get the userChat record for userId, chatId
		var userChat = await _userChatRepository.GetByIdAsync(userId, chatId);

		// Check if null
		if (userChat == null)
			throw new ArgumentException($"UserChat record with user id {userId} and chat id {chatId} doesnt exist");

		// Authority: isSameUser/isAdmin/moderator
		// Same user -> User is in the chat of MSG => userChat != null && IsRequesterSameUser()
		// Moderator -> message is in the chat where the requestor is Moderator => UserChat != null && UserChat.Status == moderator
		// Is admin -> whenever

		if (!IsRequesterAdmin() && !IsRequesterSameUser(userId) && userChat.UserStatus != Enums.UserStatus.Moderator)
			throw new UnauthorizedAccessException("Permission denied: unauthorized call of Remove message");

		// Get message
		var msg = await _msgRepository.GetByIdAsync(messageId);
		// Check for null
		if (msg == null)
			throw new ArgumentException($"Message with id {messageId} doesn't exist");

		await _msgRepository.RemoveAsync(messageId);
	}

	public async Task UpdateMessage(int messageId, int userId, string newContent)
	{
		// Get message
		var msg = await _msgRepository.GetByIdAsync(messageId);
		// Check for null
		if (msg == null)
			throw new ArgumentException($"Message with id {messageId} doesn't exist");

		// Authority
		if (!IsRequesterSameUser(userId))
			throw new UnauthorizedAccessException("Permission denied: unauthorized call of update message");

		// Update msg content
		msg.Content = newContent;

		// Call update
		await _msgRepository.UpdateAsync(msg);
	}
}
