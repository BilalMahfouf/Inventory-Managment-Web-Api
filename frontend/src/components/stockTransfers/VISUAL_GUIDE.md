# AddStockTransfer - Visual Component Guide

## 🎨 Complete UI Layout

```
┌────────────────────────────────────────────────────────────────────────┐
│  📦 Create Stock Transfer                                           ✕  │
├────────────────────────────────────────────────────────────────────────┤
│  Product  │  Transfer Details  │  History                             │
│  (Active) │                    │                                       │
├────────────────────────────────────────────────────────────────────────┤
│                                                                        │
│  [Tab Content - Scrollable Area]                                      │
│                                                                        │
│                                                                        │
├────────────────────────────────────────────────────────────────────────┤
│                                           [Cancel]  [Create Transfer] │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 📑 Tab 1: Product

### Search Section

```
┌────────────────────────────────────────────────────────────────┐
│  📦 Product Information                                         │
│                                                                │
│  Search Product *                                              │
│  ┌──────────────────────────────────────────┬────────────────┐ │
│  │ Enter Product ID                         │ [🔍 Search]   │ │
│  └──────────────────────────────────────────┴────────────────┘ │
│  Enter the Product ID to search                               │
└────────────────────────────────────────────────────────────────┘
```

### Selected Product Display

```
┌────────────────────────────────────────────────────────────────┐
│  Selected Product                                              │
│                                                                │
│  Product Name          │  SKU                                  │
│  AirPods Pro          │  APP-2HD-GEN                          │
│                                                                │
│  Category             │  Product ID                            │
│  Audio                │  #1                                   │
│                                                                │
│  Description                                                   │
│  Premium wireless earbuds with active noise cancellation...   │
│                                                                │
│  Unit Price           │  Status                                │
│  $249.99             │  ● Active                             │
└────────────────────────────────────────────────────────────────┘
```

### Empty State

```
┌────────────────────────────────────────────────────────────────┐
│                                                                │
│                         📦                                     │
│                                                                │
│           Search for a product to get started                 │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

---

## 📑 Tab 2: Transfer Details

### Transfer Route Section

```
┌────────────────────────────────────────────────────────────────┐
│  📍 Transfer Route                                             │
│                                                                │
│  From Warehouse *             │  To Warehouse *                │
│  ┌─────────────────────────┐  │  ┌──────────────────────────┐ │
│  │ Main Warehouse      ▼  │  │  │ Secondary Warehouse  ▼  │ │
│  └─────────────────────────┘  │  └──────────────────────────┘ │
│                                                                │
│  From Location                │  To Location                   │
│  ┌─────────────────────────┐  │  ┌──────────────────────────┐ │
│  │ Main Warehouse         │  │  │ Secondary Warehouse      │ │
│  └─────────────────────────┘  │  └──────────────────────────┘ │
└────────────────────────────────────────────────────────────────┘
```

### Visual Route Display

```
┌────────────────────────────────────────────────────────────────┐
│  ┌───────────────────┐      ➡️      ┌──────────────────────┐ │
│  │ Main Warehouse    │              │ Secondary Warehouse  │ │
│  │ Any Location      │              │ Any Location         │ │
│  └───────────────────┘              └──────────────────────┘ │
└────────────────────────────────────────────────────────────────┘
```

### Quantity & Notes

```
┌────────────────────────────────────────────────────────────────┐
│  Requested Quantity *                                          │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ 100                                                      │ │
│  └──────────────────────────────────────────────────────────┘ │
│                                                                │
│  Transfer Notes                                                │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ Additional notes about this transfer...                  │ │
│  │                                                          │ │
│  │                                                          │ │
│  └──────────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────────────┘
```

### Transfer Summary

```
┌────────────────────────────────────────────────────────────────┐
│  Transfer Summary                        Total Value           │
│                                              $5,000.00         │
│                                                                │
│  1 product, 100 total items                                   │
└────────────────────────────────────────────────────────────────┘
```

---

## 📑 Tab 3: History

### Add Mode (Before Creation)

```
┌────────────────────────────────────────────────────────────────┐
│  📜 Transfer History                                           │
│                                                                │
│                         📜                                     │
│                                                                │
│                No History Available                            │
│        History will be available after                         │
│           the transfer is created                              │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

### View Mode (After Creation)

```
┌────────────────────────────────────────────────────────────────┐
│  📜 Transfer History                                           │
│                                                                │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ Current Status                    [📈 Change Status]     │ │
│  │                                                          │ │
│  │  ⚠ Pending                                              │ │
│  └──────────────────────────────────────────────────────────┘ │
│                                                                │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ Transfer Timeline                                        │ │
│  │                                                          │ │
│  │  🕐  Transfer Created      Oct 23, 2025, 10:30 AM      │ │
│  │      👤 John Doe                                        │ │
│  │                                                          │ │
│  │  📈  Status: Pending       Oct 23, 2025, 10:30 AM      │ │
│  └──────────────────────────────────────────────────────────┘ │
│                                                                │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ Transfer Information                                     │ │
│  │                                                          │ │
│  │  Transfer ID      │  Created By                         │ │
│  │  #123            │  John Doe                           │ │
│  │                                                          │ │
│  │  User ID          │  Created Date                       │ │
│  │  #456            │  Oct 23, 2025, 10:30 AM            │ │
│  └──────────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────────────┘
```

---

## 🎨 Component Hierarchy

```
AddStockTransfer
│
├── Header
│   ├── Icon (📦)
│   ├── Title
│   └── Close Button (✕)
│
├── Tab Navigation
│   ├── Product Tab
│   ├── Transfer Details Tab
│   └── History Tab
│
├── Tab Content (Scrollable)
│   │
│   ├── ProductInfoTab
│   │   ├── Search Section
│   │   │   ├── Search Input
│   │   │   └── Search Button
│   │   ├── Product Display
│   │   │   └── Product Details Grid
│   │   └── Empty State
│   │
│   ├── TransferDetailsTab
│   │   ├── Route Selection
│   │   │   ├── From Warehouse Select
│   │   │   ├── To Warehouse Select
│   │   │   ├── From Location Input (readonly)
│   │   │   └── To Location Input (readonly)
│   │   ├── Visual Route Display
│   │   │   ├── From Warehouse Box
│   │   │   ├── Arrow
│   │   │   └── To Warehouse Box
│   │   ├── Quantity Input
│   │   ├── Notes Textarea
│   │   └── Transfer Summary
│   │       ├── Summary Header
│   │       ├── Total Value
│   │       └── Item Count
│   │
│   └── HistoryTab
│       ├── Current Status Section
│       │   ├── Status Badge
│       │   └── Change Status Button
│       ├── Transfer Timeline
│       │   ├── Creation Event
│       │   └── Status Events
│       └── Transfer Information
│           └── Details Grid
│
└── Footer
    ├── Cancel Button
    └── Create Transfer Button
```

---

## 🎯 User Flow Diagram

```
START
  │
  ▼
┌─────────────────┐
│ Click "Add      │
│ Stock Transfer" │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Tab 1: Product  │ ◄──── Can navigate back
└────────┬────────┘
         │
         │ Search & Select Product
         ▼
┌─────────────────────┐
│ Tab 2: Transfer     │ ◄──── Can navigate back
│ Details             │
└────────┬────────────┘
         │
         │ Select Locations
         │ Enter Quantity
         │ Add Notes (optional)
         ▼
┌─────────────────────┐
│ Review Summary      │
└────────┬────────────┘
         │
         │ Click "Create Transfer"
         ▼
┌─────────────────────┐
│ Validation          │
│ (Client-side)       │
└────────┬────────────┘
         │
         │ Valid?
         ├─── No ──► Show Error Toast
         │           Navigate to Error Tab
         │
         ▼ Yes
┌─────────────────────┐
│ Submit to API       │
└────────┬────────────┘
         │
         │ Success?
         ├─── No ──► Show Error Toast
         │
         ▼ Yes
┌─────────────────────┐
│ Show Success Toast  │
│ Call onSuccess()    │
│ Close Modal         │
└─────────────────────┘
         │
         ▼
       END
```

---

## 🎨 State Flow

```
Initial State:
├── activeTab: 0
├── selectedProduct: null
├── fromLocationId: ''
├── toLocationId: ''
├── quantity: 0
├── notes: ''
├── isLoading: false
└── mode: 'add'

After Product Selection:
├── selectedProduct: { id, name, sku, ... }
└── [User navigates to Tab 2]

After Location Selection:
├── fromLocationId: "5"
├── toLocationId: "7"
└── [Transfer summary updates]

After Quantity Entry:
├── quantity: 100
└── [Transfer summary updates with total value]

During Submit:
├── isLoading: true
└── [All inputs disabled]

After Success:
├── [Reset all state]
├── [Close modal]
└── [Call onSuccess callback]
```

---

## 🎨 Responsive Breakpoints

### Desktop (≥ 768px)

- Two-column grid for inputs
- Side-by-side route display
- Full modal width (max-w-4xl)

### Mobile (< 768px)

- Single-column layout
- Stacked route display
- Full viewport width
- Touch-friendly buttons

---

## 🎨 Color Reference

| Element         | Color  | Hex Code |
| --------------- | ------ | -------- |
| Active Tab      | Blue   | #2563EB  |
| Primary Button  | Blue   | #2563EB  |
| Success         | Green  | #10B981  |
| Warning         | Yellow | #F59E0B  |
| Error           | Red    | #EF4444  |
| Info Background | Blue   | #EFF6FF  |
| Gray Background | Gray   | #F9FAFB  |
| Border          | Gray   | #E5E7EB  |

---

## 📐 Spacing & Sizing

```css
/* Modal */
max-width: 4xl (56rem)
max-height: 90vh
padding: 1.5rem (24px)

/* Tabs */
height: 3rem (48px)
padding: 1rem (16px)

/* Inputs */
height: 3rem (48px)
padding: 0.75rem (12px)

/* Buttons */
min-height: 2.5rem (40px)
min-width: 6.25rem (100px)
padding: 0.5rem 1rem (8px 16px)

/* Gaps */
grid-gap: 1.5rem (24px)
section-gap: 1.5rem (24px)
```

---

This visual guide provides a complete overview of the AddStockTransfer component structure and design!
