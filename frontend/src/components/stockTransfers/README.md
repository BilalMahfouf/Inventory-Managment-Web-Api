# AddStockTransfer Component - Complete Documentation

## 🚀 Overview

The `AddStockTransfer` component is a comprehensive modal dialog for managing stock transfers between warehouses. It features a modern, tabbed interface that guides users through the transfer creation process with clear visual feedback and validation.

## ✨ Features

- **3-Tab Interface**: Product Selection, Transfer Details, and History tracking
- **Real-time Validation**: Form validation with helpful error messages
- **Reusable Components**: Modular tab components for easy maintenance
- **Visual Route Display**: Clear visualization of transfer route between warehouses
- **Transfer Summary**: Real-time calculation of total transfer value
- **Toast Notifications**: Success and error feedback using the toast system
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **Loading States**: Proper loading indicators during API calls

## 📁 File Structure

```
frontend/src/components/stockTransfers/
├── AddStockTransfer.jsx          # Main modal component
├── ProductInfoTab.jsx             # Product search and selection tab
├── TransferDetailsTab.jsx         # Transfer route and quantity tab
├── HistoryTab.jsx                 # Transfer history and status tab
├── AddStockTransferButton.jsx     # Button to open the modal
├── StockTransferDataTable.jsx     # Data table for stock transfers
└── index.js                       # Export file for clean imports
```

## 🎯 Component Architecture

### 1. AddStockTransfer (Main Component)

The main modal component that orchestrates the entire stock transfer creation flow.

**Props:**

- `isOpen` (boolean): Controls modal visibility
- `onClose` (function): Callback when modal is closed
- `onSuccess` (function): Optional callback after successful transfer creation
- `transferId` (number): ID of transfer to view (0 for new transfer)

**State Management:**

- Product selection state
- Transfer details (from/to locations, quantity)
- Transfer notes
- Loading states
- Form validation

### 2. ProductInfoTab

Reusable component for product search and selection.

**Props:**

- `selectedProduct` (object): Currently selected product
- `onProductSelect` (function): Callback when product is selected
- `disabled` (boolean): Whether search is disabled
- `showSearch` (boolean): Whether to show search input (default: true)

**Features:**

- Product search by ID
- Product details display
- Empty state handling
- Loading state during search

### 3. TransferDetailsTab

Component for managing transfer route and quantity.

**Props:**

- `locations` (array): Array of available locations
- `fromLocationId` (number): Selected from location ID
- `toLocationId` (number): Selected to location ID
- `quantity` (number): Transfer quantity
- `onFromLocationChange` (function): Callback for from location change
- `onToLocationChange` (function): Callback for to location change
- `onQuantityChange` (function): Callback for quantity change
- `selectedProduct` (object): Selected product for value calculation
- `notes` (string): Transfer notes
- `onNotesChange` (function): Callback for notes change
- `disabled` (boolean): Whether inputs are disabled

**Features:**

- Warehouse selection dropdowns
- Visual route display with arrow
- Quantity input with validation
- Transfer notes textarea
- Real-time total value calculation
- Transfer summary section

### 4. HistoryTab

Read-only component for displaying transfer history and status.

**Props:**

- `transferData` (object): Stock transfer data
- `onStatusChange` (function): Callback for status change
- `canChangeStatus` (boolean): Whether status can be changed
- `mode` (string): Component mode ('add' or 'view')

**Features:**

- Transfer status display with color coding
- Transfer timeline visualization
- Creation date and user information
- Status change button (when enabled)

## 📖 Usage Examples

### Basic Usage

```jsx
import { AddStockTransferButton } from '@/components/stockTransfers';

function InventoryPage() {
  const handleSuccess = transferData => {
    console.log('Transfer created:', transferData);
    // Refresh your data table or show confirmation
  };

  return (
    <div>
      <AddStockTransferButton onSuccess={handleSuccess} />
    </div>
  );
}
```

### Manual Control

```jsx
import { useState } from 'react';
import { AddStockTransfer } from '@/components/stockTransfers';
import Button from '@/components/Buttons/Button';

function CustomComponent() {
  const [isOpen, setIsOpen] = useState(false);

  const handleSuccess = data => {
    console.log('Transfer created:', data);
    setIsOpen(false);
  };

  return (
    <>
      <Button onClick={() => setIsOpen(true)}>Create Transfer</Button>

      <AddStockTransfer
        isOpen={isOpen}
        onClose={() => setIsOpen(false)}
        onSuccess={handleSuccess}
      />
    </>
  );
}
```

### Using Individual Tab Components

```jsx
import { ProductInfoTab } from '@/components/stockTransfers';

function CustomForm() {
  const [product, setProduct] = useState(null);

  return (
    <ProductInfoTab
      selectedProduct={product}
      onProductSelect={setProduct}
      disabled={false}
      showSearch={true}
    />
  );
}
```

## 🔧 API Integration

### Backend Endpoints

The component uses the following API endpoints:

#### 1. Create Stock Transfer

```
POST /api/stock-transfers
```

**Request Body:**

```json
{
  "productId": 1,
  "fromLocationId": 5,
  "toLocationId": 7,
  "quantity": 100.0
}
```

**Response:**

```json
{
  "id": 123,
  "status": "Pending"
}
```

#### 2. Get Locations

```
GET /api/locations/names
```

**Response:**

```json
[
  { "id": 1, "name": "Main Warehouse" },
  { "id": 2, "name": "Secondary Warehouse" }
]
```

#### 3. Get Product by ID

```
GET /api/products/{id}
```

**Response:**

```json
{
  "id": 1,
  "name": "Product Name",
  "sku": "SKU-123",
  "categoryName": "Category",
  "unitPrice": 50.0,
  "isActive": true,
  "description": "Product description"
}
```

### Service Functions

Located in `frontend/src/services/stock/stockTransferService.js`:

```javascript
// Create a new stock transfer
const result = await createStockTransfer({
  productId: 1,
  fromLocationId: 5,
  toLocationId: 7,
  quantity: 100,
});

// Get all stock transfers (paginated)
const transfers = await getAllStockTransfers({
  page: 1,
  pageSize: 10,
  sortColumn: 'createdAt',
  sortOrder: 'desc',
  search: 'search term',
});

// Update transfer status
const updated = await updateStockTransferStatus(transferId, 'Approved');

// Get transfer by ID
const transfer = await getStockTransferById(transferId);
```

## 🎨 UI/UX Design

### Tab Navigation

The component uses a clean tab interface:

- **Product Tab**: Blue icon and text when active
- **Transfer Details Tab**: Contains the main form
- **History Tab**: Read-only information display

### Visual Elements

1. **Route Visualization**: Shows warehouse transfer route with arrow
2. **Transfer Summary**: Displays total value and item count
3. **Status Badges**: Color-coded status indicators
4. **Empty States**: Helpful placeholder messages

### Color Scheme

- **Primary**: Blue (#2563EB) - Active tabs, primary buttons
- **Success**: Green (#10B981) - Completed transfers, total value
- **Warning**: Yellow (#F59E0B) - Pending transfers
- **Error**: Red (#EF4444) - Rejected/Failed transfers
- **Info**: Blue (#3B82F6) - Information displays

## ✅ Form Validation

The component includes comprehensive validation:

1. **Product Selection**: Must select a product before proceeding
2. **From Location**: Required, must be selected
3. **To Location**: Required, must be selected and different from "From"
4. **Quantity**: Must be greater than 0
5. **Unsaved Changes**: Warns user before closing with unsaved data

### Validation Flow

```javascript
const isFormValid = () => {
  if (!selectedProduct) {
    showError('Validation Error', 'Please select a product');
    setActiveTab(0); // Navigate to Product tab
    return false;
  }
  if (!fromLocationId || !toLocationId) {
    showError('Validation Error', 'Please select locations');
    setActiveTab(1); // Navigate to Transfer Details tab
    return false;
  }
  if (fromLocationId === toLocationId) {
    showError('Validation Error', 'Locations must be different');
    return false;
  }
  if (quantity <= 0) {
    showError('Validation Error', 'Quantity must be greater than 0');
    return false;
  }
  return true;
};
```

## 🔄 State Management

### Form State Structure

```javascript
{
  // Product
  selectedProduct: {
    id: 1,
    name: "Product Name",
    sku: "SKU-123",
    categoryName: "Category",
    unitPrice: 50.00,
    isActive: true,
    description: "..."
  },

  // Transfer Details
  fromLocationId: "5",
  toLocationId: "7",
  quantity: 100,
  notes: "Transfer notes...",

  // UI State
  activeTab: 0,
  isLoading: false,
  mode: "add"
}
```

## 🚦 Status Types

Based on the `TransferStatus` enum from the backend:

1. **Pending** (1): Initial status, awaiting approval
2. **Approved** (2): Transfer approved, ready for processing
3. **InTransit** (3): Items are being transferred
4. **Completed** (4): Transfer successfully completed
5. **Cancelled** (5): Transfer cancelled by user
6. **Rejected** (6): Transfer rejected during approval
7. **Failed** (7): Transfer failed during processing

## 🎯 Future Enhancements

### Planned Features

1. **Bulk Transfer**: Support multiple products in one transfer
2. **Status Updates**: Allow changing transfer status from History tab
3. **Transfer Templates**: Save common transfer routes as templates
4. **Barcode Scanning**: Scan products for quick selection
5. **Inventory Check**: Show available stock at source location
6. **Transfer History**: View full timeline of status changes
7. **Print Transfer**: Generate printable transfer documents
8. **Email Notifications**: Notify relevant users of transfer events

### Backend Enhancements Needed

1. `GET /api/stock-transfers/{id}` - Get transfer details by ID
2. `PATCH /api/stock-transfers/{id}/status` - Update transfer status
3. `GET /api/products/search` - Search products by name/SKU
4. `GET /api/locations/{id}/inventory/{productId}` - Check stock levels

## 🐛 Troubleshooting

### Common Issues

**Issue**: "Failed to create stock transfer"

- **Solution**: Check that the backend API is running and accessible
- **Check**: Verify the product exists and locations are valid
- **Verify**: Ensure sufficient stock at the source location

**Issue**: Product search not working

- **Solution**: Currently only supports search by ID
- **Note**: Name/SKU search requires additional API endpoint

**Issue**: Locations not loading

- **Solution**: Check `/api/locations/names` endpoint
- **Verify**: User has proper authentication

## 📚 Dependencies

### Required Packages

```json
{
  "react": "^18.x",
  "lucide-react": "^0.x",
  "@radix-ui/react-dialog": "^1.x"
}
```

### Custom Dependencies

- `@/components/Buttons/Button` - Custom button component
- `@/components/ui/input` - Input component
- `@/context/ToastContext` - Toast notification system
- `@/lib/utils` - Utility functions (cn for className merging)

## 🔐 Security Considerations

1. **Authentication**: All API calls use `fetchWithAuth` with token validation
2. **Validation**: Server-side validation in addition to client-side
3. **Authorization**: Backend verifies user permissions
4. **Data Sanitization**: User inputs are sanitized before submission

## 📝 Best Practices

### Component Usage

1. Always provide `onSuccess` callback to update parent components
2. Handle errors gracefully with toast notifications
3. Validate form data before submission
4. Clear form state after successful creation
5. Provide loading states for better UX

### Code Quality

1. Use TypeScript for better type safety (future enhancement)
2. Follow React hooks best practices
3. Keep components focused and modular
4. Document props and behavior clearly
5. Write meaningful error messages

## 🎓 Learning Resources

- [React Documentation](https://react.dev/)
- [Radix UI Components](https://www.radix-ui.com/)
- [Lucide Icons](https://lucide.dev/)
- [TailwindCSS](https://tailwindcss.com/)

---

## 📞 Support

For issues or questions:

1. Check this documentation first
2. Review the component source code
3. Check the browser console for errors
4. Verify backend API responses

---

**Created**: October 23, 2025  
**Last Updated**: October 23, 2025  
**Version**: 1.0.0
