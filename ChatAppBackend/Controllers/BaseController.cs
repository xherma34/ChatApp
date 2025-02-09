using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppBackend.Controllers;


[ApiController]
public abstract class BaseController : ControllerBase
{
	protected int RequestorId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
	protected bool IsAdmin => User.IsInRole("Admin");

	protected IActionResult HandleError(Exception ex)
	{
		return ex switch
		{
			UnauthorizedAccessException => Forbid(),
			KeyNotFoundException => NotFound(ex.Message),
			ArgumentException => BadRequest(ex.Message),
			_ => StatusCode(500, "An unexpected error occurred.")
		};
	}
}

