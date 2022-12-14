using AutoMapper;
using BLL.Dto;
using BLL.Exceptions;
using BLL.Helpers;
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
    public class BankAccountService : IBankAccountService
    {
        readonly IUnitOfWork _dataBase;
        private readonly UserManager<AppUser> _userManager;
        readonly MonoHelper _monoHelper;
        readonly PrivatHelper _privatHelper;
        readonly IMapper _mapper;


        public BankAccountService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, MonoHelper monoHelper, PrivatHelper privatHelper, IMapper mapper)
        {
            _dataBase = unitOfWork;
            _userManager = userManager;
            _monoHelper = monoHelper;
            _monoHelper.InitializeClient();
            _privatHelper = privatHelper;
            _mapper = mapper;
            //TODO: Create bank aggregation class to avoid injecting all helpers and initializing them in otherservices
        }

        public async Task AddBankAccountAsync(BankAccountDto account)
        {
            switch (account.Bank)
            {
                case "Mono":
                    await ValidateMonoAsync(account);
                    break;
                case "Privat":
                    await ValidatePrivatAsync(account);
                    break;
                case "Other":
                    break;
                default:
                    throw new ModelException("Invalid Bank Name");
            }
            await _dataBase.BankAccountRepository.AddAsync(_mapper.Map<BankAccountDto, UserBankAccount>(account));
            await _dataBase.SaveAsync();
        }

        private async Task ValidateMonoAsync(BankAccountDto account)
        {
            if (account.Token == null)
                throw new ModelException("X-Token sholdn't be null");
            if (await _monoHelper.GetRecentExpensesAsync(account.Token, null) == null)
                throw new ModelException("Invalid Bank Account");
        }

        private async Task ValidatePrivatAsync(BankAccountDto account)
        {
            if (account.MerchantId == null)
                throw new ModelException("MerchantId shold be specified");
            if (account.Card == null)
                throw new ModelException("Card number sholdn't be null");
            if (account.Password == null)
                throw new ModelException("Merchant password sholdn't be null");
            if (await _privatHelper.GetRecentExpensesAsync(account.MerchantId, account.Card, account.Password) == null)
                throw new ModelException("Invalid Bank Account");
        }
    }
}
