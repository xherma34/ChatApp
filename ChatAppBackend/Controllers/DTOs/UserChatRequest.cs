using System;

namespace ChatAppBackend.Controllers.DTOs;

public class UserChatRequest
{
	public int? RequestorId { get; set; }
	public int? UserId { get; set; }
	public int? ChatId { get; set; }
	public bool? IsAdmin { get; set; }
}
