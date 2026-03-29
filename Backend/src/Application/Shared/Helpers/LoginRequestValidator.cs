using Application.Shared.Contracts;
using Application.Authentication.DTOs.Login;
using Domain.Shared.Results;
using Domain.Shared.Entities;
using Domain.Shared.Errors;
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
                return  Result<User>.Failure(Error.Unauthorized("Invalid credentials"));
            }
            var user =await repo.FindAsync(u=>u.Email==request.Email);
            if (user is null)
                return Result<User>.Failure(Error.Unauthorized("Invalid credentials"));

            if (!user.IsActive)
                return Result<User>.Failure(Error.Unauthorized("Account is deactivated"));
            if (!user.EmailConfirmed)
            {
                return Result<User>.Failure(Error.Unauthorized("Email not confirmed"));
            }
           

            // Verify password
            if (!hasher.VerifyPassword(user.PasswordHash,request.Password))
                return Result<User>.Failure(Error.Unauthorized("Invalid credentials"));

            return Result<User>.Success(user);

        }
    }
}
