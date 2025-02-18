using System;
using ChatAppBackend.Data;
using ChatAppBackend.Enums;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace ChatAppBackend.Tests.Repositories;

public class UserChatRepositoryTests : BaseRepositoryTests
{

	[Fact]
	public async void AddUserToChatAsync_Should_Add_User_To_Chat()
	{
		var opts = GetInMemoryOptions("AddUCDB");

		// Arrange
		using (var context = new ApplicationDbContext(opts))
		{
			// Create user & chat to be able to create userchat when ACTing
			context.Users.Add(new User { Nickname = "user0" });
			context.Chats.Add(new Chat { Name = "chat0" });
			await context.SaveChangesAsync();
		}

		int userId;
		int chatId;

		// Act
		using (var context = new ApplicationDbContext(opts))
		{
			// Get id's of the chat and user
			var user = await context.Users.FirstOrDefaultAsync(
				u => u.Nickname == "user0"
			);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
			userId = user.Id;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

			var chat = await context.Chats.FirstOrDefaultAsync(
				c => c.Name == "chat0"
			);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
			chatId = chat.Id;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

			var repo = new UserChatRepository(context);

			await repo.AddUserToChatAsync(
				new UserChat
				{
					UserId = userId,
					ChatId = chatId,
					UserRole = UserChatRole.Regular
				}
			);
		}

		// Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var fetchedUc = await context.UserChats.FirstOrDefaultAsync(
				uc => uc.UserId == userId && uc.ChatId == chatId
			);

			Assert.NotNull(fetchedUc);
			Assert.Equal(UserChatRole.Regular, fetchedUc.UserRole);
		}

		// Ensure DB cleanup
		using (var context = new ApplicationDbContext(opts))
		{
			await context.Database.EnsureDeletedAsync();
		}

	}

	[Fact]
	public async void RemoveUserFromChatAsync_Should_Remove_User_From_chat()
	{
		var opts = GetInMemoryOptions("RemoveUCDB");
		// Arrange
		int userId;
		int chatId;

		using (var context = new ApplicationDbContext(opts))
		{
			(userId, chatId) = await ArrangeUserChatForTest("user0", "chat0", context, UserChatRole.Regular);
		}



		// Act & Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new UserChatRepository(context);
			await repo.RemoveUserFromChatAsync(userId, chatId);
		}

		using (var context = new ApplicationDbContext(opts))
		{
			var removedUc = await context.UserChats.FirstOrDefaultAsync(
				uc => uc.UserId == userId && uc.ChatId == chatId
			);

			Assert.Null(removedUc);
		}

		// Ensure DB cleanup
		using (var context = new ApplicationDbContext(opts))
		{
			await context.Database.EnsureDeletedAsync();
		}
	}

	[Fact]
	public async void UpdateUserChatRoleAsync_Should_Update_Status()
	{
		var opts = GetInMemoryOptions("UpdateUCDB");

		// Arrange
		int userId;
		int chatId;

		using (var context = new ApplicationDbContext(opts))
		{
			(userId, chatId) = await ArrangeUserChatForTest("user0", "chat0", context, UserChatRole.Regular);
		}

		// Act & Assert
		using (var context = new ApplicationDbContext(opts))
		{

			// ACT
			var uc = await context.UserChats.FirstOrDefaultAsync(
				uc => uc.UserId == userId && uc.ChatId == chatId
			);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
			uc.UserRole = UserChatRole.Moderator;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			var repo = new UserChatRepository(context);
			await repo.UpdateUserChatRoleAsync(uc);

			// Assert
			var fetchedUc = await context.UserChats.FirstOrDefaultAsync(
				uc => uc.UserId == userId && uc.ChatId == chatId
			);

			Assert.NotNull(fetchedUc);
			Assert.Equal(UserChatRole.Moderator, fetchedUc.UserRole);
		}

		// Ensure DB cleanup
		using (var context = new ApplicationDbContext(opts))
		{
			await context.Database.EnsureDeletedAsync();
		}
	}

	[Fact]
	public async void GetByIdAsync_Should_Fetch_UserChat_From_DB()
	{
		var opts = GetInMemoryOptions("GetByIdUCDB");

		int userId;
		int chatId;

		// Arrange
		using (var context = new ApplicationDbContext(opts))
		{
			(userId, chatId) = await ArrangeUserChatForTest("user1", "chat1", context, UserChatRole.Regular);
		}

		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new UserChatRepository(context);
			var uc = repo.GetByIdAsync(userId, chatId);

			Assert.NotNull(uc);
		}

		// Ensure DB cleanup
		using (var context = new ApplicationDbContext(opts))
		{
			await context.Database.EnsureDeletedAsync();
		}

	}

	[Fact]
	public async void GetAllUsersInChatAsync_Should_Fetch_All_Correct_Users()
	{
		var opts = GetInMemoryOptions("GetAllUsersInUCDB");

		int userId;
		int chatId;

		// Arrange
		using (var context = new ApplicationDbContext(opts))
		{
			(userId, chatId) = await ArrangeUserChatForTest("user1", "chat1", context, UserChatRole.Moderator);

			// Add one more user
			context.Users.Add(new User
			{
				Nickname = "user2"
			});
			await context.SaveChangesAsync();

			// Get the second user's id
			var user2 = await context.Users.FirstOrDefaultAsync(
				u => u.Nickname == "user2"
			);


			// Create new userchat
#pragma warning disable CS8602 // Dereference of a possibly null reference.
			context.UserChats.Add(new UserChat
			{
				UserId = user2.Id,
				ChatId = chatId
			});
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			await context.SaveChangesAsync();

		}

		// Act & Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new UserChatRepository(context);
			var users = await repo.GetAllUsersInChatAsync(chatId);

			// Assert single
			Assert.NotNull(users);
			Assert.Equal(2, users.Count());
		}

	}

	[Fact]
	public async void GetAllChatsOfUserAsync_Should_Fetch_All_Correct_Chats()
	{
		var opts = GetInMemoryOptions("GetAllChatsFromUserUCDB");

		int userId;
		int chatId;

		// Arrange
		using (var context = new ApplicationDbContext(opts))
		{
			(userId, chatId) = await ArrangeUserChatForTest("user1", "chat1", context, UserChatRole.Moderator);

			// Add one more Chat
			context.Chats.Add(new Chat
			{
				Name = "chat2"
			});
			await context.SaveChangesAsync();

			// Get the second chat's id
			var chat2 = await context.Chats.FirstOrDefaultAsync(
				c => c.Name == "chat2"
			);

			// Create new userchat
#pragma warning disable CS8602 // Dereference of a possibly null reference.
			context.UserChats.Add(new UserChat
			{
				UserId = userId,
				ChatId = chat2.Id
			});
#pragma warning restore CS8602 // Dereference of a possibly null reference.
			await context.SaveChangesAsync();
		}

		// Act & Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new UserChatRepository(context);
			var chats = await repo.GetAllChatsOfUserAsync(userId);

			// Assert single
			Assert.NotNull(chats);
			Assert.Equal(2, chats.Count());
		}

	}


	/// <summary>
	/// Method creates User, Chat and UserChat within the context passed
	/// </summary>
	/// <param name="userNickname">Nickname of user to be created</param>
	/// <param name="chatName">Name of chat to be created</param>
	/// <param name="context">DB context</param>
	/// <param name="regular">Role of user within the chat</param>
	/// <returns></returns>
	private async Task<(int userId, int chatId)> ArrangeUserChatForTest(string userNickname,
		string chatName,
		ApplicationDbContext context,
		UserChatRole role)
	{
		context.Users.Add(
			new User
			{
				Nickname = userNickname
			}
		);
		context.Chats.Add(
			new Chat
			{
				Name = chatName
			}
		);
		await context.SaveChangesAsync();

		var user = await context.Users.FirstOrDefaultAsync(
			u => u.Nickname == userNickname
		);
		var chat = await context.Chats.FirstOrDefaultAsync(
			c => c.Name == chatName
		);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
		context.UserChats.Add(
			new UserChat
			{
				UserId = user.Id,
				ChatId = chat.Id,
				UserRole = role
			}
		);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

		await context.SaveChangesAsync();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
		return (user.Id, chat.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
		// var user = context.Users
	}
}
