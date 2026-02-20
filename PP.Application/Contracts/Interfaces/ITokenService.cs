using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts.Interfaces
{
	public interface ITokenService
	{
		string GenerateAccessToken(string username, string[] roles);
		Task<string> GenerateRefreshTokenAsync(string username, string createdByIp);
		Task<(string? accessToken, string? refreshToken, string? error)> RefreshAsync(string refreshToken, string ipAddress);
		bool ValidateAccessToken(string token, out ClaimsPrincipal? principal);
	}
}
