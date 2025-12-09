import React, { useState, useEffect } from 'react';
import { X, Package, MapPin, History as HistoryIcon } from 'lucide-react';
import Button from '@components/Buttons/Button';
import { cn } from '@/lib/utils';
import { useToast } from '@/context/ToastContext';
import { createStockTransfer } from '@/services/stock/stockTransferService';
import ProductInfoTab from './ProductInfoTab';
import TransferDetailsTab from './TransferDetailsTab';
import HistoryTab from './HistoryTab';
import { getInventoriesByProductId } from '@/services/products/productService';
import ConfirmationDialog from '../ui/ConfirmationDialog';

/**
 * AddStockTransfer Component
 *
 * A comprehensive modal component for creating stock transfers between warehouses.
 * Features a tabbed interface with Product Selection, Transfer Details, and History tracking.
 *
 * @param {Object} props - Component props
 * @param {boolean} props.isOpen - Controls modal visibility
 * @param {function} props.onClose - Callback function when modal is closed
 * @param {function} props.onSuccess - Optional callback after successful transfer creation
 * @param {number} props.transferId - ID of transfer to view (0 for new transfer)
 *
 * @description
 * The component has 3 tabs:
 * 1. Product Tab: Search and select product for transfer
 * 2. Transfer Details Tab: Select From/To locations, set quantity, view summary
 * 3. History Tab: View transfer status and timeline (read-only, shows after creation)
 */
const AddStockTransfer = ({ isOpen, onClose, onSuccess, transferId = 0 }) => {
  const { showSuccess, showError } = useToast();
  const [activeTab, setActiveTab] = useState(0);
  const [mode, setMode] = useState('add'); // 'add' or 'view'
  const [isLoading, setIsLoading] = useState(false);

  // Product state
  const [selectedProduct, setSelectedProduct] = useState(null);

  // Transfer details state
  const [fromLocationId, setFromLocationId] = useState('');
  const [toLocationId, setToLocationId] = useState('');
  const [quantity, setQuantity] = useState(0);
  const [notes, setNotes] = useState('');

  // Locations data
  const [locations, setLocations] = useState([]);

  // History/Transfer data (for view mode)
  const [transferData, setTransferData] = useState(null);
  const [error, setError] = useState({ isError: false, message: '' });

  // Tabs configuration
  const tabs = [
    { id: 0, label: 'Product', icon: Package },
    { id: 1, label: 'Transfer Details', icon: MapPin },
    { id: 2, label: 'History', icon: HistoryIcon },
  ];

  /**
   * Validate form before submission
   */
  const isFormValid = () => {
    if (!selectedProduct) {
      showError('Validation Error', 'Please select a product');
      setActiveTab(0);
      return false;
    }
    if (!fromLocationId) {
      showError('Validation Error', 'Please select a from location');
      setActiveTab(1);
      return false;
    }
    if (!toLocationId) {
      showError('Validation Error', 'Please select a to location');
      setActiveTab(1);
      return false;
    }
    if (fromLocationId === toLocationId) {
      showError('Validation Error', 'From and To locations must be different');
      setActiveTab(1);
      return false;
    }
    if (!quantity || quantity <= 0) {
      showError('Validation Error', 'Please enter a valid quantity');
      setActiveTab(1);
      return false;
    }
    return true;
  };

  /**
   * Handle form submission
   */
  const handleSubmit = async () => {
    if (!isFormValid()) return;

    setIsLoading(true);
    try {
      const transferRequest = {
        productId: selectedProduct.id,
        fromLocationId: parseInt(fromLocationId),
        toLocationId: parseInt(toLocationId),
        quantity: parseFloat(quantity),
      };

      const result = await createStockTransfer(transferRequest);

      if (result.success) {
        showSuccess(
          'Transfer Created Successfully',
          `Stock transfer from ${locations.find(l => l.id === parseInt(fromLocationId))?.name} to ${locations.find(l => l.id === parseInt(toLocationId))?.name} has been initiated.`
        );

        // Call onSuccess callback if provided
        if (onSuccess) {
          onSuccess(result.data);
        }

        handleClose();
      } else {
        showError(
          'Transfer Creation Failed',
          result.error || 'Failed to create stock transfer. Please try again.'
        );
      }
    } catch (error) {
      console.error('Transfer creation error:', error);
      showError(
        'Transfer Creation Failed',
        error.message || 'An unexpected error occurred. Please try again.'
      );
    } finally {
      setIsLoading(false);
    }
  };

  /**
   * Reset form to initial state
   */
  const resetForm = () => {
    setSelectedProduct(null);
    setFromLocationId('');
    setToLocationId('');
    setQuantity(0);
    setNotes('');
    setActiveTab(0);
    setTransferData(null);
  };

  /**
   * Handle modal close
   */
  const handleClose = () => {
    resetForm();
    onClose();
  };

  /**
   * Handle cancel button
   */
  const handleCancel = () => {
    handleClose();
  };

  /**
   * Load locations on component mount
   */
  useEffect(() => {
    const fetchLocations = async () => {
      try {
        const response = await getInventoriesByProductId(selectedProduct?.id);
        if (response.success) {
          if (response.data.length < 2) {
            console.log('location lenght is less then 2 ');
            setError({
              isError: true,
              message:
                'The selected Product has only one inventory location, please select a different product',
            });
            return;
          }
          const locations = response.data.map(e => {
            return {
              id: e.locationId,
              name: e.locationName,
            };
          });
          setLocations(locations);
          setError({ isError: false, message: '' });
          return;
        } else {
          setError({
            isError: true,
            message:
              'The selected Product has no inventories, please select a different product ' +
              'or create inventory for this product',
          });
        }
      } catch (error) {
        console.error('Error fetching locations:', error);
      }
    };

    if (selectedProduct) {
      fetchLocations();
    }
  }, [selectedProduct]);

  /**
   * Load transfer data if viewing existing transfer
   */
  useEffect(() => {
    if (isOpen && transferId > 0) {
      setMode('view');
      // TODO: Fetch transfer data by ID
      // const fetchTransferData = async () => {
      //   const data = await getStockTransferById(transferId);
      //   setTransferData(data);
      //   // Populate form fields for viewing
      // };
      // fetchTransferData();
    } else {
      setMode('add');
    }
  }, [isOpen, transferId]);

  if (!isOpen) return null;

  return (
    <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4'>
      <div className='bg-white rounded-lg w-full max-w-4xl max-h-[90vh] overflow-hidden flex flex-col'>
        {/* Header */}
        <div className='flex items-center justify-between p-6 border-b flex-shrink-0'>
          <div className='flex items-center gap-3'>
            <Package className='h-6 w-6 text-gray-600' />
            <h2 className='text-xl font-semibold'>
              {mode === 'add' ? 'Create Stock Transfer' : 'View Stock Transfer'}
            </h2>
          </div>
          <button
            onClick={handleCancel}
            className='p-2 hover:bg-gray-100 rounded-lg transition-colors'
            disabled={isLoading}
          >
            <X className='h-5 w-5' />
          </button>
        </div>

        {/* Tabs Navigation */}
        <div className='flex bg-gray-50 border-b flex-shrink-0'>
          {tabs.map(tab => {
            const IconComponent = tab.icon;
            return (
              <button
                key={tab.id}
                onClick={() => setActiveTab(tab.id)}
                className={cn(
                  'flex-1 px-6 py-4 text-sm font-medium transition-colors border-b-2',
                  activeTab === tab.id
                    ? 'text-blue-600 border-blue-600 bg-white'
                    : 'text-gray-600 border-transparent hover:text-gray-800 hover:bg-gray-100'
                )}
                disabled={isLoading || selectedProduct === null}
              >
                <div className='flex items-center justify-center gap-2'>
                  {IconComponent && <IconComponent className='h-4 w-4' />}
                  {tab.label}
                </div>
              </button>
            );
          })}
        </div>

        {/* Tab Content */}
        <div className='flex-1 p-6 overflow-y-auto'>
          {/* Product Tab */}
          {activeTab === 0 && (
            <ProductInfoTab
              selectedProduct={selectedProduct}
              onProductSelect={setSelectedProduct}
              disabled={mode === 'view' || isLoading}
            />
          )}

          {/* Transfer Details Tab */}
          {activeTab === 1 && (
            <TransferDetailsTab
              locations={locations}
              fromLocationId={fromLocationId}
              toLocationId={toLocationId}
              quantity={quantity}
              onFromLocationChange={setFromLocationId}
              onToLocationChange={setToLocationId}
              onQuantityChange={setQuantity}
              selectedProduct={selectedProduct}
              disabled={mode === 'view' || isLoading}
              notes={notes}
              onNotesChange={setNotes}
            />
          )}

          {/* History Tab */}
          {activeTab === 2 && (
            <HistoryTab
              transferData={transferData}
              mode={mode}
              canChangeStatus={false} // Can be enabled later with status update functionality
            />
          )}
        </div>

        {/* Footer */}
        <div className='flex items-center justify-end gap-3 p-6 border-t bg-gray-50 flex-shrink-0'>
          <Button
            variant='secondary'
            onClick={handleCancel}
            disabled={isLoading}
            className='min-w-[100px]'
          >
            Cancel
          </Button>
          {mode === 'add' && (
            <Button
              onClick={handleSubmit}
              disabled={isLoading}
              loading={isLoading}
              className='min-w-[150px]'
            >
              Create Transfer
            </Button>
          )}
        </div>
      </div>
      {error.isError && (
        <ConfirmationDialog
          title='Error'
          message={error.message}
          confirmText='Ok'
          type='delete'
          onConfirm={() => {
            setError({ isError: false, message: '' });
            resetForm();
          }}
          onClose={() => {
            setError({ isError: false, message: '' });
            resetForm();
          }}
          isOpen={error.isError}
        />
      )}
    </div>
  );
};

export default AddStockTransfer;
