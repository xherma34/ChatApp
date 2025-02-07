using System;
using ChatAppBackend.DTOs;
using ChatAppBackend.Models;

namespace ChatAppBackend.Services.Interfaces;

public interface IUserService
{
	/// <summary>
	///	Get user by ID -> Admin || same ID 
	/// </summary>
	Task<UserDto> GetByIdAsync(int id);

	/// <summary>
	/// Get all users -> Admin 
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<UserDto>> GetAllAsync();


	/// <summary>
	/// Add user
	/// </summary>
	/// <param name="user">User data</param>
	Task AddAsync(UserDto user);


	/// <summary>
	/// Delete user -> same id
	/// </summary>
	Task RemoveByIdAsync(int id);

	/// <summary>
	/// Update user -> same id || admin
	/// </summary>
	Task UpdateAsync(UserDto userDto, int userId);

	Task UpdateMailAddressAsync(int userId, string mail, string password);

	Task UpdatePasswordAsync(string password, string oldPassword, int userId);

}
