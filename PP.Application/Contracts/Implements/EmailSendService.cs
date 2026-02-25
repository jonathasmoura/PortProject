using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using PP.Application.Contracts.Interfaces;
using PP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
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
				
				var msg = new MimeMessage();
				var bb = new BodyBuilder();

				bb.TextBody = "This is a test email.";
				bb.HtmlBody = emailRequestDto.Body;

				msg.From.Add(new MailboxAddress(_emailConfig.FromName, _emailConfig.FromAddress));
				msg.To.Add(new MailboxAddress("User", emailRequestDto.To));
				msg.Subject = emailRequestDto.Subject;
				msg.Body = bb.ToMessageBody();



				using (var smtp = new SmtpClient())
				{
					await smtp.ConnectAsync(_emailConfig.SmtpServer, 
											_emailConfig.SmtpPort,
											_emailConfig.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

					await smtp.SendAsync(msg);
					await smtp.DisconnectAsync(true);
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
