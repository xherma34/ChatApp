using System;

namespace ChatAppBackend.POCO;

public class JwtOptions
{
	public string Audience { get; set; }
	public string Issuer { get; set; }
	public string SecretKey { get; set; }
}
