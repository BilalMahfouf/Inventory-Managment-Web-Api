# Toast Notification System - Usage Guide

## ✅ Complete Implementation Summary

Your toast notification system has been successfully integrated into your Inventory Management System! Here's what was implemented:

### 📁 Files Created/Modified

1. **`src/components/Toast.jsx`** - Individual toast component
2. **`src/components/ToastContainer.jsx`** - Toast container with positioning
3. **`src/hooks/useToast.js`** - Custom hook for toast management
4. **`src/layout/Layout.jsx`** - ✅ Integrated ToastContainer (App-wide)
5. **`src/components/products/AddProduct.jsx`** - ✅ Integrated toast notifications

---

## 🚀 Quick Start - Using Toasts in Your Components

### Step 1: Import the Hook

```jsx
import useToast from '@/hooks/useToast';
```

### Step 2: Initialize in Your Component

```jsx
const MyComponent = () => {
  const { showSuccess, showError, showWarning, showInfo } = useToast();

  // Your component code...
};
```

### Step 3: Show Notifications

```jsx
// ✅ Success Notification
showSuccess('Product Updated Successfully', 'Your product has been updated.');

// ❌ Error Notification
showError(
  'Product Update Failed',
  "Product can't be updated. Please try again."
);

// ⚠️ Warning Notification
showWarning('Price Warning', 'Cost price exceeds selling price.');

// ℹ️ Info Notification
showInfo('Processing', 'Your request is being processed...');
```

---

## 📝 Real-World Example: AddProduct Component

Here's how it's currently used in `AddProduct.jsx`:

```jsx
import useToast from '@/hooks/useToast';

const AddProduct = ({ isOpen, onClose, productId = 0 }) => {
  const { showSuccess, showError, showWarning } = useToast();

  // ✅ On Successful Product Creation
  const addNewProduct = async productData => {
    try {
      const data = await createProduct(productData);

      if (data) {
        showSuccess(
          'Product Created Successfully',
          `${productData.name} has been added to your inventory.`
        );
        // ... rest of success logic
      }
    } catch (error) {
      showError(
        'Product Creation Failed',
        error.message || "Product can't be created. Please try again."
      );
    }
  };

  // ✅ On Successful Product Update
  const editProduct = async (id, productData) => {
    try {
      const data = await updateProduct(id, productData);

      if (data) {
        showSuccess(
          'Product Updated Successfully',
          `${productData.name} has been updated.`
        );
        // ... rest of success logic
      }
    } catch (error) {
      showError(
        'Product Update Failed',
        error.message || "Product can't be updated. Please try again."
      );
    }
  };

  // ⚠️ Validation Warning
  const saveProduct = () => {
    if (formData.costPrice > formData.sellingPrice) {
      showWarning(
        'Price Warning',
        'Cost price exceeds selling price. Profit margin will be negative.'
      );
    }
    // ... rest of save logic
  };
};
```

---

## 🎨 Toast Types & When to Use Them

| Type        | Method          | Color            | Use Case               | Example                          |
| ----------- | --------------- | ---------------- | ---------------------- | -------------------------------- |
| **Success** | `showSuccess()` | Green (#059669)  | Successful operations  | Product created, data saved      |
| **Error**   | `showError()`   | Red (#dc2626)    | Failed operations      | API errors, validation failures  |
| **Warning** | `showWarning()` | Orange (#d97706) | Cautionary messages    | Price warnings, low stock alerts |
| **Info**    | `showInfo()`    | Blue (#2563eb)   | Informational messages | Processing status, tips          |

---

## 🔧 API Reference

### useToast Hook

```jsx
const {
  toasts, // Array of current toasts
  showSuccess, // Show success toast
  showError, // Show error toast
  showWarning, // Show warning toast
  showInfo, // Show info toast
  removeToast, // Manually remove a toast
  clearAllToasts, // Clear all toasts
} = useToast();
```

### Show Methods Signature

```jsx
showSuccess(title, message, (duration = 5000));
showError(title, message, (duration = 5000));
showWarning(title, message, (duration = 5000));
showInfo(title, message, (duration = 5000));
```

**Parameters:**

- `title` (string) - Main heading of the toast
- `message` (string) - Detailed message text
- `duration` (number, optional) - How long to show toast in milliseconds (default: 5000ms / 5 seconds)

---

## 💡 Common Use Cases

### 1. Form Submission Success/Error

```jsx
const handleSubmit = async formData => {
  try {
    await submitForm(formData);
    showSuccess('Submitted!', 'Your form has been submitted successfully.');
  } catch (error) {
    showError('Submission Failed', error.message);
  }
};
```

### 2. Validation Errors

```jsx
const validateForm = () => {
  if (!formData.name) {
    showError('Validation Error', 'Product name is required.');
    return false;
  }

  if (!formData.price || formData.price <= 0) {
    showError('Validation Error', 'Please enter a valid price.');
    return false;
  }

  return true;
};
```

### 3. Business Logic Warnings

```jsx
const checkStock = quantity => {
  if (quantity < reorderLevel) {
    showWarning(
      'Low Stock Alert',
      `Only ${quantity} units remaining. Consider reordering.`
    );
  }
};
```

### 4. Delete Confirmation Success

```jsx
const handleDelete = async productId => {
  try {
    await deleteProduct(productId);
    showSuccess('Deleted!', 'Product has been removed from inventory.');
  } catch (error) {
    showError('Delete Failed', "Product couldn't be deleted.");
  }
};
```

### 5. Background Process Info

```jsx
const startExport = () => {
  showInfo(
    'Export Started',
    'Your data is being exported. This may take a few moments.'
  );
  // Start export process...
};
```

---

## 🎯 Best Practices

### ✅ DO:

- Use clear, concise titles
- Provide actionable messages
- Use appropriate toast types for different scenarios
- Keep messages user-friendly (avoid technical jargon)
- Use try-catch blocks for async operations

### ❌ DON'T:

- Don't show multiple toasts simultaneously for the same action
- Don't use generic messages like "Error" or "Success" alone
- Don't make messages too long (keep under 50 words)
- Don't use toasts for critical actions requiring user input (use dialogs instead)

---

## 🎨 Customization

### Changing Toast Duration

```jsx
// Show toast for 3 seconds
showSuccess('Quick Message', 'This will disappear faster.', 3000);

// Show toast for 10 seconds
showError('Important Error', 'Read this carefully!', 10000);
```

### Manually Removing Toasts

```jsx
const toastId = showInfo('Processing...', 'Please wait.');

// Later, remove it manually
removeToast(toastId);
```

### Clearing All Toasts

```jsx
// Clear all visible toasts at once
clearAllToasts();
```

---

## 📍 Toast Positioning

Toasts appear in the **top-right corner** of the screen with:

- Fixed positioning
- Stacked vertically with spacing
- Slide-in animation from the right
- Auto-dismiss after 5 seconds (default)

---

## 🐛 Troubleshooting

### Toast not showing?

1. **Check Layout.jsx**: Ensure `ToastContainer` is added:

   ```jsx
   <ToastContainer toasts={toasts} onRemoveToast={removeToast} />
   ```

2. **Verify Hook Usage**: Make sure you're calling the hook correctly:

   ```jsx
   const { showSuccess } = useToast();
   ```

3. **Check Console**: Look for JavaScript errors that might prevent rendering

### Multiple toasts appearing?

- Ensure you're not calling show methods multiple times
- Check if the component is re-rendering unexpectedly

---

## 🎓 Next Steps

Want to use toasts in other components? Follow this pattern:

1. **Import the hook**: `import useToast from '@/hooks/useToast';`
2. **Initialize in component**: `const { showSuccess, showError } = useToast();`
3. **Call in event handlers**: `showSuccess('Title', 'Message');`

### Suggested Components to Enhance:

- ✅ **AddProduct.jsx** - Already integrated!
- 📋 **InventoryPage.jsx** - Add for stock updates
- 🛒 **SalesPage.jsx** - Add for order confirmations
- 👥 **CustomersPage.jsx** - Add for customer operations
- ⚙️ **SettingsPage.jsx** - Add for settings updates

---

## 📚 Full Documentation

For more details, see:

- **`TOAST_SYSTEM_README.md`** - Complete technical documentation
- **`TOAST_QUICK_START.md`** - Implementation guide
- **`TOAST_IMPLEMENTATION_SUMMARY.md`** - Architecture overview

---

## ✨ Your Toast System is Ready!

Test it out by:

1. Running your app: `npm run dev`
2. Creating or editing a product in the Products page
3. Watch the toast notifications appear!

**Example Results:**

- ✅ "Product Created Successfully" - When adding a new product
- ✅ "Product Updated Successfully" - When editing a product
- ❌ "Product Creation Failed" - On API errors
- ⚠️ "Price Warning" - When cost > selling price

---

**Need Help?** Check the console for any errors or refer to the full documentation files.

Happy coding! 🚀
