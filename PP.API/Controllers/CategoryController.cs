using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PP.Application.Contracts.Interfaces;
using PP.Application.DTOs;

namespace PP.API.Controllers
{
	[ApiController]
	[Route("v1/[controller]")]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryService _categoryService;

		public CategoryController(ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetCategories()
		{
			try
			{
				var categories = await _categoryService.GetAllCategoriesAsync();

				return Ok(categories);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetCategory(Guid id)
		{
			try
			{
				var category = await _categoryService.GetCategoryByIdAsync(id);
				if (category == null)
				{
					return NotFound("Categoria não encontrada");
				}

				return Ok(category);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> AddCategory(CreateCategoryDto categoryToAdd)
		{
			try
			{
				await _categoryService.AddCategoryAsync(categoryToAdd);
				return CreatedAtAction(nameof(AddCategory), categoryToAdd);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto categoryToUpdate)
		{
			try
			{
				if (id != categoryToUpdate.Id)
				{
					return BadRequest("Category ID mismatch");
				}
				var existingCategory = await _categoryService.GetCategoryByIdAsync(id);
				if (existingCategory == null)
				{
					return NotFound("Categoria não encontrada!");
				}

				await _categoryService.UpdateCategoryAsync(categoryToUpdate);
				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCategory(Guid id)
		{
			try
			{
				var category = await _categoryService.GetCategoryByIdAsync(id);
				if (category == null)
				{
					return NotFound("Categoria não encontrada!");
				}
				await _categoryService.DeleteCategoryAsync(category);
				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}
	}
}
