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
    ///<inheritdoc cref="IBankAccountRepository"/>
    class BankAccountRepository : IBankAccountRepository
    {
        /// <summary>
        /// Context to use.
        /// </summary>
        private readonly AppDbContext _context;

        /// <summary>
        /// Standart constructor.
        /// </summary>
        /// <param name="context">Context to use.</param>
        public BankAccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public UserBankAccount GetBankAccount(string userId, string bankName)
        {
            return GetUserBankAccounts(userId).FirstOrDefault(ba => ba.Bank == bankName);
        }


        public async Task AddAsync(UserBankAccount entity)
        {
            await _context.UserBankAccounts.AddAsync(entity);
        }

        public void Delete(UserBankAccount entity)
        {
            _context.UserBankAccounts.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            UserBankAccount account = await _context.UserBankAccounts.FindAsync(id);
            _context.UserBankAccounts.Remove(account);
        }

        public IQueryable<UserBankAccount> FindAll()
        {
            return _context.UserBankAccounts;
        }


        public async Task<UserBankAccount> GetByIdAsync(int id)
        {
            return await _context.UserBankAccounts.FindAsync(id);
        }

        public IQueryable<UserBankAccount> GetUserBankAccounts(string userId)
        {
            return _context.UserBankAccounts.Where(a => a.AppUserId == userId);
        }

        public void Update(UserBankAccount entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
