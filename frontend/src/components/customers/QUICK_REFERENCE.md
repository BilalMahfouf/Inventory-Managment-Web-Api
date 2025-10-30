# Customer Components - Quick Reference Card

## 🚀 Quick Start (Copy & Paste)

### Option 1: Simple Button

```jsx
import { AddCustomerButton } from '@/components/customers';

<AddCustomerButton onSuccess={() => console.log('Added!')} />;
```

### Option 2: Full Control

```jsx
import { AddUpdateCustomer } from '@/components/customers';
const [open, setOpen] = useState(false);
const [id, setId] = useState(0);

<AddUpdateCustomer
  isOpen={open}
  onClose={() => setOpen(false)}
  customerId={id}
  onSuccess={() => console.log('Success!')}
/>;
```

## 📋 Props Reference

### AddUpdateCustomer

```typescript
{
  isOpen: boolean;        // Required - Show/hide dialog
  onClose: () => void;    // Required - Close handler
  customerId?: number;    // Optional - 0=add, >0=edit
  onSuccess?: () => void; // Optional - Success callback
}
```

### AddCustomerButton

```typescript
{
  onSuccess?: () => void; // Optional - Success callback
}
```

## 📐 Form Fields

### Required Fields (\*)

- Full name\*
- Email\*
- Phone\*
- Customer Type\*
- Street\*
- City\*
- State\*
- Zip Code\*
- Credit Limit\*

### Optional Fields

- Payment Terms (default: "Net 30")
- Credit Status (default: Active)

## 🎯 Modes

| Mode | customerId | Tabs                            | Action Button |
| ---- | ---------- | ------------------------------- | ------------- |
| Add  | 0          | Basic Info → Business           | Save Customer |
| Edit | >0         | Basic Info → Business → Summary | Save Changes  |

## 🌈 Validation

```
Email:     user@domain.com ✅
          user@domain     ✗

Credit:    5000.00 ✅
          -1000   ✗
          abc     ✗
```

## 🎨 Tabs Overview

```
Tab 1: Basic Info
  ├─ Personal Details (name, email, phone, type)
  └─ Address (street, city, state, zip)

Tab 2: Business
  ├─ Credit Limit ($)
  ├─ Credit Status (Active/On Hold/Suspended)
  └─ Payment Terms

Tab 3: Summary (Update only)
  ├─ General Information
  ├─ Address
  ├─ Business Information
  └─ System Information
```

## 📡 API Endpoints

```
GET    /api/customers/{id}         → Load customer
POST   /api/customers              → Create
PUT    /api/customers/{id}         → Update
GET    /api/customer-categories    → Load types
```

## 🎭 Integration Examples

### With DataTable

```jsx
const [editOpen, setEditOpen] = useState(false);
const [customerId, setCustomerId] = useState(0);

// In your edit handler
const handleEdit = row => {
  setCustomerId(row.id);
  setEditOpen(true);
};

// In your render
<AddUpdateCustomer
  isOpen={editOpen}
  onClose={() => setEditOpen(false)}
  customerId={customerId}
  onSuccess={() => refreshTable()}
/>;
```

### Standalone Page

```jsx
function CustomersPage() {
  return (
    <div>
      <h1>Customers</h1>
      <AddCustomerButton onSuccess={() => alert('Added!')} />
    </div>
  );
}
```

## 🐛 Quick Troubleshooting

| Issue                  | Fix                                       |
| ---------------------- | ----------------------------------------- |
| Categories not loading | Check `/api/customer-categories` endpoint |
| Data not refreshing    | Call `onSuccess` callback                 |
| Form not opening       | Check `isOpen` prop is true               |
| Fields not pre-filled  | Verify `customerId` is correct            |

## 📦 Import Paths

```javascript
// Individual imports
import { AddUpdateCustomer } from '@/components/customers';
import { AddCustomerButton } from '@/components/customers';
import { ViewCustomer } from '@/components/customers';

// Or import multiple
import {
  AddUpdateCustomer,
  AddCustomerButton,
  ViewCustomer,
} from '@/components/customers';
```

## 🎨 UI States

### Loading

```jsx
isLoading={true}  → Shows spinner, disables buttons
```

### Success

```jsx
onSuccess={() => {
  showSuccess('Customer saved!');
  refreshData();
}}
```

### Error

```jsx
// Automatic error handling with toast notifications
// Field errors shown inline
```

## 💾 Data Structure

### Send to API

```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "phone": "(123) 456-7890",
  "customerCategoryId": 1,
  "street": "123 Main St",
  "city": "Anytown",
  "state": "CA",
  "zipCode": "12345",
  "creditLimit": 5000.0,
  "creditStatus": 0,
  "paymentTerms": "Net 30"
}
```

## 🔢 Credit Status Values

```
0 = Active
1 = On Hold
2 = Suspended
```

## ⚡ Performance Tips

1. Don't mount dialog until needed
2. Use memoization for customer categories
3. Implement debouncing for search
4. Lazy load large lists

## 📝 Quick Testing

```bash
# Test Add Mode
1. Open dialog with customerId={0}
2. Fill all required fields
3. Navigate tabs
4. Save

# Test Edit Mode
1. Open dialog with customerId={123}
2. Verify data loads
3. Modify fields
4. Check Summary tab
5. Save

# Test Validation
1. Leave required fields empty
2. Enter invalid email
3. Enter negative credit limit
4. Verify error messages
```

## 🎯 Best Practices

✅ DO:

- Always provide `onSuccess` callback
- Refresh data after save
- Handle loading states
- Use toast notifications
- Test with real API

❌ DON'T:

- Forget to set `customerId={0}` for add mode
- Skip validation on backend
- Mount multiple dialogs simultaneously
- Ignore error handling

## 📞 Support

For detailed documentation, see:

- `CUSTOMER_COMPONENTS_README.md`
- `CUSTOMER_FORM_VISUAL_GUIDE.md`
- `INTEGRATION_EXAMPLE.md`
- `IMPLEMENTATION_SUMMARY.md`

---

**Quick Tip**: Start with `AddCustomerButton` for simplest integration!
