using BLL.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IBankAccountService
    {
        Task AddBankAccountAsync(BankAccountDto account);
    }
}
