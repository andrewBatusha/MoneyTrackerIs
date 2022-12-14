using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Dto
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        public int UserBankAccountId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ExpenseIdentInBank { get; set; }
        public DateTime Time { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public int Balance { get; set; }
    }
}
