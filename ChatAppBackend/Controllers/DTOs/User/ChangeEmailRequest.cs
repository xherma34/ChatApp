using System;

namespace ChatAppBackend.Controllers.DTOs;

public class ChangeEmailRequest
{
	public string Password { get; set; } = string.Empty;
	public string EmailAddress { get; set; } = string.Empty;
}
