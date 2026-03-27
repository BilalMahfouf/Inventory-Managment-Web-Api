# AddProduct Component Documentation

## Overview

The `AddProduct` component is a comprehensive modal dialog for adding new products to your inventory management system. It features a multi-step tabbed interface that guides users through entering product information in logical sections.

## Features

- **Multi-step Interface**: Four organized tabs (Basic Info, Pricing, Inventory, Details)
- **Real-time Calculations**: Automatic profit margin, markup, and profit per unit calculations
- **Form Validation**: Built-in validation with disabled submit until required fields are filled
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **Accessible**: Keyboard navigation and screen reader friendly

## Props

| Prop        | Type     | Required | Default | Description                                             |
| ----------- | -------- | -------- | ------- | ------------------------------------------------------- |
| `isOpen`    | boolean  | Yes      | -       | Controls the modal visibility                           |
| `onClose`   | function | Yes      | -       | Callback fired when modal is closed                     |
| `onSubmit`  | function | Yes      | -       | Callback fired when form is submitted with product data |
| `isLoading` | boolean  | No       | false   | Shows loading state on submit button                    |

## Usage

### Basic Implementation

```jsx
import React, { useState } from 'react';
import AddProduct from '@components/products/AddProduct';

function ProductsPage() {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const handleAddProduct = async productData => {
    setIsLoading(true);
    try {
      // Your API call here
      const response = await fetch('/api/products', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(productData),
      });

      if (response.ok) {
        setIsModalOpen(false);
        // Refresh your products list
        console.log('Product created successfully!');
      }
    } catch (error) {
      console.error('Error creating product:', error);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div>
      <button onClick={() => setIsModalOpen(true)}>Add New Product</button>

      <AddProduct
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleAddProduct}
        isLoading={isLoading}
      />
    </div>
  );
}
```

### Integration with ProductsPage

```jsx
// In your ProductsPage.jsx
import AddProduct from '@components/products/AddProduct';

export default function ProductsPage() {
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);

  // Replace your existing "Add Product" button with:
  <Button
    LeftIcon={Plus}
    onClick={() => setIsAddModalOpen(true)}
  >
    Add Product
  </Button>

  // Add the modal before the closing div:
  <AddProduct
    isOpen={isAddModalOpen}
    onClose={() => setIsAddModalOpen(false)}
    onSubmit={handleAddProduct}
    isLoading={isSubmitting}
  />
}
```

## Form Data Structure

The component manages the following product data structure:

```javascript
{
  // Basic Info
  productName: string,
  sku: string,
  category: string,
  brand: string,
  description: string,
  status: 'Active' | 'Inactive' | 'Draft',

  // Pricing
  costPrice: number,
  sellingPrice: number,

  // Inventory
  currentStock: number,
  minimumStock: number,
  maximumStock: number,
  unitOfMeasurement: string,
  storageLocation: string
}
```

## Tabs Overview

### 1. Basic Info Tab

- **Product Name**: Required field for the product name
- **SKU**: Required field for Stock Keeping Unit
- **Category**: Dropdown with predefined categories
- **Brand**: Text input for brand name
- **Description**: Textarea for detailed product description
- **Status**: Dropdown to set product status (Active/Inactive/Draft)

### 2. Pricing Tab

- **Cost Price**: Numeric input for product cost
- **Selling Price**: Numeric input for selling price
- **Automatic Calculations**:
  - Profit Margin percentage
  - Profit per unit in dollars
  - Markup percentage

### 3. Inventory Tab

- **Current Stock**: Initial stock quantity
- **Minimum Stock**: Low stock threshold
- **Maximum Stock**: Maximum stock capacity
- **Unit of Measurement**: Dropdown with common units
- **Storage Location**: Text input for warehouse location

### 4. Details Tab

- **Summary View**: Read-only summary of entered information
- **Validation Check**: Visual confirmation before submission

## Styling and Customization

The component uses Tailwind CSS classes and can be customized by:

1. **Colors**: Modify the color classes in the component
2. **Layout**: Adjust grid layouts and spacing
3. **Icons**: Replace Lucide React icons with your preferred icons
4. **Validation**: Add custom validation rules in the handleSubmit function

## Dependencies

- React (hooks: useState)
- Lucide React (for icons)
- Tailwind CSS (for styling)
- Your existing Button component
- Your existing Input component
- Utils function (cn) for class name merging

## Error Handling

The component includes basic client-side validation:

- Required fields: Product Name and SKU
- Numeric validation for price and stock fields
- Form reset on cancel/close

For server-side error handling, implement error states in your `onSubmit` callback:

```javascript
const handleAddProduct = async productData => {
  try {
    // API call
  } catch (error) {
    // Handle specific error cases
    if (error.status === 409) {
      alert('SKU already exists');
    } else {
      alert('Failed to create product');
    }
  }
};
```

## Accessibility

- Keyboard navigation between tabs
- Focus management within modal
- ARIA labels and roles
- Screen reader compatible
- Escape key to close modal

## Browser Support

Compatible with all modern browsers that support:

- ES6+ JavaScript features
- CSS Grid and Flexbox
- React 16.8+ (hooks)
