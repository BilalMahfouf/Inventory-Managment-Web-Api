# AddUpdateLocation Component - Visual Guide

## 🎨 Component Preview

### Add Mode

```
┌─────────────────────────────────────────────────────┐
│ 📍 Add Location                               ✕     │
├─────────────────────────────────────────────────────┤
│                                                     │
│ 📍 Location Information                             │
│                                                     │
│ Location Name *                                     │
│ ┌─────────────────────────────────────────────┐   │
│ │ e.g., Main Warehouse, Store A...            │   │
│ └─────────────────────────────────────────────┘   │
│                                                     │
│ Address *                                           │
│ ┌─────────────────────────────────────────────┐   │
│ │                                             │   │
│ │ Enter the full address...                  │   │
│ │                                             │   │
│ └─────────────────────────────────────────────┘   │
│                                                     │
│ Location Type *                                     │
│ ┌─────────────────────────────────────────────┐   │
│ │ Select a location type              ▼      │   │
│ └─────────────────────────────────────────────┘   │
│                                                     │
├─────────────────────────────────────────────────────┤
│                         [ Cancel ] [ Create Location]│
└─────────────────────────────────────────────────────┘
```

### Update Mode

```
┌─────────────────────────────────────────────────────┐
│ 📍 Edit Location                              ✕     │
├─────────────────────────────────────────────────────┤
│                                                     │
│ 📍 Location Information                             │
│                                                     │
│ Location Name *                                     │
│ ┌─────────────────────────────────────────────┐   │
│ │ Main Warehouse                              │   │
│ └─────────────────────────────────────────────┘   │
│                                                     │
│ Address *                                           │
│ ┌─────────────────────────────────────────────┐   │
│ │                                             │   │
│ │ 123 Main St, City, State 12345             │   │
│ │                                             │   │
│ └─────────────────────────────────────────────┘   │
│                                                     │
│ Location Type *                                     │
│ ┌─────────────────────────────────────────────┐   │
│ │ Warehouse - Storage facility        ▼      │   │
│ └─────────────────────────────────────────────┘   │
│                                                     │
│ ┌─────────────────────────────────────────────┐   │
│ │ Location Status                  [●─────○]  │   │
│ │ This location is currently active           │   │
│ └─────────────────────────────────────────────┘   │
│                                                     │
├─────────────────────────────────────────────────────┤
│                         [ Cancel ] [ Save Changes ] │
└─────────────────────────────────────────────────────┘
```

---

## 🎨 Color Palette

### Primary Colors

```
┌─────────────────────────────────────────┐
│ Blue Primary:     #2563eb (rgb(37, 99, 235))
│ Blue Hover:       #1d4ed8
│ Blue 50:          #eff6ff (status card bg)
│ Blue 200:         #bfdbfe (status card border)
│ Blue 600:         #2563eb (icons, buttons)
│ Blue 700:         #1d4ed8 (button hover)
└─────────────────────────────────────────┘
```

### Neutral Colors

```
┌─────────────────────────────────────────┐
│ Gray 900:         #111827 (headings)
│ Gray 700:         #374151 (labels)
│ Gray 600:         #4b5563 (icons, text)
│ Gray 300:         #d1d5db (borders)
│ Gray 100:         #f3f4f6 (hover states)
│ Gray 50:          #f9fafb (footer bg)
└─────────────────────────────────────────┘
```

### State Colors

```
┌─────────────────────────────────────────┐
│ Error (Red):      #dc2626
│ Error Border:     #ef4444
│ Success (Green):  #059669
└─────────────────────────────────────────┘
```

---

## 📐 Layout Dimensions

```
Modal Container
├── Width: max-w-2xl (672px)
├── Height: auto
├── Padding: 0
└── Border Radius: rounded-lg (8px)

Header
├── Padding: p-6 (24px)
├── Border Bottom: 1px solid gray-200
└── Height: auto

Content Area
├── Padding: p-6 (24px)
├── Space Between: space-y-6 (24px vertical gap)
└── Height: auto

Inputs
├── Height: h-12 (48px) for text/select
├── Height: h-24 (96px) for textarea
└── Border: 1px solid gray-300

Footer
├── Padding: p-6 (24px)
├── Background: bg-gray-50
├── Border Top: 1px solid gray-200
└── Height: auto
```

---

## 🔤 Typography

```
┌─────────────────────────────────────────┐
│ Modal Title:      text-xl font-semibold (20px)
│ Section Title:    text-lg font-semibold (18px)
│ Field Labels:     text-sm font-medium (14px)
│ Input Text:       text-base (16px)
│ Error Text:       text-sm text-red-500 (14px)
│ Helper Text:      text-sm text-gray-600 (14px)
└─────────────────────────────────────────┘
```

---

## 🎯 Interactive Elements

### Buttons

**Primary Button (Create/Save)**

```
┌──────────────────────┐
│  Create Location     │  ← bg-blue-600, text-white
│  Save Changes        │  ← hover:bg-blue-700
└──────────────────────┘
```

**Secondary Button (Cancel)**

```
┌──────────────────────┐
│      Cancel          │  ← bg-gray-200, text-gray-700
└──────────────────────┘  ← hover:bg-gray-300
```

### Toggle Switch (Update Mode Only)

**Active State**

```
  Location Status    [●─────○]  ← Blue (active)
```

**Inactive State**

```
  Location Status    [○─────●]  ← Gray (inactive)
```

### Close Button

```
┌───┐
│ ✕ │  ← p-2, hover:bg-gray-100, rounded-lg
└───┘
```

---

## 📱 Responsive Behavior

### Desktop (> 672px)

```
┌──────────────────────────────────────┐
│         Centered Modal               │
│         Max Width: 672px             │
│         Padding: 16px                │
└──────────────────────────────────────┘
```

### Mobile (< 672px)

```
┌──────────────────────────────────────┐
│         Full Width Modal             │
│         With Side Padding            │
│         Responsive Inputs            │
└──────────────────────────────────────┘
```

---

## 🎭 State Indicators

### Loading State

```
┌─────────────────────────────────────────┐
│ 📍 Add Location                    ✕    │
├─────────────────────────────────────────┤
│                                         │
│ [All inputs disabled with gray bg]     │
│                                         │
├─────────────────────────────────────────┤
│              [ Cancel ] [ Creating...] │
└─────────────────────────────────────────┘
```

### Error State

```
┌─────────────────────────────────────────┐
│ Location Name *                         │
│ ┌───────────────────────────────┐       │
│ │ [Empty field - red border]    │       │
│ └───────────────────────────────┘       │
│ ⚠️ Location name is required            │
└─────────────────────────────────────────┘
```

### Success State (Toast)

```
┌─────────────────────────────────────────┐
│ ✅ Location Created                     │
│ Main Warehouse has been added           │
│ successfully.                           │
└─────────────────────────────────────────┘
```

---

## 🗂️ Field Layout

```
┌─────────────────────────────────────────────────┐
│ Section Header                                  │
│ ├── Icon (MapPin) - h-5 w-5 text-blue-600      │
│ └── Text (Location Information) - text-lg      │
│                                                 │
│ Field Group                                     │
│ ├── Label - text-sm font-medium text-gray-700  │
│ │   └── Required Indicator (*) - text-red-500  │
│ ├── Input/Textarea/Select                      │
│ │   ├── Border: border-gray-300               │
│ │   ├── Focus: ring-1 ring-blue-600           │
│ │   └── Error: border-red-500                 │
│ └── Error Message (conditional) - text-red-500 │
│                                                 │
│ [Repeat for each field]                        │
│                                                 │
│ Status Card (update mode only)                 │
│ └── Blue info card with toggle                 │
└─────────────────────────────────────────────────┘
```

---

## 🔄 User Flow Diagram

### Add Mode Flow

```
Start
  ↓
[User clicks "Add New"]
  ↓
[Modal Opens (empty form)]
  ↓
[Load Location Types from API]
  ↓
[User fills form fields]
  ↓
[Validate on Submit] → Valid? → Yes → [Create API Call]
                         ↓                    ↓
                        No               Success?
                         ↓                    ↓
                    [Show errors]      Yes → [Success Toast]
                                       No → [Error Toast]
                                             ↓
                                       [Close Modal]
                                             ↓
                                       [Refresh Table]
                                             ↓
                                           End
```

### Update Mode Flow

```
Start
  ↓
[User clicks "Edit" on row]
  ↓
[Modal Opens with locationId]
  ↓
[Fetch Location Data by ID]
  ↓
[Load Location Types from API]
  ↓
[Pre-fill form with data]
  ↓
[Show Active Status Toggle]
  ↓
[User modifies fields]
  ↓
[Validate on Submit] → Valid? → Yes → [Update API Call]
                         ↓                    ↓
                        No               Success?
                         ↓                    ↓
                    [Show errors]      Yes → [Success Toast]
                                       No → [Error Toast]
                                             ↓
                                       [Close Modal]
                                             ↓
                                       [Refresh Table]
                                             ↓
                                           End
```

---

## 🎨 Icon Usage

```
📍 MapPin Icon
├── Header: h-6 w-6 text-blue-600
└── Section: h-5 w-5 text-blue-600

✕ Close Icon
└── Button: h-5 w-5 text-gray-600
```

---

## 📊 Component Hierarchy

```
AddUpdateLocation
│
├── Modal Backdrop (fixed, inset-0, bg-black/50)
│   │
│   └── Modal Container (bg-white, rounded-lg, max-w-2xl)
│       │
│       ├── Header
│       │   ├── Icon + Title (flex items-center)
│       │   └── Close Button
│       │
│       ├── Form (onSubmit)
│       │   │
│       │   ├── Content Area
│       │   │   ├── Section Header (Icon + Text)
│       │   │   │
│       │   │   ├── Location Name Field
│       │   │   │   ├── Label with asterisk
│       │   │   │   ├── Input component
│       │   │   │   └── Error message (conditional)
│       │   │   │
│       │   │   ├── Address Field
│       │   │   │   ├── Label with asterisk
│       │   │   │   ├── Textarea
│       │   │   │   └── Error message (conditional)
│       │   │   │
│       │   │   ├── Location Type Field
│       │   │   │   ├── Label with asterisk
│       │   │   │   ├── Select dropdown
│       │   │   │   └── Error message (conditional)
│       │   │   │
│       │   │   └── Active Status (update mode only)
│       │   │       └── Blue info card with toggle
│       │   │
│       │   └── Footer
│       │       ├── Cancel Button (secondary)
│       │       └── Submit Button (primary)
│       │
│       └── [End Modal]
```

---

## 🎭 Animation Notes

- Modal fades in with backdrop (no custom animation, native CSS)
- Buttons have hover transitions (transition-colors)
- Toggle switch has smooth slide animation (after:transition-all)
- No complex animations to keep it fast and simple

---

## ✅ Accessibility Features

- ✅ All inputs have labels
- ✅ Required fields marked with asterisk
- ✅ Error messages associated with fields
- ✅ Disabled state during loading
- ✅ Keyboard navigation supported
- ✅ Focus states visible (blue ring)
- ✅ Backdrop click to close
- ✅ ESC key support (via onClose)

---

**Component**: AddUpdateLocation.jsx  
**Location**: `frontend/src/components/locations/`  
**Documentation**: See `ADD_UPDATE_LOCATION_GUIDE.md`
