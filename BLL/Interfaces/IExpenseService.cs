using BLL.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpenseDto>> GetNotChackedExpensesAsync(string userId);
        Task AddExpense(string userId, ExpenseDto expenseDto);
        IEnumerable<ExpenseDto> GetExpenseHistory(string userId);
        IEnumerable<Statistic> GetStatistics(string userId);
        Task DeleteExpense(string userId, int expenseId);
    }
}
