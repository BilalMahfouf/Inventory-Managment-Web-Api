# ProductViewDialog Component

A comprehensive dialog component for displaying detailed product information in a tabbed interface.

## Features

- ✅ **4 Tabs**: Basic Info, Pricing, Inventory, and Details
- ✅ **Real-time Calculations**: Automatic profit margin, markup, and profit per unit
- ✅ **Stock Status Indicators**: Visual alerts for low stock and out-of-stock situations
- ✅ **Responsive Design**: Works on all screen sizes
- ✅ **Complete Audit Trail**: Shows creation, update, and deletion information
- ✅ **No Brand Field**: Removed as per your data structure

## Installation

No additional installation required. The component uses existing UI components from your project.

## Basic Usage

```jsx
import { useState } from 'react';
import ProductViewDialog from '@/components/ui/ProductsTables/ProductViewDialog';

function ProductsPage() {
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);

  const handleView = product => {
    setSelectedProduct(product);
    setViewDialogOpen(true);
  };

  return (
    <>
      <button onClick={() => handleView(productData)}>View Product</button>

      <ProductViewDialog
        open={viewDialogOpen}
        onOpenChange={setViewDialogOpen}
        product={selectedProduct}
      />
    </>
  );
}
```

## Integration with DataTable

```jsx
import DataTable from '@components/DataTable/DataTable';
import ProductViewDialog from '@/components/ui/ProductsTables/ProductViewDialog';
import { useState } from 'react';

function ProductDataTable() {
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);

  const handleView = row => {
    setSelectedProduct(row);
    setViewDialogOpen(true);
  };

  return (
    <>
      <DataTable
        data={products}
        columns={columns}
        onView={handleView}
        onEdit={row => console.log('Edit', row)}
        onDelete={row => console.log('Delete', row)}
      />

      <ProductViewDialog
        open={viewDialogOpen}
        onOpenChange={setViewDialogOpen}
        product={selectedProduct}
      />
    </>
  );
}
```

## Props

### Required Props

| Prop           | Type       | Description                               |
| -------------- | ---------- | ----------------------------------------- |
| `open`         | `boolean`  | Controls dialog visibility                |
| `onOpenChange` | `function` | Callback when dialog state changes        |
| `product`      | `object`   | Product data object (see structure below) |

### Optional Props

| Prop          | Type       | Default | Description                          |
| ------------- | ---------- | ------- | ------------------------------------ |
| `inventory`   | `object`   | `null`  | Inventory data (see structure below) |
| `onDuplicate` | `function` | `null`  | Callback for duplicate action        |

## Product Object Structure

Based on your `ProductReadResponse`:

```javascript
{
  // Basic Information
  id: "uuid",
  sku: "P001",
  name: "Product Name",
  description: "Product description",
  categoryId: "uuid",
  categoryName: "Category Name",
  unitOfMeasureId: "uuid",
  unitOfMeasureName: "pcs",

  // Pricing
  costPrice: 10.00,
  unitPrice: 15.00,

  // Status
  isActive: true,

  // Audit Trail
  createdAt: "2024-01-01T00:00:00Z",
  createdByUserId: "uuid",
  createdByUserName: "admin",

  updatedAt: "2024-01-15T00:00:00Z",
  updatedByUserId: "uuid",
  updatedByUserName: "admin",

  // Deletion (if applicable)
  isDeleted: false,
  deleteAt: null,
  deletedByUserId: null,
  deletedByUserName: null
}
```

## Inventory Object Structure (Optional)

```javascript
{
  currentStock: 100,
  minimumStock: 10,
  maximumStock: 500,
  storageLocation: "Warehouse A"
}
```

## Full Example with All Features

```jsx
import { useState } from 'react';
import DataTable from '@components/DataTable/DataTable';
import ProductViewDialog from '@/components/ui/ProductsTables/ProductViewDialog';

export default function ProductDataTable() {
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);

  const handleView = row => {
    setSelectedProduct(row);
    setViewDialogOpen(true);
  };

  const handleDuplicate = product => {
    console.log('Duplicating product:', product);
    // Your duplicate logic here
  };

  return (
    <>
      <DataTable
        data={products}
        columns={columns}
        onView={handleView}
        onEdit={row => console.log('Edit', row)}
        onDelete={row => console.log('Delete', row)}
      />

      <ProductViewDialog
        open={viewDialogOpen}
        onOpenChange={setViewDialogOpen}
        product={selectedProduct}
        inventory={{
          currentStock: 100,
          minimumStock: 10,
          maximumStock: 500,
          storageLocation: 'Warehouse A - Shelf B3',
        }}
        onDuplicate={handleDuplicate}
      />
    </>
  );
}
```

## Features by Tab

### 1. Basic Info Tab

- Product name, SKU, category
- Unit of measure
- Description
- Active/Inactive status badges
- Stock status alerts

### 2. Pricing Tab

- Cost price and selling price
- **Auto-calculated metrics:**
  - Profit Margin: `((unitPrice - costPrice) / unitPrice) × 100`
  - Profit per Unit: `unitPrice - costPrice`
  - Markup: `((unitPrice - costPrice) / costPrice) × 100`

### 3. Inventory Tab

- Current stock level
- Minimum and maximum stock thresholds
- Unit of measurement
- Storage location
- **Smart stock status:**
  - "Out of Stock" (red) when current stock = 0
  - "Low Stock - Reorder Soon" (red) when current stock ≤ minimum
  - "In Stock" (green) otherwise

### 4. Details Tab

- **Creation information:**
  - Created date/time
  - Created by username
  - Created by user ID
- **Update information:**
  - Last updated date/time
  - Updated by username
  - Updated by user ID
- **System information:**
  - Product ID, Category ID, Unit of Measure ID
  - Deletion details (if deleted)

## Styling

The component uses Tailwind CSS classes and follows your existing design system:

- Colors: Blue for primary actions, Green for positive status, Red for warnings/errors
- Responsive grid layouts
- Hover states and transitions
- Status badges with icons

## Tips

1. **Without Inventory Data**: If you don't pass the `inventory` prop, the Inventory tab will show "No inventory information available"

2. **Duplicate Button**: Only shows if you provide the `onDuplicate` callback

3. **Date Formatting**: Dates are automatically formatted using `toLocaleString()` for user's locale

4. **Null Safety**: All fields safely handle null/undefined values with fallbacks

5. **Responsive**: Scrollable content area with max height constraint

## Common Issues

### Dialog doesn't close

Make sure you're properly managing the `open` state:

```jsx
const [open, setOpen] = useState(false);
<ProductViewDialog
  open={open}
  onOpenChange={setOpen} // This handles close
  product={product}
/>;
```

### Product data not showing

Ensure your product object matches the expected structure. Check console for errors.

### Tabs not switching

This is handled internally. No action needed from parent component.

## Browser Support

Works in all modern browsers that support:

- ES6+ JavaScript
- CSS Grid
- Flexbox
- Tailwind CSS

## Dependencies

- React 16.8+ (uses hooks)
- Radix UI Dialog (from your existing `@/components/ui/dialog`)
- Lucide React (for icons)
- Tailwind CSS
