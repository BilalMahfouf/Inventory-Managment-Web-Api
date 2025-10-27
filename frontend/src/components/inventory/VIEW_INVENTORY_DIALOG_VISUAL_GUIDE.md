# ViewInventoryDialog - Visual Guide

## 🎨 Component Structure

```
┌──────────────────────────────────────────────────────────────┐
│  📦 Inventory Details                   ID: 123           ✕  │
├──────────────────────────────────────────────────────────────┤
│  Product & Location  │  Stock Levels  │  System Info         │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  [Tab Content Area - Scrollable]                            │
│                                                              │
│                                                              │
├──────────────────────────────────────────────────────────────┤
│                                              [Close]         │
└──────────────────────────────────────────────────────────────┘
```

## 📑 Tab 1: Product & Location (Two Columns)

```
┌───────────────────────────────────────────────────────────────┐
│                                                               │
│  ┌─────────────────────────┐  ┌─────────────────────────┐   │
│  │ 📦 Product Information  │  │ 📍 Location Information │   │
│  │ (Blue Background)       │  │ (Green Background)      │   │
│  │                         │  │                         │   │
│  │ Product Name            │  │ Name                    │   │
│  │ AirPods Pro             │  │ Warehouse A             │   │
│  │                         │  │                         │   │
│  │ SKU                     │  │ Type                    │   │
│  │ APP-2HD-GEN             │  │ Warehouse               │   │
│  │                         │  │                         │   │
│  │ Category                │  │ Address                 │   │
│  │ Audio                   │  │ 123 Main Street         │   │
│  │                         │  │                         │   │
│  │ Unit of Measure         │  │ Location ID             │   │
│  │ pcs                     │  │ #2                      │   │
│  └─────────────────────────┘  └─────────────────────────┘   │
│                                                               │
└───────────────────────────────────────────────────────────────┘
```

## 📑 Tab 2: Stock Levels

```
┌───────────────────────────────────────────────────────────────┐
│                                                               │
│                    🟢 In Stock                                │
│                                                               │
│  ┌──────────────────┐  ┌──────────────────┐                 │
│  │ Quantity on Hand │  │ Available Stock  │                 │
│  │ (Blue Gradient)  │  │ (Green Gradient) │                 │
│  │                  │  │                  │                 │
│  │      150.00      │  │      150.00      │                 │
│  │       pcs        │  │ Ready for use    │                 │
│  └──────────────────┘  └──────────────────┘                 │
│                                                               │
│  ┌──────────────────┐  ┌──────────────────┐                 │
│  │ Reorder Level    │  │ Maximum Level    │                 │
│  │ (Yellow Gradient)│  │ (Purple Gradient)│                 │
│  │                  │  │                  │                 │
│  │      50.00       │  │     500.00       │                 │
│  │ Minimum threshold│  │ Storage capacity │                 │
│  └──────────────────┘  └──────────────────┘                 │
│                                                               │
│  Stock Level                                          30%    │
│  ████████████░░░░░░░░░░░░░░░░░░░░░░░░░░░░                   │
│  150.00 / 500.00 pcs                                         │
│                                                               │
└───────────────────────────────────────────────────────────────┘
```

## 📑 Tab 3: System Info

```
┌───────────────────────────────────────────────────────────────┐
│                                                               │
│  System Identifiers                                          │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ Inventory ID   │ Product ID     │ Location ID       │    │
│  │ 123            │ 5              │ 2                 │    │
│  └─────────────────────────────────────────────────────┘    │
│                                                               │
│  Audit Trail                                                 │
│  ┌─────────────────────────┐  ┌─────────────────────────┐   │
│  │ 📅 Created              │  │ 📅 Updated              │   │
│  │ (Blue Background)       │  │ (Purple Background)     │   │
│  │                         │  │                         │   │
│  │ Date & Time             │  │ Date & Time             │   │
│  │ January 1, 2024         │  │ January 15, 2024        │   │
│  │ 10:30 AM                │  │ 02:20 PM                │   │
│  │                         │  │                         │   │
│  │ 👤 By User Name         │  │ 👤 By User Name         │   │
│  │ admin                   │  │ admin                   │   │
│  │                         │  │                         │   │
│  │ User ID                 │  │ User ID                 │   │
│  │ 1                       │  │ 1                       │   │
│  └─────────────────────────┘  └─────────────────────────┘   │
│                                                               │
└───────────────────────────────────────────────────────────────┘
```

## 🎨 Color Scheme

### Product & Location Tab

```
┌─────────────────────────────────────────┐
│ Product Section (Blue Theme)            │
│ - Background: bg-blue-50                │
│ - Border: border-blue-200               │
│ - Labels: text-blue-700                 │
│ - Values: text-blue-900                 │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│ Location Section (Green Theme)          │
│ - Background: bg-green-50               │
│ - Border: border-green-200              │
│ - Labels: text-green-700                │
│ - Values: text-green-900                │
└─────────────────────────────────────────┘
```

### Stock Levels Tab

```
┌─────────────────────────────────────────┐
│ Status Badges                           │
│ - In Stock: bg-green-100 text-green-800 │
│ - Low Stock: bg-yellow-100 text-yellow-800 │
│ - Out of Stock: bg-red-100 text-red-800 │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│ Stock Cards                             │
│ - Quantity: Blue Gradient               │
│ - Available: Green Gradient             │
│ - Reorder: Yellow Gradient              │
│ - Maximum: Purple Gradient              │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│ Progress Bar                            │
│ - >50%: bg-green-500                    │
│ - 25-50%: bg-yellow-500                 │
│ - <25%: bg-red-500                      │
└─────────────────────────────────────────┘
```

### System Info Tab

```
┌─────────────────────────────────────────┐
│ System Identifiers                      │
│ - Background: bg-gray-50                │
│ - Border: border-gray-200               │
│ - Text: text-gray-900                   │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│ Created Section (Blue Theme)            │
│ - Background: bg-blue-50                │
│ - Labels: text-blue-700                 │
│ - Values: text-blue-900                 │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│ Updated Section (Purple Theme)          │
│ - Background: bg-purple-50              │
│ - Labels: text-purple-700               │
│ - Values: text-purple-900               │
└─────────────────────────────────────────┘
```

## 📐 Responsive Breakpoints

### Desktop (>768px)

```
┌────────────────────────────────────────────────┐
│  [  Full Width - 2 Column Grid Layout  ]       │
│  [  Product Info  ]  [  Location Info  ]       │
│  [  Created Info  ]  [  Updated Info   ]       │
└────────────────────────────────────────────────┘
```

### Mobile (<768px)

```
┌───────────────────┐
│  [ Stacked ]      │
│  [ Product   ]    │
│  [ Info      ]    │
│                   │
│  [ Location  ]    │
│  [ Info      ]    │
│                   │
│  [ Created   ]    │
│  [ Info      ]    │
│                   │
│  [ Updated   ]    │
│  [ Info      ]    │
└───────────────────┘
```

## 🎯 Component Hierarchy

```
ViewInventoryDialog
├── Dialog (Radix UI)
│   ├── DialogContent
│   │   ├── DialogHeader
│   │   │   ├── Archive Icon
│   │   │   ├── DialogTitle
│   │   │   └── ID Badge
│   │   │
│   │   ├── Tabs Navigation
│   │   │   ├── Product & Location Tab Button
│   │   │   ├── Stock Levels Tab Button
│   │   │   └── System Info Tab Button
│   │   │
│   │   ├── Tab Content (Scrollable)
│   │   │   ├── Tab 1: Product & Location
│   │   │   │   ├── Product Info Card (Blue)
│   │   │   │   └── Location Info Card (Green)
│   │   │   │
│   │   │   ├── Tab 2: Stock Levels
│   │   │   │   ├── Stock Status Badge
│   │   │   │   ├── Stock Cards Grid (4 cards)
│   │   │   │   └── Progress Bar
│   │   │   │
│   │   │   └── Tab 3: System Info
│   │   │       ├── System Identifiers
│   │   │       └── Audit Trail Grid
│   │   │           ├── Created Info (Blue)
│   │   │           └── Updated Info (Purple)
│   │   │
│   │   └── Footer Actions
│   │       └── Close Button
│   │
│   └── DialogClose (X button)
```

## 🔄 State Flow

```
User clicks "View" in DataTable
    │
    ├─> setCurrentInventoryId(row.id)
    │
    └─> setViewDialogOpen(true)
            │
            └─> ViewInventoryDialog opens
                    │
                    ├─> useEffect triggers
                    │
                    ├─> Fetches inventory data from API
                    │
                    ├─> Displays data in active tab
                    │
                    ├─> User switches tabs
                    │
                    ├─> User clicks Close
                    │
                    └─> onOpenChange(false)
                            │
                            └─> Dialog closes
                                    │
                                    └─> Tab resets to first tab
```

## 📊 Data Flow

```
API Response (/api/inventory/{id})
        │
        ├─> id ───────────────────────────> Header, System Tab
        │
        ├─> product ──────────────────────> Tab 1 (Left Column)
        │   ├─> id ───────────────────────> System Tab
        │   ├─> name ─────────────────────> Product Info
        │   ├─> sku ──────────────────────> Product Info
        │   ├─> categoryName ─────────────> Product Info
        │   └─> unitOfMeasureName ────────> Product Info, Stock Tab
        │
        ├─> location ─────────────────────> Tab 1 (Right Column)
        │   ├─> id ───────────────────────> System Tab
        │   ├─> name ─────────────────────> Location Info
        │   ├─> type ─────────────────────> Location Info
        │   └─> address ──────────────────> Location Info
        │
        ├─> quantityOnHand ───────────────> Tab 2 (Stock Levels)
        ├─> reorderLevel ─────────────────> Tab 2 (Stock Levels)
        ├─> maxLevel ─────────────────────> Tab 2 (Stock Levels)
        │
        ├─> createdAt ────────────────────> Tab 3 (Created Section)
        ├─> createdByUserName ────────────> Tab 3 (Created Section)
        ├─> createdByUserId ──────────────> Tab 3 (Created Section)
        │
        ├─> updatedAt ────────────────────> Tab 3 (Updated Section)
        ├─> updatedByUserName ────────────> Tab 3 (Updated Section)
        └─> updatedByUserId ──────────────> Tab 3 (Updated Section)
```

## 🎯 Key Features Map

```
Feature                  Location        Implementation
─────────────────────────────────────────────────────────
Two-Column Layout     → Tab 1         → Grid with Product/Location
Stock Status Badges   → Tab 2         → Based on quantity vs reorder
Gradient Cards        → Tab 2         → 4 cards with different colors
Progress Bar          → Tab 2         → Visual stock percentage
System IDs            → Tab 3         → Inventory, Product, Location
Audit Trail           → Tab 3         → Created & Updated info
Date Formatting       → Tab 3         → toLocaleString()
Null Safety           → All tabs      → Fallback to '-'
Responsive Layout     → All tabs      → CSS Grid + Flexbox
Tab Navigation        → Header        → State management
Auto Data Fetch       → useEffect     → On open with inventoryId
Tab Reset             → useEffect     → On close
```

## 📱 Interaction Flow

```
1. User Journey:
   ┌─────────────────────────────────────┐
   │ 1. User clicks ⋯ menu              │
   ├─────────────────────────────────────┤
   │ 2. User clicks "View"              │
   ├─────────────────────────────────────┤
   │ 3. Dialog opens with Tab 1 active  │
   ├─────────────────────────────────────┤
   │ 4. User reviews Product & Location │
   ├─────────────────────────────────────┤
   │ 5. User clicks "Stock Levels" tab  │
   ├─────────────────────────────────────┤
   │ 6. User reviews stock information  │
   ├─────────────────────────────────────┤
   │ 7. User clicks "System Info" tab   │
   ├─────────────────────────────────────┤
   │ 8. User reviews audit trail        │
   ├─────────────────────────────────────┤
   │ 9. User clicks "Close" button      │
   ├─────────────────────────────────────┤
   │ 10. Dialog closes, tab resets      │
   └─────────────────────────────────────┘
```

## 🎨 Visual Examples

### Status Badge Colors

```
🟢 In Stock
┌─────────────────────────────────┐
│ bg-green-100 text-green-800     │
│ 📦 In Stock                     │
└─────────────────────────────────┘

🟡 Low Stock
┌─────────────────────────────────┐
│ bg-yellow-100 text-yellow-800   │
│ ⚠️ Low Stock                     │
└─────────────────────────────────┘

🔴 Out of Stock
┌─────────────────────────────────┐
│ bg-red-100 text-red-800         │
│ ❌ Out of Stock                  │
└─────────────────────────────────┘
```

### Stock Progress Bar Examples

```
High Stock (75%)
███████████████████████████████████░░░░░░░░░░
🟢 Green

Medium Stock (45%)
██████████████████░░░░░░░░░░░░░░░░░░░░░░░░░░
🟡 Yellow

Low Stock (15%)
██████░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
🔴 Red
```

## ✨ Summary

The ViewInventoryDialog component provides a clean, organized way to view complete inventory details with:

- **Tab 1**: Side-by-side product and location info
- **Tab 2**: Visual stock levels with status indicators
- **Tab 3**: System information and audit trail

All data is fetched automatically from the API and displayed in a user-friendly, color-coded interface that follows the app's design system.
