using System;

namespace ChatAppBackend.Models;

public class Message
{
	// Properties
	public int Id { get; set; }
	public required string Content { get; set; }
	public DateTime TimeStamp { get; set; }

	// Message N - 1 User
	// User foreign key
	public int UserId { get; set; }
	// User reference
	public required User User { get; set; }

	// Message N - 1 Chat
	public int ChatId { get; set; }
	public required Chat Chat { get; set; }

}
