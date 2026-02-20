using PP.Domain.Entities.DomainBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Domain.Interfaces
{
	public interface IGenericRepository<T> where T : EntityBase

	{
		Task<T> GetByIdAsync(Guid id);
		Task<IEnumerable<T>> GetAllAsync();
		Task AddAsync(T entity);
		void DeleteAsync(T entity);
		void UpdateAsync(T entity);
	}
}
