using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums;

public enum StockMovementTypeEnum
{
    PurchaseReceipt = 1,
    SalesShipment = 2,
    StockAdjustment = 3,
    TransferIn = 4,
    TransferOut = 5,
    ReturnFromCustomer = 6,
    ReturnToSupplier = 7,
    ManufacturingUse = 8,
    ProductionOutput = 9,
    CycleCountAdjustment = 10,
    DamageLoss = 11,
    PromotionalSample = 12,
    InitialStock = 13,
    StockDecreaseAdjustment = 14,
    StockIncreaseAdjustment = 15,
}
