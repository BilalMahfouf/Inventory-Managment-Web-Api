import React, { useState } from 'react';
import { Package, Search } from 'lucide-react';
import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import { getProductById } from '@/services/products/productService';
import { useToast } from '@/context/ToastContext';

/**
 * ProductInfoTab Component
 *
 * A reusable component for searching and displaying product information.
 * Used in AddStockTransfer and can be used in other inventory-related forms.
 *
 * @param {Object} props - Component props
 * @param {Object} props.selectedProduct - Currently selected product object
 * @param {function} props.onProductSelect - Callback when a product is selected
 * @param {boolean} props.disabled - Whether the search is disabled (e.g., in update mode)
 * @param {boolean} props.showSearch - Whether to show the search input (default: true)
 */
const ProductInfoTab = ({
  selectedProduct,
  onProductSelect,
  disabled = false,
  showSearch = true,
}) => {
  const { showSuccess, showError } = useToast();
  const [productSearchTerm, setProductSearchTerm] = useState('');
  const [isSearching, setIsSearching] = useState(false);

  /**
   * Handle product search
   * Searches for a product by ID
   */
  const handleProductSearch = async () => {
    if (!productSearchTerm.trim()) {
      showError('Search Error', 'Please enter a product ID');
      return;
    }

    setIsSearching(true);
    try {
      const searchId = parseInt(productSearchTerm);
      if (isNaN(searchId)) {
        showError('Invalid Input', 'Please enter a valid product ID');
        setIsSearching(false);
        return;
      }

      const product = await getProductById(searchId);
      if (product) {
        onProductSelect(product);
        showSuccess('Product Found', `Found: ${product.name}`);
        setProductSearchTerm(''); // Clear search after successful selection
      } else {
        showError('Product Not Found', 'No product found with this ID');
      }
    } catch (error) {
      showError('Search Failed', error.message || 'Failed to search product');
      onProductSelect(null);
    } finally {
      setIsSearching(false);
    }
  };

  /**
   * Handle Enter key press in search input
   */
  const handleKeyPress = e => {
    if (e.key === 'Enter' && !disabled) {
      handleProductSearch();
    }
  };

  return (
    <div>
      <div className='flex items-center gap-2 mb-6'>
        <Package className='h-5 w-5' />
        <h3 className='text-lg font-semibold'>Product Information</h3>
      </div>

      {/* Search Section */}
      {showSearch && (
        <div className='mb-6'>
          <label className='block text-sm font-medium mb-2'>
            Search Product <span className='text-red-500'>*</span>
          </label>
          <div className='flex gap-2'>
            <div className='flex-1'>
              <Input
                placeholder='Enter Product ID'
                value={productSearchTerm}
                onChange={e => setProductSearchTerm(e.target.value)}
                onKeyPress={handleKeyPress}
                disabled={disabled || isSearching}
                className='h-12'
              />
            </div>
            <Button
              onClick={handleProductSearch}
              disabled={disabled || isSearching || !productSearchTerm.trim()}
              loading={isSearching}
              className='h-12 px-6'
            >
              <Search className='h-5 w-5 mr-2' />
              Search
            </Button>
          </div>
          <p className='text-sm text-gray-500 mt-2'>
            Enter the Product ID to search
          </p>
        </div>
      )}

      {/* Product Details Display */}
      {selectedProduct && (
        <div className='bg-blue-50 border border-blue-200 rounded-lg p-6'>
          <h4 className='font-semibold text-blue-900 mb-4'>Selected Product</h4>
          <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
            <div>
              <p className='text-sm text-gray-600 mb-1'>Product Name</p>
              <p className='font-medium text-gray-900'>
                {selectedProduct.name}
              </p>
            </div>
            <div>
              <p className='text-sm text-gray-600 mb-1'>SKU</p>
              <p className='font-medium text-gray-900'>{selectedProduct.sku}</p>
            </div>
            <div>
              <p className='text-sm text-gray-600 mb-1'>Category</p>
              <p className='font-medium text-gray-900'>
                {selectedProduct.categoryName || 'N/A'}
              </p>
            </div>
            <div>
              <p className='text-sm text-gray-600 mb-1'>Product ID</p>
              <p className='font-medium text-gray-900'>#{selectedProduct.id}</p>
            </div>

            <div>
              <p className='text-sm text-gray-600 mb-1'>Unit Price</p>
              <p className='font-medium text-gray-900'>
                ${selectedProduct.unitPrice?.toFixed(2) || '0.00'}
              </p>
            </div>
            <div>
              <p className='text-sm text-gray-600 mb-1'>Status</p>
              <span
                className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                  selectedProduct.isActive
                    ? 'bg-green-100 text-green-800'
                    : 'bg-red-100 text-red-800'
                }`}
              >
                {selectedProduct.isActive ? 'Active' : 'Inactive'}
              </span>
            </div>
          </div>
        </div>
      )}

      {/* Empty State */}
      {!selectedProduct && (
        <div className='text-center py-8 text-gray-500'>
          <Package className='h-12 w-12 mx-auto mb-3 text-gray-400' />
          <p>Search for a product to get started</p>
        </div>
      )}
    </div>
  );
};

export default ProductInfoTab;
