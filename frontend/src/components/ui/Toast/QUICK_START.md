# 🚀 Quick Start Guide - Toast System

## ⚡ Setup in 3 Steps

### Step 1: Add ToastContainer (One Time Setup)

Open your main `App.jsx` or `MainLayout.jsx`:

```jsx
import useToast from '@/hooks/useToast';
import ToastContainer from '@/components/ui/Toast/ToastContainer';

function App() {
  const { toasts, removeToast } = useToast();

  return (
    <>
      {/* Your existing app content */}
      <Routes>{/* Your routes */}</Routes>

      {/* Add this at the end - ONE TIME ONLY */}
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

function YourComponent() {
  const { showSuccess, showError } = useToast();

  const handleAction = async () => {
    try {
      await doSomething();
      showSuccess('Success!', 'Operation completed.');
    } catch (error) {
      showError('Failed!', 'Something went wrong.');
    }
  };

  return <button onClick={handleAction}>Do Something</button>;
}
```

### Step 3: Done! ✅

That's it! You can now use toasts anywhere in your app.

---

## 📝 Common Use Cases

### Product Operations

```jsx
// Create
showSuccess('Product Created Successfully', 'AirPods Pro has been added.');

// Update
showSuccess('Product Updated Successfully');

// Delete
showSuccess('Product Deleted', 'The product has been removed.');

// Error
showError('Operation Failed', 'Product could not be updated.');
```

### Form Validation

```jsx
if (!formData.name) {
  showError('Validation Error', 'Product name is required.');
  return;
}

if (formData.costPrice > formData.sellingPrice) {
  showWarning('Price Warning', 'Cost exceeds selling price.');
}
```

### Network Errors

```jsx
try {
  await api.call();
} catch (error) {
  if (error.code === 'NETWORK_ERROR') {
    showError('Network Error', 'Check your internet connection.');
  } else {
    showError('Something Went Wrong', 'Please try again.');
  }
}
```

---

## 🎨 All Toast Types

```jsx
const { showSuccess, showError, showWarning, showInfo } = useToast();

showSuccess('Success!', 'Operation completed.'); // ✅ Green
showError('Error!', 'Something went wrong.'); // ❌ Red
showWarning('Warning!', 'Please review.'); // ⚠️ Orange
showInfo('Info', 'New features available.'); // ℹ️ Blue
```

---

## ⚙️ Options

### Custom Duration

```jsx
// Quick toast (2 seconds)
showSuccess('Saved!', '', { duration: 2000 });

// Standard (5 seconds - default)
showSuccess('Product Created');

// No auto-dismiss (manual close only)
showError('Critical Error', 'Contact support.', { duration: null });
```

### Hide Close Button

```jsx
showInfo('Processing', 'Please wait...', { showClose: false });
```

---

## 💡 Integration with AddProduct

Here's how to add toasts to your `AddProduct.jsx`:

```jsx
import useToast from '@/hooks/useToast';

const AddProduct = ({ isOpen, onClose, productId = 0 }) => {
  const { showSuccess, showError, showWarning } = useToast();
  const [formData, setFormData] = useState({});

  const handleSubmit = async () => {
    // Validation
    if (!formData.productName) {
      showError('Validation Error', 'Product name is required.');
      return;
    }

    if (!formData.sku) {
      showError('Validation Error', 'SKU is required.');
      return;
    }

    // Warning for negative margin
    if (formData.costPrice > formData.sellingPrice) {
      showWarning(
        'Price Warning',
        'Cost price exceeds selling price. Profit margin will be negative.'
      );
    }

    try {
      if (mode === 'add') {
        await addNewProduct(formData);
        showSuccess(
          'Product Created Successfully',
          `${formData.productName} has been added to your inventory.`
        );
        onClose();
      } else {
        await editProduct(productId, formData);
        showSuccess(
          'Product Updated Successfully',
          `${formData.productName} has been updated.`
        );
      }
    } catch (error) {
      const action = mode === 'add' ? 'create' : 'update';
      showError(
        `Product ${action === 'add' ? 'Creation' : 'Update'} Failed`,
        error.message || `Could not ${action} the product. Please try again.`
      );
    }
  };

  return (
    <div>
      {/* Your form */}
      <Button onClick={handleSubmit}>
        {mode === 'add' ? 'Add Product' : 'Save Changes'}
      </Button>
    </div>
  );
};
```

---

## 🎯 Works with Everything!

Not just products - use with:

- ✅ Products
- ✅ Orders
- ✅ Customers
- ✅ Inventory
- ✅ Sales
- ✅ Suppliers
- ✅ Settings
- ✅ Any operation!

---

## 📊 Quick Reference

| Method          | When to Use         | Color     |
| --------------- | ------------------- | --------- |
| `showSuccess()` | Operation succeeded | 🟢 Green  |
| `showError()`   | Operation failed    | 🔴 Red    |
| `showWarning()` | Important warning   | 🟠 Orange |
| `showInfo()`    | General info        | 🔵 Blue   |

---

## 🐛 Troubleshooting

**Toasts not showing?**

- Make sure `ToastContainer` is added to your App.jsx
- Check that you imported `useToast` correctly

**Wrong position?**

```jsx
<ToastContainer position='top-right' /> // Change here
```

---

## ✅ Checklist

- [ ] Added `ToastContainer` to App.jsx
- [ ] Imported `useToast` in your component
- [ ] Used `showSuccess` or `showError` in your functions
- [ ] Tested it works!

---

## 📚 Full Documentation

See [README.md](./README.md) for complete documentation with more examples.

---

**That's it! Start using toasts in your app now! 🎉**
