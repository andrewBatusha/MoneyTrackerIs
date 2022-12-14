using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Interfaces
{
    public interface IExpenseRepository : IRepository<Expense>
    {
        /// <summary>
        /// Gets recorded user expenses of last 30 days.
        /// </summary>
        /// <param name="iserId"></param>
        /// <returns></returns>
        IQueryable<Expense> GetRecentUserExpenses(string userId);

        /// <summary>
        /// Gets all recorded user expenses.
        /// </summary>
        /// <param name="iserId"></param>
        /// <returns></returns>
        IQueryable<Expense> GetUserExpenses(string userId);

        /// <summary>
        /// Gets all expenses with categories.
        /// </summary>
        /// <param name="iserId"></param>
        /// <returns></returns>
        IQueryable<Expense> FindAllWithCategories(string userId);
    }
}
