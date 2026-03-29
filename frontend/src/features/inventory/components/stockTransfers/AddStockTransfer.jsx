import React, { useState, useEffect } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { X, Package, MapPin, History as HistoryIcon } from 'lucide-react';
import Button from '@components/Buttons/Button';
import { cn } from '@shared/lib/utils';
import { useToast } from '@shared/context/ToastContext';
import { createStockTransfer } from '@features/inventory/services/stockTransferApi';
import ProductInfoTab from './ProductInfoTab';
import TransferDetailsTab from './TransferDetailsTab';
import HistoryTab from './HistoryTab';
import { getInventoriesByProductId } from '@features/products/services/productApi';
import ConfirmationDialog from '@components/ui/ConfirmationDialog';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';

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
  const { t } = useTranslation();
  const { showSuccess, showError } = useToast();
  const queryClient = useQueryClient();
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

  // History/Transfer data (for view mode)
  const [transferData, setTransferData] = useState(null);
  const [error, setError] = useState({ isError: false, message: '' });

  const { data: productLocationsResponse } = useQuery({
    queryKey: [...queryKeys.inventory.stockTransfers('product-locations'), selectedProduct?.id],
    queryFn: () => getInventoriesByProductId(selectedProduct?.id),
    enabled: isOpen && Boolean(selectedProduct?.id),
  });

  const locations =
    productLocationsResponse?.success && Array.isArray(productLocationsResponse?.data)
      ? productLocationsResponse.data.map(e => ({
          id: e.locationId,
          name: e.locationName,
        }))
      : [];

  const createTransferMutation = useMutation({
    mutationFn: createStockTransfer,
    onSuccess: async result => {
      if (result.success) {
        showSuccess(
          t(
            i18nKeyContainer.inventory.stockTransfers.form.toasts
              .transferCreateSuccessTitle
          ),
          t(
            i18nKeyContainer.inventory.stockTransfers.form.toasts
              .transferCreateSuccessMessage,
            {
              from: locations.find(l => l.id === parseInt(fromLocationId))?.name,
              to: locations.find(l => l.id === parseInt(toLocationId))?.name,
            }
          )
        );

        if (onSuccess) {
          onSuccess(result.data);
        }

        await queryClient.invalidateQueries({ queryKey: queryKeys.inventory.all });
        await queryClient.invalidateQueries({ queryKey: queryKeys.products.all });
        handleClose();
        return;
      }

      showError(
        t(
          i18nKeyContainer.inventory.stockTransfers.form.toasts
            .transferCreateFailedTitle
        ),
        result.error ||
          t(
            i18nKeyContainer.inventory.stockTransfers.form.toasts
              .transferCreateFailedMessage
          )
      );
    },
    onError: mutationError => {
      showError(
        t(
          i18nKeyContainer.inventory.stockTransfers.form.toasts
            .transferCreateFailedTitle
        ),
        mutationError.message ||
          t(
            i18nKeyContainer.inventory.stockTransfers.form.toasts
              .transferCreateFailedMessage
          )
      );
    },
  });

  // Tabs configuration
  const tabs = [
    {
      id: 0,
      label: t(i18nKeyContainer.inventory.stockTransfers.form.tabs.product),
      icon: Package,
    },
    {
      id: 1,
      label: t(i18nKeyContainer.inventory.stockTransfers.form.tabs.transferDetails),
      icon: MapPin,
    },
    {
      id: 2,
      label: t(i18nKeyContainer.inventory.stockTransfers.form.tabs.history),
      icon: HistoryIcon,
    },
  ];

  /**
   * Validate form before submission
   */
  const isFormValid = () => {
    if (!selectedProduct) {
      showError(
        t(i18nKeyContainer.inventory.stockTransfers.form.toasts.searchErrorTitle),
        t(i18nKeyContainer.inventory.stockTransfers.form.validation.selectProduct)
      );
      setActiveTab(0);
      return false;
    }
    if (!fromLocationId) {
      showError(
        t(i18nKeyContainer.inventory.stockTransfers.form.toasts.searchErrorTitle),
        t(i18nKeyContainer.inventory.stockTransfers.form.validation.selectFromLocation)
      );
      setActiveTab(1);
      return false;
    }
    if (!toLocationId) {
      showError(
        t(i18nKeyContainer.inventory.stockTransfers.form.toasts.searchErrorTitle),
        t(i18nKeyContainer.inventory.stockTransfers.form.validation.selectToLocation)
      );
      setActiveTab(1);
      return false;
    }
    if (fromLocationId === toLocationId) {
      showError(
        t(i18nKeyContainer.inventory.stockTransfers.form.toasts.searchErrorTitle),
        t(
          i18nKeyContainer.inventory.stockTransfers.form.validation
            .locationsMustDiffer
        )
      );
      setActiveTab(1);
      return false;
    }
    if (!quantity || quantity <= 0) {
      showError(
        t(i18nKeyContainer.inventory.stockTransfers.form.toasts.searchErrorTitle),
        t(i18nKeyContainer.inventory.stockTransfers.form.validation.invalidQuantity)
      );
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

      await createTransferMutation.mutateAsync(transferRequest);
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

  useEffect(() => {
    if (!selectedProduct) {
      setError({ isError: false, message: '' });
      return;
    }

    if (productLocationsResponse?.success) {
      if (productLocationsResponse.data.length < 2) {
        setError({
          isError: true,
          message: t(
            i18nKeyContainer.inventory.stockTransfers.form.toasts
              .productSingleLocationError
          ),
        });
        return;
      }

      setError({ isError: false, message: '' });
      return;
    }

    if (productLocationsResponse && !productLocationsResponse.success) {
      setError({
        isError: true,
        message: t(
          i18nKeyContainer.inventory.stockTransfers.form.toasts
            .productNoInventoryError
        ),
      });
    }
  }, [selectedProduct, productLocationsResponse, t]);

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
              {mode === 'add'
                ? t(i18nKeyContainer.inventory.stockTransfers.form.title.create)
                : t(i18nKeyContainer.inventory.stockTransfers.form.title.view)}
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
            {t(i18nKeyContainer.inventory.stockTransfers.form.actions.cancel)}
          </Button>
          {mode === 'add' && (
            <Button
              onClick={handleSubmit}
              disabled={isLoading}
              loading={isLoading}
              className='min-w-[150px]'
            >
              {t(i18nKeyContainer.inventory.stockTransfers.form.actions.createTransfer)}
            </Button>
          )}
        </div>
      </div>
      {error.isError && (
        <ConfirmationDialog
          title={t(i18nKeyContainer.inventory.stockTransfers.form.toasts.searchErrorTitle)}
          message={error.message}
          confirmText={t(i18nKeyContainer.inventory.stockTransfers.form.actions.ok)}
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
