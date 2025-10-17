# AddUpdateInventory Component - Visual Guide

## 📱 Component Layout

```
┌────────────────────────────────────────────────────────────────┐
│  📦 Add New Inventory                                       ✕  │
├────────────────────────────────────────────────────────────────┤
│  📦 Product  |  📍 Location  |  📊 Stock Levels              │ ← Tabs
├────────────────────────────────────────────────────────────────┤
│                                                                │
│  [Tab Content Area - Scrollable]                              │
│                                                                │
│  (See individual tab layouts below)                           │
│                                                                │
├────────────────────────────────────────────────────────────────┤
│  ⚠️ Status Message           [Cancel] [Create Inventory]      │ ← Footer
└────────────────────────────────────────────────────────────────┘
```

---

## 📦 Tab 1: Product Search

### Add Mode

```
┌────────────────────────────────────────────────────────────────┐
│  📦 Product Search                                             │
│                                                                │
│  Search Product *                                              │
│  ┌──────────────────────────────────────┐  ┌────────────┐    │
│  │ Enter product name, SKU, or ID      │  │ 🔍 Search │    │
│  └──────────────────────────────────────┘  └────────────┘    │
│                                                                │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ 🔵 Product Information                                   │ │
│  │                                                          │ │
│  │  Product Name        Category                           │ │
│  │  iPhone 13 Pro       Electronics                        │ │
│  │                                                          │ │
│  │  Unit of Measure     Product ID                         │ │
│  │  Piece               #42                                │ │
│  └──────────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────────────┘
```

### Update Mode

```
┌────────────────────────────────────────────────────────────────┐
│  📦 Product Search                                             │
│                                                                │
│  Search Product *                                              │
│  ┌──────────────────────────────────────┐  ┌────────────┐    │
│  │ iPhone 13 Pro                       │  │ 🔍 Search │    │ ← Disabled
│  └──────────────────────────────────────┘  └────────────┘    │
│  Product cannot be changed in update mode                     │
│                                                                │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ 🔵 Product Information                                   │ │
│  │  [Same as Add Mode - Pre-filled]                        │ │
│  └──────────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────────────┘
```

### No Product Selected

```
┌────────────────────────────────────────────────────────────────┐
│  📦 Product Search                                             │
│                                                                │
│  Search Product *                                              │
│  ┌──────────────────────────────────────┐  ┌────────────┐    │
│  │ Enter product name, SKU, or ID      │  │ 🔍 Search │    │
│  └──────────────────────────────────────┘  └────────────┘    │
│                                                                │
│                     📦                                         │
│                                                                │
│           Search for a product to get started                 │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

---

## 📍 Tab 2: Location Selection

### Add Mode

```
┌────────────────────────────────────────────────────────────────┐
│  📍 Location Selection                                         │
│                                                                │
│  Select Location *                                             │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ Warehouse A                                        ▼    │ │ ← Dropdown
│  └──────────────────────────────────────────────────────────┘ │
│                                                                │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ 🟢 Location Information                                  │ │
│  │                                                          │ │
│  │  Location Name       Location ID                        │ │
│  │  Warehouse A         #5                                 │ │
│  │                                                          │ │
│  │  Address             Type                               │ │
│  │  123 Main Street     Warehouse                          │ │
│  └──────────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────────────┘
```

### Update Mode

```
┌────────────────────────────────────────────────────────────────┐
│  📍 Location Selection                                         │
│                                                                │
│  Select Location *                                             │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ Warehouse A                                        ▼    │ │ ← Disabled
│  └──────────────────────────────────────────────────────────┘ │
│  Location cannot be changed in update mode                    │
│                                                                │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ 🟢 Location Information                                  │ │
│  │  [Same as Add Mode - Pre-filled]                        │ │
│  └──────────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────────────┘
```

### No Location Selected

```
┌────────────────────────────────────────────────────────────────┐
│  📍 Location Selection                                         │
│                                                                │
│  Select Location *                                             │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ Choose a location...                               ▼    │ │
│  └──────────────────────────────────────────────────────────┘ │
│                                                                │
│                     📍                                         │
│                                                                │
│          Select a location from the dropdown above            │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

---

## 📊 Tab 3: Stock Levels

### Add Mode

```
┌────────────────────────────────────────────────────────────────┐
│  📊 Stock Levels                                               │
│                                                                │
│  Quantity on Hand *                                            │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ 100                                                      │ │
│  └──────────────────────────────────────────────────────────┘ │
│  Current stock quantity at this location                      │
│                                                                │
│  Reorder Level *                                               │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ 20                                                       │ │
│  └──────────────────────────────────────────────────────────┘ │
│  Minimum quantity before reordering is needed                 │
│                                                                │
│  Maximum Level *                                               │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ 500                                                      │ │
│  └──────────────────────────────────────────────────────────┘ │
│  Maximum storage capacity for this product at this location   │
│                                                                │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ Summary                                                  │ │
│  │                                                          │ │
│  │  Product:         iPhone 13 Pro                         │ │
│  │  Location:        Warehouse A                           │ │
│  │  Quantity:        100                                   │ │
│  │  Reorder Level:   20                                    │ │
│  │  Max Level:       500                                   │ │
│  └──────────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────────────┘
```

### Update Mode

```
┌────────────────────────────────────────────────────────────────┐
│  📊 Stock Levels                                               │
│                                                                │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ 📦 Current Available Stock                               │ │
│  │ 100.00                                                   │ │ ← Yellow Badge
│  └──────────────────────────────────────────────────────────┘ │
│                                                                │
│  Quantity on Hand *                                            │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ 150                                                      │ │ ← Editable
│  └──────────────────────────────────────────────────────────┘ │
│                                                                │
│  Reorder Level *                                               │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ 25                                                       │ │ ← Editable
│  └──────────────────────────────────────────────────────────┘ │
│                                                                │
│  Maximum Level *                                               │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ 600                                                      │ │ ← Editable
│  └──────────────────────────────────────────────────────────┘ │
│                                                                │
│  [Summary section same as Add Mode]                          │
└────────────────────────────────────────────────────────────────┘
```

### Validation Error

```
┌────────────────────────────────────────────────────────────────┐
│  Maximum Level *                                               │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ 10                                                       │ │
│  └──────────────────────────────────────────────────────────┘ │
│                                                                │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ 🔴 ⚠️ Reorder level cannot be greater than maximum level│ │ ← Error
│  └──────────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────────────┘
```

---

## 🎨 Color Coding

### Info Cards

```
🔵 Blue Card (Product Info)
┌──────────────────────────────────────┐
│ bg-blue-50                           │
│ border-blue-200                      │
│ text-blue-900 (heading)              │
└──────────────────────────────────────┘

🟢 Green Card (Location Info)
┌──────────────────────────────────────┐
│ bg-green-50                          │
│ border-green-200                     │
│ text-green-900 (heading)             │
└──────────────────────────────────────┘

🟡 Yellow Card (Available Stock)
┌──────────────────────────────────────┐
│ bg-yellow-50                         │
│ border-yellow-200                    │
│ text-yellow-900 (heading)            │
└──────────────────────────────────────┘

🔴 Red Card (Validation Error)
┌──────────────────────────────────────┐
│ bg-red-50                            │
│ border-red-200                       │
│ text-red-700 (message)               │
└──────────────────────────────────────┘
```

### Tab States

```
Active Tab:
  text-blue-600
  border-blue-600
  bg-white

Inactive Tab:
  text-gray-600
  border-transparent
  hover:text-gray-800
  hover:bg-gray-100
```

---

## 🔄 State Indicators

### Footer Messages

```
❌ Not Ready:
┌──────────────────────────────────────────────────────────────┐
│ ⚠️ Please complete Product and Location sections first      │
│                                    [Cancel] [Create Inventory]│
└──────────────────────────────────────────────────────────────┘
                                            ↑
                                         Disabled

✅ Ready:
┌──────────────────────────────────────────────────────────────┐
│ ✓ Ready to create                  [Cancel] [Create Inventory]│
└──────────────────────────────────────────────────────────────┘
                                            ↑
                                         Enabled

⏳ Loading:
┌──────────────────────────────────────────────────────────────┐
│                                    [Cancel] [Processing...]   │
└──────────────────────────────────────────────────────────────┘
                                            ↑
                                         Loading
```

---

## 📱 Responsive Behavior

### Desktop (≥768px)

```
┌──────────────────────────────────────────────────────────┐
│  Two-column grid for info cards:                        │
│  ┌────────────────┐  ┌────────────────┐                │
│  │ Product Name   │  │ Category       │                │
│  └────────────────┘  └────────────────┘                │
│  ┌────────────────┐  ┌────────────────┐                │
│  │ Unit of Measure│  │ Product ID     │                │
│  └────────────────┘  └────────────────┘                │
└──────────────────────────────────────────────────────────┘
```

### Mobile (<768px)

```
┌────────────────────────────┐
│  Single-column layout:     │
│  ┌────────────────────────┐│
│  │ Product Name           ││
│  └────────────────────────┘│
│  ┌────────────────────────┐│
│  │ Category               ││
│  └────────────────────────┘│
│  ┌────────────────────────┐│
│  │ Unit of Measure        ││
│  └────────────────────────┘│
│  ┌────────────────────────┐│
│  │ Product ID             ││
│  └────────────────────────┘│
└────────────────────────────┘
```

---

## 🎯 User Flow Visualization

### Add Mode Flow

```
Start
  ↓
[Open Dialog]
  ↓
Tab 1: Product
  ↓
[Search Product] → [Product Found?] → Yes → [Show Info]
                         ↓
                        No → [Show Error]
  ↓
[Click Tab 2]
  ↓
Tab 2: Location
  ↓
[Select Location] → [Show Location Info]
  ↓
[Click Tab 3]
  ↓
Tab 3: Stock Levels
  ↓
[Enter Stock Data] → [Valid?] → Yes → [Enable Save]
                         ↓
                        No → [Show Warning]
  ↓
[Click Save]
  ↓
[API Call] → [Success?] → Yes → [Show Success Toast]
                 ↓
                No → [Show Error Toast]
  ↓
[Close Dialog]
  ↓
End
```

### Update Mode Flow

```
Start
  ↓
[Open Dialog with ID]
  ↓
[Fetch Inventory Data]
  ↓
[Pre-fill Product & Location] (Read-only)
  ↓
Tab 3: Stock Levels
  ↓
[Show Available Stock]
  ↓
[Modify Stock Levels] → [Valid?] → Yes → [Enable Update]
                           ↓
                          No → [Show Warning]
  ↓
[Click Update]
  ↓
[API Call] → [Success?] → Yes → [Show Success Toast]
                 ↓
                No → [Show Error Toast]
  ↓
[Close Dialog]
  ↓
End
```

---

## 🖼️ Component Hierarchy

```
AddUpdateInventory
│
├── Header
│   ├── Icon (Archive)
│   ├── Title
│   └── Close Button (X)
│
├── Tabs
│   ├── Product Tab Button
│   ├── Location Tab Button
│   └── Stock Levels Tab Button
│
├── Content (Scrollable)
│   │
│   ├── Tab 0: Product Search
│   │   ├── Search Section
│   │   │   ├── Input Field
│   │   │   └── Search Button
│   │   └── Product Info Card (conditional)
│   │
│   ├── Tab 1: Location Selection
│   │   ├── Location Dropdown
│   │   └── Location Info Card (conditional)
│   │
│   └── Tab 2: Stock Levels
│       ├── Available Stock Badge (update mode)
│       ├── Quantity Input
│       ├── Reorder Level Input
│       ├── Max Level Input
│       ├── Validation Warning (conditional)
│       └── Summary Card
│
└── Footer
    ├── Status Message
    ├── Cancel Button
    └── Submit Button
```

---

**Component**: `AddUpdateInventory.jsx`  
**Location**: `frontend/src/components/inventory/`  
**Documentation**: See `ADD_UPDATE_INVENTORY_GUIDE.md`
