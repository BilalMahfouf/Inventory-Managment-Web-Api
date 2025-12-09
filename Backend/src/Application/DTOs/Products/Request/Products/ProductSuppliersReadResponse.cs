using Application.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products.Request.Products
{
    public sealed record ProductSuppliersReadResponse : BaseDeletableReadResponse
    {
        public int ProductId { get; init ; }
        public string ProductName { get; init; } = string.Empty;

        public int SupplierId { get; init ; }
        public string SupplierName { get; init ;} = string.Empty;

        public string SupplierProductCode {  get; init ;} = string.Empty;
        public decimal MinOrderQuantity { get; init; }
        public int LeadTimeDay { get; init; }

       

    }
}
