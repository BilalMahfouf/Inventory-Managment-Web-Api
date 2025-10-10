# ConfirmationDialog Component - Complete Guide

## Overview

A **generic, reusable confirmation dialog** component for Create, Update, and Delete operations with different visual themes.

---

## Component API

### Props

| Prop          | Type     | Required | Default    | Description                                        |
| ------------- | -------- | -------- | ---------- | -------------------------------------------------- |
| `isOpen`      | boolean  | ✅ Yes   | -          | Controls dialog visibility                         |
| `onClose`     | function | ✅ Yes   | -          | Called when Cancel clicked or backdrop clicked     |
| `onConfirm`   | function | ✅ Yes   | -          | Called when Confirm button clicked                 |
| `type`        | string   | No       | `'delete'` | Dialog type: `'delete'`, `'update'`, or `'create'` |
| `title`       | string   | ✅ Yes   | -          | Dialog title (e.g., "Delete Product")              |
| `message`     | string   | ✅ Yes   | -          | Confirmation message text                          |
| `itemName`    | string   | No       | -          | Name of item being acted upon (optional)           |
| `confirmText` | string   | No       | Auto       | Text for confirm button (auto-set based on type)   |
| `cancelText`  | string   | No       | `'Cancel'` | Text for cancel button                             |
| `loading`     | boolean  | No       | `false`    | Show loading state on confirm button               |

---

## Visual Themes

### 🗑️ Delete Theme (Red)

- **Color:** Red (#dc2626)
- **Icon:** Trash icon
- **Background:** Light red header
- **Use for:** Deleting products, customers, orders, etc.

### 🔄 Update Theme (Blue)

- **Color:** Blue (#2563eb)
- **Icon:** Alert circle
- **Background:** Light blue header
- **Use for:** Updating product info, changing settings, etc.

### ✅ Create Theme (Green)

- **Color:** Green (#059669)
- **Icon:** Check circle
- **Background:** Light green header
- **Use for:** Creating new products, confirming additions, etc.

---

## Usage Examples

### 1. Delete Confirmation (Red Theme)

```jsx
import { useState } from 'react';
import ConfirmationDialog from '@/components/ui/ConfirmationDialog';
import { useToast } from '@/context/ToastContext';

const ProductList = () => {
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);
  const { showSuccess, showError } = useToast();

  const handleDeleteClick = product => {
    setSelectedProduct(product);
    setShowDeleteDialog(true);
  };

  const handleConfirmDelete = async () => {
    try {
      await deleteProduct(selectedProduct.id);
      showSuccess('Deleted!', `${selectedProduct.name} has been deleted.`);
      setShowDeleteDialog(false);
    } catch (error) {
      showError('Delete Failed', 'Could not delete product.');
    }
  };

  return (
    <>
      <button onClick={() => handleDeleteClick(product)}>Delete</button>

      <ConfirmationDialog
        isOpen={showDeleteDialog}
        onClose={() => setShowDeleteDialog(false)}
        onConfirm={handleConfirmDelete}
        type='delete'
        title='Delete Product'
        itemName={selectedProduct?.name}
        message='This action cannot be undone and will permanently remove this product from the system.'
      />
    </>
  );
};
```

### 2. Update Confirmation (Blue Theme)

```jsx
import { useState } from 'react';
import ConfirmationDialog from '@/components/ui/ConfirmationDialog';
import { useToast } from '@/context/ToastContext';

const EditProduct = () => {
  const [showUpdateDialog, setShowUpdateDialog] = useState(false);
  const [loading, setLoading] = useState(false);
  const { showSuccess, showError } = useToast();

  const handleSaveClick = () => {
    setShowUpdateDialog(true);
  };

  const handleConfirmUpdate = async () => {
    setLoading(true);
    try {
      await updateProduct(productData);
      showSuccess('Updated!', 'Product has been updated successfully.');
      setShowUpdateDialog(false);
    } catch (error) {
      showError('Update Failed', 'Could not update product.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <button onClick={handleSaveClick}>Save Changes</button>

      <ConfirmationDialog
        isOpen={showUpdateDialog}
        onClose={() => setShowUpdateDialog(false)}
        onConfirm={handleConfirmUpdate}
        type='update'
        title='Update Product'
        message='Are you sure you want to save these changes?'
        loading={loading}
      />
    </>
  );
};
```

### 3. Create Confirmation (Green Theme)

```jsx
import { useState } from 'react';
import ConfirmationDialog from '@/components/ui/ConfirmationDialog';
import { useToast } from '@/context/ToastContext';

const AddProduct = () => {
  const [showCreateDialog, setShowCreateDialog] = useState(false);
  const [loading, setLoading] = useState(false);
  const { showSuccess, showError } = useToast();

  const handleCreateClick = () => {
    // Validate form first
    if (validateForm()) {
      setShowCreateDialog(true);
    }
  };

  const handleConfirmCreate = async () => {
    setLoading(true);
    try {
      await createProduct(formData);
      showSuccess('Created!', 'Product has been created successfully.');
      setShowCreateDialog(false);
    } catch (error) {
      showError('Creation Failed', 'Could not create product.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <button onClick={handleCreateClick}>Create Product</button>

      <ConfirmationDialog
        isOpen={showCreateDialog}
        onClose={() => setShowCreateDialog(false)}
        onConfirm={handleConfirmCreate}
        type='create'
        title='Create Product'
        itemName={formData.productName}
        message='Are you sure you want to create this product?'
        loading={loading}
      />
    </>
  );
};
```

### 4. Custom Button Text

```jsx
<ConfirmationDialog
  isOpen={showDialog}
  onClose={() => setShowDialog(false)}
  onConfirm={handleConfirm}
  type='delete'
  title='Delete Product'
  message='This action cannot be undone.'
  confirmText='Yes, Delete It'
  cancelText='No, Keep It'
/>
```

### 5. Without Item Name

```jsx
<ConfirmationDialog
  isOpen={showDialog}
  onClose={() => setShowDialog(false)}
  onConfirm={handleConfirm}
  type='update'
  title='Save Changes'
  message='Are you sure you want to save your changes? This will update the product information.'
/>
```

---

## Complete Integration Example

Here's a full example showing how to integrate into a Product management page:

```jsx
import React, { useState } from 'react';
import ConfirmationDialog from '@/components/ui/ConfirmationDialog';
import { useToast } from '@/context/ToastContext';
import {
  deleteProduct,
  updateProduct,
  createProduct,
} from '@/services/products/productService';

const ProductManagement = () => {
  // Dialog states
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [showUpdateDialog, setShowUpdateDialog] = useState(false);
  const [showCreateDialog, setShowCreateDialog] = useState(false);

  // Selected product and form data
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [formData, setFormData] = useState({});

  // Loading states
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [updateLoading, setUpdateLoading] = useState(false);
  const [createLoading, setCreateLoading] = useState(false);

  const { showSuccess, showError } = useToast();

  // Delete handlers
  const handleDeleteClick = product => {
    setSelectedProduct(product);
    setShowDeleteDialog(true);
  };

  const handleConfirmDelete = async () => {
    setDeleteLoading(true);
    try {
      await deleteProduct(selectedProduct.id);
      showSuccess('Deleted!', `${selectedProduct.name} has been deleted.`);
      setShowDeleteDialog(false);
      // Refresh list
    } catch (error) {
      showError('Delete Failed', error.message);
    } finally {
      setDeleteLoading(false);
    }
  };

  // Update handlers
  const handleUpdateClick = () => {
    setShowUpdateDialog(true);
  };

  const handleConfirmUpdate = async () => {
    setUpdateLoading(true);
    try {
      await updateProduct(selectedProduct.id, formData);
      showSuccess('Updated!', 'Product has been updated.');
      setShowUpdateDialog(false);
      // Refresh data
    } catch (error) {
      showError('Update Failed', error.message);
    } finally {
      setUpdateLoading(false);
    }
  };

  // Create handlers
  const handleCreateClick = () => {
    if (validateForm()) {
      setShowCreateDialog(true);
    }
  };

  const handleConfirmCreate = async () => {
    setCreateLoading(true);
    try {
      await createProduct(formData);
      showSuccess('Created!', 'Product has been created.');
      setShowCreateDialog(false);
      // Clear form and refresh list
    } catch (error) {
      showError('Creation Failed', error.message);
    } finally {
      setCreateLoading(false);
    }
  };

  return (
    <div>
      {/* Your UI with buttons */}
      <button onClick={handleCreateClick}>Create Product</button>
      <button onClick={handleUpdateClick}>Update Product</button>
      <button onClick={() => handleDeleteClick(product)}>Delete</button>

      {/* Delete Confirmation Dialog */}
      <ConfirmationDialog
        isOpen={showDeleteDialog}
        onClose={() => setShowDeleteDialog(false)}
        onConfirm={handleConfirmDelete}
        type='delete'
        title='Delete Product'
        itemName={selectedProduct?.name}
        message='This action cannot be undone and will permanently remove this product from the system.'
        loading={deleteLoading}
      />

      {/* Update Confirmation Dialog */}
      <ConfirmationDialog
        isOpen={showUpdateDialog}
        onClose={() => setShowUpdateDialog(false)}
        onConfirm={handleConfirmUpdate}
        type='update'
        title='Update Product'
        itemName={selectedProduct?.name}
        message='Are you sure you want to save these changes?'
        loading={updateLoading}
      />

      {/* Create Confirmation Dialog */}
      <ConfirmationDialog
        isOpen={showCreateDialog}
        onClose={() => setShowCreateDialog(false)}
        onConfirm={handleConfirmCreate}
        type='create'
        title='Create Product'
        itemName={formData.productName}
        message='Are you sure you want to create this product?'
        loading={createLoading}
      />
    </div>
  );
};

export default ProductManagement;
```

---

## Callback Functions

### onClose (Cancel Button)

Called when:

- User clicks Cancel button
- User clicks backdrop (outside dialog)
- User presses Escape key (can be added)

```jsx
const handleClose = () => {
  console.log('Dialog closed/cancelled');
  setShowDialog(false);
  // Reset any temporary state if needed
};

<ConfirmationDialog
  onClose={handleClose}
  // ... other props
/>;
```

### onConfirm (Confirm Button)

Called when:

- User clicks the Confirm button (Delete/Update/Create)

```jsx
const handleConfirm = async () => {
  console.log('User confirmed the action');

  try {
    // Perform the operation
    await performOperation();

    // Close dialog on success
    setShowDialog(false);

    // Show success toast
    showSuccess('Success!', 'Operation completed.');
  } catch (error) {
    // Keep dialog open and show error
    showError('Failed', error.message);
  }
};

<ConfirmationDialog
  onConfirm={handleConfirm}
  // ... other props
/>;
```

---

## Best Practices

### ✅ DO:

1. **Use appropriate types** for different operations
   - `delete` for deletions
   - `update` for modifications
   - `create` for new items

2. **Provide clear messages**

   ```jsx
   message =
     'This action cannot be undone and will permanently remove this product.';
   ```

3. **Include item names** when possible

   ```jsx
   itemName={product.name}
   ```

4. **Show loading states** during async operations

   ```jsx
   loading = { isDeleting };
   ```

5. **Integrate with toast notifications**
   ```jsx
   showSuccess('Deleted!', 'Product removed successfully.');
   ```

### ❌ DON'T:

1. **Don't use generic messages**

   ```jsx
   // Bad
   message = 'Are you sure?';

   // Good
   message =
     'This action cannot be undone and will permanently remove this product.';
   ```

2. **Don't forget to handle loading states**

   ```jsx
   // Bad - button can be clicked multiple times
   const handleConfirm = async () => {
     await deleteProduct();
   };

   // Good - disable during operation
   const handleConfirm = async () => {
     setLoading(true);
     try {
       await deleteProduct();
     } finally {
       setLoading(false);
     }
   };
   ```

3. **Don't forget to close dialog on success**
   ```jsx
   const handleConfirm = async () => {
     await operation();
     setShowDialog(false); // Remember to close!
   };
   ```

---

## Accessibility Features

✅ **Keyboard Support:**

- Dialog has proper ARIA attributes
- Focus management
- Backdrop click closes dialog

✅ **Screen Reader Support:**

- `role="dialog"`
- `aria-modal="true"`
- `aria-labelledby` for title

✅ **Visual Feedback:**

- Clear color coding by operation type
- Loading spinners
- Disabled states

---

## Styling

The component uses **only Tailwind CSS** classes:

- No custom CSS files
- Fully responsive
- Consistent with your app's design system

### Color Scheme:

- **Delete:** Red (#dc2626)
- **Update:** Blue (#2563eb)
- **Create:** Green (#059669)

---

## File Location

```
src/components/ui/ConfirmationDialog.jsx
```

## Import

```jsx
import ConfirmationDialog from '@/components/ui/ConfirmationDialog';
```

---

## Testing Checklist

- [ ] Delete dialog shows red theme
- [ ] Update dialog shows blue theme
- [ ] Create dialog shows green theme
- [ ] Cancel button closes dialog
- [ ] Confirm button triggers onConfirm callback
- [ ] Backdrop click closes dialog
- [ ] Loading state disables buttons
- [ ] Item name displays correctly
- [ ] Custom button text works
- [ ] Dialog is centered on screen
- [ ] Responsive on mobile devices

---

## Summary

The `ConfirmationDialog` component is a **generic, reusable solution** for all confirmation needs:

✅ **Three themes:** Delete (red), Update (blue), Create (green)  
✅ **Two callbacks:** `onClose` (cancel) and `onConfirm` (confirm)  
✅ **Loading states:** Built-in support  
✅ **Fully responsive:** Works on all devices  
✅ **Accessible:** ARIA labels, keyboard support  
✅ **100% Tailwind:** No custom CSS needed

Use it anywhere you need user confirmation for critical actions!

---

_Component Created: October 9, 2025_  
_Status: Production Ready ✅_
