using ChatAppBackend.Data;
using ChatAppBackend.Models;
using ChatAppBackend.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace ChatAppBackend.Tests.Repositories;

public class UserRepositoryTests : BaseRepositoryTests
{


    // private DbContextOptions<ApplicationDbContext> GetInMemoryOptions(string dbName) =>
    //         new DbContextOptionsBuilder<ApplicationDbContext>()
    //             .UseInMemoryDatabase(databaseName: dbName)
    //             .Options;

    [Fact]
    public async void AddAsync_Should_Add_User_To_DB()
    {
        // Arrange
        var opts = GetInMemoryOptions("AddUserDB");
        var user = new User { Nickname = "user0" };

        // Act
        using (var context = new ApplicationDbContext(opts))
        {
            var repo = new UserRepository(context);
            await repo.AddAsync(user);
        }

        // Assert: verify that the user was added by creating a new context
        using (var context = new ApplicationDbContext(opts))
        {
            var addUser = await context.Users.FirstOrDefaultAsync(
                u => u.Nickname == "user0"
            );
            Assert.NotNull(addUser);
            Assert.Equal("user0", addUser.Nickname);
        }
    }

    [Fact]
    public async void GetByIdAsync_Should_Return_Correct_User()
    {
        // Arrange
        var opts = GetInMemoryOptions("GetByIdDB");
        using (var context = new ApplicationDbContext(opts))
        {
            context.Users.Add(new User { Nickname = "user1" });
            await context.SaveChangesAsync();
        }

        // Act & Assert
        using (var context = new ApplicationDbContext(opts))
        {
            var repo = new UserRepository(context);
            var user = await context.Users.FirstOrDefaultAsync(
                u => u.Nickname == "user1"
            );
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var getUser = await repo.GetByIdAsync(user.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            Assert.NotNull(getUser);
            Assert.Equal("user1", getUser.Nickname);
        }
    }

    [Fact]
    public async void GetAllAsync_Should_Return_All_Users()
    {
        // Arrange
        var opts = GetInMemoryOptions("GetAllDB");
        using (var context = new ApplicationDbContext(opts))
        {
            context.Users.AddRange(
                new User { Nickname = "user2" },
                new User { Nickname = "user3" }
            );
            await context.SaveChangesAsync();
        }

        // Act & Arrange
        using (var context = new ApplicationDbContext(opts))
        {
            var repo = new UserRepository(context);
            var getUsers = await repo.GetAllAsync();
            Assert.NotNull(getUsers);
            Assert.Equal(2, getUsers.Count());
        }
    }

    [Fact]
    public async void GetByMailAddressAsync_Should_Return_Correct_User()
    {
        // Arrange
        var opts = GetInMemoryOptions("GetByMailAddressDB");
        using (var context = new ApplicationDbContext(opts))
        {
            context.Users.Add(new User { Nickname = "user4", MailAddress = "user4@email.com" });
            await context.SaveChangesAsync();
        }

        // Act & Assert
        using (var context = new ApplicationDbContext(opts))
        {
            var repo = new UserRepository(context);
            var getUser = await repo.GetByMailAddressAsync("user4@email.com");
            Assert.NotNull(getUser);
            Assert.Equal("user4", getUser.Nickname);
        }
    }

    [Fact]
    public async void UpdateAsync_Should_Update_User()
    {
        // Arrange
        var opts = GetInMemoryOptions("UpdateUserDb");
        using (var context = new ApplicationDbContext(opts))
        {
            context.Users.Add(new User { Nickname = "user5", MailAddress = "user5@email.com" });
            await context.SaveChangesAsync();
        }

        // Act: update user's nickname
        using (var context = new ApplicationDbContext(opts))
        {
            var userUpdated = await context.Users.FirstOrDefaultAsync(
                u => u.MailAddress == "user5@email.com"
            );
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            userUpdated.Nickname = "notUser5";
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            var repo = new UserRepository(context);
            await repo.UpdateAsync(userUpdated);
        }


        // Assert
        using (var context = new ApplicationDbContext(opts))
        {
            var updatedUser = await context.Users.FirstOrDefaultAsync(
                u => u.MailAddress == "user5@email.com"
            );
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Assert.Equal("notUser5", updatedUser.Nickname);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }

    [Fact]
    public async void RemoveAsync_Should_Remove_User()
    {
        // Arrange
        var opts = GetInMemoryOptions("UpdateUserDb");
        using (var context = new ApplicationDbContext(opts))
        {
            context.Users.Add(new User { Nickname = "user6", MailAddress = "user6@email.com" });
            await context.SaveChangesAsync();
        }

        // Act: Remove user
        using (var context = new ApplicationDbContext(opts))
        {
            var user = await context.Users.FirstOrDefaultAsync(
                u => u.MailAddress == "user6@email.com"
            );
            var repo = new UserRepository(context);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            await repo.RemoveAsync(user.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        // Assert
        using (var context = new ApplicationDbContext(opts))
        {
            var user = await context.Users.FirstOrDefaultAsync(
                u => u.MailAddress == "user6@email.com"
            );
            Assert.Null(user);
        }
    }

}







