using Application.Shared.Contracts;
using Application.Authentication.DTOs.Login;
using Domain.Shared.Results;
using Domain.Shared.Entities;
using Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared.Helpers
{
    public static class LoginRequestValidator
    {
        public static async Task< Result<User>> ValidateRequest(this LoginRequest request
            ,IBaseRepository<User> repo,IPasswordHasher hasher)
        {
            if(request is null || string.IsNullOrWhiteSpace(request.Email) 
                || string.IsNullOrWhiteSpace(request.Password) )
            {
                return  Result<User>.Failure("Invalid credentials"
                    , ErrorType.Unauthorized);
            }
            var user =await repo.FindAsync(u=>u.Email==request.Email);
            if (user is null)
                return Result<User>.Failure("Invalid credentials"
                    , ErrorType.Unauthorized);

            if (!user.IsActive)
                return Result<User>.Failure("Account is deactivated"
                    , ErrorType.Unauthorized);
            if (!user.EmailConfirmed)
            {
                return Result<User>.Failure("Email not confirmed"
                    , ErrorType.Unauthorized);
            }
           

            // Verify password
            if (!hasher.VerifyPassword(user.PasswordHash,request.Password))
                return Result<User>.Failure("Invalid credentials"
                    , ErrorType.Unauthorized);

            return Result<User>.Success(user);

        }
    }
}
