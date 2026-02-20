using PP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts.Interfaces
{
	public interface IUserService
	{
		Task<RegisterResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto);
		Task<LoginResponseDto> LoginUserAsync(LoginUserDto loginUserDto);
		Task<LoginUserDto?> FindUserByEmail(string email);
		public string GenerateRefreshToken();
		public void WriteRefreshTokenInCookie(string refreshToken);
	}
}
