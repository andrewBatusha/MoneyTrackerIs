using BLL.Dto;
using BLL.Dto.BankSpecificExpenseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    //TODO: Remove this abstraction. Every bank helper will be too unique to be generalized as IBankHelper
    public interface IBankHelper
    {
        Task<IEnumerable<MonoApiExpense>> GetRecentExpensesAsync(string account, DateTime? from);
    }
}
