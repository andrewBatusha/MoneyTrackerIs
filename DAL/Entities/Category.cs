using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public string Name { get; set; }

        public virtual AppUser AppUser { get; set; }

        public virtual ICollection<Expense> Expenses { get; set; }
    }
}
