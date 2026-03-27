# ViewStockTransfer - Quick Reference

## Import

```jsx
import { ViewStockTransfer } from '@/components/stockTransfers';
```

## Basic Usage

```jsx
const [viewOpen, setViewOpen] = useState(false);
const [transferId, setTransferId] = useState(null);

<ViewStockTransfer
  open={viewOpen}
  onOpenChange={setViewOpen}
  transferId={transferId}
/>;
```

## Props

| Prop           | Type     | Required | Default |
| -------------- | -------- | -------- | ------- |
| `open`         | boolean  | ✓        | -       |
| `onOpenChange` | function | ✓        | -       |
| `transferId`   | number   | ✓        | -       |

## Tabs

1. **Transfer Details** - Route, warehouses, locations, notes
2. **Items** - Products, quantities, costs, summary
3. **Tracking** - Shipping info, progress timeline
4. **History** - Creation details, status changes

## Status Colors

| Status     | Color  | Badge                           |
| ---------- | ------ | ------------------------------- |
| Completed  | Green  | `bg-green-100 text-green-800`   |
| In Transit | Blue   | `bg-blue-100 text-blue-800`     |
| Pending    | Yellow | `bg-yellow-100 text-yellow-800` |
| Cancelled  | Red    | `bg-red-100 text-red-800`       |

## Example with DataTable

```jsx
import { useState } from 'react';
import { StockTransferDataTable } from '@/components/stockTransfers';

// Component is already integrated!
<StockTransferDataTable />;
```

## API Service

```javascript
// Already imported and used internally
import { getStockTransferById } from '@/services/stock/stockTransferService';
```

## Key Features

✓ Automatic data fetching  
✓ Loading states  
✓ Null safety  
✓ Responsive design  
✓ Status visualization  
✓ Progress timeline  
✓ Color-coded badges

## Common Patterns

### Open from button click

```jsx
<button
  onClick={() => {
    setTransferId(123);
    setViewOpen(true);
  }}
>
  View Transfer
</button>
```

### Open from DataTable row

```jsx
const handleView = row => {
  setTransferId(row.id);
  setViewOpen(true);
};

<DataTable onView={handleView} />;
```

## Customization

### Change dialog size

```jsx
// In ViewStockTransfer.jsx line 100
<DialogContent className='max-w-3xl max-h-[90vh]...'>
```

### Add custom tab

```jsx
// Add to tabs array
const tabs = [
  ...existing tabs,
  { id: 'custom', label: 'Custom' },
];
```

## Icons Used

- `Truck` - Main header
- `MapPin` - Transfer route
- `Package` - Items
- `Clock` - Tracking timeline
- `CheckCircle` - Completed steps
- `Calendar` - Dates
- `User` - User information
- `ArrowRight` - Route visualization

## Troubleshooting

| Issue               | Solution                                      |
| ------------------- | --------------------------------------------- |
| Not loading         | Check `transferId` is valid                   |
| Wrong data          | Verify API endpoint returns correct structure |
| Dialog won't open   | Ensure `open={true}` is set                   |
| Status colors wrong | Check status string matches expected values   |

## File Location

```
frontend/src/components/stockTransfers/view/ViewStockTransfer.jsx
```

## Related Files

- `StockTransferDataTable.jsx` - Integration point
- `stockTransferService.js` - API service
- `ProductViewDialog.jsx` - Similar pattern
- `UnitOfMeasureView.jsx` - Similar pattern

---

**Quick Tip**: This component follows the same pattern as ProductViewDialog. If you know how to use that, you already know this one!
