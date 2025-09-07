using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Users.Request;
using Application.DTOs.Users.Response;
using Application.Results;
using Application.Services.Shared;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Users
{
    public class UserRoleService : DeleteService<UserRole>
    {
        private readonly IValidator<UserRoleRequest> _validator;

        public UserRoleService(IBaseRepository<UserRole> repo
            , IUnitOfWork uow
            , ICurrentUserService currentUserService,
IValidator<UserRoleRequest> validator)
            : base(repo, currentUserService, uow)
        {
            _validator = validator;
        }
        private UserRoleReadResponse Map(UserRole role)
        {
            var result = new UserRoleReadResponse()
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsDeleted = role.IsDeleted,
                CreatedAt = role.CreatedAt,
                LastUpdatedAt = role.UpdatedAt,
                DeletedAt = role.DeletedAt,

                CreatedByUserId = role.CreatedByUserId,
                CreatedByUserName = role.CreatedByUser?.UserName,

                UpdatedByUserId = role.UpdatedByUserId,
                UpdatedByUserName = role.UpdatedByUser?.UserName,

                DeleteByUserId = role.DeletedByUserId,
                DeleteByUserName = role.DeletedByUser?.UserName,

            };
            return result;
        }


        public async Task<Result<IReadOnlyCollection<UserRoleReadResponse>>>
            GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var userRoles = await _repository.GetAllAsync(null!, cancellationToken);
                if (userRoles is null || !userRoles.Any())
                {
                    return Result<IReadOnlyCollection<UserRoleReadResponse>>
                        .NotFound(nameof(userRoles));
                }
                var result = new List<UserRoleReadResponse>();
                foreach (var userRole in userRoles)
                {
                    result.Add(Map(userRole));
                }
                return Result<IReadOnlyCollection<UserRoleReadResponse>>
                    .Success(result);
            }
            catch (Exception ex)
            {
                return Result<IReadOnlyCollection<UserRoleReadResponse>>
                    .Failure($"Error:{ex.Message}", ErrorType.InternalServerError);
            }
        }

        public async Task<Result<UserRoleReadResponse>> FindAsync(int id
            ,CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return Result<UserRoleReadResponse>.InvalidId();
            }
            try
            {
                var role = await _repository.FindAsync(u => u.Id == id
                , cancellationToken, "CreatedByUser,UpdatedByUser,DeletedByUser");
                if(role is null)
                {
                    return Result<UserRoleReadResponse>.NotFound("user role");
                }
                var result = Map(role);
                return Result<UserRoleReadResponse>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<UserRoleReadResponse>
                    .Failure($"Error:{ex.Message}", ErrorType.InternalServerError);
            }
        }
        public async Task<Result<UserRoleReadResponse>> AddAsync(UserRoleRequest
            request, CancellationToken cancellationToken)
        {
            try
            {
                var result = _validator.Validate(request);
                if (!result.IsValid)
                {
                    var errorMessage = string.Join(";",
                        result.Errors.Select(e => e.ErrorMessage));
                    return Result<UserRoleReadResponse>.Failure(errorMessage
                        , ErrorType.BadRequest);
                }
                var role = new UserRole()
                {
                    Name = request.Name,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = _currentUserService.UserId,
                    IsDeleted = false,
                };
                _repository.Add(role);
                await _uow.SaveChangesAsync(cancellationToken);
                return await FindAsync(role.Id, cancellationToken);

            }
            catch (Exception ex)
            {
                return Result<UserRoleReadResponse>
                    .Failure($"Error:{ex.Message}", ErrorType.InternalServerError);
            }
        }

        public async Task<Result> UpdateAsync(int id,UserRoleRequest request
            , CancellationToken cancellationToken)
        {
            try
            {
                var result = _validator.Validate(request);
                if (!result.IsValid)
                {
                    var errorMessage = string.Join(";",
                        result.Errors.Select(e => e.ErrorMessage));
                    return Result<UserRoleReadResponse>.Failure(errorMessage
                        , ErrorType.BadRequest);
                }
                var role =await _repository.FindAsync(r => r.Id == id
                , cancellationToken);
                if(role is null )
                {
                    return Result.NotFound("user role");
                }
                role.Name= request.Name;
                role.Description= request.Description;
                role.UpdatedAt = DateTime.UtcNow;
                role.UpdatedByUserId=_currentUserService.UserId;
               
                _repository.Update(role);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result.Success;

            }
            catch (Exception ex)
            {
                return Result<UserRoleReadResponse>
                    .Failure($"Error:{ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
