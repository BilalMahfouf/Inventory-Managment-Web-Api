# ConfirmationDialog Component - Visual Guide

## 🎨 Visual Themes

### 1. Delete Theme (Red) 🗑️

**Appearance:**

```
┌──────────────────────────────────────────┐
│ 🔴 Delete Product                        │
│    iPhone 15 Pro                         │
├──────────────────────────────────────────┤
│                                          │
│  Are you sure you want to delete         │
│  "iPhone 15 Pro"? This action cannot     │
│  be undone and will permanently remove   │
│  this product from the system.           │
│                                          │
├──────────────────────────────────────────┤
│                  [Cancel] [Delete Product]│
└──────────────────────────────────────────┘
```

**Colors:**

- Header Background: Light Red (#fef2f2)
- Icon Background: Red (#fee2e2)
- Icon Color: Dark Red (#dc2626)
- Confirm Button: Red (#dc2626)
- Border: Light Red (#fecaca)

**When to use:**

- Deleting products
- Removing customers
- Canceling orders
- Permanently removing data

---

### 2. Update Theme (Blue) 🔄

**Appearance:**

```
┌──────────────────────────────────────────┐
│ 🔵 Update Product                        │
│    iPhone 15 Pro                         │
├──────────────────────────────────────────┤
│                                          │
│  Are you sure you want to update         │
│  "iPhone 15 Pro"? This will save your    │
│  changes to the product.                 │
│                                          │
├──────────────────────────────────────────┤
│                  [Cancel] [Update Product]│
└──────────────────────────────────────────┘
```

**Colors:**

- Header Background: Light Blue (#eff6ff)
- Icon Background: Blue (#dbeafe)
- Icon Color: Dark Blue (#2563eb)
- Confirm Button: Blue (#2563eb)
- Border: Light Blue (#bfdbfe)

**When to use:**

- Updating product information
- Saving changes to settings
- Modifying customer data
- Editing order details

---

### 3. Create Theme (Green) ✅

**Appearance:**

```
┌──────────────────────────────────────────┐
│ ✅ Create Product                        │
│    iPhone 15 Pro                         │
├──────────────────────────────────────────┤
│                                          │
│  Are you sure you want to create         │
│  "iPhone 15 Pro"? This will add a new    │
│  product to your inventory.              │
│                                          │
├──────────────────────────────────────────┤
│                  [Cancel] [Create Product]│
└──────────────────────────────────────────┘
```

**Colors:**

- Header Background: Light Green (#f0fdf4)
- Icon Background: Green (#dcfce7)
- Icon Color: Dark Green (#059669)
- Confirm Button: Green (#059669)
- Border: Light Green (#bbf7d0)

**When to use:**

- Creating new products
- Adding new customers
- Creating new orders
- Confirming new entries

---

## 📱 Responsive Behavior

### Desktop (> 768px)

```
     [                Backdrop                ]
     [                                        ]
     [         ┌────────────────┐             ]
     [         │   Dialog Box   │             ]
     [         │   (max 448px)  │             ]
     [         └────────────────┘             ]
     [                                        ]
```

### Mobile (< 768px)

```
[              Backdrop              ]
[                                    ]
[  ┌──────────────────────────┐     ]
[  │      Dialog Box          │     ]
[  │   (Full width - 32px)    │     ]
[  └──────────────────────────┘     ]
[                                    ]
```

---

## 🎯 Component Anatomy

```
┌─────────────────────────────────────────┐
│ ┌─────────────────────────────────────┐ │ ← Header (Colored)
│ │ [Icon]  Title                       │ │
│ │         Item Name (optional)        │ │
│ └─────────────────────────────────────┘ │
├─────────────────────────────────────────┤
│                                         │ ← Content Area (White)
│  Message Text                           │
│  (Can span multiple lines)              │
│                                         │
├─────────────────────────────────────────┤
│                         [Cancel] [OK]   │ ← Footer (Light Gray)
└─────────────────────────────────────────┘
```

### Elements:

1. **Header Section** (Colored Background)
   - Icon in circle (left)
   - Title text (large, bold)
   - Item name (optional, medium weight)

2. **Content Section** (White Background)
   - Message text
   - Auto-includes item name in message if provided

3. **Footer Section** (Light Gray Background)
   - Cancel button (left)
   - Confirm button (right, colored)

---

## 🎭 Interactive States

### Normal State

```
[Cancel]  [Delete Product]
 ↑ Gray     ↑ Red
```

### Hover State

```
[Cancel]  [Delete Product]
 ↑ Darker   ↑ Darker Red
```

### Loading State

```
[Cancel]  [⟳ Delete Product]
 ↑ Disabled  ↑ Spinning icon + Disabled
```

### Focus State (Keyboard Navigation)

```
[Cancel]  [Delete Product]
          ↑ Blue ring around button
```

---

## 🖱️ User Interactions

### Click Zones

```
┌─────────────────────────────────────────┐
│ ✱ Click here closes dialog (backdrop)  │
│                                         │
│  ┌───────────────────────────────────┐ │
│  │ ⊗ Dialog - clicks here do nothing│ │
│  │                                   │ │
│  │                                   │ │
│  │                                   │ │
│  │              [Cancel] [Confirm]   │ │
│  │               ↑ Close  ↑ Action   │ │
│  └───────────────────────────────────┘ │
│                                         │
└─────────────────────────────────────────┘
```

### User Actions:

1. **Click Backdrop** → Closes dialog (calls `onClose`)
2. **Click Cancel** → Closes dialog (calls `onClose`)
3. **Click Confirm** → Executes action (calls `onConfirm`)
4. **Press Escape** → (Can be added) Closes dialog

---

## 🎨 Color Palette Reference

### Delete (Red)

```css
Header BG:     #fef2f2  (bg-red-50)
Icon BG:       #fee2e2  (bg-red-100)
Icon Color:    #dc2626  (text-red-600)
Button:        #dc2626  (bg-red-600)
Button Hover:  #b91c1c  (hover:bg-red-700)
Button Active: #991b1b  (active:bg-red-800)
Border:        #fecaca  (border-red-200)
```

### Update (Blue)

```css
Header BG:     #eff6ff  (bg-blue-50)
Icon BG:       #dbeafe  (bg-blue-100)
Icon Color:    #2563eb  (text-blue-600)
Button:        #2563eb  (bg-blue-600)
Button Hover:  #1d4ed8  (hover:bg-blue-700)
Button Active: #1e40af  (active:bg-blue-800)
Border:        #bfdbfe  (border-blue-200)
```

### Create (Green)

```css
Header BG:     #f0fdf4  (bg-green-50)
Icon BG:       #dcfce7  (bg-green-100)
Icon Color:    #059669  (text-green-600)
Button:        #059669  (bg-green-600)
Button Hover:  #047857  (hover:bg-green-700)
Button Active: #065f46  (active:bg-green-800)
Border:        #bbf7d0  (border-green-200)
```

### Common Elements

```css
Backdrop:      rgba(0,0,0,0.5)  (bg-black/50)
Content BG:    #ffffff          (bg-white)
Footer BG:     #f9fafb          (bg-gray-50)
Text Color:    #111827          (text-gray-900)
Message Text:  #4b5563          (text-gray-600)
Cancel Button: #f9fafb          (bg-white border-gray-300)
```

---

## 📐 Sizing & Spacing

### Dialog Box

- **Width:** Max 448px (28rem)
- **Margin:** 16px on mobile
- **Border Radius:** 12px (rounded-xl)
- **Shadow:** 2xl (shadow-2xl)

### Header Section

- **Padding:** 24px (p-6)
- **Gap:** 16px between icon and text
- **Icon Size:** 24px (w-6 h-6)
- **Icon Circle:** 48px with padding

### Content Section

- **Padding:** 24px (p-6)
- **Font Size:** 14px (text-sm)
- **Line Height:** Relaxed (leading-relaxed)

### Footer Section

- **Padding:** 16px horizontal, 16px vertical
- **Gap:** 12px between buttons
- **Button Height:** 40px (py-2)
- **Button Padding:** 16px horizontal (px-4)

---

## ✨ Animation & Transitions

### Dialog Appearance

```css
/* Fade in backdrop */
backdrop: opacity 0 → 1 (150ms)

/* Scale up dialog */
dialog: scale 0.95 → 1.0 (200ms)
       opacity 0 → 1 (200ms)
```

### Button Hover

```css
transition: background-color 200ms ease;
```

### Loading Spinner

```css
animation: spin 1s linear infinite;
```

---

## 🎪 Z-Index Layering

```
Layer 9999: Dialog & Backdrop
Layer 9998: (Available)
...
Layer 50: Toast Notifications
...
Layer 10: Modals (Standard)
...
Layer 1: Page Content
```

The dialog uses `z-[9999]` to ensure it appears above everything.

---

## 📋 Complete Prop Examples

### Minimal (Required Only)

```jsx
<ConfirmationDialog
  isOpen={true}
  onClose={() => {}}
  onConfirm={() => {}}
  type='delete'
  title='Delete Product'
  message='Are you sure?'
/>
```

### With Item Name

```jsx
<ConfirmationDialog
  isOpen={true}
  onClose={() => {}}
  onConfirm={() => {}}
  type='delete'
  title='Delete Product'
  itemName='iPhone 15 Pro'
  message='This action cannot be undone.'
/>
```

### With Custom Buttons

```jsx
<ConfirmationDialog
  isOpen={true}
  onClose={() => {}}
  onConfirm={() => {}}
  type='delete'
  title='Delete Product'
  message='Are you sure?'
  confirmText='Yes, Delete'
  cancelText='No, Cancel'
/>
```

### With Loading State

```jsx
<ConfirmationDialog
  isOpen={true}
  onClose={() => {}}
  onConfirm={() => {}}
  type='delete'
  title='Delete Product'
  message='Are you sure?'
  loading={true}
/>
```

### Full Example (All Props)

```jsx
<ConfirmationDialog
  isOpen={showDialog}
  onClose={handleClose}
  onConfirm={handleConfirm}
  type='delete'
  title='Delete Product'
  itemName='iPhone 15 Pro'
  message='This action cannot be undone and will permanently remove this product from the system.'
  confirmText='Delete Product'
  cancelText='Cancel'
  loading={isDeleting}
/>
```

---

## 🎯 Real-World Visual Examples

### Example 1: Delete Product (Like in your image)

```
┌────────────────────────────────────────────┐
│ 🔴  Delete Product                         │
│     iPhone 15 Pro                          │
├────────────────────────────────────────────┤
│                                            │
│ Are you sure you want to delete            │
│ "iPhone 15 Pro"? This action cannot be     │
│ undone and will permanently remove this    │
│ product from the system.                   │
│                                            │
├────────────────────────────────────────────┤
│                    [Cancel] [Delete Product]│
└────────────────────────────────────────────┘
```

### Example 2: Update Product

```
┌────────────────────────────────────────────┐
│ 🔵  Update Product                         │
│     iPhone 15 Pro                          │
├────────────────────────────────────────────┤
│                                            │
│ Are you sure you want to update            │
│ "iPhone 15 Pro"? This will save all your   │
│ changes to the product information.        │
│                                            │
├────────────────────────────────────────────┤
│                    [Cancel] [Update Product]│
└────────────────────────────────────────────┘
```

### Example 3: Create Product

```
┌────────────────────────────────────────────┐
│ ✅  Create Product                         │
│     iPhone 15 Pro                          │
├────────────────────────────────────────────┤
│                                            │
│ Are you sure you want to create            │
│ "iPhone 15 Pro"? This will add a new       │
│ product to your inventory.                 │
│                                            │
├────────────────────────────────────────────┤
│                    [Cancel] [Create Product]│
└────────────────────────────────────────────┘
```

---

## 🎨 Theme Comparison

| Aspect | Delete      | Update      | Create   |
| ------ | ----------- | ----------- | -------- |
| Color  | Red         | Blue        | Green    |
| Icon   | 🗑️ Trash    | ⚠️ Alert    | ✅ Check |
| Mood   | Destructive | Informative | Positive |
| Use    | Remove      | Modify      | Add      |

---

## ✅ Component Features Checklist

- ✅ Three distinct visual themes
- ✅ Responsive design (mobile & desktop)
- ✅ Loading state support
- ✅ Custom button text
- ✅ Optional item name display
- ✅ Backdrop click closes dialog
- ✅ ARIA accessibility labels
- ✅ Keyboard navigation ready
- ✅ Smooth animations
- ✅ 100% Tailwind CSS
- ✅ No custom CSS files needed
- ✅ Generic and reusable
- ✅ Two clear callbacks (onClose, onConfirm)

---

_Visual Guide Created: October 9, 2025_  
_Component Status: Production Ready ✅_
