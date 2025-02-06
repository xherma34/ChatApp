using System;
using ChatAppBackend.Data;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatAppBackend.Repositories.Implementations;

/// <summary>
/// Class implementing CRUD methods for User model
/// </summary>
public class UserRepository : IUserRepository
{
	private readonly ApplicationDbContext _dbContext;

	public UserRepository(ApplicationDbContext context)
	{
		_dbContext = context;
	}

	// ----------------------- GET METHODS -----------------------
	public async Task<User> GetByIdAsync(int id)
	{
		var user = await _dbContext.Users.FindAsync(id);
		if (user == null) throw new KeyNotFoundException($"User with ID {id} not found.");
		return user;
	}

	public async Task<IEnumerable<User>> GetAllAsync()
	{
		return await _dbContext.Users.ToListAsync();
	}

	// ----------------------- ADD METHODS -----------------------
	public async Task AddAsync(User user)
	{
		await _dbContext.Users.AddAsync(user);
		await _dbContext.SaveChangesAsync();
	}

	// ----------------------- UPDATE METHODS -----------------------
	public async Task UpdateAsync(User user)
	{
		_dbContext.Users.Update(user);
		await _dbContext.SaveChangesAsync();
	}

	// ----------------------- REMOVE METHODS -----------------------
	public async Task RemoveAsync(int id)
	{
		var user = await _dbContext.Users.FindAsync(id);

		if (user != null)
		{
			_dbContext.Users.Remove(user);
			await _dbContext.SaveChangesAsync();
		}
	}

}
