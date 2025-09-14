using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inventories.Request
{
    public sealed record InventoryUpdateRequest
    {
        public decimal QuantityOnHand { get; init; }
        public decimal ReorderLevel { get; init; }
        public decimal MaxLevel { get; init; }
    }
}
