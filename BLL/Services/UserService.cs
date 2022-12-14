using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Dto;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DAL.Interfaces;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        readonly IUnitOfWork _dataBase;

        public UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataBase = unitOfWork;
        }

        /// <summary>
        /// Registers new User.
        /// </summary>
        /// <param name="username">user's username.</param>
        /// <param name="password">user's password.</param>
        public async Task<IdentityResult> Register(RegisterModel registerModel)
        {
            var newUser = new AppUser()
            {
                UserName = registerModel.Email,
                Email = registerModel.Email,
            };
            var result = await _userManager.CreateAsync(newUser, registerModel.Password);
            if (!result.Succeeded)
                return result;
            await _userManager.AddToRoleAsync(newUser, "User");
            await _userManager.AddClaimAsync(newUser, new Claim(ClaimTypes.Email, registerModel.Email));
            await _userManager.AddClaimAsync(newUser, new Claim(ClaimTypes.NameIdentifier, newUser.Id));

            //Creating default bank account
            var defaultBankAccount = new UserBankAccount
            {
                AppUserId = newUser.Id,
                Bank = "Other"
            };
            await _dataBase.BankAccountRepository.AddAsync(defaultBankAccount);
            await _dataBase.SaveAsync();
            return result;
        }

        /// <summary>
        /// Generates JWT for user with specified username and password.
        /// </summary>
        /// <param name="email">user's username.</param>
        /// <param name="password">user's password.</param>
        /// <returns>Encoded JWT</returns>
        public async Task<string> GetToken(string email, string password)
        {
            //Getting user identity
            var identity = await GetIdentity(email, password);
            if (identity is null)
                throw new UserException("Failed to Log In. Incorrect username or password");

            //Generating JWT
            var encodedJwt = await GenerateToken(identity);

            return encodedJwt;
        }

        /// <summary>
        /// Gets Identity by username and password.
        /// </summary>
        /// <param name="email">email.</param>
        /// <param name="password">user's password.</param>
        /// <returns><see cref="IdentityUser"/> object of found user or null.</returns>
        private async Task<AppUser> GetIdentity(string email, string password)
        {
            if (email is null || password is null)
                return null;
            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser != null)
            {
                var result = _userManager.PasswordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash, password);
                return result == PasswordVerificationResult.Failed ? null : identityUser;
            }

            return null;
        }


        private async Task<string> GenerateToken(AppUser gsUser)
        {
            var userClaims = await GetValidClaims(gsUser);
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: userClaims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }


        /// <summary>
        /// Get all the user claims including role claims and claims specific for each role of the user.
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>List of all claims of the user.</returns>
        private async Task<List<Claim>> GetValidClaims(AppUser user)
        {
            var claims = new List<Claim>();
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            claims.AddRange(userClaims);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (Claim roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }
            return claims;
        }
    }
}
