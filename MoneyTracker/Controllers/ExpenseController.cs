using BLL.Dto;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoneyTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IExpenseService _expenseService;

        public ExpenseController(IUserService userService, IExpenseService expenseService)
        {
            _userService = userService;
            _expenseService = expenseService;
        }

        [HttpGet("/api/Expense/notChecked")]
        [Authorize]
        public async Task<IEnumerable<ExpenseDto>> GetNotCheckedExpenses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notCheckedExpenses = await _expenseService.GetNotChackedExpensesAsync(userId);

            return notCheckedExpenses;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddExpense(ExpenseDto expense)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _expenseService.AddExpense(userId, expense);

            return Ok();
        }

        [HttpGet("/api/Expense/History")]
        [Authorize]
        public IEnumerable<ExpenseDto> GetChechedExpenseHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var expenseHistory = _expenseService.GetExpenseHistory(userId);

            return expenseHistory;
        }

        [HttpGet("/api/Expense/Statistics")]
        [Authorize]
        public IEnumerable<Statistic> GetStatistics()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var statistics = _expenseService.GetStatistics(userId);

            return statistics;
        }

        [HttpDelete("/api/Expense/{expenseId}")]
        [Authorize]
        public async Task<IActionResult> AddExpense(int expenseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _expenseService.DeleteExpense(userId, expenseId);

            return Ok();
        }
    }
}
