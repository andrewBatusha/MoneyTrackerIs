using BLL.Dto;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        Task<string> GetToken(string email, string password);
        Task<IdentityResult> Register(RegisterModel registerModel);
    }

}
