using System;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using ChatAppBackend.Controllers.DTOs;
using ChatAppBackend.Enums;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Interfaces;
using ChatAppBackend.Services.Implementations;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.VisualBasic;
using Moq;

namespace ChatAppBackend.Tests.Services;

[Trait("Category", "Services")]
[Trait("Feature", "UserChat")]
public class UserChatServiceTests
{

	private readonly Mock<IUserRepository> _mockUserRepo;
	private readonly Mock<IChatRepository> _mockChatRepo;
	private readonly Mock<IUserChatRepository> _mockUserChatRepo;
	private readonly UserChatService _service;

	public UserChatServiceTests()
	{
		_mockChatRepo = new Mock<IChatRepository>();
		_mockUserRepo = new Mock<IUserRepository>();
		_mockUserChatRepo = new Mock<IUserChatRepository>();

		_service = new UserChatService(_mockUserRepo.Object, _mockChatRepo.Object, _mockUserChatRepo.Object);
	}

	#region GET methods of service

	#region Get By ID

	[Fact]
	public async Task GetByIdAsync_Should_Fetch_UC()
	{
		// Arrange
		int userId = 0;
		int chatId = 10;

		// Create the UC that is supposed to be returned
		var expectedUC = new UserChat
		{
			UserId = userId,
			ChatId = chatId,
			UserRole = UserChatRole.Moderator
		};

		// Setup mock for getByIdAsync
		_mockUserChatRepo
			.Setup(repo => repo.GetByIdAsync(userId, chatId))
			.ReturnsAsync(expectedUC);

		// Act
		var result = await _service.GetByIdAsync(userId, chatId);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(0, result.UserId);
		Assert.Equal(10, result.ChatId);
		Assert.Equal(UserChatRole.Moderator, result.UserRole);
	}

	[Fact]
	public async Task GetByIdAsync_Should_Throw_Argument_Exception_Non_Existant_UC()
	{
		// arrange
		int userId = 0;
		int chatId = 10;

		// Setup mock
		_mockUserChatRepo
			.Setup(repo => repo.GetByIdAsync(userId, chatId))
			.ReturnsAsync((UserChat?)null);

		// Act & Assert
		var exception = await Assert.ThrowsAsync<ArgumentException>(
			() => _service.GetByIdAsync(userId, chatId));

		Assert.Contains($"User {userId} is not in chat {chatId}", exception.Message);

	}

	#endregion

	#region Get all users in chat

	[Fact]
	public async Task GetAllUsersInChat_Should_Fetch_All_Users()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 0,
			ChatId = 10,
			IsAdmin = true
		};

		// Setup: IsUserInChat
		_mockUserChatRepo
			.Setup(repo => repo.IsUserInChat(request.RequestorId.Value, request.ChatId.Value))
			.Returns(true);

		// Setup: What should be returned
		var usersReturn = new List<User>
		{
			new User { Id = 0, Nickname = "user0", IsBanned = false },
			new User { Id = 1, Nickname = "user1", IsBanned = false }
		};

		// Setup: mock 
		_mockUserChatRepo
			.Setup(repo => repo.GetAllUsersInChatAsync(request.ChatId.Value))
			.ReturnsAsync(usersReturn);

		// Act
		var result = await _service.GetAllUsersInChatAsync(request);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(2, result.Count());
		Assert.Contains(result, dto => dto.Id == 0 && dto.Nickname == "user0");
		Assert.Contains(result, dto => dto.Id == 1 && dto.Nickname == "user1");

	}

	[Fact]
	public async Task GetAllUsersInChat_Should_Throw_Argument_Exception_Missing_Arguments()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = null,
			UserId = 0,
			IsAdmin = false,
			ChatId = 10
		};

		// Act && Assert
		var exception = await Assert.ThrowsAsync<ArgumentException>(
			() => _service.GetAllUsersInChatAsync(request));

		// Assert
		Assert.Equal($"Missing required data to fetch users of chat", exception.Message);
	}

	[Fact]
	public async Task GetAllUsersInChat_Should_Throw_Unauthorized_Access()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 0,
			IsAdmin = false,
			ChatId = 10
		};

		// Setup: IsUserInChat
		_mockUserChatRepo
			.Setup(repo => repo.IsUserInChat(request.UserId.Value, request.ChatId.Value))
			.Returns(false);


		// Act
		var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
			() => _service.GetAllUsersInChatAsync(request));

		// Assert
		Assert.Equal($"Permission denied: unauthorized call of get all users in chat", exception.Message);
	}

	[Fact]
	public async Task GetAllUsersInChat_Should_Throw_Argument_Exception_No_Existing_Users()
	{
		// $"No records of users in chat ${(int)data.ChatId}"

		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 0,
			IsAdmin = true,
			ChatId = 10
		};

		// Setup: IsUserInChat
		_mockUserChatRepo
			.Setup(repo => repo.IsUserInChat(request.RequestorId.Value, request.ChatId.Value))
			.Returns(true);

		// Setup: repository returns an empty collection.
		_mockUserChatRepo
			.Setup(repo => repo.GetAllUsersInChatAsync(request.ChatId.Value))
			.ReturnsAsync(new List<User>());

		// Act
		var exception = await Assert.ThrowsAsync<ArgumentException>(
			() => _service.GetAllUsersInChatAsync(request));

		// Assert
		Assert.Equal($"No records of users in chat ${request.ChatId.Value}", exception.Message);
	}

	#endregion

	#region Get all chats of user

	[Fact]
	public async Task GetAllChatsOfUserAsync_Should_Fetch_All_Chats()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 0,
			ChatId = 10,
			IsAdmin = true
		};

		var chatsReturn = new List<Chat>
		{
			new Chat { Id = 0, Name = "chat0" },
			new Chat { Id = 1, Name = "chat1" }
		};

		// Setup mock userexists of userrepository
		_mockUserRepo
			.Setup(repo => repo.UserExists(request.UserId.Value))
			.ReturnsAsync(true);


		// Setup mock GetAllChatsOfUser to return the list of users
		_mockUserChatRepo
			.Setup(repo => repo.GetAllChatsOfUserAsync(request.UserId.Value))
			.ReturnsAsync(chatsReturn);

		// Act
		var result = await _service.GetAllChatsOfUserAsync(request);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(2, result.Count());
		Assert.Contains(result, dto => dto.Id == 0 && dto.Name == "chat0");
		Assert.Contains(result, dto => dto.Id == 1 && dto.Name == "chat1");
	}

	[Fact]
	public async Task GetAllChatsOfUserAsync_Should_Not_Throw_When_Requestor_Same_User()
	{
		// Arrange: requestor is the same user
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 0,
			IsAdmin = false,
			ChatId = 10
		};

		var chatsReturn = new List<Chat>
		{
			new Chat { Id = 0, Name = "chat0" },
			new Chat { Id = 1, Name = "chat1" }
		};

		// Setup mock userexists of userrepository
		_mockUserRepo
			.Setup(repo => repo.UserExists(request.UserId.Value))
			.ReturnsAsync(true);


		// Setup mock GetAllChatsOfUser to return the list of users
		_mockUserChatRepo
			.Setup(repo => repo.GetAllChatsOfUserAsync(request.UserId.Value))
			.ReturnsAsync(chatsReturn);

		// Act
		var result = await _service.GetAllChatsOfUserAsync(request);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(2, result.Count());
		Assert.Contains(result, dto => dto.Id == 0 && dto.Name == "chat0");
		Assert.Contains(result, dto => dto.Id == 1 && dto.Name == "chat1");

	}

	// TODO: Make methods for the correct version so you can test with different requests and not cmd c, cmd v

	[Fact]
	public async Task GetAllChatsOfUserAsync_Should_Not_Throw_When_Is_Admin()
	{
		// Arrange: requestor is admin
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 1,
			IsAdmin = true,
			ChatId = 10
		};

		var chatsReturn = new List<Chat>
		{
			new Chat { Id = 0, Name = "chat0" },
			new Chat { Id = 1, Name = "chat1" }
		};

		// Setup mock userexists of userrepository
		_mockUserRepo
			.Setup(repo => repo.UserExists(request.UserId.Value))
			.ReturnsAsync(true);


		// Setup mock GetAllChatsOfUser to return the list of users
		_mockUserChatRepo
			.Setup(repo => repo.GetAllChatsOfUserAsync(request.UserId.Value))
			.ReturnsAsync(chatsReturn);

		// Act
		var result = await _service.GetAllChatsOfUserAsync(request);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(2, result.Count());
		Assert.Contains(result, dto => dto.Id == 0 && dto.Name == "chat0");
		Assert.Contains(result, dto => dto.Id == 1 && dto.Name == "chat1");

	}

	[Fact]
	public async Task GetAllChatsOfUserAsync_Should_Throw_Argument_Exception_Missing_Arguments()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = null,
			ChatId = 10,
			IsAdmin = true
		};

		// Act & assert
		var exception = await Assert.ThrowsAsync<ArgumentException>(
			() => _service.GetAllChatsOfUserAsync(request));

		Assert.Equal($"Missing required data to fetch chats of user", exception.Message);

	}

	[Fact]
	public async Task GetAllChatsOfUserAsync_Should_Throw_Unauthorized_Access_Not_Admin_Not_Same_User()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 1,
			IsAdmin = false,
			ChatId = 10
		};

		// Act & Assert
		var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
			() => _service.GetAllChatsOfUserAsync(request));

		Assert.Equal("Permission denied: unauthorized call of get all user's chats", exception.Message);
	}

	[Fact]
	public async Task GetAllChatsOfUserAsync_Should_Throw_Key_Not_Found_Non_Existant_User()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 0,
			IsAdmin = false,
			ChatId = 10
		};

		// Setup mock for user exists == false
		_mockUserRepo
			.Setup(repo => repo.UserExists(request.RequestorId.Value))
			.ReturnsAsync(false);

		var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
			() => _service.GetAllChatsOfUserAsync(request));

		Assert.Equal($"User with id {request.RequestorId} doesn't exist", exception.Message);

	}

	[Fact]
	public async Task GetAllChatsOfUserAsync_Should_Throw_Argument_Exception_User_No_Chats()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 0,
			IsAdmin = false,
			ChatId = 10
		};

		// Setup mock for user exists == true
		_mockUserRepo
			.Setup(repo => repo.UserExists(request.RequestorId.Value))
			.ReturnsAsync(true);

		// Setup mock for GetAllChatsOfUser == empty list
		_mockUserChatRepo
			.Setup(repo => repo.GetAllChatsOfUserAsync(request.RequestorId.Value))
			.ReturnsAsync(new List<Chat>());

		// Act
		var exception = await Assert.ThrowsAsync<ArgumentException>(
			() => _service.GetAllChatsOfUserAsync(request));

		// Assert
		Assert.Equal($"User {request.RequestorId.Value} isn't part of any chats", exception.Message);

	}

	// End of Get all chats region
	#endregion

	// End of GET region
	#endregion




	#region ADD methods service

	[Fact]
	public async Task AddUserToChatAsync_Should_Add_User()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 0,
			IsAdmin = false,
			ChatId = 10
		};

		var user = new User
		{
			Id = 0,
			Nickname = "user0"
		};

		var chat = new Chat
		{
			Id = 10,
			Name = "chat10"
		};

		// Setup mock for user
		_mockUserRepo
			.Setup(repo => repo.GetByIdAsync(request.UserId.Value))
			.ReturnsAsync(user);

		// Setup mock for chat
		_mockChatRepo
			.Setup(repo => repo.GetByIdAsync(request.ChatId.Value))
			.ReturnsAsync(chat);

		// Act
		await _service.AddUserToChatAsync(request);

		// Assert
		_mockUserChatRepo.Verify(repo => repo.AddUserToChatAsync
		(
			It.Is<UserChat>(uc =>
				uc.UserId == request.UserId &&
				uc.ChatId == request.ChatId)
		), Times.Once);

	}

	[Fact]
	public async Task AddUserToChatAsync_Should_Throw_Argument_Exception_Missing_Params()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = null,
			IsAdmin = false,
			ChatId = 10
		};

		// Act 
		var exception = await Assert.ThrowsAsync<ArgumentException>(
			() => _service.AddUserToChatAsync(request));

		// Assert
		Assert.Equal("Missing required data to fetch add a user", exception.Message);
	}

	[Fact]
	public async Task AddUserToChatAsync_Should_Throw_Unauthorized_Access_Exception_Diff_Requestor()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 1,
			IsAdmin = false,
			ChatId = 10
		};

		// Act
		var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
				() => _service.AddUserToChatAsync(request));

		// Assert
		Assert.Equal("Permission denied: unauthorized call of add user to chat", exception.Message);

	}

	[Fact]
	public async Task AddUserToChatAsync_Should_Throw_Key_Not_Found_Exception_Non_Existant_User()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 0,
			IsAdmin = false,
			ChatId = 10
		};

		// Setup mock for GetByIdAsync of user repo
		_mockUserRepo
			.Setup(repo => repo.GetByIdAsync(request.UserId.Value))
			.ReturnsAsync((User?)null);

		// Act
		var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
				() => _service.AddUserToChatAsync(request));

		// Assert
		Assert.Equal($"User with id {request.UserId.Value} doesn't exist", exception.Message);
	}

	[Fact]
	public async Task AddUserToChatAsync_Should_Throw_Key_Not_Found_Exception_Non_Existant_Chat()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 0,
			IsAdmin = false,
			ChatId = 10
		};

		var user = new User
		{
			Id = 0,
			Nickname = "user0"
		};

		// Setup mock for GetByIdAsync of user repo
		_mockUserRepo
			.Setup(repo => repo.GetByIdAsync(request.UserId.Value))
			.ReturnsAsync(user);

		// Setup mock for GetByIdAsync of chat repo

		_mockChatRepo
			.Setup(repo => repo.GetByIdAsync(request.ChatId.Value))
			.ReturnsAsync((Chat?)null);

		// Act
		var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
				() => _service.AddUserToChatAsync(request));

		// Assert
		Assert.Equal($"Chat with id {request.ChatId.Value} doesn't exist", exception.Message);
	}


	// End of ADD region
	#endregion



	#region REMOVE service methods
	[Fact]
	public async Task RemoveUserFromChat_Should_Remove_User_Chat_Same_Requestor()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 0,
			IsAdmin = false,
			ChatId = 10
		};

		UserChat uc = new UserChat
		{
			UserId = request.UserId.Value,
			ChatId = request.ChatId.Value,
			UserRole = UserChatRole.Regular
		};

		// Setup, act, assert
		await PerformRemoveAsync(request, uc);
	}

	[Fact]
	public async Task RemoveUserFromChat_Should_Remove_User_Chat_Moderator()
	{

		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 1,
			IsAdmin = false,
			ChatId = 10
		};

		UserChat uc = new UserChat
		{
			UserId = request.UserId.Value,
			ChatId = request.ChatId.Value,
			UserRole = UserChatRole.Moderator
		};

		await PerformRemoveAsync(request, uc);

	}

	[Fact]
	public async Task RemoveUserFromChat_Should_Throw_Argument_Exception_Missing_Params()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = null,
			IsAdmin = false,
			ChatId = 10
		};

		// Act
		var exception = await Assert.ThrowsAsync<ArgumentException>(
			() => _service.RemoveUserFromChatAsync(request));

		// Assert
		Assert.Equal("Missing required data to remove user from chat", exception.Message);


	}

	[Fact]
	public async Task RemoveUserFromChat_Should_Throw_Key_Not_Found_Exception_Non_Existant_UC()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 0,
			IsAdmin = false,
			ChatId = 10
		};

		// Setup mock for GetByIdAsync of userChat repo
		_mockUserChatRepo
			.Setup(repo => repo.GetByIdAsync(request.UserId.Value, request.ChatId.Value))
			.ReturnsAsync((UserChat?)null);

		// Act
		var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
				() => _service.RemoveUserFromChatAsync(request));

		// Assert
		Assert.Equal($"User {request.UserId.Value} is not in chat {request.ChatId.Value}: cannot remove", exception.Message);
	}

	[Fact]
	public async Task RemoveUserFromChat_Should_Throw_Unauthorized_Access_Exception()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 1,
			IsAdmin = false,
			ChatId = 10
		};

		var uc = new UserChat
		{
			UserId = request.UserId.Value,
			ChatId = request.ChatId.Value,
			UserRole = UserChatRole.Regular
		};

		// setup mock to return this UC
		_mockUserChatRepo
			.Setup(repo => repo.GetByIdAsync(request.UserId.Value, request.ChatId.Value))
			.ReturnsAsync(uc);

		// Act
		var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
			() => _service.RemoveUserFromChatAsync(request));

		// Assert
		// "Permission denied: unauthorized call of remove user from chat"
		Assert.Equal("Permission denied: unauthorized call of remove user from chat", exception.Message);


	}


	// HELPER METHOD FOR ADD
	private async Task PerformRemoveAsync(UserChatRequest request, UserChat returnUC)
	{
		// Setup mock to return returnUC
#pragma warning disable CS8629 // Nullable value type may be null.
		_mockUserChatRepo
			.Setup(repo => repo.GetByIdAsync(request.UserId.Value, request.ChatId.Value))
			.ReturnsAsync(returnUC);
#pragma warning restore CS8629 // Nullable value type may be null.

		// Act
		await _service.RemoveUserFromChatAsync(request);

		// Assert
#pragma warning disable CS8629 // Nullable value type may be null.
		_mockUserChatRepo.Verify(repo => repo.RemoveUserFromChatAsync(
			request.UserId.Value,
			request.ChatId.Value),
			Times.Once,
			"Expected RemoveUserFromChatAsync to be called once with given params");
#pragma warning restore CS8629 // Nullable value type may be null.
	}

	// ADD region end
	#endregion



	#region UPDATE service methods region
	// [Fact]
	// private async Task UpdateUserChatStatus_Should_Update_User_Chat_Status()
	// {
	// 	// Arrange


	// }

	[Fact]
	private async Task UpdateUserChatStatus_Should_Throw_Argument_Exception_Missing_Property()
	{
		// Arrange: request, role
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = null,
			IsAdmin = false,
			ChatId = 10
		};

		// Act
		var exception = await Assert.ThrowsAsync<ArgumentException>(
			() => _service.UpdateUserChatStatus(request, UserChatRole.Regular));

		// Assert
		Assert.Equal("Error: missing a required property to update user's status in chat", exception.Message);
	}

	[Fact]
	private async Task UpdateUserChatStatus_Should_Throw_Key_Not_Found_Exception_Requestor_Not_In_Chat()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 0,
			IsAdmin = false,
			ChatId = 10
		};

		// Setup mock for getbyid
		_mockUserChatRepo
			.Setup(repo => repo.GetByIdAsync(request.RequestorId.Value, request.ChatId.Value))
			.ReturnsAsync((UserChat?)null);

		// Act
		var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
			() => _service.UpdateUserChatStatus(request, UserChatRole.Regular));

		// Assert
		Assert.Equal($"User {request.RequestorId} is not a part of chat {request.ChatId}", exception.Message);
	}

	[Fact]
	private async Task UpdateUserChatStatus_Should_Throw_Unauthorized_Access_Exception()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 0,
			UserId = 0,
			IsAdmin = false,
			ChatId = 10
		};

		UserChat uc = new UserChat
		{
			UserId = request.UserId.Value,
			ChatId = request.ChatId.Value,
			UserRole = UserChatRole.Regular
		};

		// Setup mock for getbyid
		_mockUserChatRepo
			.Setup(repo => repo.GetByIdAsync(request.RequestorId.Value, request.ChatId.Value))
			.ReturnsAsync(uc);

		// Act
		var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
			() => _service.UpdateUserChatStatus(request, UserChatRole.Regular));

		// Assert
		Assert.Equal("Permission denied: unauthorized call of change user chat status", exception.Message);
	}

	[Fact]
	private async Task UpdateUserChatStatus_Should_Throw_Key_Not_Found_Exception_Updated_User_Not_In_Chat()
	{
		// Arrange
		var request = new UserChatRequest
		{
			RequestorId = 1,
			UserId = 0,
			IsAdmin = true,
			ChatId = 10
		};

		UserChat uc = new UserChat
		{
			UserId = request.UserId.Value,
			ChatId = request.ChatId.Value,
			UserRole = UserChatRole.Regular
		};

		// Setup mock for getbyid for requestor
		_mockUserChatRepo
			.Setup(repo => repo.GetByIdAsync(request.RequestorId.Value, request.ChatId.Value))
			.ReturnsAsync(uc);

		// Setup mock for getbyid for requested user
		_mockUserChatRepo
			.Setup(repo => repo.GetByIdAsync(request.UserId.Value, request.ChatId.Value))
			.ReturnsAsync((UserChat?)null);

		// Act
		var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
			() => _service.UpdateUserChatStatus(request, UserChatRole.Regular));

		// Assert
		Assert.Equal($"User {request.UserId} is not a part of chat {request.ChatId}", exception.Message);
	}



	// UPDATE region end
	#endregion


}
