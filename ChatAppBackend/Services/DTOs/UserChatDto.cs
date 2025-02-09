using System;
using ChatAppBackend.Enums;

namespace ChatAppBackend.DTOs;

public class UserChatDto
{
	public int UserId { get; set; }
	public int ChatId { get; set; }
	public UserStatus UserStatus { get; set; }
}
