using Application.Shared.Contracts;
using Application.Shared.Services;
using Application.Users.Contracts;
using Domain.Shared.Results;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Customers;

public class CustomerCategoryService : DeleteService<CustomerCategory>
{
    private readonly IValidator<CreateCustomerCategoryRequest> _createValidator;
    private readonly IValidator<UpdateCustomerCategoryCommand> _updateValidator;


    public CustomerCategoryService(
        IUnitOfWork uow,
        IValidator<CreateCustomerCategoryRequest> createValidator,
        IValidator<UpdateCustomerCategoryCommand> updateValidator,
        ICurrentUserService currentUserService)
        : base(uow.CustomerCategories, currentUserService, uow)
    {
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }


    public async Task<Result<IEnumerable<object>>> GetCategoriesNamesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _uow.CustomerCategories
                .GetAllAsync(cancellationToken: cancellationToken);
            var result = categories
                .Select(c => new
                {
                    c.Id,
                    c.Name
                })
                .ToList()
                .AsReadOnly();
            if (result is null || !result.Any())
            {
                return Result<IEnumerable<object>>.NotFound("CustomerCategories");
            }
            return Result<IEnumerable<object>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<object>>.Exception(
                nameof(GetCategoriesNamesAsync),
                nameof(CustomerCategoryService),
                ex);
        }
    }

    public async Task<Result<CreateCustomerCategoryResponse>> CreateCustomerAsync(
        CreateCustomerCategoryRequest command, CancellationToken cancellationToken)
    {
        _createValidator.ValidateAndThrow(command);
        var category = CustomerCategory.Create(
            command.Name,
            command.IsIndividual,
            command.Description);

        _uow.CustomerCategories.Add(category);
        await _uow.SaveChangesAsync(cancellationToken);
        return Result<CreateCustomerCategoryResponse>.Success(
            new CreateCustomerCategoryResponse(category.Id));
    }

    public async Task<Result> UpdateCustomerAsync(
        UpdateCustomerCategoryCommand command, CancellationToken cancellationToken)
    {
        _updateValidator.ValidateAndThrow(command);

        var category = await _uow.CustomerCategories
            .FindAsync(e => e.Id == command.Id, cancellationToken);
        if (category is null)
        {
            return Result.Failure(Error.NotFound(
                $"{nameof(CustomerCategory)}.NotFound",
                "Customer Category is not found"));
        }
        category.Update(
            command.Name,
            command.IsIndividual,
            command.Description);

        _uow.CustomerCategories.Update(category);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }

    public async Task<Result<GetByIdResponse>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var category = await _uow.CustomerCategories
            .FindAsync(c => c.Id == id, cancellationToken);
        if (category is null)
        {
            return Result<GetByIdResponse>
            .Failure(Error.NotFound(
                $"{nameof(CustomerCategory)}.NotFound",
                "Customer Category is not found"));
        }
        var response = new GetByIdResponse(
            category.Id,
            category.Name,
            category.IsIndividual,
            category.Description,
            category.CreatedAt);
        return Result<GetByIdResponse>.Success(response);
    }
    public async Task<Result<IEnumerable<GetByIdResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var categories = await _uow.CustomerCategories
            .GetAllAsync(cancellationToken: cancellationToken);
        var response = categories
            .Select(category => new GetByIdResponse(
                category.Id,
                category.Name,
                category.IsIndividual,
                category.Description,
                category.CreatedAt))
            .ToList()
            .AsReadOnly();
        return Result<IEnumerable<GetByIdResponse>>.Success(response);
    }
}
    public sealed record GetByIdResponse(
        int Id,
        string Name,
        bool IsIndividual,
        string? Description,
        DateTime CreatedOnUtc);
    public sealed record UpdateCustomerCategoryCommand(
           int Id,
           string Name,
           bool IsIndividual,
           string? Description);
    public sealed class UpdateCustomerCategoryValidator : AbstractValidator<UpdateCustomerCategoryCommand>
    {
        public UpdateCustomerCategoryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(x => x.Description)
                .MaximumLength(500);
        }
    }

    public sealed record CreateCustomerCategoryRequest(
    string Name,
    bool IsIndividual,
    string? Description);

    public sealed record CreateCustomerCategoryResponse(int Id);

    public sealed class CreateCustomerCategoryValidator : AbstractValidator<CreateCustomerCategoryRequest>
    {
        public CreateCustomerCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(x => x.Description)
                .MaximumLength(500);
        }

    }
