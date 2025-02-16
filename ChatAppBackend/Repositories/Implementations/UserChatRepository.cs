using System;
using ChatAppBackend.Data;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatAppBackend.Repositories.Implementations;

public class UserChatRepository : IUserChatRepository
{
	private readonly ApplicationDbContext _dbContext;

	public UserChatRepository(ApplicationDbContext context)
	{
		_dbContext = context;
	}

	public async Task AddUserToChatAsync(UserChat userChat)
	{
		await _dbContext.UserChats.AddAsync(userChat);
		await _dbContext.SaveChangesAsync();
	}

	public async Task RemoveUserFromChatAsync(int userId, int chatId)
	{
		var userChat = await _dbContext.UserChats
			.FirstOrDefaultAsync(uc => uc.ChatId == chatId && uc.UserId == userId);

		if (userChat == null) throw new KeyNotFoundException($"User with ID {userId} is not a part of chat with ID {chatId}.");

		_dbContext.UserChats.Remove(userChat);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<UserChat> GetByIdAsync(int userId, int chatId)
	{
#pragma warning disable CS8603 // Possible null reference return.
		return await _dbContext.UserChats
			.FirstOrDefaultAsync(uc => uc.ChatId == chatId && uc.UserId == userId);
#pragma warning restore CS8603 // Possible null reference return.
	}

	public async Task<IEnumerable<Chat>> GetAllChatsOfUserAsync(int userId)
	{
		return await _dbContext.UserChats
			.Where(uc => uc.UserId == userId)
			.Select(uc => uc.Chat)
			.ToListAsync();
	}

	public async Task<IEnumerable<User>> GetAllUsersInChatAsync(int chatId)
	{
		return await _dbContext.UserChats
		.Where(uc => uc.ChatId == chatId)
		.Select(uc => uc.User)
		.ToListAsync();
	}

	public bool IsUserInChat(int userId, int chatId)
	{
		return _dbContext.UserChats
			.Any(uc => uc.UserId == userId && uc.ChatId == chatId);
	}

	public async Task UpdateUserChatRoleAsync(UserChat userChat)
	{
		_dbContext.UserChats.Update(userChat);
		await _dbContext.SaveChangesAsync();
	}

	// public async Task<bool> IsUserChatModerator(int userId, int chatId)
	// {
	// 	var userChat = await GetUserChatById(userId, chatId);
	// 	if (userChat == null)
	// 		throw new ArgumentException($"UserChat record with user id {userId} and chat id {chatId} doesnt exist");

	// 	return userChat.UserStatus == Enums.UserStatus.Moderator ? true : false;
	// }




}
