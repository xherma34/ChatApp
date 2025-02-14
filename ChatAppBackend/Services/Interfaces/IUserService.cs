using System;
using ChatAppBackend.DTOs;
using ChatAppBackend.Models;

namespace ChatAppBackend.Services.Interfaces;

public interface IUserService
{
	/// <summary>
	///	Get user by ID -> Admin || same ID 
	/// </summary>
	Task<UserDto> GetByIdAsync(int userId, int requestorId, bool isAdmin);

	/// <summary>
	/// Get all users -> Admin 
	/// </summary>
	/// <returns></returns>
	Task<IEnumerable<UserDto>> GetAllAsync(bool isAdmin);


	/// <summary>
	/// Add user
	/// </summary>
	/// <param name="user">User data</param>
	Task AddAsync(UserDto user);


	/// <summary>
	/// Delete user -> same id
	/// </summary>
	Task RemoveByIdAsync(int userId, int requestorId);

	/// <summary>
	/// Update user -> same id || admin
	/// </summary>
	Task UpdateAsync(UserDto userDto, int userId, int requestorId, bool isAdmin);

	Task UpdateMailAddressAsync(UserDto userDto, string newEmail, bool isAdmin, int requestorId);

	Task UpdatePasswordAsync(UserDto userDto, string newPassword, bool isAdmin, int requestorId);
	// Task UpdatePasswordAsync(string password, string oldPassword, int userId, bool isAdmin, int requestorId);

	Task UpdateRefreshTokenAsync(string token);
}
