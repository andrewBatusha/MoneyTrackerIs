using BLL.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICategoryService
    {
        Task AddCategoryAsync(CategoryDto category);
        IEnumerable<CategoryDto> GetCategories(string userId);
        Task RenameCategoryAsync(int categoryId, string userId, string newName);
        Task DeleteCategoryAsync(int categoryId, string userId);
    }
}
