# Failed Customer Feature Tests Report

Date: 2026-04-12
Command:

```bash
dotnet test Tests/Application.IntegrationTests/Application.IntegrationTests.csproj --filter "FullyQualifiedName~CustomerFeatureTests" -v minimal
```

## Summary

- Total tests: 27
- Passed: 26
- Failed: 1

The single remaining failure is a business-logic defect in Customer update behavior.

## Failing Test

- Test: `Application.IntegrationTests.Services.CustomerFeatureTests.UpdateAsync_ValidRequest_UpdatesPersistedCustomerFields`
- File: `Tests/Application.IntegrationTests/Services/CustomerFeatureTests.cs`
- Assertion failure: persisted email does not change after a successful update.

Observed values:

- Actual persisted email: `customer-d8c9cade@ims.local`
- Expected updated email: `updated-customer@ims.local`

## Expected Behavior

Calling `CustomerService.UpdateAsync` with a new email should persist the new email value in `customers.email`.

## Actual Behavior

`UpdateAsync` returns success, but the customer email in the database remains unchanged.

## Root Cause

`Customer.Update(...)` delegates email handling to `UpdateEmail(email)`, but `UpdateEmail` never assigns the new value.

File:

- `src/Domain/Customers/Entities/Customer.cs`

Current logic:

```csharp
public void UpdateEmail(string email)
{
	_EnsureCustomerIsActive();
	if(Email != email)
	{
		// add email validation here if needed
	}
}
```

Missing assignment:

```csharp
Email = email;
```

## Impact

- Customer email cannot be updated even though update API/service reports success.
- Downstream query/read models keep stale customer contact data.
