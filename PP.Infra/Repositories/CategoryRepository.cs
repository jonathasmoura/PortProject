
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
	public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
	{
		public CategoryRepository(DbPortContexts dbPortContext) : base(dbPortContext)
		{
		}
	}
}
