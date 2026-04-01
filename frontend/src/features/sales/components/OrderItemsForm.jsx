import { useState, useEffect, useCallback } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import { Plus, Trash2, Package } from 'lucide-react';
import { cn } from '@shared/lib/utils';
import Button from '@components/Buttons/Button';
import { getAllProducts, getInventoriesByProductId } from '@features/products/services/productApi';
import { queryKeys } from '@shared/lib/queryKeys';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

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
  const [selectedProductForRow, setSelectedProductForRow] = useState({});

  // Fetch products list
  const { data: productsData, isLoading: productsLoading } = useQuery({
    queryKey: queryKeys.products.table({ page: 1, pageSize: 100 }),
    queryFn: () => getAllProducts({ page: 1, pageSize: 100 }),
  });

  const products = productsData?.items || [];

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
    // Clean up selected product state
    setSelectedProductForRow(prev => {
      const next = { ...prev };
      delete next[index];
      return next;
    });
  };

  const updateItem = (index, field, fieldValue) => {
    const newItems = [...value];
    newItems[index] = { ...newItems[index], [field]: fieldValue };
    onChange(newItems);
  };

  const handleProductChange = (index, productId) => {
    const product = products.find(p => p.id === parseInt(productId));
    updateItem(index, 'productId', productId);
    updateItem(index, 'locationId', '');
    if (product) {
      updateItem(index, 'unitPrice', product.unitPrice || 0);
    }
    setSelectedProductForRow(prev => ({ ...prev, [index]: productId }));
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
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h3 className="text-lg font-medium">
          {t(i18nKeyContainer.sales.orders.items.title)}
        </h3>
        {!disabled && (
          <Button
            type="button"
            variant="secondary"
            size="sm"
            LeftIcon={Plus}
            onClick={addItem}
          >
            {t(i18nKeyContainer.sales.orders.items.addItem)}
          </Button>
        )}
      </div>

      {value.length === 0 ? (
        <div className="text-center py-8 bg-gray-50 rounded-lg border border-dashed border-gray-300">
          <Package className="mx-auto h-12 w-12 text-gray-400" />
          <h4 className="mt-2 text-sm font-medium text-gray-900">
            {t(i18nKeyContainer.sales.orders.items.noItems)}
          </h4>
          <p className="mt-1 text-sm text-gray-500">
            {t(i18nKeyContainer.sales.orders.items.noItemsDescription)}
          </p>
        </div>
      ) : (
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  {t(i18nKeyContainer.sales.orders.items.product)}
                </th>
                <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  {t(i18nKeyContainer.sales.orders.items.location)}
                </th>
                <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider w-24">
                  {t(i18nKeyContainer.sales.orders.items.quantity)}
                </th>
                <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider w-32">
                  {t(i18nKeyContainer.sales.orders.items.unitPrice)}
                </th>
                <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider w-32">
                  {t(i18nKeyContainer.sales.orders.items.subtotal)}
                </th>
                {!disabled && (
                  <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider w-16">
                    <span className="sr-only">{t(i18nKeyContainer.sales.orders.items.remove)}</span>
                  </th>
                )}
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {value.map((item, index) => (
                <OrderItemRow
                  key={item._tempId || item.id || index}
                  item={item}
                  index={index}
                  products={products}
                  productsLoading={productsLoading}
                  disabled={disabled}
                  onProductChange={handleProductChange}
                  onLocationChange={(idx, locId) => updateItem(idx, 'locationId', locId)}
                  onQuantityChange={(idx, qty) => updateItem(idx, 'quantity', qty)}
                  onUnitPriceChange={(idx, price) => updateItem(idx, 'unitPrice', price)}
                  onRemove={removeItem}
                  calculateSubtotal={calculateSubtotal}
                  isDuplicate={isDuplicate(index, item.productId, item.locationId)}
                  t={t}
                />
              ))}
            </tbody>
            <tfoot className="bg-gray-50">
              <tr>
                <td colSpan={disabled ? 4 : 5} className="px-4 py-3 text-right font-medium">
                  {t(i18nKeyContainer.sales.orders.create.total)}:
                </td>
                <td className="px-4 py-3 font-bold text-lg">
                  ${calculateTotal().toFixed(2)}
                </td>
              </tr>
            </tfoot>
          </table>
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
  products,
  productsLoading,
  disabled,
  onProductChange,
  onLocationChange,
  onQuantityChange,
  onUnitPriceChange,
  onRemove,
  calculateSubtotal,
  isDuplicate,
  t,
}) => {
  // Fetch inventory for selected product
  const { data: inventoryData, isLoading: inventoryLoading } = useQuery({
    queryKey: [...queryKeys.products.detail(item.productId), 'inventory'],
    queryFn: () => getInventoriesByProductId(item.productId),
    enabled: !!item.productId,
  });

  const locations =
    inventoryData?.success && Array.isArray(inventoryData?.data)
      ? inventoryData.data.map(inv => ({
          id: inv.locationId,
          name: inv.locationName,
          stock: inv.quantityOnHand,
        }))
      : [];

  const selectedLocation = locations.find(
    loc => loc.id === parseInt(item.locationId)
  );
  const availableStock = selectedLocation?.stock || 0;

  // Cap quantity at available stock
  useEffect(() => {
    if (item.locationId && item.quantity > availableStock && availableStock > 0) {
      onQuantityChange(index, availableStock);
    }
  }, [item.locationId, availableStock]);

  return (
    <tr className={cn(isDuplicate && 'bg-yellow-50')}>
      {/* Product Select */}
      <td className="px-4 py-3">
        <select
          value={item.productId}
          onChange={e => onProductChange(index, e.target.value)}
          disabled={disabled || productsLoading}
          className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm disabled:bg-gray-100"
        >
          <option value="">
            {t(i18nKeyContainer.sales.orders.items.selectProduct)}
          </option>
          {products.map(product => (
            <option key={product.id} value={product.id}>
              {product.name} ({product.sku})
            </option>
          ))}
        </select>
      </td>

      {/* Location Select */}
      <td className="px-4 py-3">
        <div className="space-y-1">
          <select
            value={item.locationId}
            onChange={e => onLocationChange(index, e.target.value)}
            disabled={disabled || !item.productId || inventoryLoading}
            className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm disabled:bg-gray-100"
          >
            <option value="">
              {!item.productId
                ? t(i18nKeyContainer.sales.orders.items.selectProductFirst)
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
          {item.locationId && (
            <p className="text-xs text-gray-500">
              {availableStock > 0
                ? t(i18nKeyContainer.sales.orders.items.availableStock, {
                    count: availableStock,
                  })
                : t(i18nKeyContainer.sales.orders.items.noStock)}
            </p>
          )}
          {isDuplicate && (
            <p className="text-xs text-yellow-600">
              {t(i18nKeyContainer.sales.orders.items.duplicateWarning)}
            </p>
          )}
        </div>
      </td>

      {/* Quantity Input */}
      <td className="px-4 py-3">
        <input
          type="number"
          min="1"
          max={availableStock || undefined}
          value={item.quantity}
          onChange={e => onQuantityChange(index, parseInt(e.target.value) || 1)}
          disabled={disabled || !item.locationId}
          className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm disabled:bg-gray-100"
        />
      </td>

      {/* Unit Price Input */}
      <td className="px-4 py-3">
        <div className="relative">
          <span className="absolute inset-y-0 left-0 flex items-center pl-3 text-gray-500">
            $
          </span>
          <input
            type="number"
            min="0"
            step="0.01"
            value={item.unitPrice}
            onChange={e => onUnitPriceChange(index, parseFloat(e.target.value) || 0)}
            disabled={disabled}
            className="block w-full pl-7 rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm disabled:bg-gray-100"
          />
        </div>
      </td>

      {/* Subtotal */}
      <td className="px-4 py-3 font-medium">
        ${calculateSubtotal(item).toFixed(2)}
      </td>

      {/* Remove Button */}
      {!disabled && (
        <td className="px-4 py-3">
          <button
            type="button"
            onClick={() => onRemove(index)}
            className="text-red-500 hover:text-red-700 transition-colors p-1 rounded hover:bg-red-50"
            title={t(i18nKeyContainer.sales.orders.items.remove)}
          >
            <Trash2 className="h-4 w-4" />
          </button>
        </td>
      )}
    </tr>
  );
};

export default OrderItemsForm;
