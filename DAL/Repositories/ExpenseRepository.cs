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
    ///<inheritdoc cref="IExpenseRepository"/>
    class ExpenseRepository : IExpenseRepository
    {
        /// <summary>
        /// Context to use.
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Standart constructor.
        /// </summary>
        /// <param name="context">Context to use.</param>
        public ExpenseRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task AddAsync(Expense entity)
        {
            await _context.Expenses.AddAsync(entity);
        }

        public void Delete(Expense entity)
        {
            _context.Expenses.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            Expense expense = await _context.Expenses.FindAsync(id);
            _context.Expenses.Remove(expense);
        }

        public IQueryable<Expense> FindAll()
        {
            return _context.Expenses;
        }

        public IQueryable<Expense> FindAllWithCategories(string userId)
        {
            return _context.Expenses.Include(e => e.Category);
        }

        public async Task<Expense> GetByIdAsync(int id)
        {
            return await _context.Expenses.FindAsync(id);
        }

        public IQueryable<Expense> GetRecentUserExpenses(string userId)
        {
            var userAccountIds = _context.UserBankAccounts.Where(uba => uba.AppUserId == userId).Select(uba => uba.Id);
            var from = DateTime.Now.AddDays(-30);
            var recentExpenses = _context.Expenses
                                        .Include(e => e.Category)
                                        .Where(e => userAccountIds.Contains(e.UserBankAccountId) && e.Time > from);
            return recentExpenses;
        }

        public IQueryable<Expense> GetUserExpenses(string userId)
        {
            var userAccountIds = _context.UserBankAccounts.Where(uba => uba.AppUserId == userId).Select(uba => uba.Id);
            var userExpenses = _context.Expenses
                                        .Include(e => e.Category)
                                        .Where(e => userAccountIds.Contains(e.UserBankAccountId));
            return userExpenses;
        }

        public void Update(Expense entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
