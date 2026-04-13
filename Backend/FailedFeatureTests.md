# Failed Feature Tests Report

## Summary
| Test Name | Service | Failure Reason | Suspected Bug Location |
|-----------|---------|----------------|------------------------|
| UserServiceTests.AddAsync_DuplicateUserName_ReturnsConflictFailure | UserService | Duplicate username is accepted and a second user is created instead of returning a conflict failure | src/Application/Users/Services/UserService.cs - AddAsync |

## Details

### UserServiceTests.AddAsync_DuplicateUserName_ReturnsConflictFailure
- **Service/Query**: UserService.AddAsync
- **What the test does**: Creates a user, then attempts to create a second user with the same username and a different email.
- **Expected behavior**: The second create should fail with a conflict-style response for duplicate username.
- **Actual behavior**: The second create succeeds (`IsSuccess = true`) and another user with the same username is persisted.
- **Suspected bug**: src/Application/Users/Services/UserService.cs - `AddAsync` does not check for existing usernames before insert.
- **Reproduction steps**:
  1. Run `dotnet test Tests/Application.IntegrationTests/Application.IntegrationTests.csproj --filter "FullyQualifiedName~Application.IntegrationTests.Services.UserServiceTests.AddAsync_DuplicateUserName_ReturnsConflictFailure"`.
  2. Observe assertion failure: expected failure/conflict, actual result is success.
