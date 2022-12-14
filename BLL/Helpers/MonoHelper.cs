using BLL.Dto;
using BLL.Dto.BankSpecificExpenseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class MonoHelper : IBankHelper
    {
        private HttpClient httpBankClient { get; set; }

        //TODO: Probably better to move to ctor.
        public void InitializeClient()
        {
            httpBankClient = new HttpClient();
            httpBankClient.BaseAddress = new Uri("https://api.monobank.ua/personal/statement/"); //TODO: move to config file
            httpBankClient.DefaultRequestHeaders.Accept.Clear();
            httpBankClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Gets recent expenses made with account with specified token.
        /// </summary>
        /// <param name="token">token for the bank account</param>
        /// <param name="from">optional start date</param>
        /// <returns>List of most recent expences or null if account is invalid</returns>
        public async Task<IEnumerable<MonoApiExpense>> GetRecentExpensesAsync(string token, DateTime? from)
        {
            httpBankClient.DefaultRequestHeaders.Add("X-Token", token);
            DateTime monthAgo = from ?? DateTime.Now.AddMonths(-1);
            long unixTime = ((DateTimeOffset)monthAgo).ToUnixTimeSeconds();
            HttpResponseMessage response = await httpBankClient.GetAsync($"0/{unixTime}");
            IEnumerable<MonoApiExpense> expenses = new List<MonoApiExpense>();

            if (response.IsSuccessStatusCode)
            {
                expenses = await response.Content.ReadAsAsync<List<MonoApiExpense>>();
                return expenses.Where(e => e.Amount < 0);
            }
            return null;
        }
    }
}
