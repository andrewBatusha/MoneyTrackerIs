using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    ///<inheritdoc cref="ICategoryRepository"/>
    class CategoryRepository : ICategoryRepository
    {
        /// <summary>
        /// Context to use.
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Standart constructor.
        /// </summary>
        /// <param name="context">Context to use.</param>
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task AddAsync(Category entity)
        {
            await _context.Categories.AddAsync(entity);
        }

        public void Delete(Category entity)
        {
            _context.Categories.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            Category category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
        }

        public IQueryable<Category> FindAll()
        {
            return _context.Categories;
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public void Update(Category entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
