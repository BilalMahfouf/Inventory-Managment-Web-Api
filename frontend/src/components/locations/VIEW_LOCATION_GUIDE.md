# ViewLocation Component - Documentation

## Overview

The `ViewLocation` component is a read-only dialog that displays comprehensive location information including location details, location type, and complete audit trail (creation and deletion information).

## Features

- ✅ **Read-only view** - No editing capabilities
- ✅ **Complete audit trail** - Shows created and deleted information
- ✅ **Location type display** - Shows type name and ID
- ✅ **Status indicators** - Active/Inactive and Deleted badges
- ✅ **Loading state** - Displays spinner while fetching data
- ✅ **System information** - Shows all IDs and status flags
- ✅ **Responsive design** - Works on all screen sizes
- ✅ **Clean UI** - Follows app design principles

## Props

| Prop           | Type     | Required | Description                             |
| -------------- | -------- | -------- | --------------------------------------- |
| `open`         | boolean  | ✅       | Controls dialog visibility              |
| `onOpenChange` | function | ✅       | Callback when dialog open state changes |
| `locationId`   | number   | ✅       | Location ID to fetch and display        |

## Usage Example

```jsx
import { useState } from 'react';
import { ViewLocation } from '@/components/locations';

function LocationManagement() {
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [selectedLocationId, setSelectedLocationId] = useState(null);

  const handleView = locationId => {
    setSelectedLocationId(locationId);
    setViewDialogOpen(true);
  };

  return (
    <>
      <button onClick={() => handleView(123)}>View Location</button>

      <ViewLocation
        open={viewDialogOpen}
        onOpenChange={setViewDialogOpen}
        locationId={selectedLocationId}
      />
    </>
  );
}
```

## Data Structure

The component displays data from the `LocationReadResponse` backend DTO:

```javascript
{
  id: number,
  name: string,
  address: string,
  isActive: boolean,
  locationTypeId: number,
  locationTypeName: string,
  createdAt: string (ISO date),
  createdByUserId: number,
  createdByUserName: string,
  isDeleted: boolean,
  deleteAt: string (ISO date) | null,
  deletedByUserId: number | null,
  deletedByUserName: string | null
}
```

## Component Sections

### 1. Header

- **Icon**: MapPin (blue color)
- **Title**: "Location Details"
- **Location ID**: Displayed next to title

### 2. Location Information Section

- **Status Badges**:
  - Active/Inactive (green/gray)
  - Deleted badge (red, if applicable)
- **Location Name**: Large, prominent display
- **Address**: Gray background box
- **Location Type**: Blue background with Building2 icon
- **Location Type ID**: Monospace font

### 3. Audit Information Section

Two-column grid layout:

#### Created Information (Blue Card)

- Date & Time (formatted)
- User Name
- User ID (monospace)

#### Deleted Information (Red Card - if deleted)

- Date & Time (formatted)
- User Name
- User ID (monospace)

If not deleted, shows a gray placeholder message.

### 4. System Information Section

Gray background grid showing:

- Location ID
- Location Type ID
- Status (Active/Inactive)
- Is Deleted (Yes/No)

### 5. Footer

- **Close Button**: Gray button to dismiss dialog

## UI Design

### Color Scheme

```
Primary Blue:     #2563eb (MapPin icon, location type card)
Status Green:     bg-green-100, text-green-800 (Active)
Status Gray:      bg-gray-100, text-gray-800 (Inactive)
Status Red:       bg-red-100, text-red-800 (Deleted)
Created Card:     bg-blue-50 (background)
Deleted Card:     bg-red-50 (background)
System Info:      bg-gray-50 (background)
```

### Icons Used

- **MapPin**: Header and location section (from lucide-react)
- **Calendar**: Audit information section
- **User**: User information labels
- **Building2**: Location type display

### Layout Specifications

```
Modal Dialog
├── Max Width: 3xl (768px)
├── Max Height: 90vh
└── Overflow: Hidden with scrollable content

Content Sections
├── Spacing: space-y-6 (24px vertical gap)
├── Section Headers: text-lg font-semibold
└── Grid Layouts: 2 columns on desktop, 1 on mobile
```

## States

### Loading State

- Displays animated spinner (blue color)
- Centered in content area
- Shown while fetching location data

### Error State

- Handled by parent component via toast notifications
- Component shows fallback data if fetch fails

### Empty State

- Shows "-" for missing values
- "No address provided" for empty address
- "System" for missing user names

## Backend Integration

### API Endpoint

- **GET** `/api/locations/{id}`
- Uses `getLocationById(locationId)` from locationService

### Response Handling

```javascript
const response = await getLocationById(locationId);
if (response.success && response.data) {
  setLocationData(response.data);
}
```

## Integration with LocationDataTable

Already integrated in `LocationDataTable.jsx`:

```jsx
const handleView = row => {
  setCurrentLocationId(row.id);
  setViewDialogOpen(true);
};

<ViewLocation
  open={viewDialogOpen}
  onOpenChange={setViewDialogOpen}
  locationId={currentLocationId}
/>;
```

## Status Badges

### Active Badge

```jsx
✓ Active
// bg-green-100 text-green-800
```

### Inactive Badge

```jsx
○ Inactive
// bg-gray-100 text-gray-800
```

### Deleted Badge

```jsx
🗑 Deleted
// bg-red-100 text-red-800
```

## Date Formatting

All dates are formatted using:

```javascript
new Date(date).toLocaleString('en-US', {
  year: 'numeric',
  month: 'long',
  day: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
});

// Example output: "October 26, 2025, 10:30 AM"
```

## Responsive Behavior

- **Desktop**: 2-column grid for audit information
- **Mobile**: Single column layout (md:grid-cols-2)
- **Scrolling**: Content area scrolls if height exceeds viewport

## Accessibility

- ✅ Dialog proper ARIA attributes (via Radix UI)
- ✅ Clear section headers with icons
- ✅ Semantic HTML structure
- ✅ Keyboard navigation support
- ✅ Close on backdrop click
- ✅ Close on ESC key

## Comparison with Similar Components

### Similar to:

- `UnitOfMeasureView` - Same layout pattern
- `ProductCategoryView` - Same audit section design
- `ViewInventoryDialog` - Same color scheme for audit cards

### Key Differences:

- Shows location type information
- Displays both created and deleted audit information
- No tabs (simpler, single-view layout)
- Includes system information section

## File Structure

```
locations/
├── ViewLocation.jsx         (Main component)
├── LocationDataTable.jsx    (Integration)
└── index.js                (Export)
```

## Testing Checklist

- [ ] View location with all data
- [ ] View location with missing data
- [ ] View deleted location
- [ ] View inactive location
- [ ] Loading state displays correctly
- [ ] Close button works
- [ ] Backdrop click closes dialog
- [ ] Responsive on mobile
- [ ] Data refreshes when locationId changes

## Notes

- Component is **read-only** - no edit functionality
- Uses the same design patterns as other view components in the app
- Follows blue color scheme for consistency
- Properly handles soft-deleted locations
- Shows comprehensive audit trail for compliance

---

**Created**: October 26, 2025  
**Component**: ViewLocation.jsx  
**Location**: `frontend/src/components/locations/`
