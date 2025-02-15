using System;
using ChatAppBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatAppBackend.Tests.Repositories;

public abstract class BaseRepositoryTests
{
	/// <summary>
	/// Creates a new set of options to configure an inmemory AppDbContext
	/// Each test can then use different database name to ensure isolation
	/// </summary>
	/// <param name="dbName">Name of the in-memory DB</param>
	/// <returns>Configured db options</returns>
	protected DbContextOptions<ApplicationDbContext> GetInMemoryOptions(string dbName) =>
			new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: dbName)
				.Options;
}
