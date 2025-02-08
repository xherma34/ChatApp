using System;
using ChatAppBackend.Enums;

namespace ChatAppBackend.DTOs;

public class NotificationDto
{
	public int? Id { get; set; }
	public string Content { get; set; } = string.Empty;
	public NotificationType Type { get; set; }
	public NotificationStatus Status { get; set; }
	public int UserId { get; set; }
}
