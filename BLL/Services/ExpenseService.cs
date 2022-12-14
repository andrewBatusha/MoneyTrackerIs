using AutoMapper;
using BLL.Dto;
using BLL.Dto.BankSpecificExpenseModels;
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
    public class ExpenseService : IExpenseService
    {
        readonly IUnitOfWork _dataBase;
        private readonly UserManager<AppUser> _userManager;
        readonly MonoHelper _monoHelper;
        readonly PrivatHelper _privatHelper;
        readonly IMapper _mapper;


        public ExpenseService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, MonoHelper monoHelper, PrivatHelper privatHelper, IMapper mapper)
        {
            _dataBase = unitOfWork;
            _userManager = userManager;
            _monoHelper = monoHelper;
            _monoHelper.InitializeClient();
            _privatHelper = privatHelper;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExpenseDto>> GetNotChackedExpensesAsync(string userId)
        {
            var userBankAccounts = _dataBase.BankAccountRepository.GetUserBankAccounts(userId);
            var checkedExpenses = _dataBase.ExpenseRepository.GetRecentUserExpenses(userId).ToList();
            var expenses = new List<ExpenseDto>();
            if (userBankAccounts.SingleOrDefault(uba => uba.Bank == "Mono") != null)
            {
                var monoAccount = userBankAccounts.First(uba => uba.Bank == "Mono");
                var monoApiExpenses = await _monoHelper.GetRecentExpensesAsync(monoAccount.Token, null);
                var monoExpenses = _mapper.Map<IEnumerable<MonoApiExpense>, IEnumerable<ExpenseDto>>(monoApiExpenses);
                var chechedMonoExpensesIds = checkedExpenses.Where(ce => ce.UserBankAccountId == monoAccount.Id).Select(ce => ce.ExpenseIdentInBank).ToList();
                var notCkeckedMonoExpenses = monoExpenses.Where(me => !chechedMonoExpensesIds.Contains(me.ExpenseIdentInBank)).ToList();
                notCkeckedMonoExpenses.ForEach(ncme => ncme.UserBankAccountId = monoAccount.Id);
                expenses.AddRange(notCkeckedMonoExpenses);
            }
            if (userBankAccounts.SingleOrDefault(uba => uba.Bank == "Privat") != null)
            {
                var privatAccount = userBankAccounts.First(uba => uba.Bank == "Privat");
                var privatApiExpenses = await _privatHelper.GetRecentExpensesAsync(privatAccount.MerchantId, privatAccount.Card, privatAccount.Password);
                var privatExpenses = _mapper.Map<IEnumerable<PrivatApiExpense>, IEnumerable<ExpenseDto>>(privatApiExpenses);
                var chechedPrivatExpensesIds = checkedExpenses.Where(ce => ce.UserBankAccountId == privatAccount.Id).Select(ce => ce.ExpenseIdentInBank).ToList();
                var notCkeckedPrivatExpenses = privatExpenses.Where(me => !chechedPrivatExpensesIds.Contains(me.ExpenseIdentInBank)).ToList();
                notCkeckedPrivatExpenses.ForEach(ncme => ncme.UserBankAccountId = privatAccount.Id);
                expenses.AddRange(notCkeckedPrivatExpenses);
            }
            //TODO: Add new bank logic here

            return expenses;
        }

        public async Task AddExpense(string userId, ExpenseDto expenseDto)
        {

            if (expenseDto.UserBankAccountId != default)
            {
                var isAdded = _dataBase.ExpenseRepository.FindAll().Any(e => e.ExpenseIdentInBank == expenseDto.ExpenseIdentInBank
                                                                    && e.UserBankAccountId == expenseDto.UserBankAccountId);
                if (isAdded)
                    throw new ModelException("This expense has already been added");
            }
            else
            {
                var defaultAccount = _dataBase.BankAccountRepository.GetBankAccount(userId, "Other");
                expenseDto.UserBankAccountId = defaultAccount.Id;
            }
            //TODO: Add validation for time
            //TODO: Add validation for case when user with specified UserBankAccountId doest own the catgory
            var expenseEntity = _mapper.Map<ExpenseDto, Expense>(expenseDto);
            await _dataBase.ExpenseRepository.AddAsync(expenseEntity);
            await _dataBase.SaveAsync();
        }

        public IEnumerable<ExpenseDto> GetExpenseHistory(string userId)
        {
            //TODO: Validate userId
            var checkedExpenses = _dataBase.ExpenseRepository.GetUserExpenses(userId).ToList();
            var checkedExpenseDtos = _mapper.Map<IEnumerable<Expense>, IEnumerable<ExpenseDto>>(checkedExpenses);
            return checkedExpenseDtos;
        }

        public IEnumerable<Statistic> GetStatistics(string userId)
        {
            var statistics = new List<Statistic>();
            var checkedExpenses = _dataBase.ExpenseRepository.GetRecentUserExpenses(userId).ToList();
            var timePeriodStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var timePeriodEnd = timePeriodStart.AddMonths(1);
            do
            {
                var statistic = CollectStatistic(checkedExpenses, timePeriodStart, timePeriodEnd);
                statistics.Add(statistic);
                timePeriodEnd = timePeriodStart;
                timePeriodStart = timePeriodStart.AddMonths(-1);
            } while (checkedExpenses.Any(ce => ce.Time < timePeriodEnd));
            return statistics.OrderBy(s => s.SDate).AsEnumerable();
        }

        private Statistic CollectStatistic(IEnumerable<Expense> expenses, DateTime sDate, DateTime eDate)
        {
            var statistic = new Statistic()
            {
                SDate = sDate,
                EDate = eDate,
                Parts = new List<CategoryTotal>()
            };
            var relevantExpenses = expenses.Where(e => e.Time > sDate && e.Time < eDate).ToList();
            var categories = relevantExpenses.Select(re => re.Category).Distinct();
            foreach(var category in categories)
            {
                var categoryTotal = relevantExpenses
                    .Where(re => re.CategoryId == category.Id)
                    .Sum(re => Math.Abs(re.Amount));
                var catPercent = new CategoryTotal
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    Amount = categoryTotal
                };
                statistic.Parts.Add(catPercent);
            }
            return statistic;
        }

        public async Task DeleteExpense(string userId, int expenseId)
        {
            var expense = await _dataBase.ExpenseRepository.GetByIdAsync(expenseId);
            if (expense == null)
                throw new ModelException("no such category Id created by the user");

            var account = await _dataBase.BankAccountRepository.GetByIdAsync(expense.UserBankAccountId);
            if (account == null || account.AppUserId != userId)
                throw new ModelException("no such category Id created by the user");

            _dataBase.ExpenseRepository.Delete(expense);
            await _dataBase.SaveAsync();
        }
    }
}
