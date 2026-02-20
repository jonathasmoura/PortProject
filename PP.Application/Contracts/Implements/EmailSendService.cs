using Microsoft.Extensions.Options;
using PP.Application.Contracts.Interfaces;
using PP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts.Implements
{
	public class EmailSendService : IEmailSendService
	{
		private readonly EmailConfigRequestDto _emailConfig;

		public EmailSendService(IOptions<EmailConfigRequestDto> emailConfigOptions)
		{
			_emailConfig = emailConfigOptions?.Value ?? new EmailConfigRequestDto();
		}

		public async Task<bool> SendEmailAsync(EmailRequestDto emailRequestDto)
		{
			try
			{
				using (var client = new SmtpClient(_emailConfig.SmtpServer))
				{
					var mailMessage = new MailMessage()
					{
						From = new MailAddress(_emailConfig.FromAddress, _emailConfig.FromName),
						Subject = emailRequestDto.Subject,
						Body = emailRequestDto.Body,
						IsBodyHtml = true
					};
					mailMessage.To.Add(emailRequestDto.To);


					client.Host = _emailConfig.SmtpServer;
					client.Port = _emailConfig.SmtpPort;
					client.UseDefaultCredentials = false;
					client.Credentials = new System.Net.NetworkCredential(_emailConfig.SmtpUsername, _emailConfig.SmtpPassword);
					client.EnableSsl = true;

					await client.SendMailAsync(mailMessage);

				}
			}
			catch (Exception ex)
			{
				var message = ex.InnerException.Message;
				return false;
			}
			return true;
		}
	}
}
