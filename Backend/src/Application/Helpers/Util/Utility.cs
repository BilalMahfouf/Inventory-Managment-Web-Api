using Application.DTOs.Inventories;
using Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers.Util
{
    internal static class Utility
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


    }
}
