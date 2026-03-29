using Application.Authentication.Contracts;
using Application.Authentication.DTOs;
using Application.Authentication.DTOs.Email;
using Application.Authentication.DTOs.Login;
using Application.Authentication.DTOs.Password;
using Application.Shared.Contracts;
using Application.Shared.DTOs;
using Application.Shared.Helpers;
using Application.Users.Contracts;
using Domain.Shared.Entities;
using Domain.Shared.Errors;
using Domain.Shared.Results;
using Domain.Users;

namespace Application.Authentication.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;
        private readonly IBaseRepository<ConfirmEmailToken> _confirmEmailRepository;

        public AuthenticationService(IBaseRepository<User> userRepository
            , IPasswordHasher passwordHasher
            , IJwtProvider jwtProvider
            , IUserSessionRepository userSessionRepository
            , IUnitOfWork uow
            , ICurrentUserService currentUserService
            , IEmailService emailService
            , IBaseRepository<ConfirmEmailToken> confirmEmailRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _userSessionRepository = userSessionRepository;
            _uow = uow;
            _currentUserService = currentUserService;
            _emailService = emailService;
            _confirmEmailRepository = confirmEmailRepository;
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
                    TokenType=(byte)TokenType.Refresh
                };
                _userSessionRepository.Add(userSession);
                var result = await _uow.SaveChangesAsync(cancellationToken);
                if (result > 0)
                {
                    return Result<string>.Success(token);
                }
                return Result<string>.Failure("Can't create this user session", ErrorType.Conflict);
            }
            catch (Exception ex)
            {

                return Result<string>.Failure($"an Error happened when saving changes to db: {ex.Message}");
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
                    return Result<LoginResponse>.Failure(result.Error.Description!
                        , result.Error.Type);
                }
                var user = result.Value!;
                var token = _jwtProvider.GenerateToken(user);
                var refreshToken = await CreateRefreshToken(user.Id,cancellationToken);
                if(!refreshToken.IsSuccess)
                {
                    return Result<LoginResponse>.Failure(refreshToken.Error.Description!
                        , refreshToken.Error.Type);
                }

                var loginResponse = new LoginResponse(token, refreshToken.Value!);
                return Result<LoginResponse>.Success(loginResponse);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"An error happened in the Login method:{ex.Message}");
                // todo  log the error
                return Result<LoginResponse>.Failure("Error in the login method");
            }
        }

        public async Task<Result<LoginResponse>> RefreshTokenAsync
            (RefreshTokenRequest request, CancellationToken cancellationToken=default)
        {
            
            try
            {
                if(string.IsNullOrWhiteSpace(request.refreshToken))
                {
                    return Result<LoginResponse>.Failure("Invalid token", ErrorType.Validation);
                }
                var refreshToken = await _userSessionRepository
                .FindAsync(r => r.Token == request.refreshToken
                , cancellationToken, "User");
                if (refreshToken is null || refreshToken.ExpiresAt < DateTime.UtcNow)
                {
                    return Result<LoginResponse>.Failure(Error.Validation("expired refresh Token"));
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
                return Result<LoginResponse>.Failure($"Error:{ex.Message}", ErrorType.Failure);
            }
        }

        public async Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest request
            ,CancellationToken cancellationToken=default)
        {
            try
            {
                var user =await _userRepository.FindAsync(u => u.Email == request.Email,
                    cancellationToken, "UserSessions");
                    if (user is null)
                    {
                        return Result<string>.Failure(UserErrors.UserNotFound(request.Email));
                }
                if (!user.UserSessions.Any(u => u.Token == request.Token &&
                u.ExpiresAt > DateTime.UtcNow
                && u.TokenType == (byte)TokenType.ResetPassword))  
                {
                    return Result<string>.Failure(UserErrors.InvalidCredentials);
                }
                var newPasswordHash=_passwordHasher.HashPassword(request.Password);
                user.PasswordHash = newPasswordHash;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedByUserId = user.Id;
                _userRepository.Update(user);

                // delete all the user tokens
                await _userSessionRepository.DeleteAllSessionsByUserIdAsync(user.Id
                    , cancellationToken);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result<string>.Success("Password changed successfully");

            }
            catch(Exception ex)
            {
                // to do log error
                return Result<string>.Failure($"Error while resetting the password: {ex.Message}");
            }
        }

        public async Task<Result<string>> ForgetPasswordAsync(
            ForgetPasswordRequest request,CancellationToken cancellationToken=default)
        {
            try
            {
                var user =await _userRepository.FindAsync(u => u.Email == request.Email
                ,cancellationToken);
                if (user is null)
                {
                    return Result<string>.Failure(UserErrors.UserNotFound(request.Email));
                }
                var token = _jwtProvider.GenerateToken(user);
                var userSession = new UserSession()
                {
                    UserId = user.Id,
                    Token = token,
                    TokenType = (byte)TokenType.ResetPassword,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15)
                };
                _userSessionRepository.Add(userSession);
                await _uow.SaveChangesAsync(cancellationToken);
                var link = Utility.GenerateResponseLink(request.Email, token
                    , request.ClientUri);
                var body = $@"
                            <p>Click here to reset your password:</p>
                            <a href=""{link}"">Reset Password</a>";

                var message = new SendEmailRequest(user.Email, "Reset Password", body);
                await _emailService.SendEmailAsync(message, cancellationToken);
                return Result<string>.Success("Check your email");
            }
            catch(Exception ex)
            {
                return Result<string>.Failure($"Error:{ex.Message}");
            }

        }

        public async Task<Result<string>> ConfirmEmailAsync(ConfirmEmailRequest request
            , CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.FindAsync(u => u.Email == request.Email
               , cancellationToken, "ConfirmEmailTokens");
                if (user is null)
                {
                    return Result<string>.Failure(Error.NotFound(nameof(user)));
                }
                if (user.EmailConfirmed)
                {
                    return Result<string>.Failure("Email Already Confirmed", ErrorType.Validation);
                }
                var confirmEmailToken = user.ConfirmEmailTokens.FirstOrDefault
                     (e => e.UserId == user.Id);
                if (confirmEmailToken is null)
                {
                    return Result<string>.Failure(Error.NotFound(nameof(confirmEmailToken)));
                }

                if (!user.ConfirmEmailTokens.Any(e => e.Token == request.Token
                && (!e.IsLocked) && e.ExpiredAt > DateTime.UtcNow)) 
                {
                    return Result<string>.Failure("Invalid Token");
                }
                user.ConfirmEmail();
                _userRepository.Update(user);

                confirmEmailToken.LockToken();
                _confirmEmailRepository.Update(confirmEmailToken);

                await _uow.SaveChangesAsync(cancellationToken);
                return Result<string>.Success("Email confirmed Successfully");
            }
            catch(Exception ex)
            {
                // log error
                return Result<string>.Failure($"Error:{ex.Message}", ErrorType.Failure);
            }
            
        }
       
        public async Task<Result<string>> SendConfirmEmailAsync(
               SendConfirmEmailRequest request
             , CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.FindAsync(u => u.Email == request.Email
                ,cancellationToken);
                if (user is null)
                {
                    return Result<string>.Failure(Error.NotFound(nameof(user)));
                }
                if (user.EmailConfirmed)
                {
                    return Result<string>.Failure("User already has confirmed email", ErrorType.Validation);
                }
                var token = Utility.GenerateGuid();
                var confirmEmailToken = new ConfirmEmailToken()
                {
                    UserId = user.Id,
                    Token = token,
                    CreatedAt = DateTime.UtcNow,
                    ExpiredAt = DateTime.UtcNow.AddDays(1),
                    IsLocked = false,
                };
                _confirmEmailRepository.Add(confirmEmailToken);
                await _uow.SaveChangesAsync(cancellationToken);
                var link = Utility.GenerateResponseLink(request.Email, token
                    , request.ClientUri);

                var body = $@"
                            <p>Click here to confirm your email:</p>
                            <a href=""{link}"">confirm email</a>";

                var message = new SendEmailRequest(user.Email, "Confirm Email", body);
                await _emailService.SendEmailAsync(message, cancellationToken);
                return Result<string>.Success("Check your email");
            }
            catch(Exception ex)
            {
                // log error 
                return Result<string>.Failure($"Error:{ex.Message}");
            }

            
        }
    }
}
