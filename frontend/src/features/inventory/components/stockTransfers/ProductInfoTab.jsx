import React, { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Package, Search } from 'lucide-react';
import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import { getProductById } from '@features/products/services/productApi';
import { useToast } from '@shared/context/ToastContext';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { formatDzdCurrency } from '@shared/utils/currencyFormatter';

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
  const { t, i18n } = useTranslation();
  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';
  const { showSuccess, showError } = useToast();
  const [productSearchTerm, setProductSearchTerm] = useState('');
  const [isSearching, setIsSearching] = useState(false);
  const productSearchMutation = useMutation({
    mutationFn: getProductById,
  });

  /**
   * Handle product search
   * Searches for a product by ID
   */
  const handleProductSearch = async () => {
    if (!productSearchTerm.trim()) {
      showError(
        t(
          i18nKeyContainer.inventory.stockTransfers.form.toasts.searchErrorTitle
        ),
        t(
          i18nKeyContainer.inventory.stockTransfers.form.toasts
            .searchErrorMessage
        )
      );
      return;
    }

    setIsSearching(true);
    try {
      const searchId = parseInt(productSearchTerm);
      if (isNaN(searchId)) {
        showError(
          t(
            i18nKeyContainer.inventory.stockTransfers.form.toasts
              .invalidInputTitle
          ),
          t(
            i18nKeyContainer.inventory.stockTransfers.form.toasts
              .invalidInputMessage
          )
        );
        setIsSearching(false);
        return;
      }

      const product = await productSearchMutation.mutateAsync(searchId);
      if (product) {
        onProductSelect(product);
        showSuccess(
          t(
            i18nKeyContainer.inventory.stockTransfers.form.toasts
              .productFoundTitle
          ),
          t(
            i18nKeyContainer.inventory.stockTransfers.form.toasts
              .productFoundMessage,
            {
              name: product.name,
            }
          )
        );
        setProductSearchTerm(''); // Clear search after successful selection
      } else {
        showError(
          t(
            i18nKeyContainer.inventory.stockTransfers.form.toasts
              .productNotFoundTitle
          ),
          t(
            i18nKeyContainer.inventory.stockTransfers.form.toasts
              .productNotFoundMessage
          )
        );
      }
    } catch (error) {
      showError(
        t(
          i18nKeyContainer.inventory.stockTransfers.form.toasts
            .searchFailedTitle
        ),
        error.message ||
          t(
            i18nKeyContainer.inventory.stockTransfers.form.toasts
              .searchFailedMessage
          )
      );
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
        <h3 className='text-lg font-semibold'>
          {t(
            i18nKeyContainer.inventory.stockTransfers.form.sections
              .productInformation
          )}
        </h3>
      </div>

      {/* Search Section */}
      {showSearch && (
        <div className='mb-6'>
          <label className='block text-sm font-medium mb-2'>
            {t(
              i18nKeyContainer.inventory.stockTransfers.form.fields
                .searchProduct
            )}{' '}
            <span className='text-red-500'>
              {t(i18nKeyContainer.inventory.shared.required)}
            </span>
          </label>
          <div className='flex gap-2'>
            <div className='flex-1'>
              <Input
                placeholder={t(
                  i18nKeyContainer.inventory.stockTransfers.form.placeholders
                    .searchProduct
                )}
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
              {t(i18nKeyContainer.inventory.shared.search)}
            </Button>
          </div>
          <p className='text-sm text-gray-500 mt-2'>
            {t(
              i18nKeyContainer.inventory.stockTransfers.form.placeholders
                .searchPrompt
            )}
          </p>
        </div>
      )}

      {/* Product Details Display */}
      {selectedProduct && (
        <div className='bg-blue-50 border border-blue-200 rounded-lg p-6'>
          <h4 className='font-semibold text-blue-900 mb-4'>
            {t(
              i18nKeyContainer.inventory.stockTransfers.form.sections
                .selectedProduct
            )}
          </h4>
          <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
            <div>
              <p className='text-sm text-gray-600 mb-1'>
                {t(
                  i18nKeyContainer.inventory.stockTransfers.form.fields
                    .productName
                )}
              </p>
              <p className='font-medium text-gray-900'>
                {selectedProduct.name}
              </p>
            </div>
            <div>
              <p className='text-sm text-gray-600 mb-1'>
                {t(i18nKeyContainer.inventory.stockTransfers.form.fields.sku)}
              </p>
              <p className='font-medium text-gray-900'>{selectedProduct.sku}</p>
            </div>
            <div>
              <p className='text-sm text-gray-600 mb-1'>
                {t(
                  i18nKeyContainer.inventory.stockTransfers.form.fields.category
                )}
              </p>
              <p className='font-medium text-gray-900'>
                {selectedProduct.categoryName ||
                  t(i18nKeyContainer.inventory.shared.notAvailable)}
              </p>
            </div>
            <div>
              <p className='text-sm text-gray-600 mb-1'>
                {t(
                  i18nKeyContainer.inventory.stockTransfers.form.fields
                    .productId
                )}
              </p>
              <p className='font-medium text-gray-900'>#{selectedProduct.id}</p>
            </div>

            <div>
              <p className='text-sm text-gray-600 mb-1'>
                {t(
                  i18nKeyContainer.inventory.stockTransfers.form.fields
                    .unitPrice
                )}
              </p>
              <p className='font-medium text-gray-900'>
                {formatDzdCurrency(selectedProduct.unitPrice || 0, {
                  locale: activeLocale,
                })}
              </p>
            </div>
            <div>
              <p className='text-sm text-gray-600 mb-1'>
                {t(
                  i18nKeyContainer.inventory.stockTransfers.form.fields.status
                )}
              </p>
              <span
                className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                  selectedProduct.isActive
                    ? 'bg-green-100 text-green-800'
                    : 'bg-red-100 text-red-800'
                }`}
              >
                {selectedProduct.isActive
                  ? t(i18nKeyContainer.inventory.shared.status.active)
                  : t(i18nKeyContainer.inventory.shared.status.inactive)}
              </span>
            </div>
          </div>
        </div>
      )}

      {/* Empty State */}
      {!selectedProduct && (
        <div className='text-center py-8 text-gray-500'>
          <Package className='h-12 w-12 mx-auto mb-3 text-gray-400' />
          <p>
            {t(
              i18nKeyContainer.inventory.stockTransfers.form.placeholders
                .searchToStart
            )}
          </p>
        </div>
      )}
    </div>
  );
};

export default ProductInfoTab;
