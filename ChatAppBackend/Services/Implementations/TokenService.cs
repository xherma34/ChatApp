using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatAppBackend.DTOs;
using ChatAppBackend.POCO;
using ChatAppBackend.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ChatAppBackend.Services.Implementations;

public class TokenService : ITokenService
{

	// Get audience, listener and secret key from config
	private readonly string _secretKey;
	private readonly string _audience;
	private readonly string _issuer;

	public TokenService(IOptions<JwtOptions> jwtOptions)
	{
		var options = jwtOptions.Value;
		_secretKey = options.SecretKey;
		_audience = options.Audience;
		_issuer = options.Issuer;
	}

	public string GenerateJwtToken(UserDto userDto)
	{

		// Check for null for nullable properties to surpress a warning (redundant.. done in authService)
		if (string.IsNullOrEmpty(userDto.MailAddress))
			throw new ArgumentException("Missing property");

		// create claims
		var claims = new List<Claim>
		{
			// Id claim
			new Claim(JwtRegisteredClaimNames.Sub, userDto.Id.ToString()),

			// Role claim
			new Claim(ClaimTypes.Role, userDto.Role.ToString()),

			// Email claim
			new Claim(JwtRegisteredClaimNames.Email, userDto.MailAddress),

			// Nickname claim
			new Claim("Nickname", userDto.Nickname)

		};

		// Create security key and signing credentials
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		// Define expiration date
		var expires = DateTime.UtcNow.AddHours(2);

		// create the jwt token object
		var token = new JwtSecurityToken
		(
			issuer: _issuer,
			audience: _audience,
			claims: claims,
			expires: expires,
			signingCredentials: creds
		);


		// Write and return as string
		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	// TODO: Implement
	public Task<string> GenerateRefreshJwtToken(UserDto userDto)
	{
		throw new NotImplementedException();
	}


}
