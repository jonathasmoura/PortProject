using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PP.Application.Contracts.Interfaces;
using PP.Domain.Entities;
using PP.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts.Implements
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration _configuration;
		private readonly IRefreshTokenRepository _refreshTokenRepository;

		private readonly string _issuer;
		private readonly string _audience;
		private readonly string _key;
		private readonly int _accessTokenMinutes;
		private readonly int _refreshTokenMinutes;

		public TokenService(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository)
		{
			_configuration = configuration;
			_refreshTokenRepository = refreshTokenRepository;

			_issuer = _configuration["Jwt:Issuer"] ?? "corp";
			_audience = _configuration["Jwt:Audience"] ?? "corp";
			_key = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key required in configuration");

			// Mantidos os tempos solicitados: access = 5 min, refresh = 20 min
			_accessTokenMinutes = int.TryParse(_configuration["Jwt:AccessTokenMinutes"], out var at) ? at : 5;
			_refreshTokenMinutes = int.TryParse(_configuration["Jwt:RefreshTokenMinutes"], out var rt) ? rt : 20;
		}

		public string GenerateAccessToken(string username, string[] roles)
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.Name, username)
			}.Concat(roles.Select(r => new Claim(ClaimTypes.Role, r)));

			var keyBytes = Encoding.UTF8.GetBytes(_key);
			var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _issuer,
				audience: _audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_accessTokenMinutes),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<string> GenerateRefreshTokenAsync(string username, string createdByIp)
		{
			var randomBytes = new byte[64];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomBytes);
			var token = Convert.ToBase64String(randomBytes);

			var refreshToken = new RefreshToken
			{
				Token = token,
				Username = username,
				CreatedAt = DateTime.UtcNow,
				CreatedByIp = createdByIp,
				ExpiresAt = DateTime.UtcNow.AddMinutes(_refreshTokenMinutes)
			};

			await _refreshTokenRepository.AddAsync(refreshToken);
			return token;
		}

		public async Task<(string? accessToken, string? refreshToken, string? error)> RefreshAsync(string refreshToken, string ipAddress)
		{
			var existing = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
			if (existing == null || !existing.IsActive)
			{
				return (null, null, "Invalid or expired refresh token");
			}

			// Revoke existing
			existing.Revoked = true;
			existing.RevokedAt = DateTime.UtcNow;
			existing.RevokedByIp = ipAddress;

			// Rotate: create new refresh token
			var newToken = await GenerateRefreshTokenAsync(existing.Username, ipAddress);
			existing.ReplacedByToken = newToken;

			await _refreshTokenRepository.UpdateAsync(existing);

			// Caller deve popular roles — aqui retornamos sem roles por padrão
			var accessToken = GenerateAccessToken(existing.Username, Array.Empty<string>());

			return (accessToken, newToken, null);
		}

		public bool ValidateAccessToken(string token, out ClaimsPrincipal? principal)
		{
			principal = null;
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_key);

			try
			{
				var parameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = _issuer,
					ValidateAudience = true,
					ValidAudience = _audience,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateLifetime = true,
					ClockSkew = TimeSpan.FromSeconds(30)
				};

				principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
