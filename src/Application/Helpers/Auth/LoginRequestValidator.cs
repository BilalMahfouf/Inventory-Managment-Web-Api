using Application.Abstractions.Repositories.Base;
using Application.Common.Abstractions;
using Application.DTOs.Authentication;
using Application.Results;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers.Auth
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
