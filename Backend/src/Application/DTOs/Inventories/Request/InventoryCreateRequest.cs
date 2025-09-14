using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inventories.Request
{
    public sealed record InventoryCreateRequest
    {
        public int ProductId { get; init; }
        public int LocationId { get; init; }
        public decimal QuantityOnHand { get; init; }
        public decimal ReorderLevel { get; init; }
        public decimal MaxLevel { get; init; }
    }
}
