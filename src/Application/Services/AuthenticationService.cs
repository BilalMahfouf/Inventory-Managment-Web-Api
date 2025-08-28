using Application.Abstractions.Auth;
using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Services;
using Application.Common.Abstractions;
using Application.DTOs.Authentication;
using Application.Result;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public AuthenticationService(IBaseRepository<User> userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            // todo validate request

            var user =await _userRepository.FindAsync(u=>u.Email==request.Email);
            if (user is null)
            {
                return Result<LoginResponse>.NotFound(nameof(user));
            }
            // todo validate email
            if(!user.EmailConfirmed)
            {
                return Result<LoginResponse>.Failure("Email not confirmed"
                    , ErrorType.BadRequest);
            }
            if(!_passwordHasher.VerifyPassword(request.Password,user.PasswordHash))
            {
                return Result<LoginResponse>.Failure("Wrong password"
                    , ErrorType.BadRequest);
            }
            var token = _jwtProvider.GenerateToken(user);
            var expireAt = _jwtProvider.GetTokenExpiration();
            var loginResponse = new LoginResponse(token, expireAt);
        }
    }
}
