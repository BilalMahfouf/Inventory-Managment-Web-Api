# Product Category Form - Visual Layout

```
┌────────────────────────────────────────────────────────────────┐
│  🌲 Add Product Category                                    ✕  │
├────────────────────────────────────────────────────────────────┤
│                                                                │
│  🌲 Category Information                                       │
│                                                                │
│  Category Name *                                              │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ e.g., Electronics, Clothing, Food & Beverages           │ │
│  └──────────────────────────────────────────────────────────┘ │
│                                                                │
│  Description                                                  │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ Optional description for this category...                │ │
│  │                                                          │ │
│  │                                                          │ │
│  └──────────────────────────────────────────────────────────┘ │
│                                                                │
│  Category Type *                                              │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ ◉ Main Category                                          │ │
│  │   A top-level category that can contain subcategories    │ │
│  └──────────────────────────────────────────────────────────┘ │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ ○ Subcategory                                            │ │
│  │   A category that belongs to a main category             │ │
│  └──────────────────────────────────────────────────────────┘ │
│                                                                │
│  [Only visible when Subcategory is selected:]                 │
│  Parent Category *                                            │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ Select a parent category                              ▼  │ │
│  └──────────────────────────────────────────────────────────┘ │
│                                                                │
├────────────────────────────────────────────────────────────────┤
│                                        [Cancel] [Create Category]│
└────────────────────────────────────────────────────────────────┘
```

## State: Main Category Selected

- Parent Category select is HIDDEN
- type = 1
- parentId = null

```
Category Type *
┌────────────────────────────────────┐
│ ◉ Main Category                    │  ← Selected
│   Top-level category               │
└────────────────────────────────────┘
┌────────────────────────────────────┐
│ ○ Subcategory                      │
│   Child category                   │
└────────────────────────────────────┘

[No parent select shown]
```

## State: Subcategory Selected

- Parent Category select is VISIBLE
- type = 2
- parentId = [selected value]

```
Category Type *
┌────────────────────────────────────┐
│ ○ Main Category                    │
│   Top-level category               │
└────────────────────────────────────┘
┌────────────────────────────────────┐
│ ◉ Subcategory                      │  ← Selected
│   Child category                   │
└────────────────────────────────────┘

Parent Category *
┌────────────────────────────────────┐
│ Select a parent category        ▼  │  ← Now visible!
└────────────────────────────────────┘
Dropdown options:
  - Electronics
  - Clothing
  - Food & Beverages
  - etc.
```

## Data Flow

### Add Mode (categoryId = 0)

```
1. Open dialog → Form is empty
2. User fills form
3. Click "Create Category"
4. POST /api/product-categories
   Body: { name, description, type, parentId }
5. Success → Toast + Refresh table
```

### Edit Mode (categoryId > 0)

```
1. Open dialog → Fetch existing data
2. GET /api/product-categories/{id}
3. Pre-fill form with data
4. User modifies form
5. Click "Save Changes"
6. PUT /api/product-categories/{id}
   Body: { name, description, type, parentId }
7. Success → Toast + Refresh table
```

## Field Behavior

| Field       | Required  | Type   | Default | Notes                  |
| ----------- | --------- | ------ | ------- | ---------------------- |
| name        | Yes       | string | ""      | Cannot be empty        |
| description | No        | string | ""      | Can be null in API     |
| type        | Yes       | number | 1       | 1=Main, 2=Sub          |
| parentId    | If type=2 | number | null    | Only for subcategories |

## Validation Messages

```
┌────────────────────────────────┐
│ Category Name *                │
├────────────────────────────────┤
│ [empty field]                  │
└────────────────────────────────┘
❌ Category name is required

┌────────────────────────────────┐
│ Parent Category *              │
├────────────────────────────────┤
│ Select a parent category    ▼  │
└────────────────────────────────┘
❌ Parent category is required for subcategories
```

## Complete Example Flow

### Scenario: Adding "Smartphones" as subcategory of "Electronics"

1. Click "Add Product Category" button
2. Dialog opens
3. Enter name: "Smartphones"
4. Enter description: "Mobile phones and accessories"
5. Select radio: ○ Subcategory
6. Parent select appears
7. Select: "Electronics" from dropdown
8. Click "Create Category"
9. API Request:
   ```json
   {
     "name": "Smartphones",
     "description": "Mobile phones and accessories",
     "type": 2,
     "parentId": 5
   }
   ```
10. Success toast: "Product Category Created - Smartphones has been added successfully."
11. Table refreshes showing new subcategory

## Button States

### Create Mode

```
[Cancel]  [Create Category]
           ↑ Enabled when name is filled
```

### Edit Mode

```
[Cancel]  [Save Changes]
           ↑ Enabled when name is filled
```

### Loading State

```
[Cancel]  [Creating...]
disabled   disabled
```

## Color Scheme

- Primary: Blue (#2563eb)
- Error: Red (#dc2626)
- Border: Gray (#d1d5db)
- Background: White (#ffffff)
- Footer: Gray-50 (#f9fafb)
- Hover: Gray-100 (#f3f4f6)
