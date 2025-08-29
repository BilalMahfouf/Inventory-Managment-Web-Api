using Application.Common.Abstractions;
using Isopoh.Cryptography.Argon2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common
{
    internal class Argon2PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return Argon2.Hash(password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            return Argon2.Verify(hashedPassword,providedPassword);
        }
    }
}
