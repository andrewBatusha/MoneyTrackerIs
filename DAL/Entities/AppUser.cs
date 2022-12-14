using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Entities
{
    public class AppUser : IdentityUser
    {
        public virtual ICollection<UserBankAccount> UserBankAccounts { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
    }
}
