using PP.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts.Interfaces
{
	public interface ICategoryService
	{
		Task<CreateCategoryDto> AddCategoryAsync(CreateCategoryDto category);
		Task<UpdateCategoryDto> UpdateCategoryAsync(UpdateCategoryDto category);
		Task DeleteCategoryAsync(CategoryDto category);
		Task<CategoryDto?> GetCategoryByIdAsync(Guid id);
		Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
	}
}
