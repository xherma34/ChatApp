using System;
using ChatAppBackend.Enums;
using ChatAppBackend.Models;

namespace ChatAppBackend.Repositories.Interfaces;

public interface INotificationRepository
{
	// ----------------------- GET METHODS -----------------------
	Task<Notification> GetByIdAsync(int id);
	Task<IEnumerable<Notification>> GetAllByUserIdAsync(int userId);
	Task<IEnumerable<Notification>> GetAllByTypeUserIdAsync(NotificationType type, int userId);
	Task<IEnumerable<Notification>> GetAllByStatusUserIdAsync(NotificationStatus status, int userId);

	// ----------------------- ADD METHODS -----------------------
	Task AddAsync(Notification notif);

	// ----------------------- UPDATE METHODS -----------------------
	Task UpdateAsync(Notification notif);

	// ----------------------- REMOVE METHODS -----------------------
	Task RemoveAsync(int id);
}
