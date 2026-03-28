import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Package, AlertCircle } from 'lucide-react';
import { useTranslation } from 'react-i18next';

import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { getProductById } from '@features/products/services/productApi';
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
  const { t, i18n } = useTranslation();
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
    {
      id: 'basic',
      label: t(i18nKeyContainer.products.productView.tabs.basicInfo),
    },
    {
      id: 'pricing',
      label: t(i18nKeyContainer.products.productView.tabs.pricing),
    },
    {
      id: 'details',
      label: t(i18nKeyContainer.products.productView.tabs.details),
    },
  ];

  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';

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
            <DialogTitle className='text-xl'>
              {t(i18nKeyContainer.products.productView.title)}
            </DialogTitle>
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
                  {t(i18nKeyContainer.products.productView.sections.productInformation)}
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
                    ✓{' '}
                    {product.isActive
                      ? t(i18nKeyContainer.products.shared.status.active)
                      : t(i18nKeyContainer.products.shared.status.inactive)}
                  </span>
                </div>

                {/* Product Details Grid */}
                <div className='grid grid-cols-2 gap-6'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.products.productView.fields.productName)}
                    </label>
                    <p className='text-gray-900'>
                      {product.name || t(i18nKeyContainer.products.shared.hyphen)}
                    </p>
                  </div>

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.products.productView.fields.sku)}
                    </label>
                    <p className='text-gray-900'>
                      {product.sku || t(i18nKeyContainer.products.shared.hyphen)}
                    </p>
                  </div>

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.products.productView.fields.category)}
                    </label>
                    <p className='text-gray-900'>
                      {product.categoryName ||
                        t(i18nKeyContainer.products.shared.hyphen)}
                    </p>
                  </div>

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.products.productView.fields.unitOfMeasure)}
                    </label>
                    <p className='text-gray-900'>
                      {product.unitOfMeasureName ||
                        t(i18nKeyContainer.products.shared.hyphen)}
                    </p>
                  </div>
                </div>

                {/* Description */}
                <div className='mt-6'>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    {t(i18nKeyContainer.products.productView.fields.description)}
                  </label>
                  <p className='text-gray-600 text-sm'>
                    {product.description ||
                      t(
                        i18nKeyContainer.products.productView.placeholders
                          .noDescriptionAvailable
                      )}
                  </p>
                </div>

                {/* Status */}
                <div className='mt-6'>
                  <label className='block text-sm font-medium text-gray-700 mb-2'>
                    {t(i18nKeyContainer.products.productView.fields.status)}
                  </label>
                  <span
                    className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium ${
                      product.isActive
                        ? 'bg-green-100 text-green-800'
                        : 'bg-gray-100 text-gray-800'
                    }`}
                  >
                    ✓{' '}
                    {product.isActive
                      ? t(i18nKeyContainer.products.shared.status.active)
                      : t(i18nKeyContainer.products.shared.status.inactive)}
                  </span>
                </div>
              </div>
            </div>
          )}

          {/* Pricing Tab */}
          {activeTab === 'pricing' && (
            <div className='space-y-6'>
              <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                {t(i18nKeyContainer.products.productView.sections.pricingInformation)}
              </h3>

              {/* Main Pricing */}
              <div className='grid grid-cols-2 gap-6'>
                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    {t(i18nKeyContainer.products.productView.fields.costPrice)}
                  </label>
                  <p className='text-2xl font-semibold text-gray-900'>
                    ${product.costPrice?.toFixed(2) || '0.00'}
                  </p>
                </div>

                <div>
                  <label className='block text-sm font-medium text-gray-700 mb-1'>
                    {t(i18nKeyContainer.products.productView.fields.sellingPrice)}
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
                    {t(i18nKeyContainer.products.productView.fields.profitMargin)}
                  </label>
                  <p className='text-2xl font-bold text-blue-600'>
                    {profitMargin}%
                  </p>
                </div>

                <div className='bg-green-50 p-4 rounded-lg'>
                  <label className='block text-sm font-medium text-green-700 mb-1'>
                    {t(i18nKeyContainer.products.productView.fields.profitPerUnit)}
                  </label>
                  <p className='text-2xl font-bold text-green-600'>
                    ${profitPerUnit.toFixed(2)}
                  </p>
                </div>

                <div className='bg-purple-50 p-4 rounded-lg'>
                  <label className='block text-sm font-medium text-purple-700 mb-1'>
                    {t(i18nKeyContainer.products.productView.fields.markup)}
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
                    {t(i18nKeyContainer.products.productView.sections.creationDetails)}
                  </h4>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.products.productView.fields.createdAt)}
                    </label>
                    <p className='text-gray-900'>
                      {product.createdAt
                        ? new Date(product.createdAt).toLocaleString(activeLocale)
                        : t(i18nKeyContainer.products.shared.hyphen)}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.products.productView.fields.createdBy)}
                    </label>
                    <p className='text-gray-900'>
                      {product.createdByUserName ||
                        t(i18nKeyContainer.products.shared.hyphen)}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.products.productView.fields.createdByUserId)}
                    </label>
                    <p className='text-gray-900 font-mono text-xs'>
                      {product.createdByUserId ||
                        t(i18nKeyContainer.products.shared.hyphen)}
                    </p>
                  </div>
                </div>

                {/* Updated Information */}
                <div className='space-y-4'>
                  <h4 className='font-semibold text-gray-900'>
                    {t(i18nKeyContainer.products.productView.sections.lastUpdateDetails)}
                  </h4>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.products.productView.fields.updatedAt)}
                    </label>
                    <p className='text-gray-900'>
                      {product.updatedAt
                        ? new Date(product.updatedAt).toLocaleString(activeLocale)
                        : t(i18nKeyContainer.products.shared.hyphen)}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.products.productView.fields.updatedBy)}
                    </label>
                    <p className='text-gray-900'>
                      {product.updatedByUserName ||
                        t(i18nKeyContainer.products.shared.hyphen)}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.products.productView.fields.updatedByUserId)}
                    </label>
                    <p className='text-gray-900 font-mono text-xs'>
                      {product.updatedByUserId ||
                        t(i18nKeyContainer.products.shared.hyphen)}
                    </p>
                  </div>
                </div>
              </div>

              {/* System Information */}
              <div className='border-t pt-6 mt-6'>
                <h4 className='font-semibold text-gray-900 mb-4'>
                  {t(i18nKeyContainer.products.productView.sections.systemInformation)}
                </h4>
                <div className='grid grid-cols-2 gap-6'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.products.productView.fields.productId)}
                    </label>
                    <p className='text-gray-900 font-mono text-xs'>
                      {product.id || t(i18nKeyContainer.products.shared.hyphen)}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.products.productView.fields.categoryId)}
                    </label>
                    <p className='text-gray-900 font-mono text-xs'>
                      {product.categoryId ||
                        t(i18nKeyContainer.products.shared.hyphen)}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(
                        i18nKeyContainer.products.productView.fields
                          .unitOfMeasureId
                      )}
                    </label>
                    <p className='text-gray-900 font-mono text-xs'>
                      {product.unitOfMeasureId ||
                        t(i18nKeyContainer.products.shared.hyphen)}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.products.productView.fields.status)}
                    </label>
                    <p className='text-gray-900'>
                      {product.isDeleted
                        ? t(i18nKeyContainer.products.shared.status.deleted)
                        : t(i18nKeyContainer.products.shared.status.active)}
                    </p>
                  </div>
                </div>

                {/* Deletion Information (if deleted) */}
                {product.isDeleted && (
                  <div className='mt-6 p-4 bg-red-50 rounded-lg'>
                    <h5 className='font-semibold text-red-900 mb-3'>
                      {t(
                        i18nKeyContainer.products.productView.sections
                          .deletionInformation
                      )}
                    </h5>
                    <div className='grid grid-cols-2 gap-4'>
                      <div>
                        <label className='block text-sm font-medium text-red-700 mb-1'>
                          {t(i18nKeyContainer.products.productView.fields.deletedAt)}
                        </label>
                        <p className='text-red-900 text-sm'>
                          {product.deleteAt
                            ? new Date(product.deleteAt).toLocaleString(activeLocale)
                            : t(i18nKeyContainer.products.shared.hyphen)}
                        </p>
                      </div>
                      <div>
                        <label className='block text-sm font-medium text-red-700 mb-1'>
                          {t(i18nKeyContainer.products.productView.fields.deletedBy)}
                        </label>
                        <p className='text-red-900 text-sm'>
                          {product.deletedByUserName ||
                            t(i18nKeyContainer.products.shared.hyphen)}
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
            {t(i18nKeyContainer.products.shared.close)}
          </button>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default ProductViewDialog;
