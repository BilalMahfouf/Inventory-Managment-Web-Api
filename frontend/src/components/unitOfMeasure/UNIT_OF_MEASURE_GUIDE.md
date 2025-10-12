# AddUnitOfMeasure Component Guide

## Overview

The `AddUnitOfMeasure` component is a modal dialog for creating and editing units of measure in the inventory system. It supports both add and update modes, similar to the `AddProduct` component.

## Features

- **Dual Mode Operation**: Supports both adding new units and editing existing ones
- **Form Validation**: Validates required fields before submission
- **Toast Notifications**: Shows success/error messages using the Toast context
- **Loading States**: Displays loading indicators during async operations
- **Auto-fetch**: Automatically loads unit data when editing (unitId > 0)

## Usage

### Basic Example

```jsx
import { AddUnitOfMeasure } from '@/components/unitOfMeasure';

function MyComponent() {
  const [isOpen, setIsOpen] = useState(false);
  const [unitId, setUnitId] = useState(0);

  return (
    <>
      <button onClick={() => setIsOpen(true)}>Add Unit</button>

      <AddUnitOfMeasure
        isOpen={isOpen}
        onClose={() => setIsOpen(false)}
        unitId={unitId}
        onSuccess={() => {
          // Refresh your data here
          console.log('Unit saved successfully');
        }}
      />
    </>
  );
}
```

### Add Mode

```jsx
<AddUnitOfMeasure
  isOpen={true}
  onClose={() => setIsOpen(false)}
  unitId={0} // 0 triggers add mode
  onSuccess={handleRefresh}
/>
```

### Edit Mode

```jsx
<AddUnitOfMeasure
  isOpen={true}
  onClose={() => setIsOpen(false)}
  unitId={5} // ID > 0 triggers edit mode
  onSuccess={handleRefresh}
/>
```

## Props

| Prop        | Type       | Default | Description                             |
| ----------- | ---------- | ------- | --------------------------------------- |
| `isOpen`    | `boolean`  | -       | Controls modal visibility               |
| `onClose`   | `function` | -       | Callback when modal is closed           |
| `unitId`    | `number`   | `0`     | ID of unit to edit (0 for new unit)     |
| `onSuccess` | `function` | -       | Optional callback after successful save |

## Form Fields

### Unit Name (Required)

- **Type**: Text input
- **Validation**: Cannot be empty
- **Placeholder**: "e.g., Pieces, Kilograms, Meters, Liters"
- **Required**: Yes

### Description (Optional)

- **Type**: Textarea
- **Validation**: None
- **Placeholder**: "Optional description for this unit of measure..."
- **Required**: No

## API Integration

The component uses the following service functions:

- `createUnitOfMeasure({ name, description })` - Creates a new unit
- `updateUnitOfMeasure(id, { name, description })` - Updates existing unit
- `getUnitOfMeasureById(id)` - Fetches unit data for editing

## State Management

### Internal State

- `formData`: Stores form field values
- `mode`: Tracks whether in 'add' or 'update' mode
- `id`: Current unit ID
- `isLoading`: Loading state for async operations
- `errors`: Form validation errors

### Mode Detection

The component automatically switches between modes based on the `unitId` prop:

- `unitId === 0`: Add mode
- `unitId > 0`: Update mode (fetches existing data)

## Validation

Form validation occurs on submit:

- **Unit Name**: Must not be empty
- Real-time error clearing when user starts typing

## Toast Notifications

### Success Messages

- **Create**: "Unit of Measure Created - {name} has been added successfully."
- **Update**: "Unit of Measure Updated - {name} has been updated successfully."

### Error Messages

- **Create Failed**: "Unit Creation Failed - Unit can't be created. Please try again."
- **Update Failed**: "Unit Update Failed - Unit can't be updated. Please try again."
- **Load Failed**: "Failed to Load Unit - Could not load unit of measure data."

## Integration with UnitOfMeasureTable

The `UnitOfMeasureTable` component includes full integration:

```jsx
// In UnitOfMeasureTable.jsx
const handleAddNew = () => {
  setCurrentUnitOfMeasureId(0);
  setAddEditDialogOpen(true);
};

const handleEdit = row => {
  setCurrentUnitOfMeasureId(row.id);
  setAddEditDialogOpen(true);
};

<AddUnitOfMeasure
  isOpen={addEditDialogOpen}
  onClose={handleCloseAddEdit}
  unitId={currentUnitOfMeasureId}
  onSuccess={handleSuccess}
/>;
```

## Styling

The component uses:

- Tailwind CSS utility classes
- Lucide React icons (`Tag`, `X`)
- Custom Button component from `@components/Buttons/Button`
- Custom Input component from `@components/ui/input`

## Dependencies

```jsx
import React, { useState, useEffect } from 'react';
import { X, Tag } from 'lucide-react';
import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import {
  createUnitOfMeasure,
  getUnitOfMeasureById,
  updateUnitOfMeasure,
} from '@/services/products/UnitOfMeasureService';
import { useToast } from '@/context/ToastContext';
```

## Keyboard Support

- **Escape**: Closes the modal (handled by onClose)
- **Enter**: Submits the form (when in input field)
- **Auto-focus**: Unit Name field receives focus when modal opens

## Best Practices

1. **Always provide onSuccess callback** to refresh data after save
2. **Set unitId to 0** when adding new units
3. **Pass the actual ID** when editing existing units
4. **Handle onClose properly** to reset state in parent component

## Example: Complete Integration

```jsx
import React, { useState } from 'react';
import {
  AddUnitOfMeasure,
  UnitOfMeasureTable,
} from '@/components/unitOfMeasure';

function UnitsOfMeasurePage() {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedUnitId, setSelectedUnitId] = useState(0);
  const [refreshTrigger, setRefreshTrigger] = useState(0);

  const handleAddNew = () => {
    setSelectedUnitId(0);
    setIsModalOpen(true);
  };

  const handleEdit = unitId => {
    setSelectedUnitId(unitId);
    setIsModalOpen(true);
  };

  const handleClose = () => {
    setIsModalOpen(false);
    setSelectedUnitId(0);
  };

  const handleSuccess = () => {
    setRefreshTrigger(prev => prev + 1); // Trigger table refresh
  };

  return (
    <div>
      <button onClick={handleAddNew}>Add New Unit</button>

      <UnitOfMeasureTable key={refreshTrigger} onEdit={handleEdit} />

      <AddUnitOfMeasure
        isOpen={isModalOpen}
        onClose={handleClose}
        unitId={selectedUnitId}
        onSuccess={handleSuccess}
      />
    </div>
  );
}
```
