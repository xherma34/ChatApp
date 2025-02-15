using System;
using ChatAppBackend.Data;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;

namespace ChatAppBackend.Tests.Repositories;

public class NotificationRepositoryTests : BaseRepositoryTests
{

	[Fact]
	public async void GetByIdAsync_Should_Fetch_Notif()
	{
		// Arrange
		var opts = GetInMemoryOptions("GetByIdNotifDB");
		using (var context = new ApplicationDbContext(opts))
		{
			context.Notifications.Add(new Notification { Content = "notification" });
			await context.SaveChangesAsync();
		}

		// Act & assert
		using (var context = new ApplicationDbContext(opts))
		{
			var notif = await context.Notifications.FirstOrDefaultAsync(
				n => n.Content == "notification"
			);

			var repo = new NotificationRepository(context);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
			var fetchedNotif = await repo.GetByIdAsync(notif.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

			Assert.NotNull(fetchedNotif);
			Assert.Equal("notification", fetchedNotif.Content);
		}
	}

	[Fact]
	public async void GetAllByUserIdAsync_Should_Fetch_All_Users_Notifs()
	{
		// Arrange
		var opts = GetInMemoryOptions("GetAllNotifsByUserIdDB");
		using (var context = new ApplicationDbContext(opts))
		{
			context.Notifications.AddRange(
				new Notification
				{
					Content = "notif0",
					UserId = 0
				},
				new Notification
				{
					Content = "notif1",
					UserId = 0
				},
				new Notification
				{
					Content = "notif2",
					UserId = 1
				}
			);
			await context.SaveChangesAsync();
		}

		// Act & Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new NotificationRepository(context);
			var notifs = await repo.GetAllByUserIdAsync(0);

			Assert.NotNull(notifs);
			Assert.Equal(2, notifs.Count());
		}
	}

	[Fact]
	public async void GetAllByTypeUserIdAsync_Should_Fetch_Correct_Notifs()
	{
		// Arrange
		var opts = GetInMemoryOptions("GetAllNotifsByTypeUserIdAsyncDB");
		using (var context = new ApplicationDbContext(opts))
		{
			context.Notifications.AddRange(
				new Notification
				{
					UserId = 0,
					Type = Enums.NotificationType.Message,
					Content = "notif"
				},
				new Notification
				{
					UserId = 1,
					Type = Enums.NotificationType.Invite,
					Content = "notif"
				},
				new Notification
				{
					UserId = 0,
					Type = Enums.NotificationType.Alert,
					Content = "notif"
				},
				new Notification
				{
					UserId = 0,
					Type = Enums.NotificationType.Alert,
					Content = "notif"
				}
			);
			await context.SaveChangesAsync();
		}

		// Act & Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new NotificationRepository(context);
			var msgNotifs = await repo.GetAllByTypeUserIdAsync(0, Enums.NotificationType.Message);
			var alertNotifs = await repo.GetAllByTypeUserIdAsync(0, Enums.NotificationType.Alert);

			Assert.NotNull(msgNotifs);
			Assert.Single(msgNotifs);
			Assert.NotNull(alertNotifs);
			Assert.Equal(2, alertNotifs.Count());
		}
	}

	[Fact]
	public async void GetAllByStatusUserIdAsync_Should_Fetch_Correct_Notifs()
	{
		// Arrange
		var opts = GetInMemoryOptions("GetAllNotifsByStatusUserIdAsyncDB");
		using (var context = new ApplicationDbContext(opts))
		{
			context.Notifications.AddRange(
				new Notification
				{
					UserId = 0,
					Type = Enums.NotificationType.Message,
					Status = Enums.NotificationStatus.read,
					Content = "notif"
				},
				new Notification
				{
					UserId = 1,
					Type = Enums.NotificationType.Invite,
					Status = Enums.NotificationStatus.read,
					Content = "notif"
				},
				new Notification
				{
					UserId = 0,
					Type = Enums.NotificationType.Alert,
					Status = Enums.NotificationStatus.read,
					Content = "notif"
				},
				new Notification
				{
					UserId = 0,
					Type = Enums.NotificationType.Alert,
					Status = Enums.NotificationStatus.read,
					Content = "notif"
				}
			);
			await context.SaveChangesAsync();
		}

		// Act & Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new NotificationRepository(context);
			var readNotifs = await repo.GetAllByStatusUserIdAsync(0, Enums.NotificationStatus.read);

			Assert.NotNull(readNotifs);
			Assert.Equal(3, readNotifs.Count());
		}
	}

	[Fact]
	public async void AddAsync_Should_Add_Notification()
	{
		// Arrange
		var opts = GetInMemoryOptions("AddASyncNotifDB");
		var notif = new Notification { Content = "notification" };

		// Act
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new NotificationRepository(context);
			await repo.AddAsync(notif);
		}

		// Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var addedNotif = await context.Notifications.FirstOrDefaultAsync(
				n => n.Content == "notification"
			);

			Assert.NotNull(addedNotif);
			Assert.Equal("notification", addedNotif.Content);
		}
	}

	[Fact]
	public async void UpdateAsync_Should_Update_Notification()
	{
		// Arrange
		var opts = GetInMemoryOptions("UpdateASyncNotifDB");
		var notif = new Notification { Content = "notification", UserId = 0 };
		using (var context = new ApplicationDbContext(opts))
		{
			context.Notifications.Add(notif);
			await context.SaveChangesAsync();
		}

		// Act
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new NotificationRepository(context);
			notif.Content = "NotNotification";
			await repo.UpdateAsync(notif);
		}

		// Assert
		using (var context = new ApplicationDbContext(opts))
		{
			var updatedNotif = await context.Notifications.FirstOrDefaultAsync(
				n => n.UserId == 0
			);

			Assert.NotNull(updatedNotif);
			Assert.Equal("NotNotification", updatedNotif.Content);
		}

	}

	[Fact]
	public async void RemoveAsync_Should_Remove_Notification()
	{
		// Arrange
		var opts = GetInMemoryOptions("MyInMemoryDbDB");
		using (var context = new ApplicationDbContext(opts))
		{

			context.Notifications.AddRange(
				new Notification { Content = "removeMe", UserId = 0 },
				new Notification { Content = "hovno", UserId = 1 }
			);
			await context.SaveChangesAsync();
		}

		// Act
		using (var context = new ApplicationDbContext(opts))
		{
			var repo = new NotificationRepository(context);
			var removeNotif = await context.Notifications.FirstOrDefaultAsync(
				n => n.UserId == 0
			);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
			await repo.RemoveAsync(removeNotif.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
		}

		using (var context = new ApplicationDbContext(opts))
		{
			var removedNotif = await context.Notifications.FirstOrDefaultAsync(
				n => n.UserId == 0
			);
			Assert.Null(removedNotif);
		}

	}
}
