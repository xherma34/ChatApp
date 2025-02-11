using System;
using System.ComponentModel.DataAnnotations;
using ChatAppBackend.Enums;

namespace ChatAppBackend.Models;

public class User
{
	// Properties
	public int Id { get; set; }
	public string Nickname { get; set; } = string.Empty;
	public string PasswordHash { get; set; } = string.Empty;
	public string MailAddress { get; set; } = string.Empty;
	public DateTime JoinDate { get; set; }
	public bool IsBanned { get; set; } = false;
	public UserRole Role { get; set; } = UserRole.Regular;
	public string RefreshToken { get; set; } = string.Empty;

	// Relationships
	// User 1 - N messages
	public ICollection<Message> Messages
	{ get; set; } = new List<Message>();

	// User 1 - N notifications
	public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

	// User 1 - N UserChat
	public ICollection<UserChat> UserChats { get; set; } = new List<UserChat>();
}
