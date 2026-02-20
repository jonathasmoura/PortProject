using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Domain.Interfaces
{
	public interface IUnitOfWork : IDisposable
	{
		
		IUserRepository Users { get; }
		ICategoryRepository Categories { get; }

		Task<int> SaveChangesAsync();
	}
}
