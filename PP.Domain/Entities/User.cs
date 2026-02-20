using PP.Domain.Entities.DomainBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Domain.Entities
{
	public class User : EntityBase
	{
		public string Name { get; set; } = null!;
		public string? LastName { get; set; }
		public string Email { get; set; } = null!;
		public string Password { get; set; } = null!;
		public string Roles { get; set; } = null!;
		public bool IsAdmin { get; set; }
	}
}
