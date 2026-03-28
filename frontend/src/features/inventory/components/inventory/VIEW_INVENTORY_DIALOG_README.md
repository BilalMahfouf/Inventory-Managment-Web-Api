# ViewInventoryDialog Component

A comprehensive dialog component for displaying detailed inventory information in a tabbed, read-only interface.

## ✨ Features

- ✅ **3 Tabs**: Product & Location Info, Stock Levels, and System Info
- ✅ **Two-Column Layout**: Product info on left, Location info on right (Tab 1)
- ✅ **Visual Stock Indicators**: Color-coded status badges and progress bars
- ✅ **Complete Audit Trail**: Shows creation and update information
- ✅ **Responsive Design**: Works on all screen sizes
- ✅ **Real-time Data**: Fetches inventory data from API on open

## 📋 Usage

### Basic Implementation

```jsx
import { useState } from 'react';
import ViewInventoryDialog from '@/components/inventory/ViewInventoryDialog';

function InventoryPage() {
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [selectedInventoryId, setSelectedInventoryId] = useState(null);

  const handleView = row => {
    setSelectedInventoryId(row.id);
    setViewDialogOpen(true);
  };

  return (
    <>
      <button onClick={() => handleView(inventoryData)}>View Inventory</button>

      <ViewInventoryDialog
        open={viewDialogOpen}
        onOpenChange={setViewDialogOpen}
        inventoryId={selectedInventoryId}
      />
    </>
  );
}
```

### Integration with DataTable

Already integrated! Click the **⋯** menu on any inventory row and select **"View"**.

## 🎯 Props

### Required Props

| Prop           | Type       | Description                        |
| -------------- | ---------- | ---------------------------------- |
| `open`         | `boolean`  | Controls dialog visibility         |
| `onOpenChange` | `function` | Callback when dialog state changes |
| `inventoryId`  | `number`   | Inventory ID to display            |

## 📊 Data Structure

The component fetches data from `/api/inventory/{id}` endpoint. Expected response:

```javascript
{
  id: 1,
  product: {
    id: 5,
    name: "Product Name",
    sku: "SKU-001",
    categoryName: "Electronics",
    unitOfMeasureName: "pcs"
  },
  location: {
    id: 2,
    name: "Warehouse A",
    address: "123 Main St",
    type: "Warehouse"
  },
  quantityOnHand: 150.00,
  reorderLevel: 50.00,
  maxLevel: 500.00,
  createdAt: "2024-01-01T00:00:00Z",
  createdByUserId: 1,
  createdByUserName: "admin",
  updatedAt: "2024-01-15T00:00:00Z",
  updatedByUserId: 1,
  updatedByUserName: "admin"
}
```

## 🎨 Tab Layouts

### Tab 1: Product & Location Info

**Two-column layout:**

**Left Column - Product Information (Blue theme):**

- Product Name
- SKU
- Category
- Unit of Measure

**Right Column - Location Information (Green theme):**

- Name
- Type
- Address
- Location ID

### Tab 2: Stock Levels

**Displays:**

- Stock Status Badge (In Stock / Low Stock / Out of Stock)
- Quantity on Hand (large display card)
- Available Stock (same as quantity on hand)
- Reorder Level (threshold indicator)
- Maximum Level (capacity indicator)
- Stock Percentage Bar (visual progress indicator)

**Color Coding:**

- 🟢 Green: In Stock (quantity > reorder level)
- 🟡 Yellow: Low Stock (quantity <= reorder level)
- 🔴 Red: Out of Stock (quantity = 0)

### Tab 3: System Info

**Displays:**

- System Identifiers
  - Inventory ID
  - Product ID
  - Location ID

- Audit Trail (Two-column layout)
  - **Created Information (Blue theme):**
    - Date & Time
    - By User Name
    - User ID
  - **Updated Information (Purple theme):**
    - Date & Time
    - By User Name
    - User ID

## 🎨 Design Principles

### Colors

- **Product Info**: Blue theme (`bg-blue-50`, `text-blue-900`)
- **Location Info**: Green theme (`bg-green-50`, `text-green-900`)
- **Stock Cards**: Gradient backgrounds with status colors
- **System Info**: Gray theme with blue/purple audit sections
- **Status Badges**: Green (In Stock), Yellow (Low Stock), Red (Out of Stock)

### Typography

- **Headers**: `text-lg font-semibold`
- **Labels**: `text-sm font-medium`
- **Values**: `font-medium` to `font-bold` depending on importance
- **IDs**: `font-mono text-xs` for system identifiers

### Layout

- **Dialog**: `max-w-4xl` width, `max-h-[90vh]` height
- **Grid**: Responsive 1-2 column grid (`grid-cols-1 md:grid-cols-2`)
- **Spacing**: Consistent `gap-6` between sections
- **Padding**: `p-6` for main sections, `p-4` for cards

## 📱 Responsive Design

- **Desktop (>768px)**: Two-column grid layout
- **Mobile (<768px)**: Single-column stacked layout
- **All sizes**: Scrollable content area with max height

## 🔍 Features by Tab

### Tab 1: Product & Location

- ✅ Side-by-side comparison
- ✅ Color-coded sections (Blue/Green)
- ✅ Complete product details
- ✅ Complete location details

### Tab 2: Stock Levels

- ✅ Real-time stock status
- ✅ Large, readable metrics
- ✅ Visual progress bar
- ✅ Stock percentage calculation
- ✅ Color-coded status indicators

### Tab 3: System Info

- ✅ All system identifiers
- ✅ Complete audit trail
- ✅ Creation information
- ✅ Update information
- ✅ User tracking

## 🚀 How to Test

1. Go to Inventory page
2. Click the **⋯** menu on any inventory row
3. Click **"View"**
4. Switch between tabs
5. Verify all data displays correctly
6. Click "Close" to dismiss

## 💡 Tips

1. **Automatic Data Fetch**: Data is fetched automatically when dialog opens
2. **Tab Reset**: Automatically returns to first tab when dialog closes
3. **Date Formatting**: Dates are formatted using `toLocaleString()` for user's locale
4. **Null Safety**: All fields safely handle null/undefined values with fallbacks
5. **Read-Only**: This is a view-only component, no editing capabilities

## 🎉 What's Different from Other Dialogs

1. **Three tabs** instead of four (no pricing tab)
2. **Two-column first tab** for product and location info
3. **Stock-focused second tab** with visual indicators
4. **System-focused third tab** with audit trail
5. **Inventory-specific metrics** (quantity, reorder, max level)

## 🛠️ Customization Options

### Change Tab Order

Edit the `tabs` array in `ViewInventoryDialog.jsx`:

```jsx
const tabs = [
  { id: 'info', label: 'Product & Location' },
  { id: 'stock', label: 'Stock Levels' },
  { id: 'system', label: 'System Info' },
];
```

### Change Colors

Modify Tailwind classes for different color scheme:

- Replace `blue` with your preferred color for product info
- Replace `green` with your preferred color for location info
- Replace status colors as needed

### Add More Fields

Add new sections in the respective tab content area. Follow the existing pattern:

```jsx
<div>
  <label className='block text-sm font-medium text-gray-700 mb-1'>
    Your Label
  </label>
  <p className='text-gray-900'>{yourData || '-'}</p>
</div>
```

## 📚 Related Files

- `InventoryDataTable.jsx` - Already integrated with View dialog
- `AddUpdateInventory.jsx` - Edit functionality
- `inventoryService.js` - API service with `getInventoryById`

## ✅ Integration Checklist

- [x] Component created
- [x] Integrated with DataTable
- [x] API service method exists
- [x] Proper error handling
- [x] Responsive design
- [x] Null-safe implementation
- [x] Documentation complete

## 🎊 Ready to Use!

The component is fully integrated and ready to use. Just click the View option in any inventory row's action menu.
