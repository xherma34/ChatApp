using System;
using ChatAppBackend.DTOs;
using ChatAppBackend.Enums;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using ChatAppBackend.Services.Interfaces;

namespace ChatAppBackend.Services.Implementations;
// TODO: Deal with authorization
public class NotificationService : INotificationService
{
	private readonly INotificationRepository _notifRepository;
	private readonly IUserRepository _userRepository;

	public NotificationService(
		INotificationRepository notifRepo,
		IUserRepository userRepo
		)
	{
		_notifRepository = notifRepo;
		_userRepository = userRepo;
	}

	public async Task AddAsync(NotificationDto notif)
	{
		await _notifRepository.AddAsync(
		new Notification
		{
			Content = notif.Content,
			Type = notif.Type,
			Status = notif.Status,
			UserId = notif.UserId
		});

	}

	public async Task<IEnumerable<NotificationDto>> GetAllByStatusUserIdAsync(int userId, NotificationStatus status)
	{
		var notifs = await _notifRepository.GetAllByStatusUserIdAsync(userId, status);
		if (notifs == null)
			throw new KeyNotFoundException($"User doesn't have any notifications of status {status}");

		return notifs.Select(n => new NotificationDto
		{
			Id = n.Id,
			Content = n.Content,
			Type = n.Type,
			Status = n.Status,
			UserId = n.UserId
		});
	}

	public async Task<IEnumerable<NotificationDto>> GetAllByTypeUserIdAsync(int userId, NotificationType type)
	{
		var notifs = await _notifRepository.GetAllByTypeUserIdAsync(userId, type);
		if (notifs == null)
			throw new KeyNotFoundException($"User doesn't have any notifications of status {type}");

		return notifs.Select(n => new NotificationDto
		{
			Id = n.Id,
			Content = n.Content,
			Type = n.Type,
			Status = n.Status,
			UserId = n.UserId
		});
	}

	public async Task<IEnumerable<NotificationDto>> GetAllByUserIdAsync(int userId)
	{
		var notifs = await _notifRepository.GetAllByUserIdAsync(userId);
		if (notifs == null)
			throw new KeyNotFoundException($"User doesn't have any notifications");

		return notifs.Select(n => new NotificationDto
		{
			Id = n.Id,
			Content = n.Content,
			Type = n.Type,
			Status = n.Status,
			UserId = n.UserId
		});
	}

	public async Task<NotificationDto> GetByIdAsync(int notifId)
	{
		var notif = await _notifRepository.GetByIdAsync(notifId);
		if (notif == null)
			throw new KeyNotFoundException($"Notification with id {notifId} doesn't exist");

		return new NotificationDto
		{
			Id = notif.Id,
			Content = notif.Content,
			Type = notif.Type,
			Status = notif.Status,
			UserId = notif.UserId
		};

	}

	public async Task RemoveAsync(int notifId)
	{
		await _notifRepository.RemoveAsync(notifId);
	}

	public async Task UpdateAsync(NotificationDto notif)
	{
		await _notifRepository.UpdateAsync(new Notification
		{
			Content = notif.Content,
			Type = notif.Type,
			Status = notif.Status,
			UserId = notif.UserId
		});
	}
}
