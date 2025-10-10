# 🎯 Generic Toast Notification System

A complete, reusable toast notification system for your React application. Works with **any form or operation** - products, orders, customers, inventory, sales, etc.

## ✨ Features

✅ **100% Generic** - Works with any entity (products, orders, customers, etc.)  
✅ **4 Toast Types** - Success, Error, Warning, Info  
✅ **Auto-dismiss** - Configurable duration  
✅ **Manual Close** - Optional close button  
✅ **Tailwind CSS Only** - No custom CSS modifications  
✅ **Your Color Scheme** - Matches your app design  
✅ **Fully Documented** - Complete JSDoc comments  
✅ **Portal Rendering** - Doesn't affect layout  
✅ **Animations** - Smooth transitions using Tailwind

## 📁 Files Created

```
src/
├── components/ui/Toast/
│   ├── Toast.jsx              # Main toast component
│   └── ToastContainer.jsx     # Container for multiple toasts
└── hooks/
    └── useToast.js            # Custom hook for toast management
```

## 🚀 Quick Start (3 Minutes)

### Step 1: Add ToastContainer to Your App

Find your main `App.jsx` or `Layout.jsx` and add:

```jsx
import useToast from '@/hooks/useToast';
import ToastContainer from '@/components/ui/Toast/ToastContainer';

function App() {
  const { toasts, removeToast } = useToast();

  return (
    <>
      {/* Your existing app content */}
      <YourRoutes />

      {/* Add ToastContainer at the end */}
      <ToastContainer
        toasts={toasts}
        removeToast={removeToast}
        position='top-right'
      />
    </>
  );
}
```

### Step 2: Use in Any Component

```jsx
import useToast from '@/hooks/useToast';

function AddProduct() {
  const { showSuccess, showError } = useToast();

  const handleSubmit = async () => {
    try {
      await createProduct(data);
      showSuccess(
        'Product Created Successfully',
        'The product has been added to your inventory.'
      );
    } catch (error) {
      showError(
        'Product Creation Failed',
        'Could not create the product. Please try again.'
      );
    }
  };

  return <form onSubmit={handleSubmit}>...</form>;
}
```

## 📖 Complete Usage Guide

### All Toast Types

```jsx
const { showSuccess, showError, showWarning, showInfo } = useToast();

// Success (Green) - For successful operations
showSuccess('Product Created Successfully', 'AirPods Pro has been added.');

// Error (Red) - For failed operations
showError('Update Failed', 'Product could not be updated.');

// Warning (Orange) - For important warnings
showWarning('Low Stock', 'Only 2 units remaining.');

// Info (Blue) - For general information
showInfo('Processing', 'Your request is being processed.');
```

### Options

```jsx
// Custom duration (3 seconds instead of default 5)
showSuccess('Saved!', 'Changes saved.', { duration: 3000 });

// No auto-dismiss (stays until manually closed)
showError('Critical Error', 'Contact support.', { duration: null });

// Hide close button
showInfo('Processing', 'Please wait...', { showClose: false });
```

## 💡 Real-World Examples

### Example 1: Product Operations

```jsx
import useToast from '@/hooks/useToast';

function ProductForm() {
  const { showSuccess, showError } = useToast();

  // Create Product
  const handleCreate = async data => {
    try {
      await createProduct(data);
      showSuccess(
        'Product Created Successfully',
        `${data.name} has been added to your inventory.`
      );
    } catch (error) {
      showError(
        'Product Creation Failed',
        'Could not create the product. Please try again.'
      );
    }
  };

  // Update Product
  const handleUpdate = async (id, data) => {
    try {
      await updateProduct(id, data);
      showSuccess('Product Updated Successfully');
    } catch (error) {
      showError('Product Update Failed', 'Product could not be updated.');
    }
  };

  // Delete Product
  const handleDelete = async id => {
    try {
      await deleteProduct(id);
      showSuccess('Product Deleted', 'The product has been removed.');
    } catch (error) {
      showError('Product Delete Failed', 'Product could not be deleted.');
    }
  };

  return <form>...</form>;
}
```

### Example 2: Order Processing

```jsx
function OrderManager() {
  const { showSuccess, showError, showInfo } = useToast();

  const createOrder = async orderData => {
    try {
      const order = await api.createOrder(orderData);
      showSuccess(
        'Order Created Successfully',
        `Order #${order.id} has been confirmed.`
      );
    } catch (error) {
      showError('Order Creation Failed', 'Could not process your order.');
    }
  };

  const cancelOrder = async orderId => {
    try {
      await api.cancelOrder(orderId);
      showInfo('Order Cancelled', `Order #${orderId} has been cancelled.`);
    } catch (error) {
      showError('Cancellation Failed', 'Could not cancel the order.');
    }
  };

  return <div>...</div>;
}
```

### Example 3: Inventory Management

```jsx
function InventoryManager() {
  const { showSuccess, showError, showWarning } = useToast();

  const updateStock = async (productId, quantity) => {
    try {
      await api.updateStock(productId, quantity);
      showSuccess(
        'Stock Updated Successfully',
        `Inventory level adjusted by ${quantity} units.`
      );
    } catch (error) {
      showError('Stock Update Failed', 'Could not update inventory.');
    }
  };

  const checkLowStock = product => {
    if (product.stock <= product.minimumStock) {
      showWarning(
        'Low Stock Alert',
        `${product.name} is running low. Current: ${product.stock}`
      );
    }
  };

  return <div>...</div>;
}
```

### Example 4: Customer Management

```jsx
function CustomerForm() {
  const { showSuccess, showError } = useToast();

  const addCustomer = async customerData => {
    try {
      await api.createCustomer(customerData);
      showSuccess(
        'Customer Added Successfully',
        `${customerData.name} has been added to your customer list.`
      );
    } catch (error) {
      showError('Customer Creation Failed', error.message);
    }
  };

  return <form>...</form>;
}
```

### Example 5: Form Validation

```jsx
function FormWithValidation() {
  const { showError, showWarning } = useToast();

  const validateForm = data => {
    if (!data.name) {
      showError('Validation Error', 'Product name is required.');
      return false;
    }

    if (!data.sku) {
      showError('Validation Error', 'SKU is required.');
      return false;
    }

    if (data.costPrice > data.sellingPrice) {
      showWarning(
        'Price Warning',
        'Cost price is higher than selling price. This will result in a loss.'
      );
    }

    return true;
  };

  return <form>...</form>;
}
```

### Example 6: Bulk Operations

```jsx
function BulkActions() {
  const { showSuccess, showError, clearAllToasts } = useToast();

  const bulkDelete = async selectedIds => {
    try {
      clearAllToasts(); // Clear any existing toasts
      await api.bulkDelete(selectedIds);
      showSuccess(
        'Bulk Delete Complete',
        `${selectedIds.length} items deleted successfully.`
      );
    } catch (error) {
      showError('Bulk Delete Failed', 'Some items could not be deleted.');
    }
  };

  return <div>...</div>;
}
```

## 🎨 Toast Positions

```jsx
<ToastContainer
  toasts={toasts}
  removeToast={removeToast}
  position='top-right' // Choose position
/>
```

Available positions:

- `top-right` (default)
- `top-left`
- `bottom-right`
- `bottom-left`
- `top-center`
- `bottom-center`

## 🎨 Color Scheme

The toast system uses colors that match your app:

| Type    | Color  | Use Case                           |
| ------- | ------ | ---------------------------------- |
| Success | Green  | Successful operations              |
| Error   | Red    | Failed operations, critical errors |
| Warning | Orange | Warnings, non-critical issues      |
| Info    | Blue   | Information, status updates        |

## 📊 Hook API Reference

### useToast()

Returns an object with the following methods:

#### `showSuccess(title, message?, options?)`

Show a success toast (green)

**Parameters:**

- `title` (string) - Toast title (required)
- `message` (string) - Detailed message (optional)
- `options` (object) - Configuration options (optional)
  - `duration` (number|null) - Auto-dismiss time in ms (default: 5000)
  - `showClose` (boolean) - Show close button (default: true)

**Returns:** Toast ID (string)

```jsx
showSuccess('Product Created Successfully', 'AirPods Pro has been added.');
```

#### `showError(title, message?, options?)`

Show an error toast (red)

```jsx
showError('Update Failed', 'Product could not be updated.');
```

#### `showWarning(title, message?, options?)`

Show a warning toast (orange)

```jsx
showWarning('Low Stock', 'Only 2 units remaining.');
```

#### `showInfo(title, message?, options?)`

Show an info toast (blue)

```jsx
showInfo('Processing', 'Your request is being processed.');
```

#### `removeToast(id)`

Remove a specific toast by ID

```jsx
const toastId = showSuccess('Processing...');
// Later...
removeToast(toastId);
```

#### `clearAllToasts()`

Remove all active toasts

```jsx
clearAllToasts();
```

## ✅ Best Practices

### 1. Keep Titles Short

✅ Good: `"Product Created Successfully"`  
❌ Bad: `"The product has been successfully created and added to your inventory system"`

### 2. Provide Context in Messages

✅ Good: `"Product could not be updated. Check your network connection."`  
❌ Bad: `"Error 500"`

### 3. Use Appropriate Toast Types

- **Success** - Operation completed successfully
- **Error** - Operation failed or validation error
- **Warning** - Important information, but not critical
- **Info** - General information or status updates

### 4. Don't Spam Toasts

```jsx
// Bad: Multiple toasts at once
showSuccess('Step 1 done');
showSuccess('Step 2 done');
showSuccess('Step 3 done');

// Good: One summary toast
showSuccess('All Steps Complete', 'Process finished successfully.');
```

### 5. Consider Duration

- Short messages: 3000ms
- Standard: 5000ms (default)
- Important errors: null (manual close)

```jsx
showSuccess('Saved!', '', { duration: 3000 });
showError('Critical Error', 'Contact support.', { duration: null });
```

## 🔧 Advanced Usage

### Managing Multiple Toasts

```jsx
const { toasts, showSuccess, clearAllToasts } = useToast();

// Show multiple toasts
showSuccess('Step 1 Complete');
showSuccess('Step 2 Complete');

// Clear all at once
clearAllToasts();
```

### Getting Toast ID for Later Removal

```jsx
const toastId = showInfo('Processing', 'Please wait...', { duration: null });

// Later, when process completes
removeToast(toastId);
showSuccess('Complete!');
```

### Conditional Toasts

```jsx
const saveData = async (data, isDraft) => {
  try {
    await api.save(data);

    if (isDraft) {
      showInfo('Draft Saved', 'Your changes have been saved as draft.');
    } else {
      showSuccess('Published Successfully', 'Your content is now live.');
    }
  } catch (error) {
    showError('Save Failed', 'Could not save your changes.');
  }
};
```

## 🐛 Troubleshooting

### Toasts Not Showing?

1. ✅ Check `ToastContainer` is added to your App/Layout
2. ✅ Verify `removeToast` function is passed correctly
3. ✅ Make sure `useToast` hook is imported properly

### Multiple Toasts Stacking Weird?

Check the `position` prop on `ToastContainer`:

```jsx
<ToastContainer position='top-right' />
```

### Toast Not Auto-Dismissing?

Make sure you're not setting `duration: null`:

```jsx
// This will auto-dismiss
showSuccess('Title', 'Message');

// This won't auto-dismiss
showSuccess('Title', 'Message', { duration: null });
```

## 📦 Dependencies

- React 16.8+ (hooks)
- lucide-react (icons)
- Tailwind CSS (styling)
- react-dom (for portal)

## 🎯 Integration Examples

### With AddProduct Component

```jsx
import useToast from '@/hooks/useToast';

const AddProduct = ({ isOpen, onClose }) => {
  const { showSuccess, showError } = useToast();
  const [formData, setFormData] = useState({});

  const handleSubmit = async () => {
    // Validate
    if (!formData.productName) {
      showError('Validation Error', 'Product name is required.');
      return;
    }

    try {
      await createProduct(formData);
      showSuccess(
        'Product Created Successfully',
        `${formData.productName} has been added to your inventory.`
      );
      onClose();
    } catch (error) {
      showError(
        'Product Creation Failed',
        'Could not create the product. Please try again.'
      );
    }
  };

  return <form>...</form>;
};
```

## 🎉 You're All Set!

The toast system is ready to use in any component, for any operation!

### Quick Copy-Paste

```jsx
// Import in any component
import useToast from '@/hooks/useToast';

const { showSuccess, showError } = useToast();

// On success
showSuccess('Operation Successful', 'Your changes have been saved.');

// On error
showError('Operation Failed', 'Please try again.');
```

---

**No CSS modifications needed!** Everything uses Tailwind CSS classes that work with your existing setup.
