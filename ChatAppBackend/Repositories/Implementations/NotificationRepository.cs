using System;
using ChatAppBackend.Data;
using ChatAppBackend.Enums;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatAppBackend.Repositories.Implementations;

public class NotificationRepository : INotificationRepository
{
	private readonly ApplicationDbContext _dbContext;

	public NotificationRepository(ApplicationDbContext context)
	{
		_dbContext = context;
	}

	// ----------------------- GET METHODS -----------------------
	public async Task<IEnumerable<Notification>> GetAllByStatusUserIdAsync(int userId, NotificationStatus status)
	{
		return await _dbContext.Notifications
			.Where(n => n.Status == status && n.UserId == userId)
			.ToListAsync();
	}

	public async Task<IEnumerable<Notification>> GetAllByTypeUserIdAsync(int userId, NotificationType type)
	{
		return await _dbContext.Notifications
			.Where(n => n.Type == type && n.UserId == userId)
			.ToListAsync();
	}

	public async Task<IEnumerable<Notification>> GetAllByUserIdAsync(int userId)
	{
		return await _dbContext.Notifications
			.Where(n => n.UserId == userId)
			.ToListAsync();
	}

	public async Task<Notification> GetByIdAsync(int id)
	{
		var notif = await _dbContext.Notifications.FindAsync(id);
		if (notif == null) throw new KeyNotFoundException($"User with ID {id} not found.");
		return notif;
	}

	// ----------------------- ADD METHODS -----------------------
	public async Task AddAsync(Notification notif)
	{
		await _dbContext.Notifications.AddAsync(notif);
		await _dbContext.SaveChangesAsync();
	}

	// ----------------------- UPDATE METHODS -----------------------
	public async Task UpdateAsync(Notification notif)
	{
		_dbContext.Update(notif);
		await _dbContext.SaveChangesAsync();
	}

	// ----------------------- REMOVE METHODS -----------------------
	public async Task RemoveAsync(int id)
	{
		var notif = await _dbContext.Notifications.FindAsync(id);

		if (notif != null)
		{
			_dbContext.Notifications.Remove(notif);
			await _dbContext.SaveChangesAsync();
		}
	}



}
