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
#pragma warning disable CS8603 // Possible null reference return.
		return await _dbContext.Users.FindAsync(id);
#pragma warning restore CS8603 // Possible null reference return.

	}

	public async Task<IEnumerable<User>> GetAllAsync()
	{
		return await _dbContext.Users.ToListAsync();
	}

	public async Task<User> GetByMailAddressAsync(string mail)
	{
#pragma warning disable CS8603 // Possible null reference return.
		return await _dbContext.Users.FirstOrDefaultAsync(u => u.MailAddress == mail);
#pragma warning restore CS8603 // Possible null reference return.
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

	public async Task<bool> UserExists(int userId)
	{
		var user = await _dbContext.Users.FindAsync(userId);

		return user == null ? false : true;
	}
}
