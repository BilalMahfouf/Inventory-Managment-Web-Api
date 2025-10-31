using Application.Abstractions.Queries;
using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Customers;
using Application.Results;
using Application.Services.Shared;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObject;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Customers;

public class CustomerService : DeleteService<Customer>
{
    private readonly IValidator<CustomerCreateRequest> _createValidator;
    private readonly ICustomerQueries _query;
    public CustomerService(
        ICurrentUserService currentUserService,
        IUnitOfWork uow,
        IValidator<CustomerCreateRequest> createValidator,
        ICustomerQueries query) : base(uow.Customers, currentUserService, uow)
    {
        _createValidator = createValidator;
        _query = query;
    }

    public async Task<Result<CustomerReadResponse>> AddAsync(
        CustomerCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var validationResult = await _createValidator
                .ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join(";"
                        , validationResult.Errors.Select(e => e.ErrorMessage));
                return Result<CustomerReadResponse>.Failure(
                    errorMessage,
                    ErrorType.BadRequest);
            }
            var customerCategory = await _uow.CustomerCategories
                .FindAsync(e => e.Id == request.CustomerCategoryId, cancellationToken);
            if (customerCategory is null)
            {
                return Result<CustomerReadResponse>.NotFound(
                    $"CustomerCategory with Id {request.CustomerCategoryId}");
            }

            var address = Address.Create(
                request.Street, request.City, request.State, request.ZipCode);
            var customer = Customer.Create(
                request.Name,
                request.CustomerCategoryId,
                request.Email,
                request.Phone,
                address,
                customerCategory.DefaultCreditLimit,
                customerCategory.DefaultPaymentTerms);
            _uow.Customers.Add(customer);
            await _uow.SaveChangesAsync(cancellationToken);
            return await _query.GetByIdAsync(customer.Id, cancellationToken);
        }
        catch (DomainException ex)
        {
            return Result<CustomerReadResponse>.Failure(
                ex.Message,
                ErrorType.Conflict);
        }
        catch (Exception ex)
        {
            return Result<CustomerReadResponse>.Exception(
                nameof(AddAsync),
                ex);

        }
    }
}
