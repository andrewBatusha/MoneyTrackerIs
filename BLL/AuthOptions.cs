using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL
{
    public class AuthOptions
    {
        public const string ISSUER = "WebApplicationwWebAPI_JWT_demoServer";
        public const string AUDIENCE = "WebApplicationwWebAPI_JWT_demoClient";
        const string KEY = "mysupersecret_secretkey!123"; // key for encrypting(idealy should be injected while deploying)
        public const int LIFETIME = 90; // token ttl - 90 minutes
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }

}
