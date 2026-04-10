import { useEffect, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import { Plus, Trash2, Package, Search } from 'lucide-react';
import { cn } from '@shared/lib/utils';
import Button from '@components/Buttons/Button';
import {
  getAllProducts,
  getInventoriesByProductId,
  getProductById,
} from '@features/products/services/productApi';
import { queryKeys } from '@shared/lib/queryKeys';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

const PRODUCT_SEARCH_PAGE_SIZE = 20;
const PRODUCT_SEARCH_DEBOUNCE_MS = 400;

function unwrapPayload(response) {
  if (response?.data !== undefined) {
    return response.data;
  }
  return response;
}

function extractArray(payload) {
  if (Array.isArray(payload)) {
    return payload;
  }
  if (Array.isArray(payload?.data)) {
    return payload.data;
  }
  if (Array.isArray(payload?.item)) {
    return payload.item;
  }
  if (Array.isArray(payload?.items)) {
    return payload.items;
  }
  return [];
}

function getProductName(product) {
  return product?.product || product?.name || `#${product?.id ?? 'N/A'}`;
}

function getProductPrice(product) {
  return Number(product?.price ?? product?.unitPrice ?? 0);
}

function ProductDisplayCard({ product, t }) {
  if (!product) {
    return null;
  }

  return (
    <div className='rounded-lg border border-blue-200 bg-blue-50/60 px-3 py-2'>
      <p className='text-sm font-semibold text-gray-900'>
        {getProductName(product)}
      </p>
      <p className='text-xs text-gray-600'>SKU: {product?.sku || 'N/A'}</p>
      <p className='text-xs text-gray-500'>ID: {product?.id ?? '-'}</p>
      <p className='text-xs text-gray-500'>
        {t(i18nKeyContainer.sales.orders.items.unitPrice)}: $
        {getProductPrice(product).toFixed(2)}
      </p>
    </div>
  );
}

function extractProduct(response) {
  const payload = unwrapPayload(response);
  if (payload?.id) {
    return payload;
  }
  if (payload?.data?.id) {
    return payload.data;
  }
  return null;
}

async function searchProductsBySku(searchTerm) {
  const listResponse = await getAllProducts({
    page: 1,
    pageSize: PRODUCT_SEARCH_PAGE_SIZE,
    search: searchTerm,
  });

  return extractArray(unwrapPayload(listResponse));
}

async function searchProductById(searchTerm) {
  const id = Number(searchTerm);
  if (!Number.isInteger(id) || id <= 0) {
    return null;
  }

  try {
    const productByIdResponse = await getProductById(id);
    return extractProduct(productByIdResponse);
  } catch {
    return null;
  }
}

function SearchModeToggle({ mode, onChange, disabled, t }) {
  return (
    <div className='inline-flex rounded-lg border border-gray-300 p-1 bg-gray-50'>
      <button
        type='button'
        onClick={() => onChange('id')}
        disabled={disabled}
        className={cn(
          'px-3 py-1.5 text-xs font-medium rounded-md transition-colors',
          mode === 'id'
            ? 'bg-white text-blue-700 shadow-sm'
            : 'text-gray-600 hover:text-gray-800'
        )}
      >
        {t(i18nKeyContainer.sales.orders.items.searchById)}
      </button>
      <button
        type='button'
        onClick={() => onChange('sku')}
        disabled={disabled}
        className={cn(
          'px-3 py-1.5 text-xs font-medium rounded-md transition-colors',
          mode === 'sku'
            ? 'bg-white text-blue-700 shadow-sm'
            : 'text-gray-600 hover:text-gray-800'
        )}
      >
        {t(i18nKeyContainer.sales.orders.items.searchBySku)}
      </button>
    </div>
  );
}

function ProductSearchBlock({
  mode,
  searchInput,
  setSearchInput,
  onSearch,
  onSearchKeyDown,
  disabled,
  loading,
  searchTerm,
  selectedProduct,
  productResults,
  onProductPick,
  errorMessage,
  t,
}) {
  const placeholder =
    mode === 'id'
      ? t(i18nKeyContainer.sales.orders.items.searchPlaceholderId)
      : t(i18nKeyContainer.sales.orders.items.searchPlaceholderSku);

  return (
    <div className='space-y-3'>
      <div className='flex flex-col gap-2 sm:flex-row sm:items-center sm:gap-3'>
        <div className='w-full sm:max-w-56'>
          <SearchModeToggle
            mode={mode}
            onChange={value => {
              setSearchInput('');
              onSearch(value, true);
            }}
            disabled={disabled}
            t={t}
          />
        </div>
        <div className='flex w-full gap-2'>
          <input
            type='text'
            value={searchInput}
            onChange={event => setSearchInput(event.target.value)}
            onKeyDown={onSearchKeyDown}
            disabled={disabled}
            placeholder={placeholder}
            className='h-10 block w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-sm disabled:bg-gray-100'
          />
          <button
            type='button'
            onClick={() => onSearch(mode, false)}
            disabled={disabled || !searchInput.trim() || loading}
            className='inline-flex h-10 items-center gap-1 px-3 text-sm rounded-lg border border-gray-300 bg-white hover:bg-gray-50 disabled:bg-gray-100 disabled:text-gray-400'
          >
            <Search className='h-4 w-4' />
            {t(i18nKeyContainer.sales.orders.items.search)}
          </button>
        </div>
      </div>

      {mode === 'sku' && (
        <div className='rounded-lg border border-gray-200 bg-white overflow-hidden'>
          {!searchTerm && (
            <p className='px-3 py-2 text-sm text-gray-500'>
              {t(i18nKeyContainer.sales.orders.items.searchToLoadProducts)}
            </p>
          )}

          {searchTerm && loading && (
            <p className='px-3 py-2 text-sm text-gray-500'>
              {t(i18nKeyContainer.sales.orders.items.searchingProducts)}
            </p>
          )}

          {searchTerm && !loading && errorMessage && (
            <p className='px-3 py-2 text-sm text-red-600'>
              {t(i18nKeyContainer.sales.orders.items.searchFailed)}
            </p>
          )}

          {searchTerm &&
            !loading &&
            !errorMessage &&
            productResults.length === 0 && (
              <p className='px-3 py-2 text-sm text-gray-500'>
                {t(i18nKeyContainer.sales.orders.items.noMatchingProducts)}
              </p>
            )}

          {searchTerm &&
            !loading &&
            !errorMessage &&
            productResults.length > 0 && (
              <div className='max-h-56 overflow-y-auto divide-y divide-gray-100'>
                {productResults.map(product => {
                  const isSelected =
                    Number(selectedProduct?.id) === Number(product.id);

                  return (
                    <button
                      key={product.id}
                      type='button'
                      onClick={() => onProductPick(product)}
                      disabled={disabled}
                      className={cn(
                        'w-full text-left px-3 py-2 transition-colors',
                        isSelected
                          ? 'bg-blue-50 hover:bg-blue-100'
                          : 'hover:bg-gray-50'
                      )}
                    >
                      <p className='text-sm font-semibold text-gray-900'>
                        {getProductName(product)}
                      </p>
                      <p className='text-xs text-gray-600'>
                        SKU: {product?.sku || 'N/A'}
                      </p>
                    </button>
                  );
                })}
              </div>
            )}
        </div>
      )}

      <ProductDisplayCard product={selectedProduct} t={t} />
      {errorMessage && <p className='text-sm text-red-600'>{errorMessage}</p>}
    </div>
  );
}

/**
 * OrderItemsForm Component
 *
 * A controlled dynamic items editor for order line items.
 * Handles product selection, location filtering by stock, and quantity validation.
 *
 * @param {Object} props - Component props
 * @param {Array} props.value - Array of order items [{productId, locationId, quantity, unitPrice}]
 * @param {function} props.onChange - Callback when items change
 * @param {boolean} props.disabled - Disable all inputs
 */
const OrderItemsForm = ({ value = [], onChange, disabled = false }) => {
  const { t } = useTranslation();

  const addItem = () => {
    onChange([
      ...value,
      {
        productId: '',
        locationId: '',
        quantity: 1,
        unitPrice: 0,
        _tempId: Date.now(),
      },
    ]);
  };

  const removeItem = index => {
    const newItems = value.filter((_, i) => i !== index);
    onChange(newItems);
  };

  const updateItem = (index, field, fieldValue) => {
    const newItems = [...value];
    newItems[index] = { ...newItems[index], [field]: fieldValue };
    onChange(newItems);
  };

  const updateItemFields = (index, fields) => {
    const newItems = [...value];
    newItems[index] = { ...newItems[index], ...fields };
    onChange(newItems);
  };

  const handleProductSelect = (index, product) => {
    updateItemFields(index, {
      productId: String(product.id),
      productName: getProductName(product),
      locationId: '',
      locationName: '',
      unitPrice: getProductPrice(product),
    });
  };

  const calculateSubtotal = item => {
    return (parseFloat(item.quantity) || 0) * (parseFloat(item.unitPrice) || 0);
  };

  const calculateTotal = () => {
    return value.reduce((sum, item) => sum + calculateSubtotal(item), 0);
  };

  const isDuplicate = (index, productId, locationId) => {
    if (!productId || !locationId) return false;
    return value.some(
      (item, i) =>
        i !== index &&
        item.productId === productId &&
        item.locationId === locationId
    );
  };

  return (
    <div className='space-y-4 w-full'>
      <div className='flex items-center justify-between'>
        <h3 className='text-lg font-medium'>
          {t(i18nKeyContainer.sales.orders.items.title)}
        </h3>
        {!disabled && (
          <Button
            type='button'
            variant='secondary'
            size='sm'
            LeftIcon={Plus}
            onClick={addItem}
          >
            {t(i18nKeyContainer.sales.orders.items.addItem)}
          </Button>
        )}
      </div>

      {value.length === 0 ? (
        <div className='text-center py-8 bg-gray-50 rounded-lg border border-dashed border-gray-300'>
          <Package className='mx-auto h-12 w-12 text-gray-400' />
          <h4 className='mt-2 text-sm font-medium text-gray-900'>
            {t(i18nKeyContainer.sales.orders.items.noItems)}
          </h4>
          <p className='mt-1 text-sm text-gray-500'>
            {t(i18nKeyContainer.sales.orders.items.noItemsDescription)}
          </p>
        </div>
      ) : (
        <div className='rounded-xl border border-gray-200 overflow-hidden bg-white shadow-sm'>
          <div className='overflow-x-auto'>
            <table className='w-full min-w-[1120px] divide-y divide-gray-200'>
              <thead className='hidden md:table-header-group bg-gray-50'>
                <tr>
                  <th className='px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                    {t(i18nKeyContainer.sales.orders.items.product)}
                  </th>
                  <th className='px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'>
                    {t(i18nKeyContainer.sales.orders.items.location)}
                  </th>
                  <th className='px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider w-24'>
                    {t(i18nKeyContainer.sales.orders.items.quantity)}
                  </th>
                  <th className='px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider w-32'>
                    {t(i18nKeyContainer.sales.orders.items.unitPrice)}
                  </th>
                  <th className='px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider w-32'>
                    {t(i18nKeyContainer.sales.orders.items.subtotal)}
                  </th>
                  {!disabled && (
                    <th className='px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider w-16'>
                      <span className='sr-only'>
                        {t(i18nKeyContainer.sales.orders.items.remove)}
                      </span>
                    </th>
                  )}
                </tr>
              </thead>
              <tbody className='bg-white divide-y divide-gray-200'>
                {value.map((item, index) => (
                  <OrderItemRow
                    key={item._tempId || item.id || index}
                    item={item}
                    index={index}
                    disabled={disabled}
                    onProductSelect={handleProductSelect}
                    onLocationChange={(idx, locId) =>
                      updateItem(idx, 'locationId', locId)
                    }
                    onQuantityChange={(idx, qty) =>
                      updateItem(idx, 'quantity', qty)
                    }
                    onUnitPriceChange={(idx, price) =>
                      updateItem(idx, 'unitPrice', price)
                    }
                    onRemove={removeItem}
                    calculateSubtotal={calculateSubtotal}
                    isDuplicate={isDuplicate(
                      index,
                      item.productId,
                      item.locationId
                    )}
                    t={t}
                  />
                ))}
              </tbody>
            </table>
          </div>
          <div className='px-4 py-3 bg-gray-50 border-t border-gray-200 flex items-center justify-between'>
            <span className='text-sm font-medium text-gray-700'>
              {t(i18nKeyContainer.sales.orders.create.total)}:
            </span>
            <span className='text-lg font-bold text-gray-900'>
              ${calculateTotal().toFixed(2)}
            </span>
          </div>
        </div>
      )}
    </div>
  );
};

/**
 * Individual order item row component
 */
const OrderItemRow = ({
  item,
  index,
  disabled,
  onProductSelect,
  onLocationChange,
  onQuantityChange,
  onUnitPriceChange,
  onRemove,
  calculateSubtotal,
  isDuplicate,
  t,
}) => {
  const [searchMode, setSearchMode] = useState('sku');
  const [searchInput, setSearchInput] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedProductData, setSelectedProductData] = useState(null);
  const [hasPrefilledSearch, setHasPrefilledSearch] = useState(false);
  const [idSearchLoading, setIdSearchLoading] = useState(false);
  const [idSearchError, setIdSearchError] = useState('');

  const itemProductName = item.productName || item.product?.name || '';
  const itemProductSku = item.sku || item.product?.sku || '';

  useEffect(() => {
    if (!item.productId || hasPrefilledSearch) {
      return;
    }

    setSelectedProductData({
      id: Number(item.productId),
      product: itemProductName || undefined,
      name: itemProductName || undefined,
      sku: itemProductSku || undefined,
      price: Number(item.unitPrice) || 0,
      unitPrice: Number(item.unitPrice) || 0,
    });

    if (itemProductName) {
      setSearchMode('sku');
      setSearchInput(itemProductName);
    } else {
      setSearchMode('id');
      setSearchInput(String(item.productId));
    }

    setHasPrefilledSearch(true);
  }, [
    hasPrefilledSearch,
    item.productId,
    item.unitPrice,
    itemProductName,
    itemProductSku,
  ]);

  useEffect(() => {
    if (searchMode !== 'sku') {
      setSearchTerm('');
      return undefined;
    }

    const trimmed = searchInput.trim();
    if (!trimmed) {
      setSearchTerm('');
      return undefined;
    }

    const timeout = setTimeout(() => {
      setSearchTerm(trimmed);
    }, PRODUCT_SEARCH_DEBOUNCE_MS);

    return () => clearTimeout(timeout);
  }, [searchInput, searchMode]);

  const {
    data: searchResults,
    isFetching: skuSearchLoading,
    isError: productSearchError,
    error: productSearchErrorDetails,
    refetch: refetchProductSearch,
  } = useQuery({
    queryKey: [
      ...queryKeys.products.all,
      'order-search',
      { search: searchTerm, pageSize: PRODUCT_SEARCH_PAGE_SIZE },
    ],
    queryFn: () => searchProductsBySku(searchTerm),
    enabled: searchMode === 'sku' && !!searchTerm,
    staleTime: 30 * 1000,
  });

  const productResults = Array.isArray(searchResults) ? searchResults : [];

  const skuSearchErrorMessage = productSearchError
    ? productSearchErrorDetails?.message || 'Failed to search products.'
    : null;

  const handleSkuSearch = () => {
    const trimmed = searchInput.trim();
    if (!trimmed) {
      return;
    }

    if (trimmed === searchTerm) {
      refetchProductSearch();
      return;
    }

    setSearchTerm(trimmed);
  };

  const handleIdSearch = async () => {
    const trimmed = searchInput.trim();
    if (!trimmed) {
      setIdSearchError('');
      return;
    }

    if (!/^\d+$/.test(trimmed)) {
      setIdSearchError(t(i18nKeyContainer.sales.orders.items.invalidProductId));
      return;
    }

    setIdSearchLoading(true);
    setIdSearchError('');

    try {
      const product = await searchProductById(trimmed);
      if (!product) {
        setIdSearchError(
          t(i18nKeyContainer.sales.orders.items.productNotFound)
        );
        return;
      }

      setSelectedProductData(product);
      onProductSelect(index, product);
    } finally {
      setIdSearchLoading(false);
    }
  };

  const handleSearch = (mode, modeChanged) => {
    if (modeChanged) {
      setSearchMode(mode);
      setSearchTerm('');
      setIdSearchError('');
      return;
    }

    if (mode === 'id') {
      handleIdSearch();
      return;
    }

    handleSkuSearch();
  };

  const handleSearchKeyDown = event => {
    if (event.key === 'Enter') {
      event.preventDefault();
      if (searchMode === 'id') {
        handleIdSearch();
      } else {
        handleSkuSearch();
      }
    }
  };

  const handleProductPick = product => {
    if (!product) {
      return;
    }

    setSelectedProductData(product);
    onProductSelect(index, product);
  };

  const selectedProductFromResults = productResults.find(
    product => Number(product.id) === Number(item.productId)
  );

  const selectedProductFromItem = item.productId
    ? {
        id: Number(item.productId),
        product: itemProductName || undefined,
        name: itemProductName || undefined,
        sku: itemProductSku || undefined,
        price: Number(item.unitPrice) || 0,
        unitPrice: Number(item.unitPrice) || 0,
      }
    : null;

  const selectedProduct =
    selectedProductFromResults ||
    selectedProductData ||
    selectedProductFromItem;

  const productSearchLoading =
    searchMode === 'id' ? idSearchLoading : skuSearchLoading;
  const productSearchErrorMessage =
    searchMode === 'id' ? idSearchError : skuSearchErrorMessage;

  // Fetch inventory for selected product
  const {
    data: inventoryData,
    isLoading: inventoryLoading,
    isFetching: inventoryFetching,
    isError: inventoryQueryError,
    error: inventoryQueryErrorDetails,
    refetch: refetchInventory,
  } = useQuery({
    queryKey: [...queryKeys.products.detail(item.productId), 'inventory'],
    queryFn: () => getInventoriesByProductId(item.productId),
    enabled: !!item.productId,
    retry: 1,
  });

  const inventoryErrorMessage = inventoryQueryError
    ? inventoryQueryErrorDetails?.message || 'Failed to load locations.'
    : inventoryData?.success === false
      ? inventoryData?.error || 'Failed to load locations.'
      : null;

  const inventoryPayload = unwrapPayload(inventoryData);
  const inventoryList = extractArray(inventoryPayload);

  const locations = !inventoryErrorMessage
    ? inventoryList.map(inv => ({
        id: inv.locationId,
        name: inv.locationName,
        stock: inv.quantityOnHand,
      }))
    : [];

  const showNoInventoryAvailable =
    !!item.productId &&
    !inventoryLoading &&
    !inventoryFetching &&
    !inventoryErrorMessage &&
    inventoryList.length === 0;

  const selectedLocation = locations.find(
    loc => loc.id === parseInt(item.locationId)
  );
  const availableStock = selectedLocation?.stock || 0;

  // Cap quantity at available stock
  useEffect(() => {
    if (
      item.locationId &&
      item.quantity > availableStock &&
      availableStock > 0
    ) {
      onQuantityChange(index, availableStock);
    }
  }, [item.locationId, item.quantity, availableStock, index, onQuantityChange]);

  const locationSelect = (
    <div className='space-y-2'>
      <select
        value={item.locationId}
        onChange={e => onLocationChange(index, e.target.value)}
        disabled={
          disabled ||
          !item.productId ||
          inventoryLoading ||
          inventoryFetching ||
          showNoInventoryAvailable
        }
        className='h-10 block w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-sm disabled:bg-gray-100'
      >
        <option value=''>
          {!item.productId
            ? t(i18nKeyContainer.sales.orders.items.selectProductFirst)
            : inventoryLoading
              ? t(i18nKeyContainer.sales.orders.items.loadingLocations)
              : inventoryErrorMessage
                ? t(i18nKeyContainer.sales.orders.items.failedToLoadLocations)
                : t(i18nKeyContainer.sales.orders.items.selectLocation)}
        </option>
        {locations
          .filter(loc => loc.stock > 0)
          .map(location => (
            <option key={location.id} value={location.id}>
              {location.name}
            </option>
          ))}
      </select>

      {showNoInventoryAvailable && (
        <p className='text-sm text-amber-700'>
          {t(i18nKeyContainer.sales.orders.items.noInventoryAvailable)}
        </p>
      )}

      {item.locationId && (
        <p className='text-sm text-gray-600'>
          {availableStock > 0
            ? t(i18nKeyContainer.sales.orders.items.availableStock, {
                count: availableStock,
              })
            : t(i18nKeyContainer.sales.orders.items.noStock)}
        </p>
      )}

      {isDuplicate && (
        <p className='text-sm text-yellow-700'>
          {t(i18nKeyContainer.sales.orders.items.duplicateWarning)}
        </p>
      )}

      {inventoryErrorMessage && (
        <div className='flex items-center gap-2'>
          <p className='text-sm text-red-600'>{inventoryErrorMessage}</p>
          <button
            type='button'
            onClick={() => refetchInventory()}
            className='text-sm text-blue-600 hover:text-blue-700'
          >
            {t(i18nKeyContainer.sales.orders.items.retry)}
          </button>
        </div>
      )}
    </div>
  );

  return (
    <tr
      className={cn(
        'block md:table-row border-b border-gray-100 md:border-b-0',
        isDuplicate && 'bg-yellow-50/40'
      )}
    >
      <td className='block md:table-cell px-4 py-4 align-top'>
        <div className='md:hidden text-xs font-medium text-gray-500 mb-2 uppercase tracking-wider'>
          {t(i18nKeyContainer.sales.orders.items.product)}
        </div>
        <ProductSearchBlock
          mode={searchMode}
          searchInput={searchInput}
          setSearchInput={setSearchInput}
          onSearch={handleSearch}
          onSearchKeyDown={handleSearchKeyDown}
          disabled={disabled}
          loading={productSearchLoading}
          searchTerm={searchTerm}
          selectedProduct={selectedProduct}
          productResults={productResults}
          onProductPick={handleProductPick}
          errorMessage={productSearchErrorMessage}
          t={t}
        />
      </td>

      <td className='block md:table-cell px-4 py-4 align-top'>
        <div className='md:hidden text-xs font-medium text-gray-500 mb-2 uppercase tracking-wider'>
          {t(i18nKeyContainer.sales.orders.items.location)}
        </div>
        {locationSelect}
      </td>

      <td className='block md:table-cell px-4 py-4 align-top md:w-32'>
        <div className='md:hidden text-xs font-medium text-gray-500 mb-2 uppercase tracking-wider'>
          {t(i18nKeyContainer.sales.orders.items.quantity)}
        </div>
        <input
          type='number'
          min='1'
          max={availableStock || undefined}
          value={item.quantity}
          onChange={e => onQuantityChange(index, parseInt(e.target.value) || 1)}
          disabled={disabled || !item.locationId}
          className='h-10 block w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-sm disabled:bg-gray-100'
        />
      </td>

      <td className='block md:table-cell px-4 py-4 align-top md:w-40'>
        <div className='md:hidden text-xs font-medium text-gray-500 mb-2 uppercase tracking-wider'>
          {t(i18nKeyContainer.sales.orders.items.unitPrice)}
        </div>
        <div className='relative'>
          <span className='absolute inset-y-0 left-0 flex items-center pl-3 text-gray-500 text-sm'>
            $
          </span>
          <input
            type='number'
            min='0'
            step='0.01'
            value={item.unitPrice}
            onChange={e =>
              onUnitPriceChange(index, parseFloat(e.target.value) || 0)
            }
            disabled={disabled}
            className='h-10 block w-full pl-7 rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 text-sm disabled:bg-gray-100'
          />
        </div>
      </td>

      <td className='block md:table-cell px-4 py-4 align-top md:w-40'>
        <div className='md:hidden text-xs font-medium text-gray-500 mb-2 uppercase tracking-wider'>
          {t(i18nKeyContainer.sales.orders.items.subtotal)}
        </div>
        <div className='text-base font-semibold text-gray-900'>
          ${calculateSubtotal(item).toFixed(2)}
        </div>
      </td>

      {!disabled && (
        <td className='block md:table-cell px-4 py-4 align-top md:w-16'>
          <div className='md:hidden text-xs font-medium text-gray-500 mb-2 uppercase tracking-wider'>
            {t(i18nKeyContainer.sales.orders.items.remove)}
          </div>
          <button
            type='button'
            onClick={() => onRemove(index)}
            className='text-red-500 hover:text-red-700 transition-colors p-2 rounded-lg hover:bg-red-50'
            title={t(i18nKeyContainer.sales.orders.items.remove)}
          >
            <Trash2 className='h-4 w-4' />
          </button>
        </td>
      )}
    </tr>
  );
};

export default OrderItemsForm;
