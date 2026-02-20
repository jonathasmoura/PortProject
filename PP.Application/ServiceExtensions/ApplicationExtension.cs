using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PP.Application.Contracts.Implements;
using PP.Application.Contracts.Interfaces;
using PP.Application.DTOs;

namespace PP.Application.ServiceExtensions
{
	public static class ApplicationExtension
	{
		public static IServiceCollection AddDIApplicationServices(this IServiceCollection services, IConfiguration configuration)
		{

			services.Configure<EmailConfigRequestDto>(configuration.GetSection("EmailSettings"));


			services.AddScoped<IUserService, UserService>();
			services.AddScoped<ITokenService, TokenService>();
			services.AddScoped<IEmailSendService, EmailSendService>();
			services.AddScoped<ICategoryService, CategoryService>();


			services.AddHttpContextAccessor();

			return services;
		}
	}
}
