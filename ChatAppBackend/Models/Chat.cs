using System;

namespace ChatAppBackend.Models;

public class Chat
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;

	// Relationships
	// Chat 1-N UserChat
	public ICollection<UserChat> UsersChats { get; set; } = new List<UserChat>();

	// Chat 1 - N message
	public ICollection<Message> Messages { get; set; } = new List<Message>();
}
