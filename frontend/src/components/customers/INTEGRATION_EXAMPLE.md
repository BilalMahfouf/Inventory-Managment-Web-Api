# Customer Components Integration Example

## Complete Integration with CustomerDataTable

This example shows how to integrate the AddUpdateCustomer component with the existing CustomerDataTable.

### Step 1: Update CustomerDataTable.jsx

```jsx
import { useToast } from '@/context/ToastContext';
import useServerSideDataTable from '@/hooks/useServerSideDataTable';
import { useState } from 'react';
import { getCustomers } from '@/services/customers/customerService';
import DataTable from '../DataTable/DataTable';
import AddUpdateCustomer from './AddUpdateCustomer';
import Button from '@components/Buttons/Button';
import { UserPlus } from 'lucide-react';

export default function CustomerDataTable() {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [currentCustomerId, setCurrentCustomerId] = useState(0);
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [addDialogOpen, setAddDialogOpen] = useState(false);
  const { showSuccess, showError } = useToast();

  const fetchCustomers = async ({
    page,
    pageSize,
    search,
    sortOrder,
    sortColumn,
  }) => {
    const response = await getCustomers({
      page: page,
      pageSize: pageSize,
      sortColumn: sortColumn,
      sortOrder: sortOrder,
      search: search,
    });

    return response.data;
  };

  const handleView = row => {
    setCurrentCustomerId(row.id);
    setViewDialogOpen(true);
  };

  const handleEdit = row => {
    setCurrentCustomerId(row.id);
    setEditDialogOpen(true);
  };

  const handleAddNew = () => {
    setAddDialogOpen(true);
  };

  const handleSuccess = () => {
    // Refresh the table data
    tableProps.refresh();
  };

  const tableProps = useServerSideDataTable(fetchCustomers);

  return (
    <>
      {/* Add Customer Button */}
      <div className='mb-4'>
        <Button onClick={handleAddNew} LeftIcon={UserPlus}>
          Add Customer
        </Button>
      </div>

      {/* Data Table */}
      <DataTable
        data={tableProps.data}
        columns={defaultColumns}
        totalRows={tableProps.totalRows}
        pageIndex={tableProps.pageIndex}
        pageSize={tableProps.pageSize}
        onPageChange={tableProps.onPageChange}
        onPageSizeChange={tableProps.onPageSizeChange}
        onSortingChange={tableProps.onSortingChange}
        onFilterChange={tableProps.onFilterChange}
        loading={tableProps.loading}
        onView={handleView}
        onEdit={handleEdit}
        onDelete={row => {
          setCurrentCustomerId(row.id);
          setDeleteDialogOpen(true);
        }}
      />

      {/* Add Customer Dialog */}
      <AddUpdateCustomer
        isOpen={addDialogOpen}
        onClose={() => setAddDialogOpen(false)}
        customerId={0}
        onSuccess={handleSuccess}
      />

      {/* Edit Customer Dialog */}
      <AddUpdateCustomer
        isOpen={editDialogOpen}
        onClose={() => setEditDialogOpen(false)}
        customerId={currentCustomerId}
        onSuccess={handleSuccess}
      />
    </>
  );
}

const defaultColumns = [
  {
    accessorKey: 'name',
    header: 'Customer',
    cell: ({ row }) => (
      <div>
        <div className='font-medium'>{row.original.name}</div>
        <div className='text-sm text-gray-500'>{row.original.email}</div>
      </div>
    ),
  },
  {
    accessorKey: 'phone',
    header: 'Phone',
  },
  {
    accessorKey: 'customerCategoryName',
    header: 'Category',
  },
  {
    accessorKey: 'totalOrders',
    header: 'Total Orders',
    cell: ({ getValue }) => `${getValue().toFixed(2)}`,
  },
  {
    accessorKey: 'totalSpent',
    header: 'Total Spent',
    cell: ({ getValue }) => `$${getValue().toFixed(2)}`,
  },
  {
    accessorKey: 'isActive',
    header: 'IsActive',
    cell: ({ getValue }) => (
      <span className='inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800'>
        ✓ {getValue()}
      </span>
    ),
  },
  {
    accessorKey: 'createdAt',
    header: 'Created At',
    cell: ({ getValue }) => new Date(getValue()).toLocaleDateString(),
  },
];
```

### Step 2: Use in a Page Component

```jsx
// src/pages/CustomersPage.jsx
import React from 'react';
import CustomerDataTable from '@/components/customers/CustomerDataTable';
import { PageHeader } from '@/components/ui';

export default function CustomersPage() {
  return (
    <div className='p-6'>
      <PageHeader
        title='Customers'
        description='Manage your customer database'
      />

      <CustomerDataTable />
    </div>
  );
}
```

### Step 3: Standalone Usage Without DataTable

```jsx
// Example: CustomerManagementPage.jsx
import React, { useState } from 'react';
import { AddCustomerButton } from '@/components/customers';
import { useToast } from '@/context/ToastContext';

export default function CustomerManagementPage() {
  const { showSuccess } = useToast();

  const handleCustomerAdded = () => {
    showSuccess('Success', 'Customer has been added to the system');
    // Add any additional logic here (e.g., refresh data, navigate, etc.)
  };

  return (
    <div className='p-6'>
      <div className='flex justify-between items-center mb-6'>
        <h1 className='text-2xl font-bold'>Customer Management</h1>
        <AddCustomerButton onSuccess={handleCustomerAdded} />
      </div>

      {/* Your other content here */}
    </div>
  );
}
```

### Step 4: Advanced - Custom Add Button

If you need a custom styled button instead of using AddCustomerButton:

```jsx
import React, { useState } from 'react';
import AddUpdateCustomer from '@/components/customers/AddUpdateCustomer';

export default function MyCustomComponent() {
  const [isDialogOpen, setIsDialogOpen] = useState(false);

  return (
    <>
      {/* Your custom button */}
      <button
        onClick={() => setIsDialogOpen(true)}
        className='bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded'
      >
        Add New Customer
      </button>

      {/* The dialog component */}
      <AddUpdateCustomer
        isOpen={isDialogOpen}
        onClose={() => setIsDialogOpen(false)}
        customerId={0}
        onSuccess={() => {
          console.log('Customer added successfully!');
          // Your custom logic here
        }}
      />
    </>
  );
}
```

### Step 5: Edit Customer from Anywhere

```jsx
import React, { useState } from 'react';
import AddUpdateCustomer from '@/components/customers/AddUpdateCustomer';

export default function CustomerCard({ customerId }) {
  const [isEditOpen, setIsEditOpen] = useState(false);

  return (
    <div className='border rounded p-4'>
      {/* Card content */}
      <button
        onClick={() => setIsEditOpen(true)}
        className='text-blue-600 hover:underline'
      >
        Edit Customer
      </button>

      <AddUpdateCustomer
        isOpen={isEditOpen}
        onClose={() => setIsEditOpen(false)}
        customerId={customerId}
        onSuccess={() => {
          console.log('Customer updated!');
          // Refresh your customer data here
        }}
      />
    </div>
  );
}
```

## Testing the Integration

### Test Checklist

- [ ] **Add New Customer**
  - [ ] Click "Add Customer" button
  - [ ] Fill in all required fields in Basic Info tab
  - [ ] Click Next to go to Business tab
  - [ ] Verify Business fields have default values
  - [ ] Click "Save Customer"
  - [ ] Verify success toast appears
  - [ ] Verify customer appears in the table

- [ ] **Edit Existing Customer**
  - [ ] Click edit button on a customer row
  - [ ] Verify dialog opens in Update mode
  - [ ] Verify all fields are pre-filled with customer data
  - [ ] Verify Summary tab is visible
  - [ ] Navigate through all tabs
  - [ ] Make changes to some fields
  - [ ] Click "Save Changes"
  - [ ] Verify success toast appears
  - [ ] Verify changes are reflected in the table

- [ ] **View Customer Summary**
  - [ ] Open customer in edit mode
  - [ ] Navigate to Summary tab
  - [ ] Verify all customer information is displayed correctly
  - [ ] Verify fields are read-only
  - [ ] Click Close button

- [ ] **Validation**
  - [ ] Try to proceed without filling required fields
  - [ ] Verify error messages appear
  - [ ] Enter invalid email format
  - [ ] Verify email validation error
  - [ ] Enter negative credit limit
  - [ ] Verify credit limit validation error

- [ ] **Customer Categories**
  - [ ] Verify customer type dropdown is populated
  - [ ] Verify categories are fetched from backend
  - [ ] Select a category and save
  - [ ] Verify category is saved correctly

- [ ] **Navigation**
  - [ ] Click Cancel button
  - [ ] Verify dialog closes without saving
  - [ ] Click X button
  - [ ] Verify dialog closes without saving
  - [ ] Use Previous/Next buttons
  - [ ] Verify tab navigation works correctly

## Common Issues and Solutions

### Issue: Customer categories not loading

**Solution:** Ensure the backend endpoint `/api/customer-categories` is accessible and returns data in the expected format:

```json
[
  { "id": 1, "name": "Retail" },
  { "id": 2, "name": "Wholesale" }
]
```

### Issue: Customer data not refreshing after save

**Solution:** Make sure you're calling the `onSuccess` callback and refreshing your data:

```jsx
const handleSuccess = () => {
  tableProps.refresh(); // or refetch your data
};
```

### Issue: Form fields not clearing after add

**Solution:** The component handles this automatically. If you see issues, ensure you're setting `customerId={0}` for add mode.

### Issue: Update mode not loading data

**Solution:** Verify the backend endpoint `/api/customers/{id}` returns the complete customer object with all necessary fields.

## Best Practices

1. **Always provide onSuccess callback** to refresh your data after save operations
2. **Use customerId={0}** explicitly for add mode
3. **Handle loading states** in your parent component if needed
4. **Provide user feedback** using the toast notifications
5. **Validate on the backend** as well as frontend
6. **Test all tab transitions** to ensure smooth UX
7. **Test with real API data** to verify all fields map correctly

## Performance Tips

1. **Lazy load the dialog** - Don't mount it until needed
2. **Memoize customer categories** if they don't change often
3. **Debounce API calls** if implementing search in customer dropdown
4. **Use React.memo** for sub-components if re-renders are an issue
