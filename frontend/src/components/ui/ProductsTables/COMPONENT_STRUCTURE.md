# ProductViewDialog Component Structure

## 🎨 Visual Layout

```
┌────────────────────────────────────────────────────────────┐
│  📦 Product Details              P001                    ✕ │
├────────────────────────────────────────────────────────────┤
│  Basic Info  │  Pricing  │  Inventory  │  Details         │
├────────────────────────────────────────────────────────────┤
│                                                            │
│  [Tab Content Area - Scrollable]                          │
│                                                            │
│                                                            │
│                                                            │
│                                                            │
│                                                            │
├────────────────────────────────────────────────────────────┤
│                              [Close]  [📦 Duplicate]      │
└────────────────────────────────────────────────────────────┘
```

## 📑 Tab Layouts

### Tab 1: Basic Info

```
┌─────────────────────────────────────────────────┐
│ 📦 Product Information                          │
│                                                 │
│ ✓ Active    ⚠ Stock: Low Stock                │
│                                                 │
│ Product Name          │ SKU                     │
│ AirPods Pro          │ APP-2HD-GEN             │
│                                                 │
│ Category             │ Unit of Measure         │
│ Audio                │ pcs                     │
│                                                 │
│ Description                                     │
│ Premium wireless earbuds with active noise...  │
│                                                 │
│ Status                                          │
│ ✓ Active                                       │
└─────────────────────────────────────────────────┘
```

### Tab 2: Pricing

```
┌─────────────────────────────────────────────────┐
│ $ Pricing Information                           │
│                                                 │
│ Cost Price           │ Selling Price           │
│ $180.00             │ $249.00                  │
│                                                 │
│ ┌─────────────┐ ┌─────────────┐ ┌───────────┐│
│ │ Profit      │ │ Profit per  │ │ Markup    ││
│ │ Margin      │ │ Unit        │ │           ││
│ │   27%       │ │   $69.00    │ │   38%     ││
│ └─────────────┘ └─────────────┘ └───────────┘│
└─────────────────────────────────────────────────┘
```

### Tab 3: Inventory

```
┌─────────────────────────────────────────────────┐
│ 📦 Inventory Management                         │
│                                                 │
│ Current Stock    │ Min Stock    │ Max Stock   │
│ 2 pcs           │ 5 pcs        │ 100 pcs     │
│                                                 │
│ Unit of Measure        │ Storage Location      │
│ pcs                    │ B1-C2-D1              │
│                                                 │
│ Stock Status                                    │
│ ⚠ Low Stock - Reorder Soon                    │
└─────────────────────────────────────────────────┘
```

### Tab 4: Details

```
┌─────────────────────────────────────────────────┐
│ Creation Details      │ Last Update Details     │
│                       │                         │
│ Created At            │ Updated At              │
│ 1/1/2024, 10:30 AM   │ 1/15/2024, 2:20 PM     │
│                       │                         │
│ Created By            │ Updated By              │
│ admin                 │ admin                   │
│                       │                         │
│ Created By User ID    │ Updated By User ID      │
│ 123e4567-e89b...     │ 123e4567-e89b...       │
├─────────────────────────────────────────────────┤
│ System Information                              │
│                                                 │
│ Product ID           │ Category ID             │
│ 123e4567-e89b...    │ 123e4567-e89b...       │
│                       │                         │
│ Unit of Measure ID   │ Status                  │
│ 123e4567-e89b...    │ Active                  │
└─────────────────────────────────────────────────┘
```

## 🎯 Component Hierarchy

```
ProductViewDialog
├── Dialog (Radix UI)
│   ├── DialogContent
│   │   ├── DialogHeader
│   │   │   ├── Package Icon
│   │   │   ├── DialogTitle
│   │   │   └── SKU Badge
│   │   │
│   │   ├── Tabs Navigation
│   │   │   ├── Basic Info Tab Button
│   │   │   ├── Pricing Tab Button
│   │   │   ├── Inventory Tab Button
│   │   │   └── Details Tab Button
│   │   │
│   │   ├── Tab Content (Scrollable)
│   │   │   ├── Basic Info Content
│   │   │   │   ├── Status Badges
│   │   │   │   ├── Product Details Grid
│   │   │   │   ├── Description
│   │   │   │   └── Status
│   │   │   │
│   │   │   ├── Pricing Content
│   │   │   │   ├── Price Grid
│   │   │   │   └── Profit Metrics Cards
│   │   │   │
│   │   │   ├── Inventory Content
│   │   │   │   ├── Stock Levels Grid
│   │   │   │   ├── Unit & Location Grid
│   │   │   │   └── Stock Status Badge
│   │   │   │
│   │   │   └── Details Content
│   │   │       ├── Creation & Update Grid
│   │   │       ├── System Info Grid
│   │   │       └── Deletion Info (if deleted)
│   │   │
│   │   └── Footer Actions
│   │       ├── Close Button
│   │       └── Duplicate Button (optional)
│   │
│   └── DialogClose (X button)
```

## 🔄 State Flow

```
DataTable
    │
    ├─ User clicks ⋯ menu
    │
    ├─ User clicks "View"
    │
    └─> Triggers onView(row)
            │
            ├─ setSelectedProduct(row)
            │
            └─ setViewDialogOpen(true)
                    │
                    └─> ProductViewDialog renders
                            │
                            ├─ Shows product data
                            │
                            ├─ User switches tabs
                            │
                            ├─ User clicks Close
                            │
                            └─> onOpenChange(false)
                                    │
                                    └─ Dialog closes
```

## 🎨 Color Scheme

```
┌─────────────────────────────────────────┐
│ Status Colors                           │
├─────────────────────────────────────────┤
│ Active:        Green (bg-green-100)     │
│ Inactive:      Gray  (bg-gray-100)      │
│ Low Stock:     Red   (bg-red-100)       │
│ Out of Stock:  Red   (bg-red-100)       │
│ In Stock:      Green (bg-green-100)     │
├─────────────────────────────────────────┤
│ Metric Cards                            │
├─────────────────────────────────────────┤
│ Profit Margin: Blue   (bg-blue-50)      │
│ Profit/Unit:   Green  (bg-green-50)     │
│ Markup:        Purple (bg-purple-50)    │
├─────────────────────────────────────────┤
│ UI Elements                             │
├─────────────────────────────────────────┤
│ Border:        Gray-200                 │
│ Text Primary:  Gray-900                 │
│ Text Secondary: Gray-700                │
│ Text Muted:    Gray-500                 │
│ Hover:         Gray-50                  │
└─────────────────────────────────────────┘
```

## 📐 Responsive Breakpoints

```
Desktop (>1024px)
┌─────────────────────────────────────┐
│  [  Full Width - Grid 2 columns  ]  │
└─────────────────────────────────────┘

Tablet (768px - 1024px)
┌───────────────────────────────┐
│  [  Adapted Grid Layout  ]    │
└───────────────────────────────┘

Mobile (<768px)
┌─────────────────┐
│  [ Stacked ]    │
│  [ Layout ]     │
└─────────────────┘
```

## 🔌 Props Interface

```typescript
interface ProductViewDialogProps {
  // Required
  open: boolean;
  onOpenChange: (open: boolean) => void;
  product: {
    id: string;
    sku: string;
    name: string;
    description: string;
    categoryId: string;
    categoryName: string;
    unitOfMeasureId: string;
    unitOfMeasureName: string;
    costPrice: number;
    unitPrice: number;
    isActive: boolean;
    createdAt: string;
    createdByUserId: string;
    createdByUserName: string;
    updatedAt: string;
    updatedByUserId: string;
    updatedByUserName: string;
    isDeleted: boolean;
    deleteAt: string;
    deletedByUserId: string;
    deletedByUserName: string;
  };

  // Optional
  inventory?: {
    currentStock: number;
    minimumStock: number;
    maximumStock: number;
    storageLocation: string;
  };
  onDuplicate?: (product: object) => void;
}
```

## 📊 Data Flow Diagram

```
API Response (ProductReadResponse)
        │
        ├─> id ────────────────────────> Details Tab (Product ID)
        ├─> sku ───────────────────────> Header Badge, Basic Info
        ├─> name ──────────────────────> Basic Info (Product Name)
        ├─> description ───────────────> Basic Info (Description)
        ├─> categoryName ──────────────> Basic Info (Category)
        ├─> unitOfMeasureName ─────────> Basic Info, Inventory
        ├─> costPrice ─────────────────> Pricing (Cost Price)
        ├─> unitPrice ─────────────────> Pricing (Selling Price)
        │                                      │
        │                                      └─> Calculate Metrics
        │                                            ├─> Profit Margin
        │                                            ├─> Profit per Unit
        │                                            └─> Markup
        │
        ├─> isActive ──────────────────> Status Badge
        ├─> createdAt ─────────────────> Details (Created At)
        ├─> createdByUserName ─────────> Details (Created By)
        ├─> updatedAt ─────────────────> Details (Updated At)
        └─> updatedByUserName ─────────> Details (Updated By)
```

## 🎯 Key Features Map

```
Feature                  Location        Implementation
─────────────────────────────────────────────────────────
Status Badges         → Basic Info    → isActive + stockStatus
Profit Calculations   → Pricing       → Auto-calculated
Stock Alerts          → Inventory     → Based on thresholds
Audit Trail           → Details       → Created/Updated info
Null Safety           → All tabs      → Fallback to '-'
Date Formatting       → Details       → toLocaleString()
Responsive Layout     → All tabs      → CSS Grid + Flexbox
Tab Navigation        → Header        → State management
Duplicate Button      → Footer        → Optional callback
Close Button          → Footer        → onOpenChange(false)
```
