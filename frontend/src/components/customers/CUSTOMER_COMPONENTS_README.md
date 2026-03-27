# Customer Management Components

This directory contains comprehensive components for managing customers in the Inventory Management System.

## Components Overview

### 1. AddUpdateCustomer

The main component for creating and editing customers. Features a multi-step tabbed interface.

**Features:**

- ✅ Multi-step form with 3 tabs: Basic Info, Business, Summary
- ✅ Create new customers (Add mode)
- ✅ Edit existing customers (Update mode)
- ✅ Fetches customer categories from backend
- ✅ Full validation on all fields
- ✅ Toast notifications for success/error
- ✅ Responsive design
- ✅ Loading states

**Tabs:**

- **Basic Info**: Personal details (name, email, phone, customer type) and address
- **Business**: Credit limit, credit status, payment terms (visible in both modes)
- **Summary**: Read-only view of all customer data (only in Update mode)

### 2. ViewCustomer

A read-only component displaying comprehensive customer information organized into sections.

**Sections:**

- General Information
- Address
- Business Information
- System Information

### 3. CustomerDataTable

The main data table for displaying and managing customers with server-side pagination.

### 4. AddCustomerButton

A convenient button component that opens the AddUpdateCustomer dialog in "add" mode.

## Usage Examples

### Basic Usage - Add Customer

```jsx
import { AddCustomerButton } from '@/components/customers';

function CustomersPage() {
  const handleSuccess = () => {
    // Refresh your customer list
    console.log('Customer added!');
  };

  return (
    <div>
      <AddCustomerButton onSuccess={handleSuccess} />
    </div>
  );
}
```

### Advanced Usage - Add/Edit Customer

```jsx
import { useState } from 'react';
import { AddUpdateCustomer } from '@/components/customers';

function CustomersPage() {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedCustomerId, setSelectedCustomerId] = useState(0);

  const handleAddNew = () => {
    setSelectedCustomerId(0); // 0 = Add mode
    setDialogOpen(true);
  };

  const handleEdit = customerId => {
    setSelectedCustomerId(customerId); // Pass customer ID for edit mode
    setDialogOpen(true);
  };

  const handleSuccess = () => {
    // Refresh your customer list here
    console.log('Operation successful!');
  };

  return (
    <div>
      <button onClick={handleAddNew}>Add Customer</button>

      <AddUpdateCustomer
        isOpen={dialogOpen}
        onClose={() => setDialogOpen(false)}
        customerId={selectedCustomerId}
        onSuccess={handleSuccess}
      />
    </div>
  );
}
```

### Integration with DataTable

```jsx
import { useState } from 'react';
import { CustomerDataTable, AddUpdateCustomer } from '@/components/customers';

function CustomersPage() {
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [addDialogOpen, setAddDialogOpen] = useState(false);
  const [currentCustomerId, setCurrentCustomerId] = useState(0);
  const [refreshTrigger, setRefreshTrigger] = useState(0);

  const handleEdit = row => {
    setCurrentCustomerId(row.id);
    setEditDialogOpen(true);
  };

  const handleSuccess = () => {
    setRefreshTrigger(prev => prev + 1); // Trigger data refresh
  };

  return (
    <>
      <button onClick={() => setAddDialogOpen(true)}>Add New Customer</button>

      <CustomerDataTable onEdit={handleEdit} refreshTrigger={refreshTrigger} />

      {/* Add Dialog */}
      <AddUpdateCustomer
        isOpen={addDialogOpen}
        onClose={() => setAddDialogOpen(false)}
        customerId={0}
        onSuccess={handleSuccess}
      />

      {/* Edit Dialog */}
      <AddUpdateCustomer
        isOpen={editDialogOpen}
        onClose={() => setEditDialogOpen(false)}
        customerId={currentCustomerId}
        onSuccess={handleSuccess}
      />
    </>
  );
}
```

## Component Props

### AddUpdateCustomer Props

| Prop         | Type     | Required | Default | Description                              |
| ------------ | -------- | -------- | ------- | ---------------------------------------- |
| `isOpen`     | boolean  | Yes      | -       | Controls dialog visibility               |
| `onClose`    | function | Yes      | -       | Callback when dialog is closed           |
| `customerId` | number   | No       | 0       | Customer ID for edit mode (0 = add mode) |
| `onSuccess`  | function | No       | -       | Callback after successful save           |

### ViewCustomer Props

| Prop       | Type    | Required | Default | Description          |
| ---------- | ------- | -------- | ------- | -------------------- |
| `customer` | object  | Yes      | -       | Customer data object |
| `loading`  | boolean | No       | false   | Loading state        |

### AddCustomerButton Props

| Prop        | Type     | Required | Default | Description                                 |
| ----------- | -------- | -------- | ------- | ------------------------------------------- |
| `onSuccess` | function | No       | -       | Callback after successful customer creation |

## Form Fields

### Basic Info Tab

**Personal Details:**

- Full name (required)
- Email (required, validated)
- Phone (required)
- Customer Type (required, dropdown from backend)

**Address:**

- Street (required)
- City (required)
- State (required)
- Zip Code (required)

### Business Tab

- Credit Limit (required, numeric, defaults to $5000.00)
- Credit Status (dropdown: Active, On Hold, Suspended)
- Payment Terms (text, defaults to "Net 30")

### Summary Tab (Update Mode Only)

Read-only view displaying:

- All personal details
- Complete address
- Business information
- System information (created date, created by)

## Validation Rules

### Email

- Must be a valid email format
- Pattern: `user@domain.com`

### Phone

- Required field
- No specific format enforced (allows international formats)

### Credit Limit

- Must be a positive number
- Supports decimals (e.g., 5000.00)
- Cannot be negative

### Required Fields

All fields except:

- Payment Terms (optional, has default)
- Credit Status (has default value)

## Backend Integration

### API Endpoints Used

```javascript
// Get all customers (paginated)
GET /api/customers?page=1&pageSize=10&search=...

// Get customer by ID
GET /api/customers/{id}

// Create customer
POST /api/customers
Body: CustomerCreateRequest

// Update customer
PUT /api/customers/{id}
Body: CustomerUpdateRequest

// Get customer categories
GET /api/customer-categories
```

### Expected Data Structure

**CustomerCreateRequest:**

```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "phone": "(123) 456-7890",
  "customerCategoryId": 1,
  "street": "123 Main Street",
  "city": "Anytown",
  "state": "CA",
  "zipCode": "12345",
  "creditLimit": 5000.0,
  "creditStatus": 0,
  "paymentTerms": "Net 30"
}
```

**Customer Response:**

```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@example.com",
  "phone": "(123) 456-7890",
  "customerCategoryId": 1,
  "customerCategoryName": "Retail",
  "address": {
    "street": "123 Main Street",
    "city": "Anytown",
    "state": "CA",
    "zipCode": "12345"
  },
  "creditLimit": 5000.0,
  "creditStatus": 0,
  "paymentTerms": "Net 30",
  "isActive": true,
  "createdAt": "2023-10-29T10:00:00Z",
  "createdBy": "admin@system.com"
}
```

## Styling and UI

The components follow the application's design system:

- Uses Tailwind CSS for styling
- Consistent with other forms in the application
- Follows the same patterns as ProductCategory components
- Responsive design (mobile-friendly)
- Accessible form controls
- Loading states and error handling

## State Management

The component manages several state variables:

- `formData`: All form field values
- `errors`: Validation error messages
- `mode`: 'add' or 'update'
- `activeTab`: Current tab ('basic', 'business', 'summary')
- `isLoading`: Save operation in progress
- `isFetchingData`: Loading customer data
- `customerCategories`: List of available customer categories
- `customerData`: Full customer data for view mode

## Error Handling

- Field-level validation with inline error messages
- Toast notifications for API errors
- Loading states during async operations
- Graceful handling of missing data

## Notes

- The Summary tab only appears in Update mode
- Customer categories are fetched from the backend
- Form validates before allowing tab navigation
- All required fields must be filled before saving
- The component automatically switches to Update mode after creating a new customer

## Future Enhancements

Potential improvements:

- Add customer avatar/photo upload
- Include order history in Summary tab
- Add notes/comments section
- Support for bulk operations
- Export customer data
- Advanced filtering options
