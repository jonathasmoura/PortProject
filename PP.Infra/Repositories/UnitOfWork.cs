using PP.Domain.Interfaces;
using PP.Infra.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Infra.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly DbPortContexts _dbPortContext;

		public IUserRepository Users { get; }

		public UnitOfWork(IUserRepository userRepository, DbPortContexts dbPortContext)
		{
			_dbPortContext = dbPortContext;
			Users = userRepository;
			_dbPortContext = dbPortContext;
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _dbPortContext.SaveChangesAsync();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_dbPortContext.Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
