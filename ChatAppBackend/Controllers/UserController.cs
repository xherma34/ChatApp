using System;
using ChatAppBackend.Controllers.DTOs;
using ChatAppBackend.DTOs;
using ChatAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using MvcRoute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace ChatAppBackend.Controllers;

[Authorize]
[MvcRoute("api/users")]
public class UserController : BaseController
{
	private readonly IUserService _userService;
	private readonly IUserChatService _userChatService;

	public UserController(IUserService usrServ, IUserChatService ucServ)
	{
		_userService = usrServ;
		_userChatService = ucServ;
	}

	/// <summary>
	/// Api endpoint to fetch user by id
	/// </summary>
	/// <param name="id">user's id</param>
	/// <returns>Fetched user data</returns>
	[HttpGet("{id}")]
	public async Task<IActionResult> GetUserById(int id)
	{
		try
		{
			var user = await _userService.GetByIdAsync(id, (int)RequestorId, IsAdmin);
			return Ok(user);
		}
		catch (Exception ex)
		{
			return HandleError(ex);
		}
	}

	/// <summary>
	/// Api endpoint to fetch all users
	/// </summary>
	/// <returns>List of all registered users</returns>
	[HttpGet("all")]
	public async Task<IActionResult> GetAllUsers()
	{
		try
		{
			var users = await _userService.GetAllAsync(IsAdmin);
			return Ok(users);
		}
		catch (Exception ex)
		{

			return HandleError(ex);
		}
	}

	/// <summary>
	/// Fetches all chats that user is a part of
	/// </summary>
	[HttpGet("{userId}/chats")]
	public async Task<IActionResult> GetAllUserChats(int userId)
	{
		UserChatRequest ucReq = new UserChatRequest
		{
			RequestorId = base.RequestorId,
			UserId = userId,
			IsAdmin = base.IsAdmin
		};

		try
		{
			var chats = await _userChatService.GetAllChatsOfUser(ucReq);
			return Ok(chats);
		}
		catch (Exception ex)
		{
			return HandleError(ex);
		}
	}

	/// <summary>
	/// Api endpoint to add new user
	/// </summary>
	/// <param name="userDto">User registration data</param>
	[AllowAnonymous]
	[HttpPost]
	public async Task<IActionResult> AddUser([FromBody] UserDto userDto)
	{
		try
		{
			await _userService.AddAsync(userDto);
			return CreatedAtAction(nameof(GetUserById), new { id = userDto.Id }, userDto);
		}
		catch (Exception ex)
		{

			return HandleError(ex);
		}
	}


	/// <summary>
	/// Api endpoint to change user's info
	/// </summary>
	/// <param name="id">id of the user</param>
	/// <param name="userDto">COntains data to be changed</param>
	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
	{
		try
		{
			await _userService.UpdateAsync(userDto, id, (int)RequestorId, IsAdmin);
			return Ok($"User {userDto.Id} updated");
		}
		catch (Exception ex)
		{

			return HandleError(ex);
		}
	}

	/// <summary>
	/// Api endpoint to change user's email address
	/// </summary>
	/// <param name="id">id of the user</param>
	/// <param name="data">Contains the email address and password</param>
	[HttpPut("{id}/email")]
	public async Task<IActionResult> UpdateUserMail(int id, [FromBody] ChangeEmailRequest data)
	{
		try
		{
			// Update email call
			await _userService.UpdateMailAddressAsync(
				new UserDto { Id = id, Password = data.Password },
				data.EmailAddress, IsAdmin, (int)RequestorId);

			// Return
			return Ok($"Email address updated succesfully {data.EmailAddress}");
		}
		catch (Exception ex)
		{

			return HandleError(ex);
		}
	}

	/// <summary>
	/// Api endpoint to change user's password
	/// </summary>
	/// <param name="id">ID of user</param>
	/// <param name="data">Contains the old and new password by user</param>
	[HttpPut("{id}/password")]
	public async Task<IActionResult> UpdateUserPassword(int id, [FromBody] ChangePasswordRequest data)
	{
		try
		{
			// Call update service
			await _userService.UpdatePasswordAsync(new UserDto { Id = id, Password = data.OldPassword },
				data.NewPassword, IsAdmin, (int)RequestorId);

			// Return
			return Ok("Password updated succesfully");
		}
		catch (Exception ex)
		{

			return HandleError(ex);
		}
	}

	/// <summary>
	/// Api endpoint that removes user with ID id
	/// </summary>
	/// <param name="id">ID of user to be removed</param>
	[HttpDelete("{id}")]
	public async Task<IActionResult> RemoveUser(int id)
	{
		try
		{
			await _userService.RemoveByIdAsync(id, (int)RequestorId);
			return Ok($"User with ID {id} removed succesfully");
		}
		catch (Exception ex)
		{

			return HandleError(ex);
		}
	}

}
