using System;
using ChatAppBackend.Data;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using NuGet.Frameworks;

namespace ChatAppBackend.Tests.Repositories;

public class MessageRepositoryTests : BaseRepositoryTests
{
	[Fact]
	public async void GetByIdAsync_Should_Fetch_Correct_Message()
	{
		// Arrange
		var opts = GetInMemoryOptions("GetByIdMsgDB");
		using (var context = new ApplicationDbContext(opts))
		{
			context.Messages.Add(new Message
			{
				Content = "xxx",
				TimeStamp = DateTime.Parse("1.1.2020"),
				ChatId = 0,
				UserId = 0,
			});
			await context.SaveChangesAsync();
		}

		// Act & Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var msg = await context.Messages.FirstOrDefaultAsync(
				m => m.ChatId == 0 && m.UserId == 0
			);

			var repo = new MessageRepository(context);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
			var getMsg = await repo.GetByIdAsync(msg.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

			Assert.NotNull(getMsg);
			Assert.Equal(0, getMsg.ChatId);
			Assert.Equal(0, getMsg.UserId);
		}
	}

	[Fact]
	public async void GetAllByChatId_Should_Fetch_All_Msgs_With_ChatId()
	{
		// Arrange
		var opts = GetInMemoryOptions("GetByChatIdMsgDB");
		using (var context = new ApplicationDbContext(opts))
		{
			context.Messages.AddRange(
				new Message
				{
					Content = "xx",
					UserId = 1,
					ChatId = 1,
				},
				new Message
				{
					Content = "yy",
					UserId = 2,
					ChatId = 1
				}
			);

			await context.SaveChangesAsync();
		}

		// Act & Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new MessageRepository(context);
			var chats = await repo.GetAllByChatIdAsync(1);
			Assert.NotNull(chats);
			Assert.Equal(2, chats.Count());
		}
	}

	[Fact]
	public async void GetAllByChatIdUserId_Should_Fetch_Correct_Msgs()
	{
		// Arrange
		var opts = GetInMemoryOptions("GetByUserAndChatIdMsgDB");
		using (var context = new ApplicationDbContext(opts))
		{
			context.Messages.AddRange(
				new Message
				{
					Content = "xx",
					UserId = 2,
					ChatId = 2
				},
				new Message
				{
					Content = "yy",
					UserId = 2,
					ChatId = 2
				},
				new Message
				{
					Content = "zz",
					UserId = 3,
					ChatId = 2
				}
			);
			await context.SaveChangesAsync();
		}

		// Act & Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new MessageRepository(context);
			var msgs = await repo.GetAllByChatIdUserIdAsync(2, 2);

			Assert.NotNull(msgs);
			Assert.Equal(2, msgs.Count());
		}
	}

	[Fact]
	public async void GetAllByUserIdAsync_Should_Fetch_All_Msgs_With_UserId()
	{
		// Arrange
		var opts = GetInMemoryOptions("GetAllByUserIdMsgDB");
		using (var context = new ApplicationDbContext(opts))
		{
			context.Messages.AddRange(
				new Message
				{
					Content = "xx",
					UserId = 2,
					ChatId = 3
				},
				new Message
				{
					Content = "yy",
					UserId = 2,
					ChatId = 4
				},
				new Message
				{
					Content = "zz",
					UserId = 3,
					ChatId = 5
				}
			);
			await context.SaveChangesAsync();
		}

		// Act & Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new MessageRepository(context);
			var msgs = await repo.GetAllByUserIdAsync(2);

			Assert.NotNull(msgs);
			Assert.Equal(2, msgs.Count());
		}
	}

	[Fact]
	public async void AddAsync_Should_Add_Msg_To_DB()
	{
		// Arrange
		var opts = GetInMemoryOptions("AddMsgDB");
		var msg = new Message
		{
			UserId = 6,
			ChatId = 6,
			Content = "xx"
		};

		// Act
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new MessageRepository(context);
			await repo.AddAsync(msg);
		}

		// Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var addMsg = await context.Messages.FirstOrDefaultAsync(
				m => m.UserId == 6 && m.ChatId == 6
			);
			Assert.NotNull(addMsg);
			Assert.Equal(6, addMsg.ChatId);
			Assert.Equal(6, addMsg.UserId);
		}
	}

	[Fact]
	public async void UpdateAsync_Should_Update_Msg()
	{
		// Arrange
		var opts = GetInMemoryOptions("UpdateAsyncMsgDB");
		using (var context = new ApplicationDbContext(opts))
		{
			context.Messages.Add(
				new Message
				{
					UserId = 7,
					ChatId = 7,
					Content = "xx"
				}
			);

			await context.SaveChangesAsync();
		}

		// Act 
		using (var context = new ApplicationDbContext(opts))
		{
			var msgUpdate = await context.Messages.FirstOrDefaultAsync(
				m => m.UserId == 7 && m.ChatId == 7
			);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
			msgUpdate.Content = "yy";
#pragma warning restore CS8602 // Dereference of a possibly null reference.

			var repo = new MessageRepository(context);
			await repo.UpdateAsync(msgUpdate);
		}

		// Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var updatedMsg = await context.Messages.FirstOrDefaultAsync(
				m => m.UserId == 7 && m.ChatId == 7
			);

			Assert.NotNull(updatedMsg);
			Assert.Equal("yy", updatedMsg.Content);
		}
	}

	[Fact]
	public async void GetUserByMsgIdAsync_Should_Fetch_Correct_User()
	{
		// Arrange
		var opts = GetInMemoryOptions("GetUserFromMsgIdDB");


		// Create actors
		var user = new User
		{
			Nickname = "user0"
		};

		var msg = new Message
		{
			User = user,
			Content = "xx",
			ChatId = 0
		};

		using (var context = new ApplicationDbContext(opts))
		{
			context.Users.Add(user);
			context.Messages.Add(msg);
			await context.SaveChangesAsync();
		}

		// Act & Assert
		User fetchedUser;
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new MessageRepository(context);
			fetchedUser = await repo.GetUserByMsgIdAsync(msg.Id);

			Assert.NotNull(fetchedUser);
			Assert.Equal(user.Nickname, fetchedUser.Nickname);
		}

	}
}
