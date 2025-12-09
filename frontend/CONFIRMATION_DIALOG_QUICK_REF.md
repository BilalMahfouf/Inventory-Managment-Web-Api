# ConfirmationDialog - Quick Reference Card

## 📦 Import

```jsx
import ConfirmationDialog from '@/components/ui/ConfirmationDialog';
```

## 🎯 Basic Usage

### Delete (Red)

```jsx
<ConfirmationDialog
  isOpen={showDialog}
  onClose={() => setShowDialog(false)}
  onConfirm={handleDelete}
  type='delete'
  title='Delete Product'
  itemName='iPhone 15 Pro'
  message='This action cannot be undone.'
/>
```

### Update (Blue)

```jsx
<ConfirmationDialog
  isOpen={showDialog}
  onClose={() => setShowDialog(false)}
  onConfirm={handleUpdate}
  type='update'
  title='Update Product'
  message='Save these changes?'
/>
```

### Create (Green)

```jsx
<ConfirmationDialog
  isOpen={showDialog}
  onClose={() => setShowDialog(false)}
  onConfirm={handleCreate}
  type='create'
  title='Create Product'
  message='Add this product?'
/>
```

## 📋 Props

| Prop          | Type     | Required | Default    |
| ------------- | -------- | -------- | ---------- |
| `isOpen`      | boolean  | ✅       | -          |
| `onClose`     | function | ✅       | -          |
| `onConfirm`   | function | ✅       | -          |
| `type`        | string   | No       | `'delete'` |
| `title`       | string   | ✅       | -          |
| `message`     | string   | ✅       | -          |
| `itemName`    | string   | No       | -          |
| `confirmText` | string   | No       | Auto       |
| `cancelText`  | string   | No       | `'Cancel'` |
| `loading`     | boolean  | No       | `false`    |

## 🎨 Types

- `'delete'` → Red theme
- `'update'` → Blue theme
- `'create'` → Green theme

## 💡 Complete Example

```jsx
import { useState } from 'react';
import ConfirmationDialog from '@/components/ui/ConfirmationDialog';
import { useToast } from '@/context/ToastContext';

const MyComponent = () => {
  const [showDialog, setShowDialog] = useState(false);
  const [loading, setLoading] = useState(false);
  const { showSuccess, showError } = useToast();

  const handleConfirm = async () => {
    setLoading(true);
    try {
      await performAction();
      showSuccess('Success!', 'Action completed.');
      setShowDialog(false);
    } catch (error) {
      showError('Failed', error.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <button onClick={() => setShowDialog(true)}>Open Dialog</button>

      <ConfirmationDialog
        isOpen={showDialog}
        onClose={() => setShowDialog(false)}
        onConfirm={handleConfirm}
        type='delete'
        title='Delete Item'
        itemName='Product Name'
        message='This cannot be undone.'
        loading={loading}
      />
    </>
  );
};
```

## ✅ Callbacks

### onClose (Cancel)

- Called when Cancel clicked
- Called when backdrop clicked
- Close dialog and reset state

### onConfirm (OK)

- Called when Confirm clicked
- Perform the operation
- Show toast notification
- Close dialog on success

## 🎨 Visual Themes

```
DELETE (Red):
┌─────────────────────┐
│ 🔴 Delete Product   │ ← Red header
│    iPhone 15 Pro    │
├─────────────────────┤
│ Message here...     │
├─────────────────────┤
│  [Cancel] [Delete]  │ ← Red button
└─────────────────────┘

UPDATE (Blue):
┌─────────────────────┐
│ 🔵 Update Product   │ ← Blue header
│    iPhone 15 Pro    │
├─────────────────────┤
│ Message here...     │
├─────────────────────┤
│  [Cancel] [Update]  │ ← Blue button
└─────────────────────┘

CREATE (Green):
┌─────────────────────┐
│ ✅ Create Product   │ ← Green header
│    iPhone 15 Pro    │
├─────────────────────┤
│ Message here...     │
├─────────────────────┤
│  [Cancel] [Create]  │ ← Green button
└─────────────────────┘
```

## 🔧 Common Patterns

### With Loading

```jsx
const [loading, setLoading] = useState(false);

const handleConfirm = async () => {
  setLoading(true);
  try {
    await api.delete();
  } finally {
    setLoading(false);
  }
};

<ConfirmationDialog loading={loading} />;
```

### With Toast Integration

```jsx
const { showSuccess, showError } = useToast();

const handleConfirm = async () => {
  try {
    await api.delete();
    showSuccess('Deleted!', 'Item removed.');
  } catch (error) {
    showError('Failed', error.message);
  }
};
```

### Multiple Dialogs

```jsx
const [deleteDialog, setDeleteDialog] = useState(false);
const [updateDialog, setUpdateDialog] = useState(false);

<ConfirmationDialog
  isOpen={deleteDialog}
  onClose={() => setDeleteDialog(false)}
  type="delete"
  // ...
/>

<ConfirmationDialog
  isOpen={updateDialog}
  onClose={() => setUpdateDialog(false)}
  type="update"
  // ...
/>
```

## ⚠️ Common Mistakes

❌ **Don't forget to close dialog:**

```jsx
const handleConfirm = async () => {
  await api.delete();
  // Missing: setShowDialog(false);
};
```

✅ **Always close on success:**

```jsx
const handleConfirm = async () => {
  await api.delete();
  setShowDialog(false); // ✅
};
```

❌ **Don't forget loading state:**

```jsx
const handleConfirm = async () => {
  await api.delete(); // Can be clicked multiple times!
};
```

✅ **Use loading state:**

```jsx
const [loading, setLoading] = useState(false);

const handleConfirm = async () => {
  setLoading(true);
  try {
    await api.delete();
  } finally {
    setLoading(false);
  }
};
```

## 📱 Responsive

✅ Desktop: 448px max width, centered  
✅ Mobile: Full width minus 32px padding  
✅ Touch-friendly button sizes  
✅ Readable text on all screens

## ♿ Accessibility

✅ ARIA labels (`role="dialog"`, `aria-modal`)  
✅ Keyboard navigation support  
✅ Focus management  
✅ Screen reader friendly  
✅ High contrast colors

## 📍 File Location

```
src/components/ui/ConfirmationDialog.jsx
```

## 📚 Documentation

- **Full Guide:** `CONFIRMATION_DIALOG_GUIDE.md`
- **Visual Guide:** `CONFIRMATION_DIALOG_VISUAL_GUIDE.md`
- **Example:** `src/components/products/DeleteDialogExample.jsx`

---

**Status:** Production Ready ✅  
**Last Updated:** October 9, 2025
