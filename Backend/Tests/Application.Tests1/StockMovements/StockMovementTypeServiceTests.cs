//using Application.Abstractions.Repositories.Base;
//using Application.Abstractions.Services.User;
//using Application.Abstractions.UnitOfWork;
//using Application.DTOs.StockMovements.Request;
//using Application.DTOs.StockMovements.Response;
//using Application.Results;
//using Application.Services.StockMovements;
//using Domain.Entities;
//using Domain.Enums;
//using FluentValidation;
//using FluentValidation.Results;
//using Moq;
//using System.Linq.Expressions;
//using Xunit;

//namespace Application.Tests.StockMovements
//{
//    public class StockMovementTypeServiceTests
//    {
//        private readonly Mock<IUnitOfWork> _uowMock = new();
//        private readonly Mock<IBaseRepository<StockMovementType>> _repositoryMock = new();
//        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
//        private readonly Mock<IValidator<StockMovementTypeRequest>> _validatorMock = new();

//        private StockMovementTypeService CreateService()
//        {
//            // Setup the UnitOfWork to return our mocked repository
//            _uowMock.SetupGet(u => u.StockMovementTypes).Returns(_repositoryMock.Object);

//            return new StockMovementTypeService(
//                _uowMock.Object,
//                _validatorMock.Object,
//                _currentUserServiceMock.Object
//            );
//        }

//        #region Map Tests

//        [Fact]
//        public void Map_ReturnsCorrectResponse_WhenGivenValidStockMovementType()
//        {
//            // Arrange
//            var stockMovementType = new StockMovementType
//            {
//                Id = 1,
//                Name = "Stock In",
//                Description = "Stock incoming movement",
//                Direction = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                DeletedAt = null,
//                DeletedByUserId = null,
//                CreatedByUser = new User { UserName = "testuser" },
//                DeletedByUser = null
//            };

//            var service = CreateService();

//            // Act
//            var result = service.Map(stockMovementType);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(stockMovementType.Id, result.Id);
//            Assert.Equal(stockMovementType.Name, result.Name);
//            Assert.Equal(stockMovementType.Description, result.Description);
//            Assert.Equal("In", result.Direction);
//            Assert.Equal(stockMovementType.CreatedAt, result.CreatedAt);
//            Assert.Equal(stockMovementType.CreatedByUserId, result.CreatedByUserId);
//            Assert.Equal("testuser", result.CreatedByUserName);
//            Assert.Equal(stockMovementType.IsDeleted, result.IsDeleted);
//            Assert.Equal(stockMovementType.DeletedAt, result.DeleteAt);
//            Assert.Equal(stockMovementType.DeletedByUserId, result.DeletedByUserId);
//            Assert.Null(result.DeletedByUserName);
//        }

//        [Theory]
//        [InlineData(1, "In")]
//        [InlineData(2, "Out")]
//        [InlineData(3, "Transfer")]
//        [InlineData(4, "Transfer")] // Any other value should map to Transfer
//        public void Map_MapsDirectionCorrectly(byte direction, string expectedDirection)
//        {
//            // Arrange
//            var stockMovementType = new StockMovementType
//            {
//                Id = 1,
//                Name = "Test",
//                Direction = direction,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                CreatedByUser = new User { UserName = "testuser" }
//            };

//            var service = CreateService();

//            // Act
//            var result = service.Map(stockMovementType);

//            // Assert
//            Assert.Equal(expectedDirection, result.Direction);
//        }

//        [Fact]
//        public void Map_HandlesNullUserReferences()
//        {
//            // Arrange
//            var stockMovementType = new StockMovementType
//            {
//                Id = 1,
//                Name = "Test",
//                Direction = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                CreatedByUser = null!,
//                DeletedByUser = null
//            };

//            var service = CreateService();

//            // Act
//            var result = service.Map(stockMovementType);

//            // Assert
//            Assert.Null(result.CreatedByUserName);
//            Assert.Null(result.DeletedByUserName);
//        }

//        #endregion

//        #region GetAllAsync Tests

//        [Fact]
//        public async Task GetAllAsync_ReturnsSuccess_WhenStockMovementTypesExist()
//        {
//            // Arrange
//            var stockMovementTypes = new List<StockMovementType>
//            {
//                new StockMovementType
//                {
//                    Id = 1,
//                    Name = "Stock In",
//                    Description = "Stock incoming movement",
//                    Direction = 1,
//                    CreatedAt = DateTime.UtcNow,
//                    CreatedByUserId = 1,
//                    IsDeleted = false,
//                    CreatedByUser = new User { UserName = "user1" }
//                },
//                new StockMovementType
//                {
//                    Id = 2,
//                    Name = "Stock Out",
//                    Description = "Stock outgoing movement",
//                    Direction = 2,
//                    CreatedAt = DateTime.UtcNow,
//                    CreatedByUserId = 2,
//                    IsDeleted = false,
//                    CreatedByUser = new User { UserName = "user2" }
//                }
//            };

//            _repositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync(stockMovementTypes);

//            var service = CreateService();

//            // Act
//            var result = await service.GetAllAsync(CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(2, result.Value.Count());

//            var stockIn = result.Value.First(x => x.Name == "Stock In");
//            Assert.Equal(1, stockIn.Id);
//            Assert.Equal("Stock incoming movement", stockIn.Description);
//            Assert.Equal("In", stockIn.Direction);
//            Assert.Equal("user1", stockIn.CreatedByUserName);

//            var stockOut = result.Value.First(x => x.Name == "Stock Out");
//            Assert.Equal(2, stockOut.Id);
//            Assert.Equal("Stock outgoing movement", stockOut.Description);
//            Assert.Equal("Out", stockOut.Direction);
//            Assert.Equal("user2", stockOut.CreatedByUserName);
//        }

//        [Fact]
//        public async Task GetAllAsync_ReturnsNotFound_WhenNoStockMovementTypesExist()
//        {
//            // Arrange
//            var stockMovementTypes = new List<StockMovementType>();

//            _repositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync(stockMovementTypes);

//            var service = CreateService();

//            // Act
//            var result = await service.GetAllAsync(CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("stockMovementTypes", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetAllAsync_ReturnsNotFound_WhenStockMovementTypesIsNull()
//        {
//            // Arrange
//            _repositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync((List<StockMovementType>?)null);

//            var service = CreateService();

//            // Act
//            var result = await service.GetAllAsync(CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("stockMovementTypes", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetAllAsync_ReturnsException_WhenRepositoryThrows()
//        {
//            // Arrange
//            _repositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
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

//        #endregion

//        #region FindAsync Tests

//        [Fact]
//        public async Task FindAsync_ReturnsSuccess_WhenStockMovementTypeExists()
//        {
//            // Arrange
//            var stockMovementTypeId = 1;
//            var stockMovementType = new StockMovementType
//            {
//                Id = stockMovementTypeId,
//                Name = "Stock In",
//                Description = "Stock incoming movement",
//                Direction = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                CreatedByUser = new User { UserName = "testuser" }
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync(stockMovementType);

//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(stockMovementTypeId, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(stockMovementTypeId, result.Value.Id);
//            Assert.Equal("Stock In", result.Value.Name);
//            Assert.Equal("Stock incoming movement", result.Value.Description);
//            Assert.Equal("In", result.Value.Direction);
//            Assert.Equal("testuser", result.Value.CreatedByUserName);
//        }

//        [Theory]
//        [InlineData(0)]
//        [InlineData(-1)]
//        public async Task FindAsync_ReturnsInvalidId_WhenIdIsInvalid(int invalidId)
//        {
//            // Arrange
//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(invalidId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);

//            _repositoryMock.Verify(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            ), Times.Never);
//        }

//        [Fact]
//        public async Task FindAsync_ReturnsNotFound_WhenStockMovementTypeDoesNotExist()
//        {
//            // Arrange
//            var stockMovementTypeId = 1;

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync((StockMovementType?)null);

//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(stockMovementTypeId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("Stock Movement Type", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task FindAsync_ReturnsException_WhenRepositoryThrows()
//        {
//            // Arrange
//            var stockMovementTypeId = 1;

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(stockMovementTypeId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in FindAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        #endregion

//        #region AddAsync Tests

//        [Fact]
//        public async Task AddAsync_ReturnsSuccess_WhenValidRequest()
//        {
//            // Arrange
//            var request = new StockMovementTypeRequest
//            {
//                Name = "Stock In",
//                Description = "Stock incoming movement",
//                Direction = 1
//            };

//            var validationResult = new ValidationResult();
//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            // Setup to simulate the ID being set after Add
//            _repositoryMock.Setup(r => r.Add(It.IsAny<StockMovementType>()))
//                .Callback<StockMovementType>(smt => smt.Id = 1);

//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            // Setup for the FindAsync call that happens at the end
//            var createdStockMovementType = new StockMovementType
//            {
//                Id = 1,
//                Name = request.Name,
//                Description = request.Description,
//                Direction = request.Direction,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                CreatedByUser = new User { UserName = "testuser" }
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync(createdStockMovementType);

//            var service = CreateService();

//            // Act
//            var result = await service.AddAsync(request, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(request.Name, result.Value.Name);
//            Assert.Equal(request.Description, result.Value.Description);
//            Assert.Equal("In", result.Value.Direction);

//            _repositoryMock.Verify(r => r.Add(It.Is<StockMovementType>(smt => 
//                smt.Name == request.Name &&
//                smt.Description == request.Description &&
//                smt.Direction == request.Direction &&
//                smt.IsDeleted == false
//            )), Times.Once);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact]
//        public async Task AddAsync_ReturnsFailure_WhenValidationFails()
//        {
//            // Arrange
//            var request = new StockMovementTypeRequest
//            {
//                Name = "", // Invalid empty name
//                Description = "Test Description",
//                Direction = 1
//            };

//            var validationResult = new ValidationResult(new[]
//            {
//                new ValidationFailure("Name", "Name is required."),
//            });

//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            var service = CreateService();

//            // Act
//            var result = await service.AddAsync(request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Contains("Name is required.", result.ErrorMessage);

//            _repositoryMock.Verify(r => r.Add(It.IsAny<StockMovementType>()), Times.Never);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//        }

//        [Fact]
//        public async Task AddAsync_ReturnsFailure_WhenMultipleValidationErrorsExist()
//        {
//            // Arrange
//            var request = new StockMovementTypeRequest
//            {
//                Name = "",
//                Description = "Test Description",
//                Direction = 0 // Invalid direction
//            };

//            var validationResult = new ValidationResult(new[]
//            {
//                new ValidationFailure("Name", "Name is required."),
//                new ValidationFailure("Direction", "Direction must be 1 (IN), 2 (OUT) or 3 (ADJUST).")
//            });

//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            var service = CreateService();

//            // Act
//            var result = await service.AddAsync(request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Contains("Name is required.", result.ErrorMessage);
//            Assert.Contains("Direction must be 1 (IN), 2 (OUT) or 3 (ADJUST).", result.ErrorMessage);
//            Assert.Contains(";", result.ErrorMessage); // Multiple errors separated by semicolon
//        }

//        [Fact]
//        public async Task AddAsync_ReturnsException_WhenValidatorThrows()
//        {
//            // Arrange
//            var request = new StockMovementTypeRequest
//            {
//                Name = "Stock In",
//                Description = "Test Description",
//                Direction = 1
//            };

//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ThrowsAsync(new Exception("Validation error"));

//            var service = CreateService();

//            // Act & Assert
//            // The validation is not wrapped in try-catch, so it will throw
//            await Assert.ThrowsAsync<Exception>(() => service.AddAsync(request, CancellationToken.None));
//        }

//        [Fact]
//        public async Task AddAsync_ReturnsException_WhenSaveChangesThrows()
//        {
//            // Arrange
//            var request = new StockMovementTypeRequest
//            {
//                Name = "Stock In",
//                Description = "Test Description",
//                Direction = 1
//            };

//            var validationResult = new ValidationResult();
//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _repositoryMock.Setup(r => r.Add(It.IsAny<StockMovementType>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
//                .ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.AddAsync(request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in AddAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task AddAsync_ReturnsException_WhenFindAsyncAfterCreateThrows()
//        {
//            // Arrange
//            var request = new StockMovementTypeRequest
//            {
//                Name = "Stock In",
//                Description = "Test Description",
//                Direction = 1
//            };

//            var validationResult = new ValidationResult();
//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _repositoryMock.Setup(r => r.Add(It.IsAny<StockMovementType>()))
//                .Callback<StockMovementType>(smt => smt.Id = 1);

//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ThrowsAsync(new Exception("Find error"));

//            var service = CreateService();

//            // Act
//            var result = await service.AddAsync(request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in FindAsync", result.ErrorMessage); // FindAsync has its own exception handling
//            Assert.Contains("Find error", result.ErrorMessage);
//        }

//        #endregion

//        #region UpdateAsync Tests

//        [Fact]
//        public async Task UpdateAsync_ReturnsSuccess_WhenValidRequest()
//        {
//            // Arrange
//            var stockMovementTypeId = 1;
//            var request = new StockMovementTypeRequest
//            {
//                Name = "Updated Stock In",
//                Description = "Updated description",
//                Direction = 2
//            };

//            var existingStockMovementType = new StockMovementType
//            {
//                Id = stockMovementTypeId,
//                Name = "Stock In",
//                Description = "Old description",
//                Direction = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false
//            };

//            var validationResult = new ValidationResult();
//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            // Setup IsExistAsync to return false (no duplicate name)
//            _repositoryMock.Setup(r => r.IsExistAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>()
//            )).ReturnsAsync(false);

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync(existingStockMovementType);

//            _repositoryMock.Setup(r => r.Update(It.IsAny<StockMovementType>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            // Setup the second FindAsync call with include properties for the final response
//            var updatedStockMovementType = new StockMovementType
//            {
//                Id = stockMovementTypeId,
//                Name = request.Name,
//                Description = request.Description,
//                Direction = request.Direction,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                CreatedByUser = new User { UserName = "testuser" }
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync(updatedStockMovementType);

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(stockMovementTypeId, request, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(request.Name, result.Value.Name);
//            Assert.Equal(request.Description, result.Value.Description);
//            Assert.Equal("Out", result.Value.Direction);

//            // Verify the existing entity was updated with new values
//            Assert.Equal(request.Name, existingStockMovementType.Name);
//            Assert.Equal(request.Description, existingStockMovementType.Description);
//            Assert.Equal(request.Direction, existingStockMovementType.Direction);

//            _repositoryMock.Verify(r => r.Update(It.IsAny<StockMovementType>()), Times.Once);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Theory]
//        [InlineData(0)]
//        [InlineData(-1)]
//        public async Task UpdateAsync_ReturnsInvalidId_WhenIdIsInvalid(int invalidId)
//        {
//            // Arrange
//            var request = new StockMovementTypeRequest
//            {
//                Name = "Test",
//                Description = "Test Description",
//                Direction = 1
//            };

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(invalidId, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);

//            _validatorMock.Verify(v => v.ValidateAsync(It.IsAny<StockMovementTypeRequest>(), It.IsAny<CancellationToken>()), Times.Never);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsFailure_WhenValidationFails()
//        {
//            // Arrange
//            var stockMovementTypeId = 1;
//            var request = new StockMovementTypeRequest
//            {
//                Name = "", // Invalid empty name
//                Description = "Test Description",
//                Direction = 1
//            };

//            var validationResult = new ValidationResult(new[]
//            {
//                new ValidationFailure("Name", "Name is required."),
//            });

//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(stockMovementTypeId, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Contains("Name is required.", result.ErrorMessage);

//            _repositoryMock.Verify(r => r.IsExistAsync(It.IsAny<Expression<Func<StockMovementType, bool>>>(), It.IsAny<CancellationToken>()), Times.Never);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsConflict_WhenStockMovementTypeWithSameNameExists()
//        {
//            // Arrange
//            var stockMovementTypeId = 1;
//            var request = new StockMovementTypeRequest
//            {
//                Name = "Existing Name",
//                Description = "Test Description",
//                Direction = 1
//            };

//            var validationResult = new ValidationResult();
//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            // Setup IsExistAsync to return true (duplicate name exists)
//            _repositoryMock.Setup(r => r.IsExistAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>()
//            )).ReturnsAsync(true);

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(stockMovementTypeId, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.Conflict, result.ErrorType);
//            Assert.Contains("Stock Movement Type with the same name already exists", result.ErrorMessage);

//            _repositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<StockMovementType, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<string>()), Times.Never);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsNotFound_WhenStockMovementTypeDoesNotExist()
//        {
//            // Arrange
//            var stockMovementTypeId = 1;
//            var request = new StockMovementTypeRequest
//            {
//                Name = "Test",
//                Description = "Test Description",
//                Direction = 1
//            };

//            var validationResult = new ValidationResult();
//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _repositoryMock.Setup(r => r.IsExistAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>()
//            )).ReturnsAsync(false);

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync((StockMovementType?)null);

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(stockMovementTypeId, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("Stock Movement Type", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsException_WhenIsExistAsyncThrows()
//        {
//            // Arrange
//            var stockMovementTypeId = 1;
//            var request = new StockMovementTypeRequest
//            {
//                Name = "Test",
//                Description = "Test Description",
//                Direction = 1
//            };

//            var validationResult = new ValidationResult();
//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _repositoryMock.Setup(r => r.IsExistAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>()
//            )).ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(stockMovementTypeId, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in UpdateAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsException_WhenFindAsyncThrows()
//        {
//            // Arrange
//            var stockMovementTypeId = 1;
//            var request = new StockMovementTypeRequest
//            {
//                Name = "Test",
//                Description = "Test Description",
//                Direction = 1
//            };

//            var validationResult = new ValidationResult();
//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _repositoryMock.Setup(r => r.IsExistAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>()
//            )).ReturnsAsync(false);

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(stockMovementTypeId, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in UpdateAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsException_WhenSaveChangesThrows()
//        {
//            // Arrange
//            var stockMovementTypeId = 1;
//            var request = new StockMovementTypeRequest
//            {
//                Name = "Test",
//                Description = "Test Description",
//                Direction = 1
//            };

//            var existingStockMovementType = new StockMovementType
//            {
//                Id = stockMovementTypeId,
//                Name = "Old Name",
//                Description = "Old description",
//                Direction = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false
//            };

//            var validationResult = new ValidationResult();
//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _repositoryMock.Setup(r => r.IsExistAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>()
//            )).ReturnsAsync(false);

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync(existingStockMovementType);

//            _repositoryMock.Setup(r => r.Update(It.IsAny<StockMovementType>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
//                .ThrowsAsync(new Exception("Save error"));

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(stockMovementTypeId, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in UpdateAsync", result.ErrorMessage);
//            Assert.Contains("Save error", result.ErrorMessage);
//        }

//        #endregion

//        #region SoftDeleteAsync Tests (Inherited from DeleteService)

//        [Theory]
//        [InlineData(0)]
//        [InlineData(-1)]
//        public async Task SoftDeleteAsync_ReturnsInvalidId_WhenIdIsInvalid(int invalidId)
//        {
//            // Arrange
//            var service = CreateService();

//            // Act
//            var result = await service.SoftDeleteAsync(invalidId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task SoftDeleteAsync_ReturnsNotFound_WhenStockMovementTypeDoesNotExist()
//        {
//            // Arrange
//            var stockMovementTypeId = 1;

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync((StockMovementType)null!);

//            var service = CreateService();

//            // Act
//            var result = await service.SoftDeleteAsync(stockMovementTypeId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("StockMovementType", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task SoftDeleteAsync_ReturnsSuccess_WhenStockMovementTypeExists()
//        {
//            // Arrange
//            var stockMovementTypeId = 1;
//            var stockMovementType = new StockMovementType
//            {
//                Id = stockMovementTypeId,
//                Name = "Test Stock Movement Type",
//                Direction = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                DeletedAt = null,
//                DeletedByUserId = null
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync(stockMovementType);

//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(2);
//            _repositoryMock.Setup(r => r.Update(It.IsAny<StockMovementType>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var service = CreateService();

//            // Act
//            var result = await service.SoftDeleteAsync(stockMovementTypeId, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);

//            _repositoryMock.Verify(r => r.Update(It.Is<StockMovementType>(smt => 
//                smt.IsDeleted == true && 
//                smt.DeletedByUserId == 2 &&
//                smt.DeletedAt != null
//            )), Times.Once);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact]
//        public async Task SoftDeleteAsync_ReturnsFailure_WhenStockMovementTypeAlreadyDeleted()
//        {
//            // Arrange
//            var stockMovementTypeId = 1;
//            var stockMovementType = new StockMovementType
//            {
//                Id = stockMovementTypeId,
//                Name = "Test Stock Movement Type",
//                Direction = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = true, // Already deleted
//                DeletedAt = DateTime.UtcNow,
//                DeletedByUserId = 1
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync(stockMovementType);

//            var service = CreateService();

//            // Act
//            var result = await service.SoftDeleteAsync(stockMovementTypeId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Contains("StockMovementType is already deleted", result.ErrorMessage);

//            _repositoryMock.Verify(r => r.Update(It.IsAny<StockMovementType>()), Times.Never);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//        }

//        #endregion

//        #region Helper Methods and Edge Cases

//        [Fact]
//        public void StockMovementTypeService_Constructor_SetsPropertiesCorrectly()
//        {
//            // Arrange & Act
//            var service = CreateService();

//            // Assert
//            Assert.NotNull(service);
//        }

//        [Theory]
//        [InlineData("Purchase", "Stock received from suppliers", 1)]
//        [InlineData("Sale", "Stock sold to customers", 2)]
//        [InlineData("Transfer", "Stock moved between locations", 3)]
//        [InlineData("Adjustment", "Stock level corrections", 3)]
//        public async Task AddAsync_HandlesVariousStockMovementTypes(string name, string description, byte direction)
//        {
//            // Arrange
//            var request = new StockMovementTypeRequest
//            {
//                Name = name,
//                Description = description,
//                Direction = direction
//            };

//            var validationResult = new ValidationResult();
//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _repositoryMock.Setup(r => r.Add(It.IsAny<StockMovementType>()))
//                .Callback<StockMovementType>(smt => smt.Id = 1);

//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var createdStockMovementType = new StockMovementType
//            {
//                Id = 1,
//                Name = request.Name,
//                Description = request.Description,
//                Direction = request.Direction,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                CreatedByUser = new User { UserName = "testuser" }
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync(createdStockMovementType);

//            var service = CreateService();

//            // Act
//            var result = await service.AddAsync(request, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(name, result.Value.Name);
//            Assert.Equal(description, result.Value.Description);
//        }

//        [Fact]
//        public async Task UpdateAsync_AllowsSameNameForSameEntity()
//        {
//            // Arrange
//            var stockMovementTypeId = 1;
//            var request = new StockMovementTypeRequest
//            {
//                Name = "Stock In", // Same name as current
//                Description = "Updated description",
//                Direction = 1
//            };

//            var existingStockMovementType = new StockMovementType
//            {
//                Id = stockMovementTypeId,
//                Name = "Stock In", // Same name
//                Description = "Old description",
//                Direction = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false
//            };

//            var validationResult = new ValidationResult();
//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            // Setup IsExistAsync to check for different ID and same name (should return false for same entity)
//            _repositoryMock.Setup(r => r.IsExistAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>()
//            )).ReturnsAsync(false);

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync(existingStockMovementType);

//            _repositoryMock.Setup(r => r.Update(It.IsAny<StockMovementType>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var updatedStockMovementType = new StockMovementType
//            {
//                Id = stockMovementTypeId,
//                Name = request.Name,
//                Description = request.Description,
//                Direction = request.Direction,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                CreatedByUser = new User { UserName = "testuser" }
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync(updatedStockMovementType);

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(stockMovementTypeId, request, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(request.Description, result.Value.Description);
//        }

//        [Fact]
//        public async Task AddAsync_SetsDefaultValuesCorrectly()
//        {
//            // Arrange
//            var request = new StockMovementTypeRequest
//            {
//                Name = "Test Stock Movement Type",
//                Description = "Test Description",
//                Direction = 1
//            };

//            var validationResult = new ValidationResult();
//            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            StockMovementType capturedStockMovementType = null!;
//            _repositoryMock.Setup(r => r.Add(It.IsAny<StockMovementType>()))
//                .Callback<StockMovementType>(smt => 
//                {
//                    smt.Id = 1;
//                    capturedStockMovementType = smt;
//                });

//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var createdStockMovementType = new StockMovementType
//            {
//                Id = 1,
//                Name = request.Name,
//                Description = request.Description,
//                Direction = request.Direction,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                CreatedByUser = new User { UserName = "testuser" }
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<StockMovementType, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync(createdStockMovementType);

//            var service = CreateService();

//            // Act
//            var result = await service.AddAsync(request, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(capturedStockMovementType);
            
//            // Verify default values are set correctly
//            Assert.False(capturedStockMovementType.IsDeleted); // Should default to false
//            Assert.Equal(request.Name, capturedStockMovementType.Name);
//            Assert.Equal(request.Description, capturedStockMovementType.Description);
//            Assert.Equal(request.Direction, capturedStockMovementType.Direction);
//        }

//        [Fact]
//        public void Map_HandlesNullDescription()
//        {
//            // Arrange
//            var stockMovementType = new StockMovementType
//            {
//                Id = 1,
//                Name = "Test",
//                Description = null, // Null description
//                Direction = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                CreatedByUser = new User { UserName = "testuser" }
//            };

//            var service = CreateService();

//            // Act
//            var result = service.Map(stockMovementType);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Null(result.Description);
//        }

//        #endregion
//    }
//}