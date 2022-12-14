using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Dto.BankSpecificExpenseModels
{
    public class PrivatApiExpense
    {
		public string Card { get; set; }
		public string Appcode { get; set; }
		public DateTime TranDateTime { get; set; }
		public double Amount { get; set; }
		public double Cardamount { get; set; }
		public double Rest { get; set; }
		public string Terminal { get; set; }
		public string Description { get; set; }
	}
}
