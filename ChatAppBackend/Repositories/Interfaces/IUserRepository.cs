using System;
using ChatAppBackend.Models;

namespace ChatAppBackend.Repositories.Interfaces;

public interface IUserRepository
{
	// ----------------------- GET METHODS -----------------------
	/// <summary>
	/// Tries to get user by passed ID
	/// </summary>
	/// <param name="id">user's id</param>
	/// <returns>User info from DB</returns>
	/// <exception cref="KeyNotFoundException">User with passed ID isn't in the DB</exception>
	Task<User> GetByIdAsync(int id);

	/// <summary>
	/// Tries to return user with email address from the DB
	/// </summary>
	/// <param name="mail"></param>
	Task<User> GetByMailAddressAsync(string mail);

	/// <summary>
	/// Method gets all users from DB
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<User>> GetAllAsync();


	// ----------------------- ADD METHODS -----------------------
	/// <summary>
	/// Add method
	/// </summary>
	/// <param name="user">User info</param>
	Task AddAsync(User user);


	// ----------------------- UPDATE METHODS -----------------------
	/// <summary>
	/// Update method
	/// </summary>
	/// <param name="user">User to be updated</param>
	Task UpdateAsync(User user);

	// ----------------------- REMOVE METHODS -----------------------
	/// <summary>
	/// Remove method
	/// </summary>
	/// <param name="id">ID of a user to delete</param>
	Task RemoveAsync(int id);


}
