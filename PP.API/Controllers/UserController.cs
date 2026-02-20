using Microsoft.AspNetCore.Mvc;
using PP.Application.Contracts.Interfaces;
using PP.Application.DTOs;

namespace PP.API.Controllers
{
	[ApiController]
	[Route("v1/[controller]")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
		{
			try
			{
				var loginResponse = await _userService.LoginUserAsync(loginUserDto);
				if (!loginResponse.Flag)
				{
					return Unauthorized(loginResponse.Message);
				}
				return Ok(loginResponse);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
		{
			try
			{
				var registerResponse = await _userService.RegisterUserAsync(registerUserDto);
				if (!registerResponse.Flag)
				{
					return BadRequest(registerResponse.Message);
				}
				return Ok(registerResponse);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		[HttpPost("logout")]
		public IActionResult Logout()
		{
			var refreshToken = Request.Cookies["refreshToken"];
			if (!string.IsNullOrEmpty(refreshToken))
			{
				// Remove o token de atualização do banco de dados
				//RemoveRefreshToken(refreshToken);

				// Limpa o cookie HTTP-only
				Response.Cookies.Delete("refreshToken");
			}

			return NoContent();
		}

		[HttpPost("refresh")]
		public IActionResult Refresh()
		{

			return Ok();
		}

	}
}
