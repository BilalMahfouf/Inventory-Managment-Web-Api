using Application.Abstractions.Repositories.Sales;
using Application.Results;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Sales
{
    public class SalesOrderItemRepository : BaseRepository<SalesOrderItem>
        , ISalesOrderItemRepository
    {
        public SalesOrderItemRepository(InventoryManagmentDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<object>> GetTopSellingProductsAsync(
            int? numberOfProducts
            , CancellationToken cancellationToken = default)
        {
            var innerQuery =
              from s in _context.SalesOrderItems
              join p in _context.Products on s.ProductId equals p.Id
              join so in _context.SalesOrders on s.SalesOrderId equals so.Id
              where so.SalesStatus == (byte)SalesOrderStatus.Completed
              group new { s, p } by new { p.Name, s.OrderedQuantity } into g
              select new
              {
                  g.Key.Name,
                  g.Key.OrderedQuantity,
                  TotalRevenue = g.Max
                  (x => x.s.LineAmount - (x.p.Cost * x.s.OrderedQuantity))
                 };

            var result =
                (from r in innerQuery
                 group r by r.Name into g
                 orderby g.Sum(x => x.TotalRevenue) descending
                 select new
                 {
                     Name = g.Key,
                     TotalSoldUnits = g.Sum(x => x.OrderedQuantity),
                     TotalRevenue = g.Sum(x => x.TotalRevenue)
                 });
            result = numberOfProducts is null ? result :
                result.Take(numberOfProducts.Value);

            return await result.ToListAsync(cancellationToken);
        }
        public async Task<decimal> GetTotalRevenuesAsync(
            CancellationToken cancellationToken = default)
        {
            var totalRevenues = await (from s in _context.SalesOrderItems
                                       join so in _context.SalesOrders on s.SalesOrderId equals so.Id
                                       join p in _context.Products on s.ProductId equals p.Id
                                       where so.SalesStatus == (byte)SalesOrderStatus.Completed
                                       select new
                                       {
                                           Revenue = s.LineAmount - (p.Cost * s.OrderedQuantity)
                                       }).SumAsync(x => x.Revenue, cancellationToken);
            return totalRevenues;
        }
    }
}
