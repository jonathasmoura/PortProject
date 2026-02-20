using Microsoft.EntityFrameworkCore;
using PP.Domain.Entities;
using PP.Domain.Interfaces;
using PP.Infra.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Infra.Repositories
{
	public class UserRepository : GenericRepository<User>, IUserRepository
	{
		public UserRepository(DbPortContexts dbPortContext) : base(dbPortContext)
		{
		}

		public async Task<User> GetByEmailAsync(string email)
		{
			return await _dbPortContext.Users
				.Where(u => u.Email == email).FirstOrDefaultAsync();
		}
	}
}
