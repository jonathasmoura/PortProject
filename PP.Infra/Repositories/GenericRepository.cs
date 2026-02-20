
using Microsoft.EntityFrameworkCore;
using PP.Domain.Entities.DomainBase;
using PP.Domain.Interfaces;
using PP.Infra.DataContexts;

namespace PP.Infra.Repositories
{
	public abstract class GenericRepository<T> : IGenericRepository<T> where T : EntityBase
	{
		protected readonly DbPortContexts _dbPortContext;

		protected GenericRepository(DbPortContexts dbPortContext)
		{
			_dbPortContext = dbPortContext;
		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbPortContext.Set<T>().ToListAsync();
		}

		public async Task<T> GetByIdAsync(Guid id)
		{
			return await _dbPortContext.Set<T>().FindAsync(id);
		}

		public async Task AddAsync(T entity)
		{
			await _dbPortContext.Set<T>().AddAsync(entity);
		}

		public void UpdateAsync(T entity)
		{
			_dbPortContext.Set<T>().Update(entity);
		}

		public void DeleteAsync(T entity)
		{
			_dbPortContext.Set<T>().Remove(entity);
		}
	}
}
