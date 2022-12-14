using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// BankAccountRepository Repository
        /// </summary>
        IBankAccountRepository BankAccountRepository { get; }

        /// <summary>
        /// ExpenseRepository Repository
        /// </summary>
        IExpenseRepository ExpenseRepository { get; }

        /// <summary>
        /// CategoryRepository Repository
        /// </summary>
        ICategoryRepository CategoryRepository { get; }


        /// <summary>
        /// Saves all comitted changes to DBSets.
        /// </summary>
        /// <returns><see cref="Task"/> object containing information about number of added Entities.</returns>
        Task<int> SaveAsync();
    }
}
