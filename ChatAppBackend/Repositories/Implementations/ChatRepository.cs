using System;
using ChatAppBackend.Data;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatAppBackend.Repositories.Implementations;

public class ChatRepository : IChatRepository
{
	private readonly ApplicationDbContext _dbContext;

	public ChatRepository(ApplicationDbContext context)
	{
		_dbContext = context;
	}

	// ----------------------- GET METHODS -----------------------
	public async Task<Chat?> GetByIdAsync(int id)
	{
		var chat = await _dbContext.Chats.FindAsync(id);
		if (chat == null) throw new KeyNotFoundException($"Chat with ID {id} not found.");
		return chat;
	}

	public async Task<IEnumerable<Chat>> GetAllAsync()
	{
		return await _dbContext.Chats.ToListAsync();
	}

	// ----------------------- ADD METHODS -----------------------
	public async Task AddAsync(Chat chat)
	{
		await _dbContext.Chats.AddAsync(chat);
		await _dbContext.SaveChangesAsync();
	}

	// ----------------------- UPDATE METHODS -----------------------
	public async Task UpdateAsync(Chat chat)
	{
		_dbContext.Chats.Update(chat);
		await _dbContext.SaveChangesAsync();
	}

	// ----------------------- REMOVE METHODS -----------------------
	public async Task RemoveAsync(int id)
	{
		var chat = await _dbContext.Chats.FindAsync(id);

		if (chat != null)
		{
			_dbContext.Chats.Remove(chat);
			await _dbContext.SaveChangesAsync();
		}
	}
}
