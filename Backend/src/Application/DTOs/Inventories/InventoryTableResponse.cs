using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inventories
{
    public class InventoryTableResponse
    {
        public string Sku { get; set; }= string.Empty;
        public string Product { get; set; }= string.Empty;
        public string Location { get; set; }= string.Empty;
        public decimal Quantity { get; set; }
        public decimal Reorder { get; set; }
        public decimal Max { get; set; }
        public string Status { get; set; }= string.Empty;
        public double StockPercentage { get; set; }
        public decimal PotentialProfit { get; set; }

    }
}
