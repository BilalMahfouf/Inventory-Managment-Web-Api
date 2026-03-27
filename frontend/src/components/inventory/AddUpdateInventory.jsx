import React, { useState, useEffect } from 'react';
import { X, Package, MapPin, Archive, Search } from 'lucide-react';
import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import { cn } from '@/lib/utils';
import { getProductById } from '@/services/products/productService';
import {
  getLocationById,
  getLocationsNames,
} from '@/services/products/locationService';
import { useToast } from '@/context/ToastContext';
import {
  createInventory,
  updateInventory,
  getInventoryById,
} from '@services/inventoryService';

/**
 * AddUpdateInventory Component
 *
 * A modal component for adding or updating inventory records.
 * Features a tabbed interface with Product Search, Location Selection, and Stock Levels sections.
 *
 * @param {Object} props - Component props
 * @param {boolean} props.isOpen - Controls modal visibility
 * @param {function} props.onClose - Callback function when modal is closed
 * @param {number} props.inventoryId - ID of the inventory to edit (0 for new inventory)
 * @param {function} props.onSuccess - Optional callback after successful save
 *
 * @description
 * The component operates in two modes:
 * - Add Mode (inventoryId = 0): User can search for products and select locations
 * - Update Mode (inventoryId > 0): Product and location are readonly, only stock levels are editable
 *
 * The component has 3 tabs:
 * 1. Product Tab: Search products by name, SKU, or ID
 * 2. Location Tab: Select location from dropdown and view details
 * 3. Stock Levels Tab: Set quantity, reorder level, and max level
 */
const AddUpdateInventory = ({
  isOpen,
  onClose,
  inventoryId = 0,
  onSuccess,
}) => {
  const { showSuccess, showError } = useToast();
  const [activeTab, setActiveTab] = useState(0);
  const [mode, setMode] = useState('add'); // 'add' or 'update'
  const [isLoading, setIsLoading] = useState(false);

  // Product search state
  const [productSearchTerm, setProductSearchTerm] = useState('');
  const [searchedProduct, setSearchedProduct] = useState(null);
  const [isSearching, setIsSearching] = useState(false);

  // Location state
  const [locations, setLocations] = useState([]);
  const [selectedLocation, setSelectedLocation] = useState(null);

  // Stock levels state
  const [stockLevels, setStockLevels] = useState({
    quantityOnHand: 0,
    reorderLevel: 0,
    maxLevel: 0,
  });

  // Available stock (only in update mode)
  const [availableStock, setAvailableStock] = useState(0);

  // Tabs configuration
  const tabs = [
    { id: 0, label: 'Product', icon: Package },
    { id: 1, label: 'Location', icon: MapPin },
    { id: 2, label: 'Stock Levels', icon: Archive },
  ];

  /**
   * Check if product and location sections are filled
   * This determines if the user can proceed to save
   */
  const isReadyToSave = searchedProduct !== null && selectedLocation !== null;

  /**
   * Handle product search
   * Searches for a product by ID, name, or SKU
   */
  const handleProductSearch = async () => {
    if (!productSearchTerm.trim()) {
      showError('Search Error', 'Please enter a product name, SKU, or ID');
      return;
    }

    setIsSearching(true);
    try {
      // Try to search by ID first if it's a number
      const searchId = parseInt(productSearchTerm);
      if (!isNaN(searchId)) {
        const product = await getProductById(searchId);
        if (product) {
          setSearchedProduct(product);
          showSuccess('Product Found', `Found: ${product.name}`);
          return;
        }
      }

      // If not found by ID or not a number, show error
      // In a real scenario, you'd have a search endpoint that searches by name/SKU
      showError(
        'Product Not Found',
        'Please enter a valid product ID. Name/SKU search requires additional API endpoint.'
      );
    } catch (error) {
      showError('Search Failed', error.message || 'Failed to search product');
      setSearchedProduct(null);
    } finally {
      setIsSearching(false);
    }
  };

  /**
   * Handle location selection
   * Fetches full location details when a location is selected
   */
  const handleLocationSelect = async locationId => {
    if (!locationId) {
      setSelectedLocation(null);
      return;
    }

    try {
      // Find the location in the locations array
      const location = await getLocationById(locationId);
      if (!location.success) {
        showError('Error', `Error: ${location.error}, please try again`);
      } else {
        setSelectedLocation({
          id: location.data.id,
          name: location.data.name,
          address: location.data.address || 'N/A',
          type: location.data.typeName || location.locationTypeName || 'N/A',
        });
      }
    } catch {
      showError('Error', 'Failed to load location details');
    }
  };

  /**
   * Handle stock level input changes
   */
  const handleStockLevelChange = (field, value) => {
    const numValue = parseFloat(value) || 0;
    setStockLevels(prev => ({
      ...prev,
      [field]: numValue,
    }));
  };

  /**
   * Validate form data before submission
   */
  const validateForm = () => {
    if (!searchedProduct) {
      showError('Validation Error', 'Please select a product');
      setActiveTab(0);
      return false;
    }

    if (!selectedLocation) {
      showError('Validation Error', 'Please select a location');
      setActiveTab(1);
      return false;
    }

    if (stockLevels.reorderLevel < 0 || stockLevels.maxLevel < 0) {
      showError('Validation Error', 'Stock levels cannot be negative');
      setActiveTab(2);
      return false;
    }

    if (
      stockLevels.maxLevel > 0 &&
      stockLevels.reorderLevel > stockLevels.maxLevel
    ) {
      showError(
        'Validation Error',
        'Reorder level cannot be greater than max level'
      );
      setActiveTab(2);
      return false;
    }

    return true;
  };

  /**
   * Handle form submission
   * Creates or updates inventory based on mode
   */
  const addInventory = async () => {
    const response = await createInventory({
      productId: searchedProduct.id,
      locationId: selectedLocation.id,
      quantityOnHand: stockLevels.quantityOnHand,
      reorderLevel: stockLevels.reorderLevel,
      maxLevel: stockLevels.maxLevel,
    });
    if (!response.success) {
      showError(
        'Error',
        `Error while creating new inventory: ${response.error}`
      );
      return;
    }
    showSuccess(
      'Inventory Created',
      `Successfully created inventory for ${searchedProduct.name} at ${selectedLocation.name}`
    );
  };
  const updateInventoryLevels = async () => {
    const response = await updateInventory(inventoryId, {
      quantityOnHand: stockLevels.quantityOnHand,
      reorderLevel: stockLevels.reorderLevel,
      maxLevel: stockLevels.maxLevel,
    });
    if (!response.success) {
      showError('Error', `Error while updating inventory: ${response.error}`);
      return;
    }
    showSuccess(
      'Inventory Updated',
      `Successfully updated inventory for ${searchedProduct.name} at ${selectedLocation.name}`
    );
  };
  const handleSubmit = async e => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    setIsLoading(true);
    try {
      if (mode === 'add') {
        await addInventory();
      } else {
        await updateInventoryLevels();
      }
      if (onSuccess) {
        onSuccess();
      }

      handleCancel();
    } catch (error) {
      showError(
        mode === 'add' ? 'Create Failed' : 'Update Failed',
        error.message ||
          `Failed to ${mode === 'add' ? 'create' : 'update'} inventory`
      );
    } finally {
      setIsLoading(false);
    }
  };

  /**
   * Handle cancel action
   * Resets form and closes dialog
   */
  const handleCancel = () => {
    setActiveTab(0);
    setProductSearchTerm('');
    setSearchedProduct(null);
    setSelectedLocation(null);
    setStockLevels({
      quantityOnHand: 0,
      reorderLevel: 0,
      maxLevel: 0,
    });
    setMode('add');
    if (onClose) {
      onClose();
    }
  };

  /**
   * Load locations on component mount
   */
  useEffect(() => {
    const fetchLocations = async () => {
      try {
        const data = await getLocationsNames();
        if (data) {
          setLocations(data);
        }
      } catch (error) {
        console.error('Failed to fetch locations:', error);
      }
    };

    if (isOpen) {
      fetchLocations();
    }
  }, [isOpen]);

  /**
   * Load inventory data in update mode
   */
  useEffect(() => {
    const fetchInventoryData = async () => {
      if (inventoryId > 0 && isOpen) {
        setMode('update');
        setIsLoading(true);
        try {
          const inventoryResult = await getInventoryById(inventoryId);
          if (!inventoryResult.success) {
            showError(
              'Error',
              `Error: ${inventoryResult.error}, please try again`
            );
            return;
          }
          const inventoryData = inventoryResult.data;

          setSearchedProduct(inventoryData.product);
          setSelectedLocation(inventoryData.location);
          setStockLevels({
            quantityOnHand: inventoryData.quantityOnHand,
            reorderLevel: inventoryData.reorderLevel,
            maxLevel: inventoryData.maxLevel,
          });
          setAvailableStock(inventoryData.quantityOnHand);

          //   showError(
          //     'Update Mode',
          //     'API integration needed. Please implement getInventoryById endpoint.'
          //   );
        } catch {
          showError('Load Failed', 'Failed to load inventory data');
        } finally {
          setIsLoading(false);
        }
      }
    };

    fetchInventoryData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [inventoryId, isOpen]);

  if (!isOpen) return null;

  return (
    <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4'>
      <div className='bg-white rounded-lg w-full max-w-4xl max-h-[90vh] overflow-hidden flex flex-col'>
        {/* Header */}
        <div className='flex items-center justify-between p-6 border-b flex-shrink-0'>
          <div className='flex items-center gap-3'>
            <Archive className='h-6 w-6 text-gray-600' />
            <h2 className='text-xl font-semibold'>
              {mode === 'add'
                ? 'Add New Inventory'
                : `Update Inventory #${inventoryId}`}
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

        {/* Tabs */}
        <div className='flex bg-gray-50 border-b flex-shrink-0'>
          {tabs.map(tab => {
            const IconComponent = tab.icon;
            return (
              <button
                key={tab.id}
                onClick={() => setActiveTab(tab.id)}
                className={cn(
                  'flex-1 px-6 py-4 text-sm font-medium transition-colors border-b-2 cursor-pointer',
                  activeTab === tab.id
                    ? 'text-blue-600 border-blue-600 bg-white'
                    : 'text-gray-600 border-transparent hover:text-gray-800 hover:bg-gray-100'
                )}
                disabled={isLoading}
              >
                <div className='flex items-center justify-center gap-2'>
                  {IconComponent && <IconComponent className='h-4 w-4' />}
                  {tab.label}
                </div>
              </button>
            );
          })}
        </div>

        {/* Content */}
        <form
          onSubmit={handleSubmit}
          className='flex flex-col flex-1 overflow-hidden'
        >
          <div className='p-6 overflow-y-auto flex-1'>
            {/* Product Tab */}
            {activeTab === 0 && (
              <div>
                <div className='flex items-center gap-2 mb-6'>
                  <Package className='h-5 w-5' />
                  <h3 className='text-lg font-semibold'>Product Search</h3>
                </div>

                {/* Search Section */}
                <div className='mb-6'>
                  <label className='block text-sm font-medium mb-2'>
                    Search Product <span className='text-red-500'>*</span>
                  </label>
                  <div className='flex gap-2'>
                    <Input
                      placeholder='Enter product name, SKU, or ID'
                      value={productSearchTerm}
                      onChange={e => setProductSearchTerm(e.target.value)}
                      onKeyDown={e => {
                        if (e.key === 'Enter') {
                          e.preventDefault();
                          handleProductSearch();
                        }
                      }}
                      className='h-12'
                      disabled={mode === 'update' || isLoading}
                    />
                    <Button
                      type='button'
                      onClick={handleProductSearch}
                      disabled={mode === 'update' || isLoading || isSearching}
                      className='h-12 px-6 cursor-pointer'
                    >
                      <Search className='h-4 w-4 mr-2' />
                      Search
                    </Button>
                  </div>
                  {mode === 'update' && (
                    <p className='text-sm text-gray-500 mt-2'>
                      Product cannot be changed in update mode
                    </p>
                  )}
                </div>

                {/* Product Info Display */}
                {searchedProduct && (
                  <div className='bg-blue-50 border border-blue-200 rounded-lg p-6'>
                    <h4 className='font-semibold text-blue-900 mb-4'>
                      Product Information
                    </h4>
                    <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                      <div>
                        <p className='text-sm text-gray-600 mb-1'>
                          Product Name
                        </p>
                        <p className='font-medium text-gray-900'>
                          {searchedProduct.name}
                        </p>
                      </div>
                      <div>
                        <p className='text-sm text-gray-600 mb-1'>
                          Product SKU
                        </p>
                        <p className='font-medium text-gray-900'>
                          {searchedProduct.sku}
                        </p>
                      </div>
                      <div>
                        <p className='text-sm text-gray-600 mb-1'>Category</p>
                        <p className='font-medium text-gray-900'>
                          {searchedProduct.categoryName || 'N/A'}
                        </p>
                      </div>
                      <div>
                        <p className='text-sm text-gray-600 mb-1'>
                          Unit of Measure
                        </p>
                        <p className='font-medium text-gray-900'>
                          {searchedProduct.unitOfMeasureName || 'N/A'}
                        </p>
                      </div>
                      <div>
                        <p className='text-sm text-gray-600 mb-1'>Product ID</p>
                        <p className='font-medium text-gray-900'>
                          #{searchedProduct.id}
                        </p>
                      </div>
                    </div>
                  </div>
                )}

                {!searchedProduct && (
                  <div className='text-center py-8 text-gray-500'>
                    <Package className='h-12 w-12 mx-auto mb-3 text-gray-400' />
                    <p>Search for a product to get started</p>
                  </div>
                )}
              </div>
            )}

            {/* Location Tab */}
            {activeTab === 1 && (
              <div>
                <div className='flex items-center gap-2 mb-6'>
                  <MapPin className='h-5 w-5' />
                  <h3 className='text-lg font-semibold'>Location Selection</h3>
                </div>

                {/* Location Dropdown */}
                <div className='mb-6'>
                  <label className='block text-sm font-medium mb-2'>
                    Select Location <span className='text-red-500'>*</span>
                  </label>
                  <select
                    value={selectedLocation?.id || ''}
                    onChange={e => handleLocationSelect(e.target.value)}
                    className='w-full h-12 px-3 border border-gray-300 rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-600'
                    disabled={mode === 'update' || isLoading}
                  >
                    <option value=''>Choose a location...</option>
                    {locations.map(location => (
                      <option key={location.id} value={location.id}>
                        {location.name}
                      </option>
                    ))}
                  </select>
                  {mode === 'update' && (
                    <p className='text-sm text-gray-500 mt-2'>
                      Location cannot be changed in update mode
                    </p>
                  )}
                </div>

                {/* Location Info Display */}
                {selectedLocation && (
                  <div className='bg-green-50 border border-green-200 rounded-lg p-6'>
                    <h4 className='font-semibold text-green-900 mb-4'>
                      Location Information
                    </h4>
                    <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                      <div>
                        <p className='text-sm text-gray-600 mb-1'>
                          Location Name
                        </p>
                        <p className='font-medium text-gray-900'>
                          {selectedLocation.name}
                        </p>
                      </div>
                      <div>
                        <p className='text-sm text-gray-600 mb-1'>
                          Location ID
                        </p>
                        <p className='font-medium text-gray-900'>
                          #{selectedLocation.id}
                        </p>
                      </div>
                      <div>
                        <p className='text-sm text-gray-600 mb-1'>Address</p>
                        <p className='font-medium text-gray-900'>
                          {selectedLocation.address}
                        </p>
                      </div>
                      <div>
                        <p className='text-sm text-gray-600 mb-1'>Type</p>
                        <p className='font-medium text-gray-900'>
                          {selectedLocation.type}
                        </p>
                      </div>
                    </div>
                  </div>
                )}

                {!selectedLocation && (
                  <div className='text-center py-8 text-gray-500'>
                    <MapPin className='h-12 w-12 mx-auto mb-3 text-gray-400' />
                    <p>Select a location from the dropdown above</p>
                  </div>
                )}
              </div>
            )}

            {/* Stock Levels Tab */}
            {activeTab === 2 && (
              <div>
                <div className='flex items-center gap-2 mb-6'>
                  <Archive className='h-5 w-5' />
                  <h3 className='text-lg font-semibold'>Stock Levels</h3>
                </div>

                {/* Available Stock (Update Mode Only) */}
                {mode === 'update' && (
                  <div className='bg-yellow-50 border border-yellow-200 rounded-lg p-4 mb-6'>
                    <div className='flex items-center gap-2'>
                      <Archive className='h-5 w-5 text-yellow-600' />
                      <div>
                        <p className='text-sm font-medium text-yellow-900'>
                          Current Available Stock
                        </p>
                        <p className='text-2xl font-bold text-yellow-700'>
                          {availableStock.toFixed(2)}
                        </p>
                      </div>
                    </div>
                  </div>
                )}

                {/* Stock Level Inputs */}
                <div className='space-y-6'>
                  <div>
                    <label className='block text-sm font-medium mb-2'>
                      Quantity on Hand <span className='text-red-500'>*</span>
                    </label>
                    <Input
                      type='number'
                      step='1'
                      min='0'
                      placeholder='Enter quantity'
                      value={stockLevels.quantityOnHand}
                      onChange={e =>
                        handleStockLevelChange('quantityOnHand', e.target.value)
                      }
                      className='h-12'
                      disabled={isLoading}
                    />
                    <p className='text-sm text-gray-500 mt-1'>
                      Current stock quantity at this location
                    </p>
                  </div>

                  <div>
                    <label className='block text-sm font-medium mb-2'>
                      Reorder Level <span className='text-red-500'>*</span>
                    </label>
                    <Input
                      type='number'
                      step='1'
                      min='0'
                      placeholder='Enter reorder level'
                      value={stockLevels.reorderLevel}
                      onChange={e =>
                        handleStockLevelChange('reorderLevel', e.target.value)
                      }
                      className='h-12'
                      disabled={isLoading}
                    />
                    <p className='text-sm text-gray-500 mt-1'>
                      Minimum quantity before reordering is needed
                    </p>
                  </div>

                  <div>
                    <label className='block text-sm font-medium mb-2'>
                      Maximum Level <span className='text-red-500'>*</span>
                    </label>
                    <Input
                      type='number'
                      step='1'
                      min='0'
                      placeholder='Enter maximum level'
                      value={stockLevels.maxLevel}
                      onChange={e =>
                        handleStockLevelChange('maxLevel', e.target.value)
                      }
                      className='h-12'
                      disabled={isLoading}
                    />
                    <p className='text-sm text-gray-500 mt-1'>
                      Maximum storage capacity for this product at this location
                    </p>
                  </div>

                  {/* Validation Warnings */}
                  {stockLevels.maxLevel > 0 &&
                    stockLevels.reorderLevel > stockLevels.maxLevel && (
                      <div className='bg-red-50 border border-red-200 rounded-lg p-4'>
                        <p className='text-sm text-red-700'>
                          ⚠️ Reorder level cannot be greater than maximum level
                        </p>
                      </div>
                    )}
                </div>

                {/* Summary */}
                {searchedProduct && selectedLocation && (
                  <div className='mt-8 bg-gray-50 rounded-lg p-6'>
                    <h4 className='font-semibold mb-4'>Summary</h4>
                    <div className='space-y-2 text-sm'>
                      <div className='flex justify-between'>
                        <span className='text-gray-600'>Product:</span>
                        <span className='font-medium'>
                          {searchedProduct.name}
                        </span>
                      </div>
                      <div className='flex justify-between'>
                        <span className='text-gray-600'>Location:</span>
                        <span className='font-medium'>
                          {selectedLocation.name}
                        </span>
                      </div>
                      <div className='flex justify-between'>
                        <span className='text-gray-600'>Quantity:</span>
                        <span className='font-medium'>
                          {stockLevels.quantityOnHand}
                        </span>
                      </div>
                      <div className='flex justify-between'>
                        <span className='text-gray-600'>Reorder Level:</span>
                        <span className='font-medium'>
                          {stockLevels.reorderLevel}
                        </span>
                      </div>
                      <div className='flex justify-between'>
                        <span className='text-gray-600'>Max Level:</span>
                        <span className='font-medium'>
                          {stockLevels.maxLevel}
                        </span>
                      </div>
                    </div>
                  </div>
                )}
              </div>
            )}
          </div>

          {/* Footer */}
          <div className='flex items-center justify-between gap-3 p-6 border-t bg-gray-50 flex-shrink-0'>
            <div className='text-sm text-gray-600'>
              {!isReadyToSave && (
                <span className='text-yellow-600'>
                  ⚠️ Please complete Product and Location sections first
                </span>
              )}
              {isReadyToSave && (
                <span className='text-green-600'>
                  ✓ Ready to {mode === 'add' ? 'create' : 'update'}
                </span>
              )}
            </div>
            <div className='flex gap-3'>
              <Button
                type='button'
                variant='secondary'
                onClick={handleCancel}
                disabled={isLoading}
                className='cursor-pointer'
              >
                Cancel
              </Button>
              <Button
                type='submit'
                disabled={!isReadyToSave || isLoading}
                className='cursor-pointer'
              >
                {isLoading
                  ? 'Processing...'
                  : mode === 'add'
                    ? 'Create Inventory'
                    : 'Update Stock Levels'}
              </Button>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddUpdateInventory;
