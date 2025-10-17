# AddUpdateInventory - Quick Reference

## 🚀 Quick Start

```jsx
import { AddInventoryButton } from '@/components/inventory';

<AddInventoryButton onSuccess={() => refreshData()} />;
```

## 📋 Props

| Prop        | Type     | Required | Description          |
| ----------- | -------- | -------- | -------------------- |
| isOpen      | boolean  | ✅       | Modal visibility     |
| onClose     | function | ✅       | Close callback       |
| inventoryId | number   | ❌       | 0 = Add, >0 = Update |
| onSuccess   | function | ❌       | Success callback     |

## 🎯 Modes

### Add Mode

```jsx
<AddUpdateInventory isOpen={true} onClose={() => {}} inventoryId={0} />
```

### Update Mode

```jsx
<AddUpdateInventory isOpen={true} onClose={() => {}} inventoryId={123} />
```

## 📑 Tabs

1. **Product** - Search by ID (name/SKU needs API)
2. **Location** - Select from dropdown
3. **Stock Levels** - Set quantity, reorder, max

## ✅ Validation Rules

- Product must be selected
- Location must be selected
- Stock levels ≥ 0
- Reorder level ≤ Max level

## 🔧 API Integration (TODO)

```javascript
// Add to inventoryService.js:

export async function createInventory(data) {
  return await fetchWithAuth('/api/inventory', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

export async function getInventoryById(id) {
  return await fetchWithAuth(`/api/inventory/${id}`);
}

export async function updateInventory(id, data) {
  return await fetchWithAuth(`/api/inventory/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  });
}
```

## 🎨 Features

✅ Three-tab interface  
✅ Product search  
✅ Location selection  
✅ Stock level management  
✅ Real-time validation  
✅ Read-only in update mode (product/location)  
✅ Available stock display (update)  
✅ Toast notifications  
✅ Loading states  
✅ Responsive design

## ⚡ Common Use Cases

### In Inventory Page

```jsx
import { AddInventoryButton } from '@/components/inventory';

function InventoryPage() {
  return (
    <div>
      <AddInventoryButton onSuccess={() => refreshTable()} />
      <InventoryDataTable />
    </div>
  );
}
```

### With DataTable Edit

```jsx
const [dialogOpen, setDialogOpen] = useState(false);
const [inventoryId, setInventoryId] = useState(0);

const handleEdit = (row) => {
  setInventoryId(row.id);
  setDialogOpen(true);
};

<DataTable onEdit={handleEdit} />
<AddUpdateInventory
  isOpen={dialogOpen}
  onClose={() => setDialogOpen(false)}
  inventoryId={inventoryId}
/>
```

## 🐛 Common Issues

**Save button disabled?**  
→ Select product AND location first

**Product search not working?**  
→ Use product ID (number only for now)

**Update mode error?**  
→ API integration pending (expected)

**No locations in dropdown?**  
→ Check API endpoint `/api/locations/names`

## 📖 Full Documentation

See `ADD_UPDATE_INVENTORY_GUIDE.md` for complete documentation.
