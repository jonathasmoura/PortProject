using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PP.Application.Contracts.Interfaces;
using PP.Application.DTOs;
using PP.Domain.Entities;
using PP.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts.Implements
{
	public class UserService : IUserService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ITokenService _tokenService;
		private readonly IEmailSendService _emailSendService;

		public UserService(IUnitOfWork unitOfWork, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ITokenService tokenService, IEmailSendService emailSendService)
		{
			_unitOfWork = unitOfWork;
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
			_tokenService = tokenService;
			_emailSendService = emailSendService;
		}

		public async Task<LoginUserDto?> FindUserByEmail(string email)
		{
			var user = await _unitOfWork.Users.GetByEmailAsync(email);

			return new LoginUserDto
			{
				Email = user?.Email,
				Password = user?.Password
			};
		}

		public async Task<LoginResponseDto> LoginUserAsync(LoginUserDto loginUserDto)
		{
			var user = await _unitOfWork.Users.GetByEmailAsync(loginUserDto.Email!);
			if (user == null)
			{
				return new LoginResponseDto
				{
					Flag = false,
					Message = "Não foi possível localizar o usuário!"
				};
			}



			bool checkPassword = BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.Password);
			if (!checkPassword)
			{
				return new LoginResponseDto
				{
					Flag = false,
					Message = "Email ou Senha inválidos!"
				};
			}

			// Define role usando ternário a partir de IsAdmin
			var roleString = user.IsAdmin ? "Admin" : "Membro";
			user.Roles = roleString;
			var roles = new[] { roleString };

			// Gera tokens: access (5 min) e refresh (20 min)
			var username = user.Email;
			var accessToken = _tokenService.GenerateAccessToken(username, roles);

			var ip = _httpContextAccessor?.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
			var refreshToken = await _tokenService.GenerateRefreshTokenAsync(username, ip);

			// grava cookie HTTP-only com expiração de 20 minutos
			WriteRefreshTokenInCookie(refreshToken);

			// persiste Role se necessário
			await _unitOfWork.SaveChangesAsync();

			return new LoginResponseDto
			{
				Flag = true,
				Message = "Login realizado com sucesso",
				Token = accessToken
			};
		}

		public string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				var refreshToken = Convert.ToBase64String(randomNumber);
				return refreshToken;
			}
		}

		public void WriteRefreshTokenInCookie(string refreshToken)
		{
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTime.UtcNow.AddMinutes(20) // 20 minutos conforme solicitado
			};
			_httpContextAccessor?.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
		}

		public async Task<RegisterResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto)
		{
			var userByEmail = await _unitOfWork.Users.GetByEmailAsync(registerUserDto.Email);

			if (userByEmail != null)
				return new RegisterResponseDto
				{
					Flag = false,
					Message = "Usuário já existe!"
				};

			// Atribui IsAdmin e Roles conforme DTO (ternário)
			var isAdmin = registerUserDto.IsAdmin;
			var roleString = isAdmin ? "Admin" : "Membro";

			var newUser = new User
			{
				Name = registerUserDto.Name,
				LastName = registerUserDto.LastName,
				Email = registerUserDto.Email,
				Password = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password),
				IsAdmin = isAdmin,
				Roles = roleString
			};

			await _unitOfWork.Users.AddAsync(newUser);
			await _unitOfWork.SaveChangesAsync();
			await _emailSendService.SendEmailAsync(new EmailRequestDto
			{
				To = newUser.Email,
				Subject = "Bem-vindo ao Corp!",
				Body = $"Olá <b>{newUser.Name}</b>, seu registro foi realizado com sucesso!" +
				$"\nSua senha é <b>{registerUserDto.Password}</b>"
			});
			return new RegisterResponseDto
			{
				Flag = true,
				Message = "Usuário registrado com sucesso!"
			};
		}
	}
}
