using System;
using ChatAppBackend.Enums;

namespace ChatAppBackend.Models;

public class UserChat
{
	// Foreign key to user
	public int UserId { get; set; }
	// Reference to user
	public User User { get; set; } = null!;

	// Foreign key to chat
	public int ChatId { get; set; }
	// Reference to chat
	public Chat Chat { get; set; } = null!;

	public UserStatus UserStatus { get; set; } = UserStatus.Regular;
}
