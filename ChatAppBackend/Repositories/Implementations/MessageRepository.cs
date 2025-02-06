using System;
using ChatAppBackend.Data;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatAppBackend.Repositories.Implementations;

public class MessageRepository : IMessageRepository
{
	private readonly ApplicationDbContext _dbContext;

	public MessageRepository(ApplicationDbContext context)
	{
		_dbContext = context;
	}

	// ----------------------- GET METHODS -----------------------
	public async Task<IEnumerable<Message>> GetAllByChatIdAsync(int chatId)
	{
		return await _dbContext.Messages
			.Where(m => m.ChatId == chatId)
			.ToListAsync();
	}

	public async Task<IEnumerable<Message>> GetAllByChatIdUserIdAsync(int chatId, int userId)
	{
		return await _dbContext.Messages
			.Where(m => m.ChatId == chatId && m.UserId == userId)
			.ToListAsync();
	}

	public async Task GetByIdAsync(int id)
	{
		var msg = await _dbContext.Messages.FindAsync(id);
		if (msg == null) throw new KeyNotFoundException($"Message with ID {id} not found.");
	}

	// ----------------------- ADD METHODS -----------------------
	public async Task AddAsync(Message message)
	{
		await _dbContext.Messages.AddAsync(message);
		await _dbContext.SaveChangesAsync();
	}


	// ----------------------- UPDATE METHODS -----------------------
	public async Task UpdateAsync(Message message)
	{
		_dbContext.Messages.Update(message);
		await _dbContext.SaveChangesAsync();
	}

	// ----------------------- REMOVE METHODS -----------------------
	public async Task RemoveAsync(int id)
	{
		var msg = await _dbContext.Messages.FindAsync(id);

		if (msg != null)
		{
			_dbContext.Messages.Remove(msg);
			await _dbContext.SaveChangesAsync();
		}
	}

}
