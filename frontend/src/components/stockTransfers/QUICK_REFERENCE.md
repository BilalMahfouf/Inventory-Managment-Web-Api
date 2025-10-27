# AddStockTransfer - Quick Reference Guide

## 🚀 Quick Start

### 1. Import the Component

```jsx
import { AddStockTransferButton } from '@/components/stockTransfers';
```

### 2. Add to Your Page

```jsx
<AddStockTransferButton
  onSuccess={data => {
    console.log('Transfer created!', data);
  }}
/>
```

## 📋 Component Tabs

### Tab 1: Product

- **Purpose**: Select the product to transfer
- **Required**: Yes
- **Action**: Search by Product ID

### Tab 2: Transfer Details

- **Purpose**: Define transfer route and quantity
- **Required**: From Location, To Location, Quantity
- **Optional**: Transfer Notes

### Tab 3: History

- **Purpose**: View transfer status and timeline
- **Mode**: Read-only (displays after creation)

## ✅ Quick Validation Checklist

Before submitting a transfer:

- [ ] Product selected
- [ ] From Location selected
- [ ] To Location selected
- [ ] From ≠ To Location
- [ ] Quantity > 0

## 🎨 Status Colors

| Status    | Color  | Meaning           |
| --------- | ------ | ----------------- |
| Pending   | Yellow | Awaiting approval |
| Approved  | Blue   | Ready to process  |
| InTransit | Purple | Being transferred |
| Completed | Green  | Successfully done |
| Cancelled | Gray   | User cancelled    |
| Rejected  | Red    | Approval denied   |
| Failed    | Red    | Process failed    |

## 🔧 Common Props

### AddStockTransfer

```jsx
<AddStockTransfer
  isOpen={true} // Show/hide modal
  onClose={() => {}} // Close handler
  onSuccess={data => {}} // Success callback
  transferId={0} // 0 = new, >0 = view
/>
```

### ProductInfoTab

```jsx
<ProductInfoTab
  selectedProduct={product} // Product object or null
  onProductSelect={setProduct} // Selection callback
  disabled={false} // Disable search
  showSearch={true} // Show/hide search
/>
```

### TransferDetailsTab

```jsx
<TransferDetailsTab
  locations={locationArray} // Available locations
  fromLocationId={5} // From location ID
  toLocationId={7} // To location ID
  quantity={100} // Transfer quantity
  selectedProduct={product} // For value calc
  notes='...' // Transfer notes
  onFromLocationChange={setFrom} // Callbacks
  onToLocationChange={setTo}
  onQuantityChange={setQty}
  onNotesChange={setNotes}
/>
```

## 📡 API Endpoints

```javascript
// Create Transfer
POST / api / stock - transfers;
Body: {
  (productId, fromLocationId, toLocationId, quantity);
}

// Get Locations
GET / api / locations / names;

// Get Product
GET / api / products / { id };
```

## 🔄 Service Functions

```javascript
import {
  createStockTransfer,
  getAllStockTransfers,
  updateStockTransferStatus,
  getStockTransferById,
} from '@/services/stock/stockTransferService';

// Create
await createStockTransfer({
  productId,
  fromLocationId,
  toLocationId,
  quantity,
});

// Get All
await getAllStockTransfers({ page, pageSize, sortColumn, sortOrder, search });

// Update Status
await updateStockTransferStatus(transferId, 'Approved');

// Get By ID
await getStockTransferById(transferId);
```

## 🎯 Transfer Flow

```
1. Click "Add Stock Transfer"
   ↓
2. Search & Select Product (Tab 1)
   ↓
3. Select From/To Locations (Tab 2)
   ↓
4. Enter Quantity & Notes (Tab 2)
   ↓
5. Review Transfer Summary (Tab 2)
   ↓
6. Click "Create Transfer"
   ↓
7. View History (Tab 3) - After creation
```

## 💡 Tips & Tricks

### Keyboard Shortcuts

- `Enter` in search field → Search product
- `Esc` → Close modal (with confirmation if dirty)

### Best Practices

1. Validate locations are different
2. Check available stock before transfer
3. Add meaningful notes for audit trail
4. Review summary before submitting

### Reusability

```jsx
// Use individual tabs in other forms
import { ProductInfoTab } from '@/components/stockTransfers';

<ProductInfoTab selectedProduct={product} onProductSelect={setProduct} />;
```

## ⚠️ Common Errors

| Error                             | Cause                 | Solution                    |
| --------------------------------- | --------------------- | --------------------------- |
| "Please select a product"         | No product selected   | Search and select a product |
| "Locations must be different"     | Same from/to location | Choose different locations  |
| "Quantity must be greater than 0" | Invalid quantity      | Enter valid quantity > 0    |
| "Failed to load locations"        | API error             | Check backend connection    |

## 🎨 Customization

### Modify Transfer Summary

Edit `TransferDetailsTab.jsx` → Transfer Summary section

### Change Status Colors

Edit `HistoryTab.jsx` → `getStatusColor()` function

### Add Custom Validation

Edit `AddStockTransfer.jsx` → `isFormValid()` function

## 📱 Mobile Responsive

- Tabs stack vertically on mobile
- Inputs use full width
- Touch-friendly button sizes
- Scrollable content area

## 🔗 Related Components

- `StockTransferDataTable` - View all transfers
- `AddStockTransferButton` - Quick access button
- `ProductInfoTab` - Reusable product selector
- `TransferDetailsTab` - Reusable transfer form
- `HistoryTab` - Reusable history display

## 📚 File Locations

```
frontend/src/
├── components/stockTransfers/
│   ├── AddStockTransfer.jsx
│   ├── ProductInfoTab.jsx
│   ├── TransferDetailsTab.jsx
│   ├── HistoryTab.jsx
│   ├── AddStockTransferButton.jsx
│   └── index.js
└── services/stock/
    └── stockTransferService.js
```

---

**Need More Help?** Check `README.md` in the same folder for complete documentation.
