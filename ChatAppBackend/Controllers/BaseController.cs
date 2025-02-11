using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppBackend.Controllers;


[ApiController]
public abstract class BaseController : ControllerBase
{
	// protected int RequestorId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
	protected int? RequestorId
	{
		get
		{
			// Confirm the user is authenticated
			if (User?.Identity?.IsAuthenticated != true)
			{
				return null;
			}

			// Retrieve and safely parse the user ID claim
			var claim = User.FindFirst(ClaimTypes.NameIdentifier);
			if (claim == null || !int.TryParse(claim.Value, out int id))
			{
				return null;
			}

			return id;
		}
	}
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

