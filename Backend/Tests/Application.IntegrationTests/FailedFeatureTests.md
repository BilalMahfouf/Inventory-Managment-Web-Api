# Failed Product Feature Tests Report

Date: 2026-04-12
Command:

```bash
dotnet test Tests/Application.IntegrationTests/Application.IntegrationTests.csproj --filter "FullyQualifiedName~ProductFeatureTests|FullyQualifiedName~ProductCategoryFeatureTests" -v normal
```

## Summary

- Total tests: 34
- Passed: 33
- Failed: 1

The remaining failure is a business-logic defect in Product Category creation with parent categories.

## Failing Test

- Test: `Application.IntegrationTests.Services.ProductCategoryFeatureTests.AddAsync_ValidParent_CreatesChildCategory`
- File: `Tests/Application.IntegrationTests/Services/ProductCategoryFeatureTests.cs`
- Failure: expected success when creating a child category with an existing parent id, but service returns failure.

## Expected Behavior

When `ParentId` points to an existing category, `AddAsync` should create a child category successfully.

## Actual Behavior

`AddAsync` rejects valid parent ids and returns a `NotFound` result.

## Root Cause

In `ProductCategoryService.AddAsync`, parent validation checks the wrong field:

- File: `src/Application/Products/Services/ProductCategoryService.cs`
- Logic currently checks:

```csharp
await _repository.IsExistAsync(e => e.ParentId == request.ParentId, cancellationToken)
```

This checks whether **any existing category has that parent id**, instead of checking whether a category with that **id** exists.

For a valid top-level parent (whose `ParentId` is `null`), this condition is false, so child creation fails incorrectly.

## Correct Validation Direction

The existence check should validate parent entity id:

```csharp
await _repository.IsExistAsync(e => e.Id == request.ParentId, cancellationToken)
```

## Impact

- Cannot create first-level subcategories under valid parent categories.
- Breaks core Product Category hierarchy management.
