using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.Common.Abstractions;
using Application.DTOs.Users.Request;
using Application.DTOs.Users.Response;
using Application.FluentValidations.User;
using Application.Results;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserRequestValidatorContainer _validator;
        private readonly IPasswordHasher _hasher;
        public UserService(IBaseRepository<User> userRepository
            , IUnitOfWork uow
            , UserRequestValidatorContainer validator
            , ICurrentUserService currentUserService
            , IPasswordHasher hasher)
        {
            _userRepository = userRepository;
            _uow = uow;
            _validator = validator;
            _currentUserService = currentUserService;
            _hasher = hasher;
        }
        private UserReadResponse Map(User user)
        {
            var response = new UserReadResponse()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FirstName + " " + user.LastName,
                RoleId = user.RoleId,
                RoleName = user.Role.Name,
                IsActive = user.IsActive,
                IsEmailConfirmed = user.EmailConfirmed,
                IsDeleted = user.IsDeleted,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                DeletedAt = user.DeletedAt,
                CreatedByUserId = user.CreatedByUserId,
                CreatedByUserName = user.CreatedByUser?.UserName,

                UpdatedByUserId = user.UpdatedByUserId,
                UpdatedByUserName = user.UpdatedByUser?.UserName,

                DeletedByUserId = user.DeletedByUserId,
                DeletedByUserName = user.DeletedByUser?.UserName,
            };
            return response;
        }

        public async Task<Result> ActivateAsync(int id
            , CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                {
                    return Result.InvalidId();
                }
                var user = await _userRepository.FindAsync(u => u.Id == id
                , cancellationToken);
                if (user is null)
                {
                    return Result.NotFound(nameof(user));
                }
                user.Activate();
                user.UpdatedByUserId = _currentUserService.UserId;
                _userRepository.Update(user);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result.Success;
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error:{ex.Message}"
                    , ErrorType.InternalServerError);
            }
        }

        public async Task<Result<UserReadResponse>> AddAsync(UserCreateRequest request
            , CancellationToken cancellationToken)
        {
            try
            {
                var result = _validator.UserCreateRequestValidator.Validate(request);
                if (!result.IsValid)
                {
                    string errorMessage = string.Join("; ", result.Errors
                        .Select(e => e.ErrorMessage));
                    return Result<UserReadResponse>.Failure(errorMessage, ErrorType.BadRequest);
                }
                var newUser = new User()
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PasswordHash = _hasher.HashPassword(request.Password),
                    RoleId = request.RoleId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = _currentUserService.UserId,
                    IsActive = true,
                    EmailConfirmed = false
                };
                _userRepository.Add(newUser);
                await _uow.SaveChangesAsync(cancellationToken);

                return await FindByIdAsync(newUser.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                return Result<UserReadResponse>.Failure($"Error:{ex.Message}"
                    , ErrorType.InternalServerError);
            }
        }

        public async Task<Result> ChangePasswordAsync(int id
            ,ChangePasswordRequest request
            , CancellationToken cancellationToken)
        {
            try
            {
                var result = _validator.ChangePasswordRequestValidator.Validate(request);
                if (!result.IsValid)
                {
                    var errorMessage = string.Join(";", result.Errors.Select(e => e.ErrorMessage));
                    return Result.Failure(errorMessage,
                        ErrorType.BadRequest);
                }
                var user = await _userRepository.FindAsync
                    (u => u.Id == _currentUserService.UserId, cancellationToken);
                if (user is null)
                {
                    return Result.NotFound(nameof(user));
                }
                if (!_hasher.VerifyPassword(user.PasswordHash, request.OldPassword))
                {
                    return Result.Failure("Wrong old password", ErrorType.BadRequest);
                }
                user.PasswordHash = _hasher.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedByUserId = _currentUserService.UserId;

                _userRepository.Update(user);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result.Success;
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error: {ex.Message}"
                    , ErrorType.InternalServerError);
            }
        }


        public async Task<Result> DeleteAsync(int id
            , CancellationToken cancellationToken)
        {
           if(id <= 0)
            {
                return Result.InvalidId();
            }
           try
            {
                var user = await _userRepository.FindAsync(u => u.Id == id
                , cancellationToken);
                if (user is null)
                {
                    return Result.NotFound(nameof(user));
                }
                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;
                user.DeletedByUserId = _currentUserService.UserId;
                _userRepository.Update(user);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result.Success;
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error: {ex.Message}"
                    , ErrorType.InternalServerError);
            }
        }

        public async Task<Result> DesActivateAsync(int id
            , CancellationToken cancellationToken)
        {
            try
            {
                if (id <= 0)
                {
                    return Result.InvalidId();
                }
                var user = await _userRepository.FindAsync(u => u.Id == id
                , cancellationToken);
                if (user is null)
                {
                    return Result.NotFound(nameof(user));
                }
                user.DesActivate();
                user.UpdatedByUserId = _currentUserService.UserId;
                _userRepository.Update(user);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result.Success;
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error:{ex.Message}"
                    , ErrorType.InternalServerError);
            }
        }

        public async Task<Result<UserReadResponse>> FindByIdAsync(int id
            , CancellationToken cancellationToken)
        {
           if(id  <= 0)
            {
                return Result<UserReadResponse>.InvalidId();
            }
            try
            {
                var user = await _userRepository.FindAsync(u => u.Id == id
                 , cancellationToken, "Role,CreatedByUser,UpdatedByUser,DeletedByUser");
                if (user is null)
                {
                    return Result<UserReadResponse>.NotFound(nameof(user));
                }
                return Result<UserReadResponse>.Success(Map(user));
            }
            catch (Exception ex)
            {
                return Result<UserReadResponse>.Failure($"Error:{ex.Message}"
                    , ErrorType.InternalServerError);
            }

        }

        public async Task<Result<IEnumerable<UserReadResponse>>> GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userRepository.GetAllAsync(null!, cancellationToken
                , "Role,CreatedByUser,UpdatedByUser,DeletedByUser");
                if (users is null || !users.Any())
                {
                    return Result<IEnumerable<UserReadResponse>>.NotFound(nameof(users));
                }
                List<UserReadResponse> result = new List<UserReadResponse>();
                foreach (var user in users)
                {
                    var readUserResponse = Map(user);
                    result.Add(readUserResponse);
                }
                return Result<IEnumerable<UserReadResponse>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<UserReadResponse>>
                    .Failure($"Error:{ex.Message}"
                    , ErrorType.InternalServerError);
            }
        }

        public async Task<Result> UpdateAsync(int id
            ,UserUpdateRequest request, CancellationToken cancellationToken)
        {
            if(id <= 0)
            {
                return Result.InvalidId();
            }
            try
            {
                var result = _validator.UserUpdateRequestValidator.Validate(request);
                if (!result.IsValid)
                {
                    var errorMessage = string.Join(";", result.Errors.Select(e => e.ErrorMessage));
                    return Result.Failure(errorMessage, ErrorType.BadRequest);
                }
                var user = await _userRepository.FindAsync(u => u.Id == id
                , cancellationToken);
                if (user is null)
                {
                    return Result.NotFound(nameof(user));
                }
                user.UserName = request.UserName;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.RoleId = request.RoleId;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedByUserId = _currentUserService.UserId;
                _userRepository.Update(user);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result.Success;
            }
            catch (Exception ex)
            {
                return Result<UserReadResponse>.Failure($"Error:{ex.Message}"
                    , ErrorType.InternalServerError);
            }
        }
    }
}
