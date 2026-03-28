import React, { useState } from 'react';
import AddProduct from './AddProduct';
import Button from '@components/Buttons/Button';
import { Plus } from 'lucide-react';

/**
 * Demo component showing how to use the AddProduct modal
 * This is for testing/demonstration purposes
 */
const AddProductDemo = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [products, setProducts] = useState([]);

  const handleAddProduct = async productData => {
    setIsLoading(true);
    try {
      // Simulate API delay
      await new Promise(resolve => setTimeout(resolve, 2000));

      // Add product to local state (in real app, this would be an API call)
      const newProduct = {
        ...productData,
        id: Date.now(), // Simple ID generation for demo
        createdAt: new Date().toISOString(),
      };

      setProducts(prev => [...prev, newProduct]);
      setIsModalOpen(false);

      console.log('Product added successfully:', newProduct);
      alert('Product added successfully!');
    } catch (error) {
      console.error('Error adding product:', error);
      alert('Error adding product. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className='p-8'>
      <div className='max-w-6xl mx-auto'>
        <div className='flex justify-between items-center mb-8'>
          <div>
            <h1 className='text-2xl font-bold'>AddProduct Component Demo</h1>
            <p className='text-gray-600 mt-2'>
              Click the button below to test the AddProduct modal component
            </p>
          </div>
          <Button LeftIcon={Plus} onClick={() => setIsModalOpen(true)}>
            Add New Product
          </Button>
        </div>

        {/* Products List */}
        <div className='bg-white rounded-lg border p-6'>
          <h2 className='text-lg font-semibold mb-4'>
            Added Products ({products.length})
          </h2>
          {products.length === 0 ? (
            <p className='text-gray-500 text-center py-8'>
              No products added yet. Click "Add New Product" to get started.
            </p>
          ) : (
            <div className='space-y-4'>
              {products.map(product => (
                <div
                  key={product.id}
                  className='border rounded-lg p-4 bg-gray-50'
                >
                  <div className='flex justify-between items-start'>
                    <div>
                      <h3 className='font-semibold'>{product.productName}</h3>
                      <p className='text-sm text-gray-600'>
                        SKU: {product.sku}
                      </p>
                      <p className='text-sm text-gray-600'>
                        Category: {product.category}
                      </p>
                      <p className='text-sm text-gray-600'>
                        Brand: {product.brand}
                      </p>
                    </div>
                    <div className='text-right'>
                      <p className='font-semibold text-green-600'>
                        ${product.sellingPrice.toFixed(2)}
                      </p>
                      <p className='text-sm text-gray-600'>
                        Stock: {product.currentStock}{' '}
                        {product.unitOfMeasurement}
                      </p>
                      <p className='text-sm text-gray-600'>
                        Status:{' '}
                        <span
                          className={
                            product.status === 'Active'
                              ? 'text-green-600'
                              : 'text-gray-600'
                          }
                        >
                          {product.status}
                        </span>
                      </p>
                    </div>
                  </div>
                  {product.description && (
                    <p className='text-sm text-gray-600 mt-2'>
                      {product.description}
                    </p>
                  )}
                </div>
              ))}
            </div>
          )}
        </div>

        {/* AddProduct Modal */}
        <AddProduct
          isOpen={isModalOpen}
          onClose={() => setIsModalOpen(false)}
          onSubmit={handleAddProduct}
          isLoading={isLoading}
        />
      </div>
    </div>
  );
};

export default AddProductDemo;
