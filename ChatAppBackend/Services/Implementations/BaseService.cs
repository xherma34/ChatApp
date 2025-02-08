using System;
using System.Security.Claims;

namespace ChatAppBackend.Services.Implementations;

public abstract class BaseService
{
	protected readonly IHttpContextAccessor _httpContextAccessor;

	protected BaseService(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	/// <summary>
	/// Returns true if the user requesting the data has role admin
	/// </summary>
	/// <exception cref="UnauthorizedAccessException"></exception>
	protected bool IsRequestorAdmin()
	{
		var userClaims = _httpContextAccessor.HttpContext?.User
						 ?? throw new UnauthorizedAccessException("No user context available.");
		return userClaims.IsInRole("Admin");
	}

	/// <summary>
	/// Returns true if sender of request has same id as userId
	/// </summary>
	/// <param name="userId">id of requested object</param>
	/// <returns></returns>
	/// <exception cref="UnauthorizedAccessException"></exception>
	protected bool IsRequestorSameUser(int userId)
	{
		var userClaims = _httpContextAccessor.HttpContext?.User
						 ?? throw new UnauthorizedAccessException("No user context available.");
		var requesterId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value
									 ?? throw new UnauthorizedAccessException("User ID claim is missing."));
		return requesterId == userId;
	}
}
