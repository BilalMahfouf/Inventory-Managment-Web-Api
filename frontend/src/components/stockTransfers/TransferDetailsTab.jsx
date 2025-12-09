import React from 'react';
import { MapPin, ArrowRight, Package as PackageIcon } from 'lucide-react';
import { Input } from '@components/ui/input';

/**
 * TransferDetailsTab Component
 *
 * Component for managing transfer route details including locations and quantity.
 * Displays From Location, To Location, Quantity, and Transfer Summary.
 *
 * @param {Object} props - Component props
 * @param {Array} props.locations - Array of location objects
 * @param {number} props.fromLocationId - Selected from location ID
 * @param {number} props.toLocationId - Selected to location ID
 * @param {number} props.quantity - Transfer quantity
 * @param {function} props.onFromLocationChange - Callback when from location changes
 * @param {function} props.onToLocationChange - Callback when to location changes
 * @param {function} props.onQuantityChange - Callback when quantity changes
 * @param {Object} props.selectedProduct - Selected product object for calculating total value
 * @param {boolean} props.disabled - Whether inputs are disabled
 * @param {string} props.notes - Transfer notes
 * @param {function} props.onNotesChange - Callback when notes change
 */
const TransferDetailsTab = ({
  locations = [],
  fromLocationId,
  toLocationId,
  quantity,
  onFromLocationChange,
  onToLocationChange,
  onQuantityChange,
  selectedProduct,
  disabled = false,
  notes = '',
  onNotesChange,
}) => {
  // Get location names for display
  const fromLocation = locations.find(
    loc => loc.id === parseInt(fromLocationId)
  );
  const toLocation = locations.find(loc => loc.id === parseInt(toLocationId));

  // Calculate total value
  const totalValue =
    selectedProduct && quantity > 0
      ? (selectedProduct.unitPrice || 0) * quantity
      : 0;

  // Count unique products (always 1 in single transfer, but matches the UI pattern)
  const productCount = selectedProduct ? 1 : 0;

  return (
    <div>
      <div className='flex items-center gap-2 mb-6'>
        <MapPin className='h-5 w-5' />
        <h3 className='text-lg font-semibold'>Transfer Route</h3>
      </div>

      {/* From and To Warehouses */}
      <div className='grid grid-cols-1 md:grid-cols-2 gap-6 mb-6'>
        {/* From Warehouse */}
        <div>
          <label className='block text-sm font-medium mb-2'>
            From Warehouse <span className='text-red-500'>*</span>
          </label>
          <select
            value={fromLocationId || ''}
            onChange={e => onFromLocationChange(e.target.value)}
            className='w-full h-12 px-3 border border-gray-300 rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-600'
            disabled={disabled}
          >
            <option value=''>Select warehouse</option>
            {locations.map(location => (
              <option key={location.id} value={location.id}>
                {location.name}
              </option>
            ))}
          </select>
        </div>

        {/* To Warehouse */}
        <div>
          <label className='block text-sm font-medium mb-2'>
            To Warehouse <span className='text-red-500'>*</span>
          </label>
          <select
            value={toLocationId || ''}
            onChange={e => onToLocationChange(e.target.value)}
            className='w-full h-12 px-3 border border-gray-300 rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-600'
            disabled={disabled}
          >
            <option value=''>Select warehouse</option>
            {locations.map(location => (
              <option key={location.id} value={location.id}>
                {location.name}
              </option>
            ))}
          </select>
        </div>
      </div>

      {/* From and To Locations (readonly text inputs) */}
      <div className='grid grid-cols-1 md:grid-cols-2 gap-6 mb-6'>
        <div>
          <label className='block text-sm font-medium mb-2'>
            From Location
          </label>
          <Input
            value={fromLocation?.name || ''}
            disabled
            className='h-12 bg-gray-50'
            placeholder='Select a warehouse first'
          />
        </div>

        <div>
          <label className='block text-sm font-medium mb-2'>To Location</label>
          <Input
            value={toLocation?.name || ''}
            disabled
            className='h-12 bg-gray-50'
            placeholder='Select a warehouse first'
          />
        </div>
      </div>

      {/* Visual Route Display */}
      {fromLocation && toLocation && (
        <div className='bg-blue-50 border border-blue-200 rounded-lg p-6 mb-6'>
          <div className='flex items-center justify-between'>
            <div className='flex-1 text-center'>
              <div className='text-lg font-bold text-blue-900'>
                {fromLocation.name}
              </div>
            </div>

            <div className='px-4'>
              <ArrowRight className='h-8 w-8 text-blue-600' />
            </div>

            <div className='flex-1 text-center'>
              <div className='text-lg font-bold text-blue-900'>
                {toLocation.name}
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Quantity Input */}
      <div className='mb-6'>
        <label className='block text-sm font-medium mb-2'>
          Requested Quantity <span className='text-red-500'>*</span>
        </label>
        <Input
          type='number'
          placeholder='0'
          value={quantity || ''}
          onChange={e => onQuantityChange(parseFloat(e.target.value) || 0)}
          className='h-12'
          disabled={disabled}
          min='0'
          step='1;'
        />
      </div>

      {/* Transfer Notes */}
      <div className='mb-6'>
        <label className='block text-sm font-medium mb-2'>Transfer Notes</label>
        <textarea
          placeholder='Additional notes about this transfer...'
          value={notes}
          onChange={e => onNotesChange(e.target.value)}
          className='w-full min-h-[100px] px-3 py-2 border border-gray-300 rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-600 resize-vertical'
          disabled={disabled}
        />
      </div>

      {/* Transfer Summary */}
      <div className='bg-gray-50 rounded-lg p-6 border border-gray-200'>
        <div className='flex items-center justify-between mb-4'>
          <h4 className='font-semibold text-gray-900'>Transfer Summary</h4>
          <div className='text-right'>
            <div className='text-sm text-gray-600'>Total Value</div>
            <div className='text-2xl font-bold text-green-600'>
              ${totalValue.toFixed(2)}
            </div>
          </div>
        </div>

        <div className='text-sm text-gray-600'>
          {productCount} {productCount === 1 ? 'product' : 'products'},{' '}
          {quantity || 0} total items
        </div>
      </div>
    </div>
  );
};

export default TransferDetailsTab;
