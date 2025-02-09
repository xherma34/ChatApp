using System;

namespace ChatAppBackend.DTOs;

public class MessageDto
{
	public int Id { get; set; }
	public string Content { get; set; } = string.Empty;

	public DateTime? TimeStamp { get; set; }

	public DateTime? DeleteDate { get; set; }

	public int UserId { get; set; }
	public int ChatId { get; set; }
	public int? RequestorId { get; set; }
	public bool? IsAdmin { get; set; }
}
