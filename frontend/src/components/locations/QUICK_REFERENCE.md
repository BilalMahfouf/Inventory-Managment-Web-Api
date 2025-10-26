# AddUpdateLocation - Quick Reference Card

## 🚀 Quick Import

```jsx
import { AddUpdateLocation } from '@/components/locations';
```

## 📋 Props

| Prop         | Type     | Default | Description                    |
| ------------ | -------- | ------- | ------------------------------ |
| `isOpen`     | boolean  | -       | Modal visibility               |
| `onClose`    | function | -       | Close handler                  |
| `locationId` | number   | `0`     | 0 = Add mode, >0 = Update mode |
| `onSuccess`  | function | -       | Callback after successful save |

## 💡 Usage Examples

### Add New Location

```jsx
<AddUpdateLocation
  isOpen={true}
  onClose={() => setOpen(false)}
  locationId={0}
  onSuccess={() => refreshData()}
/>
```

### Edit Existing Location

```jsx
<AddUpdateLocation
  isOpen={true}
  onClose={() => setOpen(false)}
  locationId={123}
  onSuccess={() => refreshData()}
/>
```

## 📝 Form Fields

### Required

- ✅ Location Name (text)
- ✅ Address (textarea)
- ✅ Location Type (select)

### Optional

- ⚙️ Active Status (toggle, update mode only)

## 🎨 Styling

- **Primary Color**: Blue (#2563eb)
- **Icon**: MapPin (lucide-react)
- **Max Width**: 672px (2xl)
- **Height**: Auto

## 🔌 Backend Endpoints

| Method | Endpoint             | Purpose         |
| ------ | -------------------- | --------------- |
| POST   | `/api/locations`     | Create location |
| PUT    | `/api/locations/:id` | Update location |
| GET    | `/api/locations/:id` | Get location    |
| GET    | `/api/location-type` | Get types       |

## 📦 Request Format

### Create

```json
{
  "name": "string",
  "address": "string",
  "locationTypeId": number
}
```

### Update

```json
{
  "id": number,
  "name": "string",
  "address": "string",
  "locationTypeId": number,
  "isActive": boolean
}
```

## ✨ Features

- ✅ Dual mode (Add/Update)
- ✅ Form validation
- ✅ Toast notifications
- ✅ Loading states
- ✅ Error handling
- ✅ Auto-refresh on save

## 📍 Files

```
locations/
├── AddUpdateLocation.jsx
├── LocationDataTable.jsx
├── index.js
└── Docs (3 files)
```

## 🎯 Key Points

1. **Mode Detection**: Based on `locationId` (0 = add, >0 = update)
2. **Auto-fetch**: Location data loaded automatically in update mode
3. **Validation**: All required fields validated before submit
4. **Integration**: Already connected to LocationDataTable
5. **No Backend Changes**: Only frontend code created

## 🔍 Troubleshooting

| Issue                      | Solution                        |
| -------------------------- | ------------------------------- |
| Location types not loading | Check `/api/location-type` API  |
| Form not submitting        | Check required field validation |
| Toast not showing          | Verify ToastContext is set up   |
| Data not refreshing        | Ensure onSuccess callback used  |

## 📚 Documentation

- **Full Guide**: `ADD_UPDATE_LOCATION_GUIDE.md`
- **Visual Guide**: `ADD_UPDATE_LOCATION_VISUAL.md`
- **Implementation**: `IMPLEMENTATION_COMPLETE.md`

---

**Status**: ✅ Production Ready  
**Version**: 1.0.0  
**Date**: October 26, 2025
