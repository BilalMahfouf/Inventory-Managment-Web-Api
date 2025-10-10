using Application.DTOs.Inventories;
using Application.DTOs.Products.Response.Products;
using Domain.Entities;
using Domain.Entities.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers.Util
{
    public  static class Utility
    {
        internal static string GenerateGuid()
        {
            var guid = Guid.NewGuid();
            return guid.ToString();
        }
        internal static string GenerateResponseLink(string email,string token,string uri)
        {
            var param = new Dictionary<string, string>
                {
                    {"token",token},
                    {"email",email}
                };
            string link = $"{uri}?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";
            return link;
        }
        
        internal static InventoryBaseReadResponse Map(Inventory inventory)
        {
            return new InventoryBaseReadResponse
            {
                Id = inventory.Id,
                ProductId = inventory.ProductId,
                ProductName = inventory.Product?.Name ?? string.Empty,
                LocationId = inventory.LocationId,
                LocationName = inventory.Location?.Name ?? string.Empty,
                QuantityOnHand = inventory.QuantityOnHand,
                ReorderLevel = inventory.ReorderLevel,
                MaxLevel = inventory.MaxLevel
            };
        }

        public static ProductReadResponse MapToReadResponse(Product product)
        {
            return new ProductReadResponse
            {
                Id = product.Id,
                SKU = product.Sku,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                UnitOfMeasureId = product.UnitOfMeasureId,
                UnitOfMeasureName = product.UnitOfMeasure.Name,
                CostPrice = product.Cost,
                UnitPrice = product.UnitPrice,
                IsActive = product.IsActive,

                CreatedAt = product.CreatedAt,
                CreatedByUserId = product.CreatedByUserId,
                CreatedByUserName = product.CreatedByUser?.UserName,

                UpdatedAt = product.UpdatedAt,
                UpdatedByUserId = product.UpdatedByUserId,
                UpdatedByUserName = product.UpdatedByUser?.UserName,

                IsDeleted = product.IsDeleted,
                DeleteAt = product.DeletedAt,
                DeletedByUserId = product.DeletedByUserId,
                DeletedByUserName = product.DeletedByUser?.UserName,
            };
        }
    }
}
