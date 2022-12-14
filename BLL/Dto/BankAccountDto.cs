using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Dto
{
    public class BankAccountDto
    {
        public int Id { get; set; }
        public string? AppUserId { get; set; }
        public string? Bank { get; set; }
        public string? MerchantId { get; set; }
        public string? Card { get; set; }
        public string? Password { get; set; }
        public string? Token { get; set; }
    }
}
