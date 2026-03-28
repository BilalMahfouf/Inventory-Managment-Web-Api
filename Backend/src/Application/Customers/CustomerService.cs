using Application.Products.Contracts;
using Application.Shared.Contracts;
using Application.Users.Contracts;
using Application.Shared.Contracts;
using Application.Customers.Dtos;
using Application.Customers.Dtos;
using Domain.Shared.Results;
using Application.Shared.Services;
using Domain.Shared.Entities;
using Domain.Shared.Enums;
using Domain.Shared.Exceptions;
using Domain.Shared.ValueObjects;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Customers;

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
    public async Task<Result<int>> UpdateAsync(
        int id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        if(id <= 0)
        {
            return Result<int>.InvalidId();
        }
        try
        {
            var customer = await _uow.Customers
                .FindAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
            if (customer is null)
            {
                return Result<int>.NotFound(nameof(customer)); 
            }
            var address = Address.Create(
                request.Street,
                request.City,
                request.State,
                request.ZipCode);
            customer.Update(
                request.Name,
                request.CustomerCategoryId,
                request.Email,
                request.Phone,
                address,
                request.CreditLimit,
                request.PaymentTerms);
            _uow.Customers.Update(customer);
            await _uow.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(customer.Id);
        }
        catch(DomainException ex)
        {
            return Result<int>.Failure(
                ex.Message,
                ErrorType.Conflict);
        }
        catch (Exception ex)
        {
            return Result<int>.Exception(
                nameof(UpdateAsync),
                ex);
        }
    }
    

}
