using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppBackend.Controllers;


[ApiController]
public abstract class BaseController : ControllerBase
{

	/// <summary>
	/// Use this for endpoints that REQUIRE authentication (throws if not authenticated)
	/// </summary>
	protected int RequestorId
	{
		get
		{
			if (User?.Identity?.IsAuthenticated != true)
			{
				throw new UnauthorizedAccessException("User is not authenticated.");
			}

			var claim = User.FindFirst(ClaimTypes.NameIdentifier);
			if (claim == null || !int.TryParse(claim.Value, out int id))
			{
				throw new UnauthorizedAccessException("User ID is invalid.");
			}

			return id;
		}
	}

	/// <summary>
	/// Use this for endpoints that ALLOW anonymous access (returns null if not authenticated)
	/// </summary>
	protected int? RequestorIdOrNull
	{
		get
		{
			if (User?.Identity?.IsAuthenticated != true)
			{
				return null; // Returns null instead of throwing
			}

			var claim = User.FindFirst(ClaimTypes.NameIdentifier);
			return claim != null && int.TryParse(claim.Value, out int id) ? id : null;
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

