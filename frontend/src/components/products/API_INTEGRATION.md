# AddProduct API Integration Guide

## Overview

This guide shows how to integrate the AddProduct component with your backend API endpoints.

## API Endpoints Required

### 1. Create Product Endpoint

```
POST /api/products
Content-Type: application/json
```

**Request Body:**

```json
{
  "productName": "string",
  "sku": "string",
  "category": "string",
  "brand": "string",
  "description": "string",
  "status": "Active|Inactive|Draft",
  "costPrice": "number",
  "sellingPrice": "number",
  "currentStock": "number",
  "minimumStock": "number",
  "maximumStock": "number",
  "unitOfMeasurement": "string",
  "storageLocation": "string"
}
```

**Response:**

```json
{
  "success": true,
  "data": {
    "id": "number",
    "productName": "string",
    "sku": "string",
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "message": "Product created successfully"
}
```

### 2. Get Categories Endpoint (Optional)

```
GET /api/categories
```

**Response:**

```json
{
  "success": true,
  "data": [
    { "id": 1, "name": "Electronics" },
    { "id": 2, "name": "Clothing" }
  ]
}
```

## Implementation Examples

### 1. Basic API Integration

```jsx
import { AddProduct } from '@components/products';

const ProductsPage = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const handleAddProduct = async productData => {
    setIsLoading(true);

    try {
      const response = await fetch('/api/products', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${localStorage.getItem('token')}`, // If using auth
        },
        body: JSON.stringify(productData),
      });

      const result = await response.json();

      if (!response.ok) {
        throw new Error(result.message || 'Failed to create product');
      }

      // Success
      setIsModalOpen(false);

      // Refresh products list
      await refreshProductsList();

      // Show success message
      showSuccessToast('Product created successfully!');
    } catch (error) {
      console.error('Error creating product:', error);
      showErrorToast(error.message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      <AddProduct
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleAddProduct}
        isLoading={isLoading}
      />
    </>
  );
};
```

### 2. Enhanced Integration with Error Handling

```jsx
import { AddProduct } from '@components/products';
import { toast } from 'react-toastify'; // or your preferred toast library

const ProductsPage = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const handleAddProduct = async productData => {
    setIsLoading(true);

    try {
      // Validate required fields client-side
      if (!productData.productName || !productData.sku) {
        throw new Error('Product name and SKU are required');
      }

      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/products`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${getAuthToken()}`,
          },
          body: JSON.stringify({
            ...productData,
            // Transform data if needed
            costPrice: parseFloat(productData.costPrice),
            sellingPrice: parseFloat(productData.sellingPrice),
          }),
        }
      );

      const result = await response.json();

      if (!response.ok) {
        // Handle specific error codes
        switch (response.status) {
          case 400:
            throw new Error('Invalid product data provided');
          case 409:
            throw new Error('Product with this SKU already exists');
          case 401:
            throw new Error('You are not authorized to create products');
          case 422:
            throw new Error(result.errors?.join(', ') || 'Validation failed');
          default:
            throw new Error(result.message || 'Failed to create product');
        }
      }

      // Success actions
      setIsModalOpen(false);

      // Refresh data
      await Promise.all([refreshProductsList(), refreshDashboardSummary()]);

      toast.success(
        `Product "${result.data.productName}" created successfully!`
      );
    } catch (error) {
      console.error('Error creating product:', error);
      toast.error(error.message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      <AddProduct
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleAddProduct}
        isLoading={isLoading}
      />
    </>
  );
};
```

### 3. Integration with React Query/SWR

```jsx
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { AddProduct } from '@components/products';

const ProductsPage = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const queryClient = useQueryClient();

  const createProductMutation = useMutation({
    mutationFn: async productData => {
      const response = await fetch('/api/products', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(productData),
      });

      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message);
      }

      return response.json();
    },
    onSuccess: data => {
      // Invalidate and refetch products
      queryClient.invalidateQueries({ queryKey: ['products'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard-summary'] });

      setIsModalOpen(false);
      toast.success('Product created successfully!');
    },
    onError: error => {
      toast.error(error.message);
    },
  });

  const handleAddProduct = productData => {
    createProductMutation.mutate(productData);
  };

  return (
    <>
      <AddProduct
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleAddProduct}
        isLoading={createProductMutation.isPending}
      />
    </>
  );
};
```

### 4. Custom Service Layer

Create a product service (`services/productService.js`):

```javascript
const API_BASE = process.env.REACT_APP_API_URL || '';

export const productService = {
  async createProduct(productData) {
    const response = await fetch(`${API_BASE}/api/products`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${localStorage.getItem('authToken')}`,
      },
      body: JSON.stringify(productData),
    });

    const result = await response.json();

    if (!response.ok) {
      throw new Error(
        result.message || `HTTP error! status: ${response.status}`
      );
    }

    return result;
  },

  async getCategories() {
    const response = await fetch(`${API_BASE}/api/categories`);
    const result = await response.json();

    if (!response.ok) {
      throw new Error('Failed to fetch categories');
    }

    return result.data;
  },
};
```

Then use it in your component:

```jsx
import { productService } from '@services/productService';
import { AddProduct } from '@components/products';

const ProductsPage = () => {
  const handleAddProduct = async productData => {
    setIsLoading(true);

    try {
      const result = await productService.createProduct(productData);

      setIsModalOpen(false);
      await refreshData();
      showSuccess('Product created successfully!');
    } catch (error) {
      showError(error.message);
    } finally {
      setIsLoading(false);
    }
  };

  // ... rest of component
};
```

## Data Transformation

If your API expects different field names, transform the data:

```javascript
const handleAddProduct = async productData => {
  // Transform frontend data to API format
  const apiData = {
    name: productData.productName,
    sku_code: productData.sku,
    category_id: getCategoryId(productData.category),
    brand_name: productData.brand,
    description: productData.description,
    is_active: productData.status === 'Active',
    cost_price: parseFloat(productData.costPrice),
    selling_price: parseFloat(productData.sellingPrice),
    stock_quantity: parseInt(productData.currentStock),
    min_stock_level: parseInt(productData.minimumStock),
    max_stock_level: parseInt(productData.maximumStock),
    unit_type: productData.unitOfMeasurement,
    storage_location: productData.storageLocation,
  };

  // Make API call with transformed data
  await productService.createProduct(apiData);
};
```

## Error Handling Best Practices

1. **Validation Errors**: Show field-specific errors
2. **Network Errors**: Handle offline scenarios
3. **Server Errors**: Show user-friendly messages
4. **Loading States**: Disable form during submission
5. **Success Feedback**: Clear confirmation messages

## Testing the Integration

1. Test with valid data
2. Test required field validation
3. Test duplicate SKU scenarios
4. Test network failure scenarios
5. Test large description text
6. Test edge cases (negative prices, etc.)

## Environment Variables

Add to your `.env` file:

```
REACT_APP_API_URL=http://localhost:5000
REACT_APP_API_TIMEOUT=10000
```
