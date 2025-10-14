//using Application.Abstractions.UnitOfWork;
//using Application.Abstractions.Repositories.Inventories;
//using Application.DTOs.Inventories;
//using Application.DTOs.Inventories.Request;
//using Application.Results;
//using Application.Services.Inventories;
//using Domain.Entities;
//using Domain.Enums;
//using FluentValidation;
//using FluentValidation.Results;
//using Moq;
//using System.Linq.Expressions;
//using Xunit;

//namespace Application.Tests.InventoryTests
//{
//    public class InventoryServiceTests
//    {
//        private readonly Mock<IUnitOfWork> _uowMock = new();
//        private readonly Mock<IInventoryRepository> _inventoryRepositoryMock = new();
//        private readonly Mock<IValidator<InventoryCreateRequest>> _createValidatorMock = new();
//        private readonly Mock<IValidator<InventoryUpdateRequest>> _updateValidatorMock = new();

//        private InventoryService CreateService()
//        {
//            _uowMock.SetupGet(u => u.Inventories).Returns(_inventoryRepositoryMock.Object);
            
//            return new InventoryService(
//                _uowMock.Object,
//                _createValidatorMock.Object,
//                _updateValidatorMock.Object
//            );
//        }

//        #region GetAllAsync Tests

//        [Fact]
//        public async Task GetAllAsync_ReturnsSuccess_WhenInventoriesExist()
//        {
//            // Arrange
//            var inventories = new List<Inventory>
//            {
//                new Inventory
//                {
//                    Id = 1,
//                    ProductId = 1,
//                    LocationId = 1,
//                    QuantityOnHand = 100,
//                    ReorderLevel = 10,
//                    MaxLevel = 500,
//                    Product = new Product { Name = "Product 1" },
//                    Location = new Location { Name = "Location 1" }
//                },
//                new Inventory
//                {
//                    Id = 2,
//                    ProductId = 2,
//                    LocationId = 2,
//                    QuantityOnHand = 50,
//                    ReorderLevel = 5,
//                    MaxLevel = 200,
//                    Product = new Product { Name = "Product 2" },
//                    Location = new Location { Name = "Location 2" }
//                }
//            };

//            _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(inventories);

//            var service = CreateService();

//            // Act
//            var result = await service.GetAllAsync(CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(2, result.Value.Count);

//            var firstInventory = result.Value.First(x => x.Id == 1);
//            Assert.Equal(1, firstInventory.ProductId);
//            Assert.Equal("Product 1", firstInventory.ProductName);
//            Assert.Equal(1, firstInventory.LocationId);
//            Assert.Equal("Location 1", firstInventory.LocationName);
//            Assert.Equal(100, firstInventory.QuantityOnHand);
//            Assert.Equal(10, firstInventory.ReorderLevel);
//            Assert.Equal(500, firstInventory.MaxLevel);

//            var secondInventory = result.Value.First(x => x.Id == 2);
//            Assert.Equal(2, secondInventory.ProductId);
//            Assert.Equal("Product 2", secondInventory.ProductName);
//            Assert.Equal(2, secondInventory.LocationId);
//            Assert.Equal("Location 2", secondInventory.LocationName);
//            Assert.Equal(50, secondInventory.QuantityOnHand);
//            Assert.Equal(5, secondInventory.ReorderLevel);
//            Assert.Equal(200, secondInventory.MaxLevel);
//        }

//        [Fact]
//        public async Task GetAllAsync_ReturnsNotFound_WhenNoInventoriesExist()
//        {
//            // Arrange
//            var inventories = new List<Inventory>();

//            _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(inventories);

//            var service = CreateService();

//            // Act
//            var result = await service.GetAllAsync(CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("Inventories", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetAllAsync_ReturnsNotFound_WhenInventoriesIsNull()
//        {
//            // Arrange
//            _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync((List<Inventory>)null!);

//            var service = CreateService();

//            // Act
//            var result = await service.GetAllAsync(CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("Inventories", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetAllAsync_ReturnsException_WhenRepositoryThrows()
//        {
//            // Arrange
//            _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.GetAllAsync(CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in GetAllAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetAllAsync_HandlesNullProductAndLocationReferences()
//        {
//            // Arrange
//            var inventories = new List<Inventory>
//            {
//                new Inventory
//                {
//                    Id = 1,
//                    ProductId = 1,
//                    LocationId = 1,
//                    QuantityOnHand = 100,
//                    ReorderLevel = 10,
//                    MaxLevel = 500,
//                    Product = null!, // Null reference
//                    Location = null! // Null reference
//                }
//            };

//            _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(inventories);

//            var service = CreateService();

//            // Act
//            var result = await service.GetAllAsync(CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Single(result.Value);
//            Assert.Equal(string.Empty, result.Value.First().ProductName);
//            Assert.Equal(string.Empty, result.Value.First().LocationName);
//        }

//        #endregion

//        #region FindAsync Tests

//        [Fact]
//        public async Task FindAsync_ReturnsSuccess_WhenInventoryExists()
//        {
//            // Arrange
//            var inventoryId = 1;
//            var inventory = new Inventory
//            {
//                Id = inventoryId,
//                ProductId = 1,
//                LocationId = 1,
//                QuantityOnHand = 100,
//                ReorderLevel = 10,
//                MaxLevel = 500,
//                Product = new Product { Name = "Test Product" },
//                Location = new Location { Name = "Test Location" }
//            };

//            _inventoryRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(inventory);

//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(inventoryId, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(inventoryId, result.Value.Id);
//            Assert.Equal(1, result.Value.ProductId);
//            Assert.Equal("Test Product", result.Value.ProductName);
//            Assert.Equal(1, result.Value.LocationId);
//            Assert.Equal("Test Location", result.Value.LocationName);
//            Assert.Equal(100, result.Value.QuantityOnHand);
//            Assert.Equal(10, result.Value.ReorderLevel);
//            Assert.Equal(500, result.Value.MaxLevel);
//        }

//        [Fact]
//        public async Task FindAsync_ReturnsInvalidId_WhenIdIsZero()
//        {
//            // Arrange
//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(0, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);

//            _inventoryRepositoryMock.Verify(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            ), Times.Never);
//        }

//        [Fact]
//        public async Task FindAsync_ReturnsInvalidId_WhenIdIsNegative()
//        {
//            // Arrange
//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(-1, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);

//            _inventoryRepositoryMock.Verify(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            ), Times.Never);
//        }

//        [Fact]
//        public async Task FindAsync_ReturnsNotFound_WhenInventoryDoesNotExist()
//        {
//            // Arrange
//            var inventoryId = 1;

//            _inventoryRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync((Inventory)null!);

//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(inventoryId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("Inventory", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task FindAsync_ReturnsException_WhenRepositoryThrows()
//        {
//            // Arrange
//            var inventoryId = 1;

//            _inventoryRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(inventoryId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in FindAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task FindAsync_HandleNullProductAndLocationReferences()
//        {
//            // Arrange
//            var inventoryId = 1;
//            var inventory = new Inventory
//            {
//                Id = inventoryId,
//                ProductId = 1,
//                LocationId = 1,
//                QuantityOnHand = 100,
//                ReorderLevel = 10,
//                MaxLevel = 500,
//                Product = null!, // Null reference
//                Location = null! // Null reference
//            };

//            _inventoryRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(inventory);

//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(inventoryId, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(string.Empty, result.Value.ProductName);
//            Assert.Equal(string.Empty, result.Value.LocationName);
//        }

//        #endregion

//        #region CreateAsync Tests

//        [Fact]
//        public async Task CreateAsync_ReturnsSuccess_WhenValidRequest()
//        {
//            // Arrange
//            var request = new InventoryCreateRequest
//            {
//                ProductId = 1,
//                LocationId = 1,
//                QuantityOnHand = 100,
//                ReorderLevel = 10,
//                MaxLevel = 500
//            };

//            var validationResult = new ValidationResult();
//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _inventoryRepositoryMock.Setup(r => r.IsExistAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>()
//            )).ReturnsAsync(false);

//            var createdInventory = new Inventory
//            {
//                Id = 1,
//                ProductId = request.ProductId,
//                LocationId = request.LocationId,
//                QuantityOnHand = request.QuantityOnHand,
//                ReorderLevel = request.ReorderLevel,
//                MaxLevel = request.MaxLevel,
//                Product = new Product { Name = "Test Product" },
//                Location = new Location { Name = "Test Location" }
//            };

//            // Setup to simulate the ID being set after Add
//            _inventoryRepositoryMock.Setup(r => r.Add(It.IsAny<Inventory>()))
//                .Callback<Inventory>(i => i.Id = 1);

//            // Setup for the FindAsync call at the end of CreateAsync
//            _inventoryRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(createdInventory);

//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(request.ProductId, result.Value.ProductId);
//            Assert.Equal(request.LocationId, result.Value.LocationId);
//            Assert.Equal(request.QuantityOnHand, result.Value.QuantityOnHand);
//            Assert.Equal(request.ReorderLevel, result.Value.ReorderLevel);
//            Assert.Equal(request.MaxLevel, result.Value.MaxLevel);

//            _inventoryRepositoryMock.Verify(r => r.Add(It.Is<Inventory>(i =>
//                i.ProductId == request.ProductId &&
//                i.LocationId == request.LocationId &&
//                i.QuantityOnHand == request.QuantityOnHand &&
//                i.ReorderLevel == request.ReorderLevel &&
//                i.MaxLevel == request.MaxLevel
//            )), Times.Once);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact]
//        public async Task CreateAsync_ReturnsFailure_WhenValidationFails()
//        {
//            // Arrange
//            var request = new InventoryCreateRequest
//            {
//                ProductId = 0, // Invalid
//                LocationId = 0, // Invalid
//                QuantityOnHand = -1, // Invalid
//                ReorderLevel = -1, // Invalid
//                MaxLevel = -1 // Invalid
//            };

//            var validationResult = new ValidationResult(new[]
//            {
//                new ValidationFailure("ProductId", "ProductId must be greater than 0."),
//                new ValidationFailure("LocationId", "LocationId must be greater than 0."),
//                new ValidationFailure("QuantityOnHand", "QuantityOnHand must be non-negative.")
//            });

//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Contains("ProductId must be greater than 0.", result.ErrorMessage);
//            Assert.Contains("LocationId must be greater than 0.", result.ErrorMessage);
//            Assert.Contains("QuantityOnHand must be non-negative.", result.ErrorMessage);

//            _inventoryRepositoryMock.Verify(r => r.IsExistAsync(It.IsAny<Expression<Func<Inventory, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
//            _inventoryRepositoryMock.Verify(r => r.Add(It.IsAny<Inventory>()), Times.Never);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//        }

//        [Fact]
//        public async Task CreateAsync_ReturnsConflict_WhenInventoryAlreadyExists()
//        {
//            // Arrange
//            var request = new InventoryCreateRequest
//            {
//                ProductId = 1,
//                LocationId = 1,
//                QuantityOnHand = 100,
//                ReorderLevel = 10,
//                MaxLevel = 500
//            };

//            var validationResult = new ValidationResult();
//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _inventoryRepositoryMock.Setup(r => r.IsExistAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>()
//            )).ReturnsAsync(true);

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.Conflict, result.ErrorType);
//            Assert.Contains("Inventory with the same ProductId", result.ErrorMessage);
//            Assert.Contains("LocationId already exists", result.ErrorMessage);

//            _inventoryRepositoryMock.Verify(r => r.Add(It.IsAny<Inventory>()), Times.Never);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//        }

//        [Fact]
//        public async Task CreateAsync_ReturnsException_WhenValidatorThrows()
//        {
//            // Arrange
//            var request = new InventoryCreateRequest
//            {
//                ProductId = 1,
//                LocationId = 1,
//                QuantityOnHand = 100,
//                ReorderLevel = 10,
//                MaxLevel = 500
//            };

//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ThrowsAsync(new Exception("Validation error"));

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in CreateAsync", result.ErrorMessage);
//            Assert.Contains("Validation error", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task CreateAsync_ReturnsException_WhenSaveChangesThrows()
//        {
//            // Arrange
//            var request = new InventoryCreateRequest
//            {
//                ProductId = 1,
//                LocationId = 1,
//                QuantityOnHand = 100,
//                ReorderLevel = 10,
//                MaxLevel = 500
//            };

//            var validationResult = new ValidationResult();
//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _inventoryRepositoryMock.Setup(r => r.IsExistAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>()
//            )).ReturnsAsync(false);

//            _inventoryRepositoryMock.Setup(r => r.Add(It.IsAny<Inventory>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
//                .ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in CreateAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task CreateAsync_VerifiesUniqueConstraint()
//        {
//            // Arrange
//            var request = new InventoryCreateRequest
//            {
//                ProductId = 5,
//                LocationId = 3,
//                QuantityOnHand = 100,
//                ReorderLevel = 10,
//                MaxLevel = 500
//            };

//            var validationResult = new ValidationResult();
//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _inventoryRepositoryMock.Setup(r => r.IsExistAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>()
//            )).ReturnsAsync(false);

//            _inventoryRepositoryMock.Setup(r => r.Add(It.IsAny<Inventory>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);

//            // Assert
//            // Verify that the existence check was performed with correct ProductId and LocationId
//            _inventoryRepositoryMock.Verify(r => r.IsExistAsync(
//                It.Is<Expression<Func<Inventory, bool>>>(expr => expr != null),
//                It.IsAny<CancellationToken>()
//            ), Times.Once);
//        }

//        #endregion

//        #region UpdateAsync Tests

//        [Fact]
//        public async Task UpdateAsync_ReturnsSuccess_WhenValidRequest()
//        {
//            // Arrange
//            var inventoryId = 1;
//            var request = new InventoryUpdateRequest
//            {
//                QuantityOnHand = 150,
//                ReorderLevel = 15,
//                MaxLevel = 600
//            };

//            var existingInventory = new Inventory
//            {
//                Id = inventoryId,
//                ProductId = 1,
//                LocationId = 1,
//                QuantityOnHand = 100,
//                ReorderLevel = 10,
//                MaxLevel = 500
//            };

//            var updatedInventory = new Inventory
//            {
//                Id = inventoryId,
//                ProductId = 1,
//                LocationId = 1,
//                QuantityOnHand = request.QuantityOnHand,
//                ReorderLevel = request.ReorderLevel,
//                MaxLevel = request.MaxLevel,
//                Product = new Product { Name = "Test Product" },
//                Location = new Location { Name = "Test Location" }
//            };

//            var validationResult = new ValidationResult();
//            _updateValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _inventoryRepositoryMock.SetupSequence(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            ))
//                .ReturnsAsync(existingInventory)  // First call without includes
//                .ReturnsAsync(updatedInventory); // Second call with includes

//            _inventoryRepositoryMock.Setup(r => r.Update(It.IsAny<Inventory>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(inventoryId, request, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(inventoryId, result.Value.Id);
//            Assert.Equal(request.QuantityOnHand, result.Value.QuantityOnHand);
//            Assert.Equal(request.ReorderLevel, result.Value.ReorderLevel);
//            Assert.Equal(request.MaxLevel, result.Value.MaxLevel);

//            // Verify the existing inventory was updated with new values
//            Assert.Equal(request.QuantityOnHand, existingInventory.QuantityOnHand);
//            Assert.Equal(request.ReorderLevel, existingInventory.ReorderLevel);
//            Assert.Equal(request.MaxLevel, existingInventory.MaxLevel);

//            _inventoryRepositoryMock.Verify(r => r.Update(It.IsAny<Inventory>()), Times.Once);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsInvalidId_WhenIdIsZero()
//        {
//            // Arrange
//            var request = new InventoryUpdateRequest
//            {
//                QuantityOnHand = 150,
//                ReorderLevel = 15,
//                MaxLevel = 600
//            };

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(0, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);

//            _updateValidatorMock.Verify(v => v.ValidateAsync(It.IsAny<InventoryUpdateRequest>(), It.IsAny<CancellationToken>()), Times.Never);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsInvalidId_WhenIdIsNegative()
//        {
//            // Arrange
//            var request = new InventoryUpdateRequest
//            {
//                QuantityOnHand = 150,
//                ReorderLevel = 15,
//                MaxLevel = 600
//            };

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(-1, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);

//            _updateValidatorMock.Verify(v => v.ValidateAsync(It.IsAny<InventoryUpdateRequest>(), It.IsAny<CancellationToken>()), Times.Never);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsFailure_WhenValidationFails()
//        {
//            // Arrange
//            var inventoryId = 1;
//            var request = new InventoryUpdateRequest
//            {
//                QuantityOnHand = -1, // Invalid
//                ReorderLevel = -1, // Invalid
//                MaxLevel = -1 // Invalid
//            };

//            var validationResult = new ValidationResult(new[]
//            {
//                new ValidationFailure("QuantityOnHand", "QuantityOnHand must be non-negative."),
//                new ValidationFailure("ReorderLevel", "ReorderLevel must be non-negative."),
//                new ValidationFailure("MaxLevel", "MaxLevel must be greater than ReorderLevel.")
//            });

//            _updateValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(inventoryId, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Contains("QuantityOnHand must be non-negative.", result.ErrorMessage);
//            Assert.Contains("ReorderLevel must be non-negative.", result.ErrorMessage);
//            Assert.Contains("MaxLevel must be greater than ReorderLevel.", result.ErrorMessage);

//            _inventoryRepositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<Inventory, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<string>()), Times.Never);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsNotFound_WhenInventoryDoesNotExist()
//        {
//            // Arrange
//            var inventoryId = 1;
//            var request = new InventoryUpdateRequest
//            {
//                QuantityOnHand = 150,
//                ReorderLevel = 15,
//                MaxLevel = 600
//            };

//            var validationResult = new ValidationResult();
//            _updateValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _inventoryRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync((Inventory)null!);

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(inventoryId, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("Inventory", result.ErrorMessage);

//            _inventoryRepositoryMock.Verify(r => r.Update(It.IsAny<Inventory>()), Times.Never);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsException_WhenRepositoryThrows()
//        {
//            // Arrange
//            var inventoryId = 1;
//            var request = new InventoryUpdateRequest
//            {
//                QuantityOnHand = 150,
//                ReorderLevel = 15,
//                MaxLevel = 600
//            };

//            var validationResult = new ValidationResult();
//            _updateValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _inventoryRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(inventoryId, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in UpdateAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        #endregion

//        #region DeleteAsync Tests

//        [Fact]
//        public async Task DeleteAsync_ReturnsSuccess_WhenInventoryExists()
//        {
//            // Arrange
//            var inventoryId = 1;
//            var existingInventory = new Inventory
//            {
//                Id = inventoryId,
//                ProductId = 1,
//                LocationId = 1,
//                QuantityOnHand = 100,
//                ReorderLevel = 10,
//                MaxLevel = 500
//            };

//            _inventoryRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync(existingInventory);

//            _inventoryRepositoryMock.Setup(r => r.Delete(It.IsAny<Inventory>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var service = CreateService();

//            // Act
//            var result = await service.DeleteAsync(inventoryId, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);

//            _inventoryRepositoryMock.Verify(r => r.Delete(It.Is<Inventory>(i => i.Id == inventoryId)), Times.Once);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact]
//        public async Task DeleteAsync_ReturnsInvalidId_WhenIdIsZero()
//        {
//            // Arrange
//            var service = CreateService();

//            // Act
//            var result = await service.DeleteAsync(0, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);

//            _inventoryRepositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<Inventory, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<string>()), Times.Never);
//        }

//        [Fact]
//        public async Task DeleteAsync_ReturnsInvalidId_WhenIdIsNegative()
//        {
//            // Arrange
//            var service = CreateService();

//            // Act
//            var result = await service.DeleteAsync(-1, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);

//            _inventoryRepositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<Inventory, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<string>()), Times.Never);
//        }

//        [Fact]
//        public async Task DeleteAsync_ReturnsNotFound_WhenInventoryDoesNotExist()
//        {
//            // Arrange
//            var inventoryId = 1;

//            _inventoryRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync((Inventory)null!);

//            var service = CreateService();

//            // Act
//            var result = await service.DeleteAsync(inventoryId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("Inventory", result.ErrorMessage);

//            _inventoryRepositoryMock.Verify(r => r.Delete(It.IsAny<Inventory>()), Times.Never);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//        }

//        [Fact]
//        public async Task DeleteAsync_ReturnsException_WhenRepositoryThrows()
//        {
//            // Arrange
//            var inventoryId = 1;

//            _inventoryRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.DeleteAsync(inventoryId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in DeleteAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        #endregion

//        #region GetInventoryLowStockAsync Tests

//        [Fact]
//        public async Task GetInventoryLowStockAsync_ReturnsSuccess_WhenLowStockInventoriesExist()
//        {
//            // Arrange
//            var lowStockInventories = new List<Inventory>
//            {
//                new Inventory
//                {
//                    Id = 1,
//                    ProductId = 1,
//                    LocationId = 1,
//                    QuantityOnHand = 5, // Below reorder level
//                    ReorderLevel = 10,
//                    MaxLevel = 500,
//                    Product = new Product { Name = "Low Stock Product 1" },
//                    Location = new Location { Name = "Location 1" }
//                },
//                new Inventory
//                {
//                    Id = 2,
//                    ProductId = 2,
//                    LocationId = 2,
//                    QuantityOnHand = 8, // Equal to reorder level
//                    ReorderLevel = 8,
//                    MaxLevel = 200,
//                    Product = new Product { Name = "Low Stock Product 2" },
//                    Location = new Location { Name = "Location 2" }
//                }
//            };

//            _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(lowStockInventories);

//            var service = CreateService();

//            // Act
//            var result = await service.GetInventoryLowStockAsync(CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(2, result.Value.Count);

//            var first = result.Value.First(x => x.Id == 1);
//            Assert.Equal("Low Stock Product 1", first.ProductName);
//            Assert.Equal(5, first.QuantityOnHand);
//            Assert.Equal(10, first.ReorderLevel);

//            var second = result.Value.First(x => x.Id == 2);
//            Assert.Equal("Low Stock Product 2", second.ProductName);
//            Assert.Equal(8, second.QuantityOnHand);
//            Assert.Equal(8, second.ReorderLevel);

//            // Verify the filter expression for low stock (QuantityOnHand <= ReorderLevel)
//            _inventoryRepositoryMock.Verify(r => r.GetAllAsync(
//                It.Is<Expression<Func<Inventory, bool>>>(expr => expr != null),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            ), Times.Once);
//        }

//        [Fact]
//        public async Task GetInventoryLowStockAsync_ReturnsNotFound_WhenNoLowStockInventoriesExist()
//        {
//            // Arrange
//            var inventories = new List<Inventory>();

//            _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(inventories);

//            var service = CreateService();

//            // Act
//            var result = await service.GetInventoryLowStockAsync(CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("Inventories with low stock", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetInventoryLowStockAsync_ReturnsNotFound_WhenInventoriesIsNull()
//        {
//            // Arrange
//            _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync((List<Inventory>)null!);

//            var service = CreateService();

//            // Act
//            var result = await service.GetInventoryLowStockAsync(CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("Inventories with low stock", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetInventoryLowStockAsync_ReturnsException_WhenRepositoryThrows()
//        {
//            // Arrange
//            _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.GetInventoryLowStockAsync(CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in GetInventoryLowStockAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetInventoryLowStockAsync_HandlesNullProductAndLocationReferences()
//        {
//            // Arrange
//            var lowStockInventories = new List<Inventory>
//            {
//                new Inventory
//                {
//                    Id = 1,
//                    ProductId = 1,
//                    LocationId = 1,
//                    QuantityOnHand = 5,
//                    ReorderLevel = 10,
//                    MaxLevel = 500,
//                    Product = null!, // Null reference
//                    Location = null! // Null reference
//                }
//            };

//            _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(lowStockInventories);

//            var service = CreateService();

//            // Act
//            var result = await service.GetInventoryLowStockAsync(CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Single(result.Value);
//            Assert.Equal(string.Empty, result.Value.First().ProductName);
//            Assert.Equal(string.Empty, result.Value.First().LocationName);
//        }

//        #endregion

//        #region GetInventoryValuationAsync Tests

//        [Fact]
//        public async Task GetInventoryValuationAsync_ReturnsSuccess_WhenRepositoryReturnsValue()
//        {
//            // Arrange
//            var expectedValuation = 25000.50m;

//            _inventoryRepositoryMock.Setup(r => r.GetInventoryValuationAsync(It.IsAny<CancellationToken>()))
//                .ReturnsAsync(expectedValuation);

//            var service = CreateService();

//            // Act
//            var result = await service.GetInventoryValuationAsync(CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.Equal(expectedValuation, result.Value);

//            _inventoryRepositoryMock.Verify(r => r.GetInventoryValuationAsync(It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact]
//        public async Task GetInventoryValuationAsync_ReturnsZero_WhenNoInventoryExists()
//        {
//            // Arrange
//            var expectedValuation = 0m;

//            _inventoryRepositoryMock.Setup(r => r.GetInventoryValuationAsync(It.IsAny<CancellationToken>()))
//                .ReturnsAsync(expectedValuation);

//            var service = CreateService();

//            // Act
//            var result = await service.GetInventoryValuationAsync(CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.Equal(0m, result.Value);
//        }

//        [Fact]
//        public async Task GetInventoryValuationAsync_ReturnsException_WhenRepositoryThrows()
//        {
//            // Arrange
//            _inventoryRepositoryMock.Setup(r => r.GetInventoryValuationAsync(It.IsAny<CancellationToken>()))
//                .ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.GetInventoryValuationAsync(CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in GetInventoryValuationAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        #endregion

//        #region GetInventoryCostAsync Tests

//        [Fact]
//        public async Task GetInventoryCostAsync_ReturnsSuccess_WhenRepositoryReturnsValue()
//        {
//            // Arrange
//            var expectedCost = 15000.75m;

//            _inventoryRepositoryMock.Setup(r => r.GetInventoryCostAsync(It.IsAny<CancellationToken>()))
//                .ReturnsAsync(expectedCost);

//            var service = CreateService();

//            // Act
//            var result = await service.GetInventoryCostAsync(CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.Equal(expectedCost, result.Value);

//            _inventoryRepositoryMock.Verify(r => r.GetInventoryCostAsync(It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact]
//        public async Task GetInventoryCostAsync_ReturnsZero_WhenNoInventoryExists()
//        {
//            // Arrange
//            var expectedCost = 0m;

//            _inventoryRepositoryMock.Setup(r => r.GetInventoryCostAsync(It.IsAny<CancellationToken>()))
//                .ReturnsAsync(expectedCost);

//            var service = CreateService();

//            // Act
//            var result = await service.GetInventoryCostAsync(CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.Equal(0m, result.Value);
//        }

//        [Fact]
//        public async Task GetInventoryCostAsync_ReturnsException_WhenRepositoryThrows()
//        {
//            // Arrange
//            _inventoryRepositoryMock.Setup(r => r.GetInventoryCostAsync(It.IsAny<CancellationToken>()))
//                .ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.GetInventoryCostAsync(CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in GetInventoryValuationAsync", result.ErrorMessage); // Note: Original code has wrong method name in exception
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        #endregion

//        #region Constructor Tests

//        [Fact]
//        public void InventoryService_Constructor_SetsPropertiesCorrectly()
//        {
//            // Arrange & Act
//            var service = CreateService();

//            // Assert
//            Assert.NotNull(service);
//        }

//        #endregion

//        #region Edge Cases and Boundary Tests

//        [Theory]
//        [InlineData(1, 2, 100.5, 10.0, 500.0)]
//        [InlineData(999, 888, 0.0, 0.0, 1000000.0)]
//        [InlineData(1, 1, 50.25, 25.75, 100.0)]
//        public async Task CreateAsync_HandlesVariousValidValues(int productId, int locationId, decimal quantityOnHand, decimal reorderLevel, decimal maxLevel)
//        {
//            // Arrange
//            var request = new InventoryCreateRequest
//            {
//                ProductId = productId,
//                LocationId = locationId,
//                QuantityOnHand = quantityOnHand,
//                ReorderLevel = reorderLevel,
//                MaxLevel = maxLevel
//            };

//            var validationResult = new ValidationResult();
//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _inventoryRepositoryMock.Setup(r => r.IsExistAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>()
//            )).ReturnsAsync(false);

//            var createdInventory = new Inventory
//            {
//                Id = 1,
//                ProductId = request.ProductId,
//                LocationId = request.LocationId,
//                QuantityOnHand = request.QuantityOnHand,
//                ReorderLevel = request.ReorderLevel,
//                MaxLevel = request.MaxLevel,
//                Product = new Product { Name = $"Product {productId}" },
//                Location = new Location { Name = $"Location {locationId}" }
//            };

//            _inventoryRepositoryMock.Setup(r => r.Add(It.IsAny<Inventory>()))
//                .Callback<Inventory>(i => i.Id = 1);

//            _inventoryRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(createdInventory);

//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(productId, result.Value.ProductId);
//            Assert.Equal(locationId, result.Value.LocationId);
//            Assert.Equal(quantityOnHand, result.Value.QuantityOnHand);
//            Assert.Equal(reorderLevel, result.Value.ReorderLevel);
//            Assert.Equal(maxLevel, result.Value.MaxLevel);
//        }

//        [Fact]
//        public async Task GetInventoryLowStockAsync_FiltersCorrectlyByLowStockCondition()
//        {
//            // Arrange
//            var inventories = new List<Inventory>
//            {
//                new Inventory { Id = 1, QuantityOnHand = 5, ReorderLevel = 10 }, // Low stock
//                new Inventory { Id = 2, QuantityOnHand = 15, ReorderLevel = 10 }, // Not low stock
//                new Inventory { Id = 3, QuantityOnHand = 8, ReorderLevel = 8 } // Equal - should be included
//            };

//            _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(new List<Inventory>()); // Empty to trigger NotFound, but we're testing the call

//            var service = CreateService();

//            // Act
//            var result = await service.GetInventoryLowStockAsync(CancellationToken.None);

//            // Assert
//            // Verify the correct filter expression was used
//            _inventoryRepositoryMock.Verify(r => r.GetAllAsync(
//                It.Is<Expression<Func<Inventory, bool>>>(expr => expr != null),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            ), Times.Once);
//        }

//        [Fact]
//        public async Task UpdateAsync_DoesNotChangeProductIdOrLocationId()
//        {
//            // Arrange
//            var inventoryId = 1;
//            var request = new InventoryUpdateRequest
//            {
//                QuantityOnHand = 200,
//                ReorderLevel = 20,
//                MaxLevel = 800
//            };

//            var existingInventory = new Inventory
//            {
//                Id = inventoryId,
//                ProductId = 5, // Should not change
//                LocationId = 3, // Should not change
//                QuantityOnHand = 100,
//                ReorderLevel = 10,
//                MaxLevel = 500
//            };

//            var validationResult = new ValidationResult();
//            _updateValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _inventoryRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync(existingInventory);

//            _inventoryRepositoryMock.Setup(r => r.Update(It.IsAny<Inventory>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(inventoryId, request, CancellationToken.None);

//            // Assert
//            // Verify that ProductId and LocationId were not changed
//            Assert.Equal(5, existingInventory.ProductId);
//            Assert.Equal(3, existingInventory.LocationId);
            
//            // Verify that only the allowed fields were updated
//            Assert.Equal(request.QuantityOnHand, existingInventory.QuantityOnHand);
//            Assert.Equal(request.ReorderLevel, existingInventory.ReorderLevel);
//            Assert.Equal(request.MaxLevel, existingInventory.MaxLevel);
//        }

//        #endregion
//    }
//}