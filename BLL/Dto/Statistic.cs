using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Dto
{
    public class Statistic
    {
        public DateTime SDate { get; set; }
        public DateTime EDate { get; set; }
        public List<CategoryTotal> Parts { get; set; }
    }

    public class CategoryTotal
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public double Amount { get; set; }
    }
}
