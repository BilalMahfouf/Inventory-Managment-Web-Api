using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Locations.Request;
using Application.DTOs.Locations.Response;
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

namespace Application.Services.Locations
{
    public class LocationTypeService : DeleteService<LocationType>
    {
        private readonly IValidator<LocationTypeCreateRequest> _validator;
        public LocationTypeService(IUnitOfWork uow
            , IValidator<LocationTypeCreateRequest> validator
            , ICurrentUserService curerntUserService) : base(uow.LocationTypes, curerntUserService, uow)
        {
            _validator = validator;
        }

        private LocationTypeReadResponse MapToResponse(LocationType locationType)
        {
            return new LocationTypeReadResponse
            {
                Id = locationType.Id,
                Name = locationType.Name,
                Description = locationType.Description,

                CreatedAt = locationType.CreatedAt,
                CreatedByUserId = locationType.CreatedByUserId,
                CreatedByUserName = locationType.CreatedByUser?.UserName,

                IsDeleted = locationType.IsDeleted,
                DeleteAt = locationType.DeletedAt,
                DeletedByUserId = locationType.DeletedByUserId,
                DeletedByUserName = locationType.DeletedByUser?.UserName
            };
        }

        public async Task<Result<LocationTypeReadResponse>> AddLocationTypeAsync(
            LocationTypeCreateRequest request
            , CancellationToken cancellationToken = default)
        {
            try
            {
                var validationResult = await _validator.ValidateAsync(
                    request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Result<LocationTypeReadResponse>.Failure(errors, ErrorType.BadRequest);
                }
                var locationType = new LocationType()
                {
                    Name = request.Name,
                    Description = request.Description,
                    CreatedByUserId = _currentUserService.UserId,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                _uow.LocationTypes.Add(locationType);
                await _uow.SaveChangesAsync(cancellationToken);

                return await FindAsync(locationType.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                return Result<LocationTypeReadResponse>.Exception
                    (nameof(AddLocationTypeAsync), ex);
            }
        }

        public async Task<Result<IReadOnlyCollection<LocationTypeReadResponse>>> 
            GetAllLocationTypesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var locationTypes = await _uow.LocationTypes
                    .GetAllAsync(lt => !lt.IsDeleted
                    , includeProperties: "CreatedByUser,DeletedByUser"
                    , cancellationToken: cancellationToken);
                var response = locationTypes
                    .Select(MapToResponse)
                    .ToList().AsReadOnly();
                return Result<IReadOnlyCollection<LocationTypeReadResponse>>
                    .Success(response);
            }
            catch (Exception ex)
            {
                return Result<IReadOnlyCollection<LocationTypeReadResponse>>.Exception
                    (nameof(GetAllLocationTypesAsync), ex);
            }
        }

        public async Task<Result<LocationTypeReadResponse>> 
            FindAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return Result<LocationTypeReadResponse>.InvalidId();
                }
                var locationType = await _uow.LocationTypes
                    .FindAsync(lt => lt.Id == id && !lt.IsDeleted
                    , includeProperties: "CreatedByUser,DeletedByUser"
                    , cancellationToken: cancellationToken);
                if (locationType == null)
                {
                    return Result<LocationTypeReadResponse>.NotFound("LocationType");
                }
                var response = MapToResponse(locationType);
                return Result<LocationTypeReadResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<LocationTypeReadResponse>.Exception
                    (nameof(FindAsync), ex);
            }
        }

        
    }
}
