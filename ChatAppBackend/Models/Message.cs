using System;

namespace ChatAppBackend.Models;

public class Message
{
	// Properties
	public int Id { get; set; }
	// TODO: Create interface for this since it can either me a msg in a form of a string or an image
	public string Content { get; set; } = string.Empty;
	public DateTime TimeStamp { get; set; }

	// Message N - 1 User
	// User foreign key
	public int UserId { get; set; }
	// User reference
	public User User { get; set; } = null!;

	// Message N - 1 Chat
	public int ChatId { get; set; }
	public Chat Chat { get; set; } = null!;

}
