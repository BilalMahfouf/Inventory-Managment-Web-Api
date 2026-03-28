import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';

import AddProduct from './AddProduct';
import Button from '@components/Buttons/Button';
import { Plus } from 'lucide-react';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

/**
 * Demo component showing how to use the AddProduct modal
 * This is for testing/demonstration purposes
 */
const AddProductDemo = () => {
  const { t } = useTranslation();
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
      alert(t(i18nKeyContainer.products.demo.addProduct.alerts.addSuccess));
    } catch (error) {
      console.error('Error adding product:', error);
      alert(t(i18nKeyContainer.products.demo.addProduct.alerts.addError));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className='p-8'>
      <div className='max-w-6xl mx-auto'>
        <div className='flex justify-between items-center mb-8'>
          <div>
            <h1 className='text-2xl font-bold'>
              {t(i18nKeyContainer.products.demo.addProduct.title)}
            </h1>
            <p className='text-gray-600 mt-2'>
              {t(i18nKeyContainer.products.demo.addProduct.description)}
            </p>
          </div>
          <Button LeftIcon={Plus} onClick={() => setIsModalOpen(true)}>
            {t(i18nKeyContainer.products.demo.addProduct.addButton)}
          </Button>
        </div>

        {/* Products List */}
        <div className='bg-white rounded-lg border p-6'>
          <h2 className='text-lg font-semibold mb-4'>
            {t(i18nKeyContainer.products.demo.addProduct.listTitle, {
              count: products.length,
            })}
          </h2>
          {products.length === 0 ? (
            <p className='text-gray-500 text-center py-8'>
              {t(i18nKeyContainer.products.demo.addProduct.emptyState)}
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
                        {t(i18nKeyContainer.products.demo.addProduct.labels.sku)}:{' '}
                        {product.sku}
                      </p>
                      <p className='text-sm text-gray-600'>
                        {t(
                          i18nKeyContainer.products.demo.addProduct.labels
                            .category
                        )}
                        : {product.category}
                      </p>
                      <p className='text-sm text-gray-600'>
                        {t(i18nKeyContainer.products.demo.addProduct.labels.brand)}:{' '}
                        {product.brand}
                      </p>
                    </div>
                    <div className='text-right'>
                      <p className='font-semibold text-green-600'>
                        ${product.sellingPrice.toFixed(2)}
                      </p>
                      <p className='text-sm text-gray-600'>
                        {t(i18nKeyContainer.products.demo.addProduct.labels.stock)}:{' '}
                        {product.currentStock}{' '}
                        {product.unitOfMeasurement}
                      </p>
                      <p className='text-sm text-gray-600'>
                        {t(i18nKeyContainer.products.demo.addProduct.labels.status)}:{' '}
                        <span
                          className={
                            product.status === 'Active'
                              ? 'text-green-600'
                              : 'text-gray-600'
                          }
                        >
                          {product.status === 'Active'
                            ? t(i18nKeyContainer.products.shared.status.active)
                            : product.status === 'Draft'
                              ? t(i18nKeyContainer.products.shared.status.draft)
                              : t(i18nKeyContainer.products.shared.status.inactive)}
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
