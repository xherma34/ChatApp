using System;
using ChatAppBackend.Controllers.DTOs;
using ChatAppBackend.DTOs;
using ChatAppBackend.Models;
using ChatAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MvcRoute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace ChatAppBackend.Controllers;

[MvcRoute("api/auth")]
public class AuthController : BaseController
{
	private readonly IUserService _userService;
	private readonly IAuthService _authService;

	public AuthController(IUserService usrServ, IAuthService aServ)
	{
		_userService = usrServ;
		_authService = aServ;
	}

	[HttpPost("register")]
	public async Task<IActionResult> RegisterUserAsync([FromBody] AuthRequest data)
	{

#pragma warning disable CS8601 // Possible null reference assignment.
		var user = new UserDto
		{
			Nickname = data.Nickname,
			MailAddress = data.MailAddress,
			Password = data.Password
		};
#pragma warning restore CS8601 // Possible null reference assignment.

		try
		{
			await _authService.RegisterUserAsync(user);
			// await _userService.AddAsync(user);
			return Ok("User registered succesfully");
		}
		catch (Exception ex)
		{
			return HandleError(ex);
		}
	}

	[HttpPost("login")]
	public async Task<IActionResult> LoginUserAsync([FromBody] AuthRequest data)
	{
#pragma warning disable CS8601 // Possible null reference assignment.
		var user = new UserDto
		{
			MailAddress = data.MailAddress,
			Password = data.Password
		};
#pragma warning restore CS8601 // Possible null reference assignment.

		try
		{
			var jwtToken = await _authService.AuthenticateUserAsync(user);
			return Ok(new { Token = jwtToken });

		}
		catch (Exception ex)
		{
			return HandleError(ex);
		}
	}

	public Task<IActionResult> RefreshToken()
	{
		throw new NotImplementedException();
	}
}
