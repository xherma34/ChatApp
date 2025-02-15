using System;
using ChatAppBackend.Data;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace ChatAppBackend.Tests.Repositories;

public class ChatRepositoryTests : BaseRepositoryTests
{
	[Fact]
	public async void GetById_Should_Fetch_Chat()
	{
		// Arrange
		var opts = GetInMemoryOptions("GetChayById");
		using (var context = new ApplicationDbContext(opts))
		{
			context.Chats.Add(new Chat { Name = "chat0" });
			await context.SaveChangesAsync();
		}

		// Act & Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var chat = context.Chats.FirstOrDefaultAsync(
				c => c.Name == "chat0"
			);
			var repo = new ChatRepository(context);
			var getChat = await repo.GetByIdAsync(chat.Id);
			Assert.NotNull(getChat);
			Assert.Equal("chat0", getChat.Name);
		}
	}

	[Fact]
	public async void AddAsync_Should_Add_New_Chat()
	{
		// Arrange
		var opts = GetInMemoryOptions("AddChatDb");

		// Act
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new ChatRepository(context);
			await repo.AddAsync(new Chat { Name = "chat1" });
		}

		// Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var chat = await context.Chats.FirstOrDefaultAsync(
				c => c.Name == "chat1"
			);
			Assert.NotNull(chat);
		}
	}

	[Fact]
	public async void GetAllAsync_Should_Fetch_All_Chats()
	{
		// Arrange
		var opts = GetInMemoryOptions("AddChatDb");
		using (var context = new ApplicationDbContext(opts))
		{
			context.Chats.AddRange(
				new Chat { Name = "chat2" },
				new Chat { Name = "chat3" }
			);
			await context.SaveChangesAsync();
		}

		// Act & Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new ChatRepository(context);
			var chats = await repo.GetAllAsync();

			Assert.NotNull(chats);
			Assert.Equal(2, chats.Count());
		}
	}

	[Fact]
	public async void UpdateAsync_Should_Update_Chat()
	{
		// Arrange
		var opts = GetInMemoryOptions("UpdateAsyncChatDb");
		using (var context = new ApplicationDbContext(opts))
		{
			context.Chats.Add(new Chat { Name = "chat4" });
			await context.SaveChangesAsync();
		}

		// Act
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new ChatRepository(context);
			var chat = await context.Chats.FirstOrDefaultAsync(
				c => c.Name == "chat4"
			);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
			chat.Name = "notChat4";
#pragma warning restore CS8602 // Dereference of a possibly null reference.

			await repo.UpdateAsync(chat);
		}

		// Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var chatUpdated = await context.Chats.FirstOrDefaultAsync(
				c => c.Name == "notChat4"
			);

			var chatOld = await context.Chats.FirstOrDefaultAsync(
				c => c.Name == "chat4"
			);

			Assert.Null(chatOld);
			Assert.NotNull(chatUpdated);
		}

	}

	[Fact]
	public async void RemoveAsync_Should_Remove_Correct_Chat()
	{
		// Arrange
		var opts = GetInMemoryOptions("RemoveAsyncChatDb");
		using (var context = new ApplicationDbContext(opts))
		{
			context.Chats.Add(new Chat { Name = "chat5" });
			await context.SaveChangesAsync();
		}

		// Act
		using (var context = new ApplicationDbContext(opts))
		{
			var chat = await context.Chats.FirstOrDefaultAsync(
				c => c.Name == "chat5"
			);
			var repo = new ChatRepository(context);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
			await repo.RemoveAsync(chat.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
		}

		// Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var chat = await context.Chats.FirstOrDefaultAsync(
				c => c.Name == "chat5"
			);
		}
	}

}
