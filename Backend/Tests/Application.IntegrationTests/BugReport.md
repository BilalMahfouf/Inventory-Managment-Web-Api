# Integration Test Failure Bug Report

Date: 2026-04-07
Command: `dotnet test Tests/Application.IntegrationTests --no-build -v normal`

## Root Cause Summary
All failing tests fail during test fixture initialization before test logic executes.

- Suspected production area: EF Core migration pipeline and migration chain in Infrastructure
- Entry point that throws: `IntegrationTestWebAppFactory.InitializeAsync()` -> `context.Database.MigrateAsync()`
- Relevant line: `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64`
- Observed SQL error: `Cannot find the object "UserRoles" because it does not exist or you do not have permissions.`

Expected behavior:
- Applying existing migrations to a fresh SQL Server container should succeed.

Observed behavior:
- Migration execution fails immediately with missing `UserRoles`, causing every test to fail before Arrange/Act/Assert.

## Failing Tests

| Test name | Service method suspected | Observed behavior | Expected behavior | Relevant line(s) |
|---|---|---|---|---|
| CreateSalesOrderAsync_ValidOrder_PersistsOrderWithExpectedFields | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CreateSalesOrderAsync_ValidWalkInOrder_PersistsAsCompleted | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CreateSalesOrderAsync_NullItems_ReturnsValidationFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CreateSalesOrderAsync_EmptyItems_ReturnsValidationFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CreateSalesOrderAsync_MissingCustomerForNonWalkIn_ReturnsValidationFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CreateSalesOrderAsync_WalkInWithCustomer_ReturnsValidationFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CreateSalesOrderAsync_NegativeQuantity_ReturnsValidationFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CreateSalesOrderAsync_MissingInventory_ReturnsNotFoundFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CreateSalesOrderAsync_InventoryProductMismatch_ReturnsValidationFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CreateSalesOrderAsync_InsufficientStock_ReturnsConflictFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CreateSalesOrderAsync_DuplicatePayload_CreatesDistinctOrders | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| UpdateSalesOrderAsync_ValidPendingOrder_UpdatesPersistedFieldsAndItems | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| UpdateSalesOrderAsync_NonExistingOrder_ReturnsNotFoundFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| UpdateSalesOrderAsync_NonPendingOrder_ReturnsConflictFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| UpdateSalesOrderAsync_MissingInventory_ReturnsNotFoundFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| ConfirmOrderAsync_PendingOrder_TransitionsToConfirmed | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| ConfirmOrderAsync_NonPendingOrder_ReturnsConflictFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| MarkInTransitAsync_ConfirmedOrder_TransitionsToInTransit | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| MarkInTransitAsync_PendingOrder_ReturnsConflictFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| ShipOrderAsync_InTransitOrder_TransitionsToShippedAndSetsTracking | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| ShipOrderAsync_ConfirmedOrder_ReturnsConflictFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CompleteOrderAsync_ShippedOrder_TransitionsToCompleted | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CompleteOrderAsync_InTransitOrder_ReturnsConflictFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CancelOrderAsync_ExistingPendingOrder_CancelsAndRestoresStock | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CancelOrderAsync_NonExistingOrder_ReturnsNotFoundFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| CancelOrderAsync_ShippedOrder_ReturnsConflictFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| ReturnOrderAsync_CompletedOrder_TransitionsToReturned | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| ReturnOrderAsync_NonCompletedOrder_ReturnsConflictFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| GetSalesOrderByIdAsync_ExistingOrder_ReturnsExpectedAggregate | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| GetSalesOrderByIdAsync_NonExistingOrder_ReturnsNotFoundFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| GetSalesOrdersAsync_EmptyDatabase_ReturnsNotFoundFailure | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |
| GetSalesOrdersAsync_WithSeededOrders_ReturnsExpectedCountAndData | Migration pipeline (pre-service) | Fixture initialization fails with missing `UserRoles` during `MigrateAsync()` | Migrations apply successfully, then service call executes | `Tests/Application.IntegrationTests/Common/IntegrationTestWebAppFactory.cs:64` |

## Migration Evidence

- `InitialCreate` migration contains empty `Up` and `Down`, so base schema is never created:
	- `src/Infrastructure/Migrations/20250831224016_InitialCreate.cs:12`
	- `src/Infrastructure/Migrations/20250831224016_InitialCreate.cs:18`
- A later migration assumes `UserRoles` already exists and immediately issues `AddColumn` against it:
	- `src/Infrastructure/Migrations/20250903191618_UserRoles_AddUpdateAndCreate_dates_users.cs:18`
- Migration chain contains references to `Users`/`UserRoles` in `principalTable` and `table` operations, but no migration `.cs` file creates either table.

Conclusion:
- The failing behavior is consistent with a production migration chain defect, not integration test logic.
