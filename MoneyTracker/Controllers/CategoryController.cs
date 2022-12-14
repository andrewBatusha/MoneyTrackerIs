using BLL.Dto;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoneyTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;

        public CategoryController(IUserService userService, ICategoryService categoryService)
        {
            _userService = userService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public IEnumerable<CategoryDto> GetCategories()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return _categoryService.GetCategories(userId);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryDto category)
        {
            category.Id = 0;
            category.AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _categoryService.AddCategoryAsync(category);

            return Ok(); //TODO: Use CreatedAtAction()
        }

        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _categoryService.DeleteCategoryAsync(categoryId, userId);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> RenameCategory(CategoryDto category)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _categoryService.RenameCategoryAsync(category.Id, userId, category.Name);

            return Ok();
        }

    }
}
