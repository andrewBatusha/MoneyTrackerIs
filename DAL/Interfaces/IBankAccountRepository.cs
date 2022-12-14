using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IBankAccountRepository : IRepository<UserBankAccount>
    {
        UserBankAccount GetBankAccount(string userId, string bankName);
        IQueryable<UserBankAccount> GetUserBankAccounts(string userId);
    }
}
