# AddUpdateLocation Component - Usage Guide

## Overview

The `AddUpdateLocation` component is a modal dialog for creating and editing locations in the inventory system. It supports both add and update modes, following the same UI patterns as other components in the application.

## Features

- **Dual Mode Operation**: Supports both adding new locations and editing existing ones
- **Form Validation**: Validates all required fields before submission
- **Toast Notifications**: Shows success/error messages using the Toast context
- **Loading States**: Displays loading indicators during async operations
- **Auto-fetch**: Automatically loads location data when editing (locationId > 0)
- **Location Types**: Dropdown populated from the backend location types
- **Active Status Toggle**: Only visible in update mode

## Props

| Prop         | Type     | Required | Description                                 |
| ------------ | -------- | -------- | ------------------------------------------- |
| `isOpen`     | boolean  | ✅       | Controls modal visibility                   |
| `onClose`    | function | ✅       | Callback function when modal is closed      |
| `locationId` | number   | ❌       | ID of location to edit (0 for new location) |
| `onSuccess`  | function | ❌       | Optional callback after successful save     |

## Usage Examples

### Add Mode

```jsx
import { useState } from 'react';
import { AddUpdateLocation } from '@/components/locations';

function LocationManagement() {
  const [dialogOpen, setDialogOpen] = useState(false);

  const handleSuccess = () => {
    console.log('Location created successfully');
    // Refresh your data here
  };

  return (
    <>
      <button onClick={() => setDialogOpen(true)}>Add Location</button>

      <AddUpdateLocation
        isOpen={dialogOpen}
        onClose={() => setDialogOpen(false)}
        locationId={0}
        onSuccess={handleSuccess}
      />
    </>
  );
}
```

### Update Mode

```jsx
import { useState } from 'react';
import { AddUpdateLocation } from '@/components/locations';

function LocationManagement() {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedLocationId, setSelectedLocationId] = useState(0);

  const handleEdit = locationId => {
    setSelectedLocationId(locationId);
    setDialogOpen(true);
  };

  const handleSuccess = () => {
    console.log('Location updated successfully');
    // Refresh your data here
  };

  return (
    <>
      <button onClick={() => handleEdit(123)}>Edit Location</button>

      <AddUpdateLocation
        isOpen={dialogOpen}
        onClose={() => setDialogOpen(false)}
        locationId={selectedLocationId}
        onSuccess={handleSuccess}
      />
    </>
  );
}
```

## Form Fields

### Required Fields

1. **Location Name** (`name`)
   - Type: Text input
   - Validation: Cannot be empty
   - Placeholder: "e.g., Main Warehouse, Store A, Distribution Center"

2. **Address** (`address`)
   - Type: Textarea
   - Validation: Cannot be empty
   - Placeholder: "Enter the full address of this location..."

3. **Location Type** (`locationTypeId`)
   - Type: Dropdown/Select
   - Validation: Must select a valid type
   - Options: Loaded from backend (e.g., Warehouse, Office, Store, Distribution Center)

### Optional Fields

4. **Active Status** (`isActive`)
   - Type: Toggle switch
   - Only visible in update mode
   - Default: true (active) for new locations

## Backend Integration

### API Endpoints Used

1. **Create Location**: `POST /api/locations`

   ```json
   {
     "name": "string",
     "address": "string",
     "locationTypeId": number
   }
   ```

2. **Update Location**: `PUT /api/locations/{id}`

   ```json
   {
     "id": number,
     "name": "string",
     "address": "string",
     "locationTypeId": number,
     "isActive": boolean
   }
   ```

3. **Get Location by ID**: `GET /api/locations/{id}`

4. **Get All Location Types**: `GET /api/location-type`

## UI Design

### Color Scheme

- **Primary Color**: Blue (#2563eb) - Consistent with app theme
- **Icon**: MapPin from lucide-react
- **Status Toggle**: Blue when active, gray when inactive
- **Buttons**: Blue primary button, gray secondary button

### Layout

- **Header**: Icon + Title + Close button
- **Content**: Form with labeled fields
- **Footer**: Cancel + Submit buttons (right-aligned)

### Responsive

- Max width: 2xl (672px)
- Full width on mobile with padding
- Centered modal with backdrop

## Validation

The component validates:

- Location name is not empty
- Address is not empty
- Location type is selected (not 0)

Errors are displayed inline below each field in red text.

## Loading States

- **Initial Load**: When fetching location data in update mode
- **Saving**: During create/update API calls
- **Location Types**: While loading location types dropdown

All inputs and buttons are disabled during loading states.

## Integration with LocationDataTable

The component is already integrated with `LocationDataTable`:

```jsx
// In LocationDataTable.jsx
const handleAddNew = () => {
  setCurrentLocationId(0);
  setAddEditDialogOpen(true);
};

const handleEdit = row => {
  setCurrentLocationId(row.id);
  setAddEditDialogOpen(true);
};

<AddUpdateLocation
  isOpen={addEditDialogOpen}
  onClose={handleCloseAddEdit}
  locationId={currentLocationId}
  onSuccess={handleSuccess}
/>;
```

## Toast Messages

### Success Messages

- **Create**: "Location Created - {name} has been added successfully."
- **Update**: "Location Updated - {name} has been updated successfully."

### Error Messages

- **Create Failed**: "Location Creation Failed - {name} could not be created. Error: {error}"
- **Update Failed**: "Location Update Failed - Location could not be updated. Error: {error}"
- **Load Failed**: "Failed to Load Location - Could not load location data."
- **Location Types Load Failed**: "Failed to Load - Could not load location types."

## File Structure

```
frontend/src/
├── components/
│   └── locations/
│       ├── AddUpdateLocation.jsx      (Main component)
│       ├── LocationDataTable.jsx      (Integration)
│       └── index.js                   (Exports)
├── services/
│   └── products/
│       ├── locationService.js         (Location API calls)
│       └── locationTypeService.js     (Location Type API calls)
```

## Styling

Uses Tailwind CSS classes consistent with the application:

- **Input Fields**: `h-12` height, blue focus ring
- **Textarea**: `h-24` height for address
- **Buttons**: Blue primary, gray secondary
- **Toggle Switch**: Custom styled with peer classes
- **Status Card**: Blue background in update mode

## Notes

- The component automatically fetches location types when opened
- In update mode, the active status toggle is shown in a blue info card
- The component resets form data when closed
- All API calls use the `fetchWithAuth` utility for authentication
- Follows the same patterns as `AddUnitOfMeasure` and `AddProduct` components

## Dependencies

- React (useState, useEffect, useCallback)
- lucide-react (MapPin, X icons)
- Custom Button component
- Custom Input component
- Location services (locationService, locationTypeService)
- Toast context (useToast)

---

**Last Updated**: October 26, 2025
