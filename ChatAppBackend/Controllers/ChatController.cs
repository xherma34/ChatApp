using System;
using ChatAppBackend.Controllers.DTOs;
using ChatAppBackend.DTOs;
using ChatAppBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcRoute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace ChatAppBackend.Controllers;

[Authorize]
[MvcRoute("api/chats")]
public class ChatController : BaseController
{
	private readonly IChatService _chatService;
	private readonly IUserChatService _userChatService;

	public ChatController(IChatService chatServ, IUserChatService ucServ)
	{
		_chatService = chatServ;
		_userChatService = ucServ;
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetByIdAsync(int id)
	{
		try
		{
			var chat = await _chatService.GetByIdAsync(RequestorId, id);
			return Ok(chat);
		}
		catch (Exception ex)
		{
			return HandleError(ex);
		}
	}

	[HttpGet("all")]
	public async Task<IActionResult> GetAllAsync()
	{
		try
		{
			var chats = await _chatService.GetAllAsync(RequestorId, IsAdmin);
			return Ok(chats);
		}
		catch (Exception ex)
		{
			return HandleError(ex);
		}
	}

	/// <summary>
	/// Fetches all users that are part of chat with chatId
	/// </summary>
	[HttpGet("{chatId}/users")]
	public async Task<IActionResult> GetAllUsersOfChatAsync(int chatId)
	{
		UserChatRequest chatReq = new UserChatRequest
		{
			RequestorId = base.RequestorId,
			ChatId = chatId,
			IsAdmin = base.IsAdmin
		};

		try
		{
			var users = await _userChatService.GetAllUsersInChat(chatReq);
			return Ok($"All users of chat{chatId} fetched succesfully");
		}
		catch (Exception ex)
		{
			return HandleError(ex);
		}
	}

	[HttpPost]
	public async Task<IActionResult> AddAsync([FromBody] ChatDto chatDto)
	{
		try
		{
			await _chatService.AddAsync(RequestorId, chatDto);
			return Ok("New chat created");
		}
		catch (Exception ex)
		{
			return HandleError(ex);
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateNameAsync(int id, [FromBody] ChatDto chatDto)
	{
		chatDto.Id = id;
		try
		{
			await _chatService.UpdateNameAsync(RequestorId, chatDto, IsAdmin);
			return Ok($"Chatroom name changed to {chatDto.Name}");
		}
		catch (Exception ex)
		{
			return HandleError(ex);
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> RemoveAsync(int id)
	{
		try
		{
			await _chatService.RemoveAsync(RequestorId, id, IsAdmin);
			return Ok($"Chat with id {id} removed succesfully");
		}
		catch (Exception ex)
		{
			return HandleError(ex);
		}
	}


}
