using System;
using ChatAppBackend.DTOs;
using ChatAppBackend.Enums;
using ChatAppBackend.Models;

namespace ChatAppBackend.Services.Interfaces;

public interface INotificationService
{
	Task<NotificationDto> GetByIdAsync(int notifId);
	Task<IEnumerable<NotificationDto>> GetAllByUserIdAsync(int userId);
	Task<IEnumerable<NotificationDto>> GetAllByTypeUserIdAsync(int userId, NotificationType type);
	Task<IEnumerable<NotificationDto>> GetAllByStatusUserIdAsync(int userId, NotificationStatus status);

	Task AddAsync(NotificationDto notif);
	Task UpdateAsync(NotificationDto notif);
	Task RemoveAsync(int notifId);
}
