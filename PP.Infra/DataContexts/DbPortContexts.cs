using Microsoft.EntityFrameworkCore;
using PP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Infra.DataContexts
{
	
	public class DbPortContexts : DbContext
	{
		public DbPortContexts(DbContextOptions<DbPortContexts> options)
			: base(options) { }

		//public DbSet<Category> Categories { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

		}
	}
}
