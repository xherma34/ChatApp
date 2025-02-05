using System;
using ChatAppBackend.Enums;

namespace ChatAppBackend.Models;

public class Notification
{
	// Properties
	public int Id { get; set; }
	public required string Content { get; set; }
	public NotificationType Type { get; set; }

	// Relationships

	// Notifications N - 1 User
	public int UserId { get; set; }
	public required User User { get; set; }
}
