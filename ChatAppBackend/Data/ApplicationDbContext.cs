using System;
using ChatAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAppBackend.Data;

public class ApplicationDbContext : DbContext
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="dbContextOptions">Configuration settings for EF core, tells which DB provider</param>
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
	: base(options) { }

	// DB tables
	public DbSet<User> Users { get; set; }
	public DbSet<Chat> Chats { get; set; }
	public DbSet<Message> Messages { get; set; }
	public DbSet<Notification> Notifications { get; set; }
	public DbSet<UserChat> UserChats { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Define composite key for UserChat
		modelBuilder.Entity<UserChat>()
			.HasKey(uc => new { uc.UserId, uc.ChatId });

		// Define relationships:
		// User 1 -> N userchat
		modelBuilder.Entity<UserChat>()
			.HasOne(uc => uc.User)
			.WithMany(u => u.UserChats)
			.HasForeignKey(uc => uc.UserId);

		// Chat 1 -> N userchat
		modelBuilder.Entity<UserChat>()
			.HasOne(uc => uc.Chat)
			.WithMany(c => c.UserChats)
			.HasForeignKey(uc => uc.ChatId);
	}

}
