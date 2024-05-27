using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task_Management.Settings
{
    public class JwtSettings
    {
        public required string Key { get; set; }
        public required string RefreshTokenKey { get; set; }
        public required string Issuer { get; set; }
    }
}