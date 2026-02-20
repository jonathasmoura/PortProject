using PP.Application.Contracts.Interfaces;
using PP.Application.DTOs;
using PP.Domain.Entities;
using PP.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP.Application.Contracts.Implements
{
	public class CategoryService : ICategoryService
	{
		private readonly IUnitOfWork _unitOfWork;

		public CategoryService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
		{
			var allCategories = await _unitOfWork.Categories.GetAllAsync();
			var categoriesDto = new List<CategoryDto>();
			foreach (var item in allCategories)
			{
				CategoryDto objCategoryDto = new CategoryDto();
				objCategoryDto.Id = item.Id;
				objCategoryDto.Name = item.Name;
				objCategoryDto.SubCategory = item.SubCategory;
				objCategoryDto.IsActive = item.IsActive;
				objCategoryDto.DataActivation = $"{item.ActivationDate:dd/MM/yyyy}";

				categoriesDto.Add(objCategoryDto);
			}
			return categoriesDto.Where(x => x.IsActive).OrderBy(x => x.Name);
		}

		public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
		{
			var objCategory = await _unitOfWork.Categories.GetByIdAsync(id);
			var dtoCategory = new CategoryDto()
			{
				Id = objCategory.Id,
				Name = objCategory.Name,
				SubCategory = objCategory.SubCategory,
				IsActive = objCategory.IsActive
			};
			return dtoCategory;

		}

		public async Task<CreateCategoryDto> AddCategoryAsync(CreateCategoryDto category)
		{
			if (category != null)
			{
				Category objCategory = new Category();
				objCategory.Name = category.Name;
				objCategory.SubCategory = category.SubCategory;

				await _unitOfWork.Categories.AddAsync(objCategory);
				await _unitOfWork.SaveChangesAsync();
			}
			return category;
		}
		public async Task<UpdateCategoryDto> UpdateCategoryAsync(UpdateCategoryDto category)
		{
			var categoryObj = await _unitOfWork.Categories.GetByIdAsync(category.Id);
			if (categoryObj != null)
			{
				categoryObj.Name = category.Name;
				categoryObj.SubCategory = category.SubCategory;
				categoryObj.UpdatedAt = DateTime.Now;
			}
			_unitOfWork.Categories.UpdateAsync(categoryObj);
			await _unitOfWork.SaveChangesAsync();
			return category;
		}

		public async Task DeleteCategoryAsync(CategoryDto category)
		{
			var categoryObj = await _unitOfWork.Categories.GetByIdAsync(category.Id);
			if (categoryObj != null)
			{
				categoryObj.InactivationDate = DateTime.Now;
				categoryObj.UpdatedAt = DateTime.Now;
				categoryObj.IsActive = false;
				await _unitOfWork.SaveChangesAsync();
			}
		}
	}
}
