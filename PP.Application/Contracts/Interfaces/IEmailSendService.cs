using PP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts.Interfaces
{
	public interface IEmailSendService
	{
		Task<bool> SendEmailAsync(EmailRequestDto emailRequestDto);
	}
}
