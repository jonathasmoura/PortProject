using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.DTOs
{
	public class LoginResponseDto
	{
		public bool Flag { get; set; }
		public string Message { get; set; } = null!;
		public string Token { get; set; } = string.Empty;

	}
}
