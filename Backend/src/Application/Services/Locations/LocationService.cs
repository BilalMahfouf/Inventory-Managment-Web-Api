using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Inventories;
using Application.DTOs.Locations.Request;
using Application.DTOs.Locations.Response;
using Application.Helpers.Util;
using Application.Results;
using Application.Services.Shared;
using Domain.Abstractions;
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
    public class LocationService : DeleteService<Location>
    {
        private readonly IValidator<LocationCreateRequest> _validator;
        private readonly IValidator<LocationUpdateRequest> _updateValidator;
        public LocationService(IUnitOfWork uow
            , ICurrentUserService currentUserService
            , IValidator<LocationCreateRequest> validator,
              IValidator<LocationUpdateRequest> updateValidator)
            : base(uow.Locations, currentUserService, uow)
        {
            _validator = validator;
            _updateValidator = updateValidator;
        }
        private LocationReadResponse Map(Location location)
        {
            return new LocationReadResponse
            {
                Id = location.Id,
                Name = location.Name,
                Address = location.Address,
                IsActive = location.IsActive,
                LocationTypeId = location.LocationTypeId,
                CreatedAt = location.CreatedAt,
                CreatedByUserId = location.CreatedByUserId,
                CreatedByUserName = location.CreatedByUser?.UserName,
                IsDeleted = location.IsDeleted,
                DeleteAt = location.DeletedAt,
                DeletedByUserId = location.DeletedByUserId,
                DeletedByUserName = location.DeletedByUser?.UserName
            };
        }

        public async Task<Result<IReadOnlyCollection<LocationReadResponse>>>
            GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var locations = await _repository.GetAllAsync(cancellationToken: cancellationToken
                    , includeProperties: "CreatedByUser,DeletedByUser");
                if(locations is null || !locations.Any())
                {
                    return Result<IReadOnlyCollection<LocationReadResponse>>.NotFound(
                        nameof(locations));
                }
                var response = locations.Select(Map).ToList().AsReadOnly();
                return Result<IReadOnlyCollection<LocationReadResponse>>
                    .Success(response);
            }
            catch(Exception ex)
            {
                return Result<IReadOnlyCollection<LocationReadResponse>>
                    .Exception(nameof(GetAllAsync), ex);
            }
        }

        public async Task<Result<LocationReadResponse>> FindAsync(int id
            , CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return Result<LocationReadResponse>.InvalidId();
            }
            try
            {
                var location = await _repository.FindAsync(e => e.Id == id 
                && !e.IsDeleted
                , cancellationToken
                , includeProperties: "CreatedByUser,DeletedByUser");
                if (location is null)
                {
                    return Result<LocationReadResponse>.NotFound(nameof(location));
                }
                var response = Map(location);
                return Result<LocationReadResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<LocationReadResponse>
                    .Exception(nameof(FindAsync), ex);
            }
        }

        public async Task<Result<LocationReadResponse>> CreateAsync(
            LocationCreateRequest request
            , CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(request
                , cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors
                    .Select(e => e.ErrorMessage));
                return Result<LocationReadResponse>
                    .Failure(errors, ErrorType.BadRequest);
            }
            try
            {
                var location = new Location
                {
                    Name = request.Name,
                    Address = request.Address,
                    IsActive = true,
                    LocationTypeId = request.LocationTypeId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = _currentUserService.UserId,
                    IsDeleted = false
                };
                _uow.Locations.Add(location);
                await _uow.SaveChangesAsync(cancellationToken);
                var response = Map(location);
                return Result<LocationReadResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<LocationReadResponse>
                    .Exception(nameof(CreateAsync), ex);
            }
        }

        public async Task<Result> _UpdateLocationStatus(int id,bool isActive
            ,CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return Result.InvalidId();
            }
            try
            {
                var location = await _repository.FindAsync(e => e.Id == id
                && !e.IsDeleted, cancellationToken);
                if (location is null)
                {
                    return Result.NotFound(nameof(location));
                }
                if(location.IsActive == isActive)
                {
                    string status = isActive ? "active" : "inactive";
                    string errorMessage = $"Location is already {status}";
                    return Result.Failure(errorMessage, ErrorType.Conflict);
                }
                location.IsActive = isActive;
                //location.UpdatedAt = DateTime.UtcNow;
                //location.UpdatedByUserId = _currentUserService.UserId;
                _repository.Update(location);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result.Success;
            }
            catch (Exception ex)
            {
                return Result.Exception(nameof(_UpdateLocationStatus), ex);
            }
        }
        public async Task<Result> ActivateAsync(int id
            , CancellationToken cancellationToken = default)
            => await _UpdateLocationStatus(id, true, cancellationToken);
        public async Task<Result> DeactivateAsync(int id,CancellationToken cancellationToken = default)
            => await _UpdateLocationStatus(id, false, cancellationToken);

        public async Task<Result<LocationReadResponse>> UpdateAsync(int id
            , LocationUpdateRequest request
            , CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return Result<LocationReadResponse>.InvalidId();
            }
            var validationResult = await _updateValidator.ValidateAsync(request
                , cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors
                    .Select(e => e.ErrorMessage));
                return Result<LocationReadResponse>
                    .Failure(errors, ErrorType.BadRequest);
            }
            try
            {
                var location = await _repository.FindAsync(e => e.Id == id
                && !e.IsDeleted, cancellationToken);
                if (location is null)
                {
                    return Result<LocationReadResponse>.NotFound(nameof(location));
                }
                location.Name = request.Name;
                location.Address = request.Address;
                location.LocationTypeId = request.LocationTypeId;
                //location.UpdatedAt = DateTime.UtcNow;
                //location.UpdatedByUserId = _currentUserService.UserId;
                _repository.Update(location);
                await _uow.SaveChangesAsync(cancellationToken);
                var response = Map(location);
                return Result<LocationReadResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<LocationReadResponse>
                    .Exception(nameof(UpdateAsync), ex);
            }
        }

        public async Task<Result<IReadOnlyCollection<InventoryBaseReadResponse>>>
            GetLocationInventoriesAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return Result<IReadOnlyCollection<InventoryBaseReadResponse>>.InvalidId();
            }
            try
            {
                var inventories = await _uow.Inventories
                    .GetAllAsync(e => e.LocationId == id
                    , cancellationToken
                    , includeProperties: "Product,Location");
                if (inventories is null || !inventories.Any())
                {
                    return Result<IReadOnlyCollection<InventoryBaseReadResponse>>
                        .NotFound(nameof(inventories));
                }
                var response = inventories.Select(i => Utility.Map(i))
                    .ToList().AsReadOnly();
                return Result<IReadOnlyCollection<InventoryBaseReadResponse>>
                    .Success(response);
            }
            catch(Exception ex)
            {
                return Result<IReadOnlyCollection<InventoryBaseReadResponse>>
                    .Exception(nameof(GetLocationInventoriesAsync), ex);
            }
        }

        public async Task<Result<IEnumerable<object>>> GetLocationsNamesAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var locations = await _repository.GetAllAsync(
                    e => !e.IsDeleted
                    , cancellationToken: cancellationToken);
                if (locations is null || !locations.Any())
                {
                    return Result<IEnumerable<object>>.NotFound(nameof(locations));
                }
                var response = locations.Select(l => new
                {
                    l.Id,
                    l.Name
                }).ToList().AsReadOnly();
                return Result<IEnumerable<object>>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<object>>
                    .Exception(nameof(GetLocationsNamesAsync), ex);
            }
        }

    }
}
