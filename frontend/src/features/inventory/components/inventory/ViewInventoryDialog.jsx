import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Package, MapPin, Archive, Calendar, User } from 'lucide-react';
import { getInventoryById } from '@features/inventory/services/inventoryApi';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

/**
 * ViewInventoryDialog Component
 *
 * A dialog component to display detailed inventory information in a tabbed interface.
 * Shows Product & Location Info, Stock Levels, and System Info tabs.
 *
 * @param {Object} props - Component props
 * @param {boolean} props.open - Controls dialog visibility
 * @param {Function} props.onOpenChange - Callback when dialog open state changes
 * @param {number} props.inventoryId - Inventory ID to display
 *
 * @example
 * ```jsx
 * const [viewDialogOpen, setViewDialogOpen] = useState(false);
 * const [selectedInventoryId, setSelectedInventoryId] = useState(null);
 *
 * <ViewInventoryDialog
 *   open={viewDialogOpen}
 *   onOpenChange={setViewDialogOpen}
 *   inventoryId={selectedInventoryId}
 * />
 * ```
 */
const ViewInventoryDialog = ({ open, onOpenChange, inventoryId }) => {
  const { t, i18n } = useTranslation();
  const [activeTab, setActiveTab] = useState('info');
  const [inventoryData, setInventoryData] = useState({
    id: 0,
    product: {
      id: 0,
      name: '',
      sku: '',
      categoryName: '',
      unitOfMeasureName: '',
    },
    location: {
      id: 0,
      name: '',
      address: '',
      type: '',
    },
    quantityOnHand: 0,
    reorderLevel: 0,
    maxLevel: 0,
    createdAt: null,
    createdByUserId: null,
    createdByUserName: null,
    updatedAt: null,
    updatedByUserId: null,
    updatedByUserName: null,
  });

  // Tab configuration
  const tabs = [
    { id: 'info', label: t(i18nKeyContainer.inventory.inventoryView.tabs.info) },
    {
      id: 'stock',
      label: t(i18nKeyContainer.inventory.inventoryView.tabs.stock),
    },
    {
      id: 'system',
      label: t(i18nKeyContainer.inventory.inventoryView.tabs.system),
    },
  ];

  /**
   * Calculate stock status based on levels
   */
  const getStockStatus = () => {
    const { quantityOnHand, reorderLevel } = inventoryData;
    if (quantityOnHand === 0) {
      return t(i18nKeyContainer.inventory.shared.status.outOfStock);
    }
    if (quantityOnHand <= reorderLevel) {
      return t(i18nKeyContainer.inventory.shared.status.lowStock);
    }
    return t(i18nKeyContainer.inventory.shared.status.inStock);
  };

  /**
   * Calculate available stock percentage
   */
  const getStockPercentage = () => {
    const { quantityOnHand, maxLevel } = inventoryData;
    if (maxLevel === 0) return 0;
    return Math.min(Math.round((quantityOnHand / maxLevel) * 100), 100);
  };

  /**
   * Fetch inventory data when dialog opens
   */
  useEffect(() => {
    const fetchInventoryData = async () => {
      if (inventoryId && open) {
        try {
          const response = await getInventoryById(inventoryId);
          if (response.success) {
            setInventoryData(response.data);
          }
        } catch (error) {
          console.error('Error fetching inventory:', error);
        }
      }
    };
    fetchInventoryData();
  }, [inventoryId, open]);

  /**
   * Reset to first tab when dialog closes
   */
  useEffect(() => {
    if (!open) {
      setActiveTab('info');
    }
  }, [open]);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className='max-w-4xl max-h-[90vh] overflow-hidden flex flex-col'>
        {/* Header */}
        <DialogHeader>
          <div className='flex items-center gap-2'>
            <Archive className='w-5 h-5' />
            <DialogTitle className='text-xl'>
              {t(i18nKeyContainer.inventory.inventoryView.title)}
            </DialogTitle>
            <span className='text-sm text-gray-500 font-normal'>
              {t(i18nKeyContainer.inventory.inventoryView.subtitleId, {
                id: inventoryData.id,
              })}
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
          {/* Product & Location Info Tab */}
          {activeTab === 'info' && (
            <div className='space-y-6'>
              <div className='grid grid-cols-1 md:grid-cols-2 gap-6'>
                {/* Product Info Column */}
                <div className='bg-blue-50 border border-blue-200 rounded-lg p-6'>
                  <h3 className='text-lg font-semibold mb-4 flex items-center gap-2 text-blue-900'>
                    <Package className='w-5 h-5' />
                    {t(i18nKeyContainer.inventory.inventoryView.sections.productInformation)}
                  </h3>
                  <div className='space-y-4'>
                    <div>
                      <label className='block text-sm font-medium text-blue-700 mb-1'>
                        {t(i18nKeyContainer.inventory.inventoryView.fields.productName)}
                      </label>
                      <p className='text-blue-900 font-medium text-lg'>
                        {inventoryData.product.name || '-'}
                      </p>
                    </div>
                    <div>
                      <label className='block text-sm font-medium text-blue-700 mb-1'>
                        {t(i18nKeyContainer.inventory.inventoryView.fields.sku)}
                      </label>
                      <p className='text-blue-900 font-mono'>
                        {inventoryData.product.sku || '-'}
                      </p>
                    </div>
                    <div>
                      <label className='block text-sm font-medium text-blue-700 mb-1'>
                        {t(i18nKeyContainer.inventory.inventoryView.fields.category)}
                      </label>
                      <p className='text-blue-900'>
                        {inventoryData.product.categoryName || '-'}
                      </p>
                    </div>
                    <div>
                      <label className='block text-sm font-medium text-blue-700 mb-1'>
                        {t(i18nKeyContainer.inventory.inventoryView.fields.unitOfMeasure)}
                      </label>
                      <p className='text-blue-900'>
                        {inventoryData.product.unitOfMeasureName || '-'}
                      </p>
                    </div>
                  </div>
                </div>

                {/* Location Info Column */}
                <div className='bg-green-50 border border-green-200 rounded-lg p-6'>
                  <h3 className='text-lg font-semibold mb-4 flex items-center gap-2 text-green-900'>
                    <MapPin className='w-5 h-5' />
                    {t(i18nKeyContainer.inventory.inventoryView.sections.locationInformation)}
                  </h3>
                  <div className='space-y-4'>
                    <div>
                      <label className='block text-sm font-medium text-green-700 mb-1'>
                        {t(i18nKeyContainer.inventory.inventoryView.fields.name)}
                      </label>
                      <p className='text-green-900 font-medium text-lg'>
                        {inventoryData.location.name || '-'}
                      </p>
                    </div>
                    <div>
                      <label className='block text-sm font-medium text-green-700 mb-1'>
                        {t(i18nKeyContainer.inventory.inventoryView.fields.type)}
                      </label>
                      <p className='text-green-900'>
                        {inventoryData.location.type || '-'}
                      </p>
                    </div>
                    <div>
                      <label className='block text-sm font-medium text-green-700 mb-1'>
                        {t(i18nKeyContainer.inventory.inventoryView.fields.address)}
                      </label>
                      <p className='text-green-900'>
                        {inventoryData.location.address || '-'}
                      </p>
                    </div>
                    <div>
                      <label className='block text-sm font-medium text-green-700 mb-1'>
                        {t(i18nKeyContainer.inventory.inventoryView.fields.locationId)}
                      </label>
                      <p className='text-green-900 font-mono'>
                        #{inventoryData.location.id || '-'}
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Stock Levels Tab */}
          {activeTab === 'stock' && (
            <div className='space-y-6'>
              {/* Stock Status Badge */}
              <div className='flex items-center justify-center mb-6'>
                <span
                  className={`inline-flex items-center gap-2 px-4 py-2 rounded-full text-sm font-medium ${
                    getStockStatus() ===
                    t(i18nKeyContainer.inventory.shared.status.inStock)
                      ? 'bg-green-100 text-green-800'
                      : getStockStatus() ===
                          t(i18nKeyContainer.inventory.shared.status.lowStock)
                        ? 'bg-yellow-100 text-yellow-800'
                        : 'bg-red-100 text-red-800'
                  }`}
                >
                  <Archive className='w-4 h-4' />
                  {getStockStatus()}
                </span>
              </div>

              {/* Stock Level Cards */}
              <div className='grid grid-cols-1 md:grid-cols-2 gap-6'>
                {/* Quantity on Hand */}
                <div className='bg-gradient-to-br from-blue-50 to-blue-100 border border-blue-200 rounded-lg p-6'>
                  <label className='block text-sm font-medium text-blue-700 mb-2'>
                    {t(i18nKeyContainer.inventory.inventoryView.fields.quantityOnHand)}
                  </label>
                  <p className='text-4xl font-bold text-blue-900'>
                    {inventoryData.quantityOnHand.toFixed(2)}
                  </p>
                  <p className='text-sm text-blue-600 mt-2'>
                    {inventoryData.product.unitOfMeasureName || 'units'}
                  </p>
                </div>

                {/* Available Stock (same as quantity on hand in this context) */}
                <div className='bg-gradient-to-br from-green-50 to-green-100 border border-green-200 rounded-lg p-6'>
                  <label className='block text-sm font-medium text-green-700 mb-2'>
                    {t(i18nKeyContainer.inventory.inventoryView.fields.availableStock)}
                  </label>
                  <p className='text-4xl font-bold text-green-900'>
                    {inventoryData.quantityOnHand.toFixed(2)}
                  </p>
                  <p className='text-sm text-green-600 mt-2'>
                    {t(i18nKeyContainer.inventory.inventoryView.labels.readyForUseOrSale)}
                  </p>
                </div>

                {/* Reorder Level */}
                <div className='bg-gradient-to-br from-yellow-50 to-yellow-100 border border-yellow-200 rounded-lg p-6'>
                  <label className='block text-sm font-medium text-yellow-700 mb-2'>
                    {t(i18nKeyContainer.inventory.inventoryView.fields.reorderLevel)}
                  </label>
                  <p className='text-4xl font-bold text-yellow-900'>
                    {inventoryData.reorderLevel.toFixed(2)}
                  </p>
                  <p className='text-sm text-yellow-600 mt-2'>
                    {t(i18nKeyContainer.inventory.inventoryView.labels.minimumThreshold)}
                  </p>
                </div>

                {/* Max Level */}
                <div className='bg-gradient-to-br from-purple-50 to-purple-100 border border-purple-200 rounded-lg p-6'>
                  <label className='block text-sm font-medium text-purple-700 mb-2'>
                    {t(i18nKeyContainer.inventory.inventoryView.fields.maxLevel)}
                  </label>
                  <p className='text-4xl font-bold text-purple-900'>
                    {inventoryData.maxLevel.toFixed(2)}
                  </p>
                  <p className='text-sm text-purple-600 mt-2'>
                    {t(i18nKeyContainer.inventory.inventoryView.labels.storageCapacity)}
                  </p>
                </div>
              </div>

              {/* Stock Percentage Bar */}
              <div className='bg-gray-50 border border-gray-200 rounded-lg p-6'>
                <div className='flex items-center justify-between mb-2'>
                  <label className='text-sm font-medium text-gray-700'>
                    {t(i18nKeyContainer.inventory.inventoryView.fields.stockLevel)}
                  </label>
                  <span className='text-sm font-semibold text-gray-900'>
                    {getStockPercentage()}%
                  </span>
                </div>
                <div className='w-full bg-gray-200 rounded-full h-4 overflow-hidden'>
                  <div
                    className={`h-full rounded-full transition-all ${
                      getStockPercentage() > 50
                        ? 'bg-green-500'
                        : getStockPercentage() > 25
                          ? 'bg-yellow-500'
                          : 'bg-red-500'
                    }`}
                    style={{ width: `${getStockPercentage()}%` }}
                  />
                </div>
                <p className='text-xs text-gray-500 mt-2'>
                  {inventoryData.quantityOnHand.toFixed(2)} /{' '}
                  {inventoryData.maxLevel.toFixed(2)}{' '}
                  {inventoryData.product.unitOfMeasureName || 'units'}
                </p>
              </div>
            </div>
          )}

          {/* System Info Tab */}
          {activeTab === 'system' && (
            <div className='space-y-6'>
              {/* System IDs Section */}
              <div className='bg-gray-50 border border-gray-200 rounded-lg p-6'>
                <h3 className='text-lg font-semibold mb-4 text-gray-900'>
                  {t(i18nKeyContainer.inventory.inventoryView.sections.systemIdentifiers)}
                </h3>
                <div className='grid grid-cols-1 md:grid-cols-3 gap-6'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.inventory.inventoryView.fields.inventoryId)}
                    </label>
                    <p className='text-gray-900 font-mono text-sm'>
                      {inventoryData.id}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.inventory.inventoryView.fields.productId)}
                    </label>
                    <p className='text-gray-900 font-mono text-sm'>
                      {inventoryData.product.id || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.inventory.inventoryView.fields.locationId)}
                    </label>
                    <p className='text-gray-900 font-mono text-sm'>
                      {inventoryData.location.id || '-'}
                    </p>
                  </div>
                </div>
              </div>

              {/* Audit Information Section */}
              <div className='border-t pt-6'>
                <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                  <Calendar className='w-5 h-5' />
                  {t(i18nKeyContainer.inventory.inventoryView.sections.auditTrail)}
                </h3>

                <div className='grid grid-cols-1 md:grid-cols-2 gap-6'>
                  {/* Created Information */}
                  <div className='bg-blue-50 p-4 rounded-lg space-y-3'>
                    <h4 className='font-semibold text-blue-900 flex items-center gap-2'>
                      <Calendar className='w-4 h-4' />
                      {t(i18nKeyContainer.inventory.inventoryView.sections.created)}
                    </h4>
                    <div>
                      <label className='block text-sm font-medium text-blue-700 mb-1'>
                        {t(i18nKeyContainer.inventory.inventoryView.fields.dateTime)}
                      </label>
                      <p className='text-blue-900 text-sm'>
                        {inventoryData.createdAt
                          ? new Date(inventoryData.createdAt).toLocaleString(
                              i18n.resolvedLanguage || i18n.language || 'en',
                              {
                                year: 'numeric',
                                month: 'long',
                                day: 'numeric',
                                hour: '2-digit',
                                minute: '2-digit',
                              }
                            )
                          : '-'}
                      </p>
                    </div>
                    <div>
                      <label className='text-sm font-medium text-blue-700 mb-1 flex items-center gap-1'>
                        <User className='w-3 h-3' />
                        {t(i18nKeyContainer.inventory.inventoryView.fields.createdByName)}
                      </label>
                      <p className='text-blue-900 text-sm'>
                        {inventoryData.createdByUserName || '-'}
                      </p>
                    </div>
                    <div>
                      <label className='block text-sm font-medium text-blue-700 mb-1'>
                        {t(i18nKeyContainer.inventory.inventoryView.fields.userId)}
                      </label>
                      <p className='text-blue-900 font-mono text-xs'>
                        {inventoryData.createdByUserId || '-'}
                      </p>
                    </div>
                  </div>

                  {/* Updated Information */}
                  <div className='bg-purple-50 p-4 rounded-lg space-y-3'>
                    <h4 className='font-semibold text-purple-900 flex items-center gap-2'>
                      <Calendar className='w-4 h-4' />
                      {t(i18nKeyContainer.inventory.inventoryView.sections.updated)}
                    </h4>
                    <div>
                      <label className='block text-sm font-medium text-purple-700 mb-1'>
                        {t(i18nKeyContainer.inventory.inventoryView.fields.dateTime)}
                      </label>
                      <p className='text-purple-900 text-sm'>
                        {inventoryData.updatedAt
                          ? new Date(inventoryData.updatedAt).toLocaleString(
                              i18n.resolvedLanguage || i18n.language || 'en',
                              {
                                year: 'numeric',
                                month: 'long',
                                day: 'numeric',
                                hour: '2-digit',
                                minute: '2-digit',
                              }
                            )
                          : t(
                              i18nKeyContainer.inventory.inventoryView.placeholders
                                .neverUpdated
                            )}
                      </p>
                    </div>
                    <div>
                      <label className='text-sm font-medium text-purple-700 mb-1 flex items-center gap-1'>
                        <User className='w-3 h-3' />
                        {t(i18nKeyContainer.inventory.inventoryView.fields.updatedByName)}
                      </label>
                      <p className='text-purple-900 text-sm'>
                        {inventoryData.updatedByUserName || '-'}
                      </p>
                    </div>
                    <div>
                      <label className='block text-sm font-medium text-purple-700 mb-1'>
                        {t(i18nKeyContainer.inventory.inventoryView.fields.userId)}
                      </label>
                      <p className='text-purple-900 font-mono text-xs'>
                        {inventoryData.updatedByUserId || '-'}
                      </p>
                    </div>
                  </div>
                </div>
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
            {t(i18nKeyContainer.inventory.shared.close)}
          </button>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default ViewInventoryDialog;
