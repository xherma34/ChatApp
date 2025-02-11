using System;

namespace ChatAppBackend.Controllers.DTOs;

public class AuthRequest
{
	// For token refresher
	public int? RequestorId { get; set; }

	// User credentials for login
	public string? MailAddress { get; set; }
	public string? Password { get; set; }

	// User crenedtials for register
	public string? Nickname { get; set; }
}
