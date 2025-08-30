using Application.Abstractions.Auth;
using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.Common.Abstractions;
using Application.DTOs.Authentication;
using Application.Helpers.Auth;
using Application.Results;
using Domain.Entities;
using Domain.Enums;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Application.Services.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly IBaseRepository<UserSession> _userSessionRepository;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUserService;

        public AuthenticationService(IBaseRepository<User> userRepository
            , IPasswordHasher passwordHasher
            , IJwtProvider jwtProvider
            , IBaseRepository<UserSession> userSessionRepository
            , IUnitOfWork uow
            , ICurrentUserService currentUserService
            )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _userSessionRepository = userSessionRepository;
            _uow = uow;
            _currentUserService = currentUserService;
        }
        private async Task<Result<string>> CreateRefreshToken(int userId
            ,CancellationToken cancellationToken)
        {
            try
            {
                var token = _jwtProvider.GenerateRefreshToken();
                var userSession = new UserSession()
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    Token = token,
                };
                _userSessionRepository.Add(userSession);
                var result = await _uow.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return Result<string>.Success(token);
                }
                return Result<string>.Failure("Can't create this user session"
                    , ErrorType.Conflict);
            }
            catch (Exception ex)
            {

                return Result<string>.Failure($"an Error happened when saving changes to db: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
        public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request
            , CancellationToken cancellationToken = default)
        {
            
            try
            {
                // validate user credentials and return Result<User>
                var result = await request.ValidateRequest(_userRepository
                    , _passwordHasher);
                if(!result.IsSuccess)
                {
                    return Result<LoginResponse>.Failure(result.ErrorMessage!
                        , result.ErrorType);
                }
                var user = result.Value;
                var token = _jwtProvider.GenerateToken(user);
                var refreshToken = await CreateRefreshToken(user.Id,cancellationToken);
                if(!refreshToken.IsSuccess)
                {
                    return Result<LoginResponse>.Failure(refreshToken.ErrorMessage!
                        , refreshToken.ErrorType);
                }

                var loginResponse = new LoginResponse(token, refreshToken.Value!);
                return Result<LoginResponse>.Success(loginResponse);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"An error happened in the Login method:{ex.Message}");
                // todo  log the error
                return Result<LoginResponse>.Failure("Error in the login method"
                    , ErrorType.InternalServerError);
            }
        }

        public async Task<Result<LoginResponse>> RefreshTokenAsync
            (RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            
            try
            {
                if(string.IsNullOrWhiteSpace(request.refreshToken))
                {
                    return Result<LoginResponse>.Failure("Invalid token"
                        , ErrorType.BadRequest);
                }
                var refreshToken = await _userSessionRepository
                .FindAsync(r => r.Token == request.refreshToken
                , cancellationToken, "User");
                if (refreshToken is null || refreshToken.ExpiresAt < DateTime.UtcNow)
                {
                    return Result<LoginResponse>.Failure("expired refresh Token"
                        , ErrorType.BadRequest);
                }
                var accessToken = _jwtProvider.GenerateToken(refreshToken.User);
                refreshToken.Token = _jwtProvider.GenerateRefreshToken();
                refreshToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
                _userSessionRepository.Update(refreshToken);
                await _uow.SaveChangesAsync(cancellationToken);

                var response = new LoginResponse(accessToken, refreshToken.Token);
                return Result<LoginResponse>.Success(response);

            }
            catch (Exception ex)
            {
                return Result<LoginResponse>.Failure($"Error:{ex.Message}"
                    ,ErrorType.InternalServerError);
            }
        }

        public async Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest request
            ,CancellationToken cancellationToken)
        {
            try
            {
                // to do validate Email is confirmed
                var user = await _userRepository
                    .FindAsync(u => u.Id == _currentUserService.UserId
                    , cancellationToken);
                if (user is null)
                {
                    return Result<string>.NotFound(nameof(user));
                }
                if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.OldPassword))
                {
                    return Result<string>.Failure("Wrong old password", ErrorType.BadRequest);
                }
                var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);
                user.PasswordHash = newPasswordHash;
                user.UpdatedByUserId = user.Id;
                user.UpdatedAt = DateTime.UtcNow;
                _userRepository.Update(user);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result<string>.Success("Password changed successfully");
            }
            catch(Exception ex)
            {
                // to do log error
                return Result<string>.Failure($"Error while resetting the password"
                    , ErrorType.InternalServerError);
            }
        }
    }
}
