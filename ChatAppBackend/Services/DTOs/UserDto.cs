using System;
using ChatAppBackend.Enums;

namespace ChatAppBackend.DTOs;

public class UserDto
{
	public int Id { get; set; }
	public string Nickname { get; set; } = string.Empty;
	public string? MailAddress { get; set; }
	public DateTime JoinDate { get; set; }
	public bool IsBanned { get; set; } = false;
	public string Password { get; set; } = string.Empty;
	public UserRole Role { get; set; } = UserRole.Regular;
}
