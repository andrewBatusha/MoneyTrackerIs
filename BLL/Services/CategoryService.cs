using AutoMapper;
using BLL.Dto;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CategoryService : ICategoryService
    {
        readonly IUnitOfWork _dataBase;
        private readonly UserManager<AppUser> _userManager;
        readonly IMapper _mapper;


        public CategoryService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
        {
            _dataBase = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }


        public async Task AddCategoryAsync(CategoryDto category)
        {
            category.Id = 0;
            if (String.IsNullOrEmpty(category.Name))
                throw new ModelException("category name should not be empty");
            //TODO: Add check for non-existing user
            var categoryEntity = _mapper.Map<CategoryDto, Category>(category);
            await _dataBase.CategoryRepository.AddAsync(categoryEntity);
            await _dataBase.SaveAsync();
        }

        public async Task DeleteCategoryAsync(int categoryId, string userId)
        {
            var category = await _dataBase.CategoryRepository.GetByIdAsync(categoryId);
            if (category == null || category.AppUserId != userId)
                throw new ModelException("no such category Id created by the user");
            await _dataBase.CategoryRepository.DeleteByIdAsync(categoryId);
            await _dataBase.SaveAsync();
        }

        public IEnumerable<CategoryDto> GetCategories(string userId)
        {
            var categoryEntities = _dataBase.CategoryRepository.FindAll().Where(c => c.AppUserId == userId).AsEnumerable();
            return _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryDto>>(categoryEntities);
        }

        public async Task RenameCategoryAsync(int categoryId, string userId, string newName)
        {
            var category = await _dataBase.CategoryRepository.GetByIdAsync(categoryId);
            if (category == null || category.AppUserId != userId)
                throw new ModelException("no such category Id created by the user");
            if (String.IsNullOrEmpty(newName))
                throw new ModelException("category name should not be empty");
            category.Name = newName;
            _dataBase.CategoryRepository.Update(category);
            await _dataBase.SaveAsync();
        }
    }
}
