# ViewStockTransfer Component

A comprehensive dialog component for displaying detailed stock transfer information in a tabbed interface. This component follows the established UI patterns and design system used throughout the application.

## Features

- **Tabbed Interface**: Four distinct tabs for organized information display
  - Transfer Details: Route, warehouses, and locations
  - Items: Products being transferred with quantities and costs
  - Tracking: Shipping information and progress timeline
  - History: Transfer creation and status change history

- **Data Fetching**: Automatically fetches transfer details from backend using transfer ID
- **Loading States**: Displays loading indicator while fetching data
- **Responsive Design**: Adapts to different screen sizes
- **Status Visualization**: Color-coded status badges and progress indicators
- **Null Safety**: Gracefully handles missing or incomplete data

## Installation

The component is already integrated into the stock transfers module.

## Basic Usage

```jsx
import { useState } from 'react';
import { ViewStockTransfer } from '@/components/stockTransfers';

function MyComponent() {
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [selectedTransferId, setSelectedTransferId] = useState(null);

  const handleViewTransfer = transferId => {
    setSelectedTransferId(transferId);
    setViewDialogOpen(true);
  };

  return (
    <>
      <button onClick={() => handleViewTransfer(123)}>View Transfer</button>

      <ViewStockTransfer
        open={viewDialogOpen}
        onOpenChange={setViewDialogOpen}
        transferId={selectedTransferId}
      />
    </>
  );
}
```

## Integration with DataTable

The component is integrated with `StockTransferDataTable`:

```jsx
import { useState } from 'react';
import { StockTransferDataTable } from '@/components/stockTransfers';

export default function StockTransfersPage() {
  return (
    <div>
      <h1>Stock Transfers</h1>
      <StockTransferDataTable />
      {/* ViewStockTransfer is already integrated */}
    </div>
  );
}
```

## Props

| Prop           | Type       | Required | Description                             |
| -------------- | ---------- | -------- | --------------------------------------- |
| `open`         | `boolean`  | Yes      | Controls dialog visibility              |
| `onOpenChange` | `function` | Yes      | Callback when dialog open state changes |
| `transferId`   | `number`   | Yes      | Stock Transfer ID to display            |

## Transfer Data Structure

The component expects the following data structure from the backend API:

```javascript
{
  id: number,
  transferNumber: string,        // e.g., "TR001"
  trackingNumber: string,         // e.g., "TRK-2024-001"
  status: string,                 // "Pending", "In Transit", "Completed", "Cancelled"

  // Route Information
  fromWarehouse: string,
  toWarehouse: string,
  fromLocation: string,
  toLocation: string,

  // Items
  product: string,                // Product name
  quantity: number,
  items: [                        // Optional array for multiple items
    {
      productName: string,
      requestedQuantity: number,
      shippedQuantity: number,
      receivedQuantity: number,
      unitCost: number,
      status: string
    }
  ],

  // Tracking
  estimatedDelivery: string,      // ISO date string
  transitTime: string,

  // Dates
  requestedDate: string,          // ISO date string
  approvedDate: string,
  shippedDate: string,
  receivedDate: string,
  createdAt: string,

  // User Information
  userName: string,
  createdByUserName: string,

  // Additional
  notes: string,
  totalValue: number,
  totalItems: number,

  // Optional History
  statusHistory: [
    {
      status: string,
      changedAt: string,
      changedBy: string,
      notes: string
    }
  ]
}
```

## Tab Details

### 1. Transfer Details Tab

Displays:

- Transfer route with visual arrow indicator
- From/To warehouses and locations
- Transfer notes

### 2. Items Tab

Displays:

- List of products being transferred
- Requested, shipped, and received quantities
- Unit costs
- Item status badges
- Transfer summary with total value

### 3. Tracking Tab

Displays:

- Tracking number
- Estimated delivery date
- Transit time
- Progress timeline with checkmarks:
  - Transfer Requested
  - Transfer Approved
  - Items Shipped
  - Awaiting Receipt / Items Received

### 4. History Tab

Displays:

- Creation details (date, time, user)
- Status change history (if available)
- Transfer ID and current status

## Styling

The component uses Tailwind CSS classes and follows the application's design system:

- **Primary Colors**: Blue for highlights and interactive elements
- **Status Colors**:
  - Green: Completed, Active
  - Blue: In Transit
  - Yellow: Pending
  - Red: Cancelled
  - Gray: Inactive, Neutral
- **Typography**: Consistent font sizes and weights
- **Spacing**: Proper padding and margins for readability
- **Borders**: Subtle borders for content separation

## Examples

### Example 1: Simple Integration

```jsx
import { ViewStockTransfer } from '@/components/stockTransfers';

function TransferList() {
  const [viewOpen, setViewOpen] = useState(false);
  const [transferId, setTransferId] = useState(null);

  return (
    <>
      <button
        onClick={() => {
          setTransferId(123);
          setViewOpen(true);
        }}
      >
        View Transfer #123
      </button>

      <ViewStockTransfer
        open={viewOpen}
        onOpenChange={setViewOpen}
        transferId={transferId}
      />
    </>
  );
}
```

### Example 2: With DataTable Row Click

```jsx
const handleRowClick = row => {
  setSelectedTransferId(row.id);
  setViewDialogOpen(true);
};

<DataTable data={transfers} columns={columns} onView={handleRowClick} />;
```

## API Integration

The component uses the `getStockTransferById` service:

```javascript
// services/stock/stockTransferService.js
import { getStockTransferById } from '@/services/stock/stockTransferService';

// Usage in component
const response = await getStockTransferById(transferId);
if (response.success) {
  setTransfer(response.data);
}
```

## Customization

### Change Dialog Size

Modify the `max-w-5xl` class in DialogContent:

```jsx
<DialogContent className='max-w-3xl max-h-[90vh]...'>
```

### Add Custom Tabs

Add to the `tabs` array:

```jsx
const tabs = [
  { id: 'details', label: 'Transfer Details' },
  { id: 'items', label: 'Items' },
  { id: 'tracking', label: 'Tracking' },
  { id: 'history', label: 'History' },
  { id: 'custom', label: 'Custom Tab' },
];
```

Then add the tab content in the render section.

## Common Issues

### Transfer not loading

- Ensure `transferId` is valid and not null
- Check backend API endpoint is accessible
- Verify authentication token is valid

### Status colors not showing

- Check that `status` field matches expected values
- Verify Tailwind CSS classes are properly configured

### Dialog not opening

- Ensure `open` prop is being set to `true`
- Check that `onOpenChange` callback is properly connected

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Dependencies

- `@radix-ui/react-dialog` - Dialog component
- `lucide-react` - Icon library
- `react` - Core framework
- Tailwind CSS - Styling

## Related Components

- `StockTransferDataTable` - Data table for stock transfers list
- `AddStockTransfer` - Form for creating new transfers
- `ProductViewDialog` - Similar dialog pattern for products

## Tips

1. **Always provide a valid transferId** - Component will show loading state without it
2. **Handle errors gracefully** - Consider adding error states for failed API calls
3. **Use consistent status values** - Ensure backend returns standardized status strings
4. **Test with real data** - Component adapts to missing data, but works best with complete information

## Future Enhancements

Potential improvements:

- Print/Export functionality
- Edit transfer from view dialog
- Inline status updates
- Document attachments
- Email notifications
- Real-time tracking updates

## Support

For issues or questions:

1. Check this documentation
2. Review similar components (ProductViewDialog, UnitOfMeasureView)
3. Consult the main application README
4. Contact the development team

---

**Last Updated**: October 25, 2025  
**Version**: 1.0.0  
**Author**: Development Team
