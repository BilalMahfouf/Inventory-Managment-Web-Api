import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Package, AlertCircle } from 'lucide-react';
import { getProductById } from '@/services/products/productService';
/**
 * ProductViewDialog Component
 *
 * A dialog component to display detailed product information in a tabbed interface.
 * Shows Basic Info, Pricing, Inventory, and Details tabs.
 *
 * @param {Object} props - Component props
 * @param {boolean} props.open - Controls dialog visibility
 * @param {Function} props.onOpenChange - Callback when dialog open state changes
 * @param {Object} props.product - Product data object
 * @param {string} props.product.id - Product ID
 * @param {string} props.product.sku - Product SKU
 * @param {string} props.product.name - Product name
 * @param {string} props.product.description - Product description
 * @param {string} props.product.categoryName - Category name
 * @param {string} props.product.unitOfMeasureName - Unit of measure
 * @param {number} props.product.costPrice - Cost price
 * @param {number} props.product.unitPrice - Selling price
 * @param {boolean} props.product.isActive - Active status
 * @param {string} props.product.createdAt - Creation date
 * @param {string} props.product.createdByUserName - Creator username
 * @param {string} props.product.updatedAt - Last update date
 * @param {string} props.product.updatedByUserName - Last updater username
 * @param {Object} props.inventory - Inventory data (optional)
 * @param {number} props.inventory.currentStock - Current stock level
 * @param {number} props.inventory.minimumStock - Minimum stock threshold
 * @param {number} props.inventory.maximumStock - Maximum stock threshold
 * @param {string} props.inventory.storageLocation - Storage location
 *
 * @example
 * ```jsx
 * const [viewDialogOpen, setViewDialogOpen] = useState(false);
 * const [selectedProduct, setSelectedProduct] = useState(null);
 *
 * <ProductViewDialog
 *   open={viewDialogOpen}
 *   onOpenChange={setViewDialogOpen}
 *   product={selectedProduct}
 *   inventory={{
 *     currentStock: 100,
 *     minimumStock: 10,
 *     maximumStock: 500,
 *     storageLocation: 'Warehouse A'
 *   }}
 *   onDuplicate={(product) => console.log('Duplicate:', product)}
 * />
 * ```
 */
const ProductViewDialog = ({ open, onOpenChange, productId }) => {
  const [activeTab, setActiveTab] = useState('basic');
  const [product, setProduct] = useState({
    id: 0,
    name: '',
    sku: '',
    description: '',
    categoryId: 0,
    categoryName: '',
    unitOfMeasureId: 0,
    unitOfMeasureName: '',
    costPrice: 0,
    unitPrice: 0,
    isActive: false,
    isDeleted: false,
    createdAt: null,
    createdByUserId: null,
    createdByUserName: null,
    updatedAt: null,
    updatedByUserId: null,
    updatedByUserName: null,
    deleteAt: null,
    deletedByUserId: null,
    deletedByUserName: null,
  });
  // to do refactor this to be calculated on backend
  // Calculate profit metrics
  const profitPerUnit = (product.unitPrice || 0) - (product.costPrice || 0);
  const profitMargin = product.unitPrice
    ? ((profitPerUnit / product.unitPrice) * 100).toFixed(0)
    : 0;
  const markup = product.costPrice
    ? ((profitPerUnit / product.costPrice) * 100).toFixed(0)
    : 0;
  const tabs = [
    { id: 'basic', label: 'Basic Info' },
    { id: 'pricing', label: 'Pricing' },
    { id: 'details', label: 'Details' },
  ];

  useEffect(() => {
    const fetchProduct = async () => {
      const data = await getProductById(productId);
      if (data) {
        setProduct(data);
      }
    };
    fetchProduct();
  }, [productId]);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className='max-w-4xl max-h-[90vh] overflow-hidden flex flex-col'>
        {/* Header */}
        <DialogHeader>
          <div className='flex items-center gap-2'>
            <Package className='w-5 h-5' />
            <DialogTitle className='text-xl'>Product Details</DialogTitle>
            <span className='text-sm text-gray-500 font-normal'>
              {product.sku}
            </span>
          </div>
        </DialogHeader>

        {/* Tabs Navigation */}
        <div className='flex border-b border-gray-200'>
          {tabs.map(tab => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`px-6 py-3 text-sm font-medium transition-colors ${
                activeTab === tab.id
                  ? 'border-b-2 border-blue-500 text-blue-600'
                  : 'text-gray-500 hover:text-gray-700'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </div>

        {/* Tab Content */}
        <div className='flex-1 overflow-y-auto py-6'>
          {/* Basic Info Tab */}
          {activeTab === 'basic' && (
            <div className='space-y-6'>
              <div>
                <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                  <Package className='w-5 h-5' />
                  Product Information
                </h3>

                {/* Status Badges */}
                <div className='flex gap-2 mb-6'>
                  <span
                    className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium ${
                      product.isActive
                        ? 'bg-green-100 text-green-800'
                        : 'bg-gray-100 text-gray-800'
                    }`}
                  >
                    ✓ {product.isActive ? 'Active' : 'Inactive'}
                  </span>
                </div>

                {/* Product Details Grid */}
                <div className='grid grid-cols-2 gap-6'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Product Name
                    </label>
                    <p className='text-gray-900'>{product.name || '-'}</p>
                  </div>

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      SKU
                    </label>
                    <p className='text-gray-900'>{product.sku || '-'}</p>
                  </div>

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Category
                    </label>
                    <p className='text-gray-900'>
                      {product.categoryName || '-'}
                    </p>
                  </div>

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Unit of Measure
                    </label>
                    <p className='text-gray-900'>
                      {product.unitOfMeasureName || '-'}
                    </p>
                  </div>
                </div>

                {/* Description */}
                <div className='mt-6'>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Description
                  </label>
                  <p className='text-gray-600 text-sm'>
                    {product.description || 'No description available'}
                  </p>
                </div>

                {/* Status */}
                <div className='mt-6'>
                  <label className='block text-sm font-medium text-gray-700 mb-2'>
                    Status
                  </label>
                  <span
                    className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium ${
                      product.isActive
                        ? 'bg-green-100 text-green-800'
                        : 'bg-gray-100 text-gray-800'
                    }`}
                  >
                    ✓ {product.isActive ? 'Active' : 'Inactive'}
                  </span>
                </div>
              </div>
            </div>
          )}

          {/* Pricing Tab */}
          {activeTab === 'pricing' && (
            <div className='space-y-6'>
              <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                $ Pricing Information
              </h3>

              {/* Main Pricing */}
              <div className='grid grid-cols-2 gap-6'>
                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Cost Price
                  </label>
                  <p className='text-2xl font-semibold text-gray-900'>
                    ${product.costPrice?.toFixed(2) || '0.00'}
                  </p>
                </div>

                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    Selling Price
                  </label>
                  <p className='text-2xl font-semibold text-gray-900'>
                    ${product.unitPrice?.toFixed(2) || '0.00'}
                  </p>
                </div>
              </div>

              {/* Profit Metrics */}
              <div className='grid grid-cols-3 gap-4 mt-6'>
                <div className='bg-blue-50 p-4 rounded-lg'>
                  <label className='block text-sm font-medium text-blue-700 mb-1'>
                    Profit Margin
                  </label>
                  <p className='text-2xl font-bold text-blue-600'>
                    {profitMargin}%
                  </p>
                </div>

                <div className='bg-green-50 p-4 rounded-lg'>
                  <label className='block text-sm font-medium text-green-700 mb-1'>
                    Profit per Unit
                  </label>
                  <p className='text-2xl font-bold text-green-600'>
                    ${profitPerUnit.toFixed(2)}
                  </p>
                </div>

                <div className='bg-purple-50 p-4 rounded-lg'>
                  <label className='block text-sm font-medium text-purple-700 mb-1'>
                    Markup
                  </label>
                  <p className='text-2xl font-bold text-purple-600'>
                    {markup}%
                  </p>
                </div>
              </div>
            </div>
          )}

          {/* Details Tab */}
          {activeTab === 'details' && (
            <div className='space-y-6'>
              <div className='grid grid-cols-2 gap-6'>
                {/* Created Information */}
                <div className='space-y-4'>
                  <h4 className='font-semibold text-gray-900'>
                    Creation Details
                  </h4>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Created At
                    </label>
                    <p className='text-gray-900'>
                      {product.createdAt
                        ? new Date(product.createdAt).toLocaleString()
                        : '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Created By
                    </label>
                    <p className='text-gray-900'>
                      {product.createdByUserName || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Created By User ID
                    </label>
                    <p className='text-gray-900 font-mono text-xs'>
                      {product.createdByUserId || '-'}
                    </p>
                  </div>
                </div>

                {/* Updated Information */}
                <div className='space-y-4'>
                  <h4 className='font-semibold text-gray-900'>
                    Last Update Details
                  </h4>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Updated At
                    </label>
                    <p className='text-gray-900'>
                      {product.updatedAt
                        ? new Date(product.updatedAt).toLocaleString()
                        : '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Updated By
                    </label>
                    <p className='text-gray-900'>
                      {product.updatedByUserName || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Updated By User ID
                    </label>
                    <p className='text-gray-900 font-mono text-xs'>
                      {product.updatedByUserId || '-'}
                    </p>
                  </div>
                </div>
              </div>

              {/* System Information */}
              <div className='border-t pt-6 mt-6'>
                <h4 className='font-semibold text-gray-900 mb-4'>
                  System Information
                </h4>
                <div className='grid grid-cols-2 gap-6'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Product ID
                    </label>
                    <p className='text-gray-900 font-mono text-xs'>
                      {product.id || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Category ID
                    </label>
                    <p className='text-gray-900 font-mono text-xs'>
                      {product.categoryId || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Unit of Measure ID
                    </label>
                    <p className='text-gray-900 font-mono text-xs'>
                      {product.unitOfMeasureId || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Status
                    </label>
                    <p className='text-gray-900'>
                      {product.isDeleted ? 'Deleted' : 'Active'}
                    </p>
                  </div>
                </div>

                {/* Deletion Information (if deleted) */}
                {product.isDeleted && (
                  <div className='mt-6 p-4 bg-red-50 rounded-lg'>
                    <h5 className='font-semibold text-red-900 mb-3'>
                      Deletion Information
                    </h5>
                    <div className='grid grid-cols-2 gap-4'>
                      <div>
                        <label className='block text-sm font-medium text-red-700 mb-1'>
                          Deleted At
                        </label>
                        <p className='text-red-900 text-sm'>
                          {product.deleteAt
                            ? new Date(product.deleteAt).toLocaleString()
                            : '-'}
                        </p>
                      </div>
                      <div>
                        <label className='block text-sm font-medium text-red-700 mb-1'>
                          Deleted By
                        </label>
                        <p className='text-red-900 text-sm'>
                          {product.deletedByUserName || '-'}
                        </p>
                      </div>
                    </div>
                  </div>
                )}
              </div>
            </div>
          )}
        </div>

        {/* Footer Actions */}
        <div className='flex justify-end gap-2 border-t pt-4'>
          <button
            onClick={() => onOpenChange(false)}
            className='px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 transition-colors'
          >
            Close
          </button>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default ProductViewDialog;
