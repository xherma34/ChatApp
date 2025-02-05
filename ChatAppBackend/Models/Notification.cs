using System;
using ChatAppBackend.Enums;

namespace ChatAppBackend.Models;

public class Notification
{
	// Properties
	public int Id { get; set; }
	public string Content { get; set; } = string.Empty;
	public NotificationType Type { get; set; }

	// Relationships

	// Notifications N - 1 User
	public int UserId { get; set; }
	public User User { get; set; } = null!;
}
