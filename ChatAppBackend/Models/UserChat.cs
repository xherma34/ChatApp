using System;

namespace ChatAppBackend.Models;

public class UserChat
{
	// Foreign key to user
	public int UserId { get; set; }
	// Reference to user
	public required User User { get; set; }

	// Foreign key to chat
	public int ChatId { get; set; }
	// Reference to chat
	public required Chat Chat { get; set; }
}
