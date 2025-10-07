import React, { useState, useEffect } from 'react';
import { X, Package, DollarSign, Archive } from 'lucide-react';
import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import { cn } from '@/lib/utils';
import { getProductCategories } from '@/services/products/productCategoryService';
import { GetUnitsNames } from '@/services/products/UnitOfMeasureService';
import { getLocationsNames } from '@/services/products/locationService';
import {
  createProduct,
  getProductById,
  updateProduct,
} from '@/services/products/productService';
/**
 * AddProduct Component
 *
 * A multi-step modal component for adding new products to the inventory system.
 * Features a tabbed interface with Basic Info, Pricing, Inventory, and Details sections.
 *
 * @param {Object} props - Component props
 * @param {boolean} props.isOpen - Controls modal visibility
 * @param {function} props.onClose - Callback function when modal is closed
 * @param {function} props.onSubmit - Callback function when form is submitted with product data
 * @param {boolean} props.isLoading - Shows loading state on submit button
 */
const AddProduct = ({ isOpen, onClose, productId = 0, isLoading = false }) => {
  const [activeTab, setActiveTab] = useState(0);
  const [formData, setFormData] = useState({
    // Basic Info
    productName: '',
    sku: '',
    category: 0,
    description: '',
    status: 'Active',

    // Pricing
    costPrice: 0,
    sellingPrice: 0,

    // Inventory
    currentStock: 0,
    minimumStock: 0,
    maximumStock: 0,
    unitOfMeasurement: 0,
    storageLocation: 0,
  });
  const [mode, setMode] = useState('add'); // 'add' or 'update'
  // to do make these in custom hook
  const [categories, setCategories] = useState([]);
  const [unitOfMeasurement, setUnitOfMeasurement] = useState([]);
  const [locations, setLocations] = useState([]);
  const tabs = [
    { id: 0, label: 'Basic Info', icon: Package },
    { id: 1, label: 'Pricing', icon: DollarSign },
    { id: 2, label: 'Inventory', icon: Archive },
    { id: 3, label: 'Details' },
  ];
  const statusOptions = ['Active', 'Inactive', 'Draft'];
  const [id, setId] = useState(productId);
  // Calculate profit metrics
  const profitPerUnit = formData.sellingPrice - formData.costPrice;
  const profitMargin =
    formData.sellingPrice > 0
      ? ((profitPerUnit / formData.sellingPrice) * 100).toFixed(1)
      : '0';
  const markup =
    formData.costPrice > 0
      ? ((profitPerUnit / formData.costPrice) * 100).toFixed(1)
      : '0';

  const handleInputChange = (field, value) => {
    setFormData(prev => ({
      ...prev,
      [field]: value,
    }));
  };
  const addNewProduct = async ({
    sku,
    name,
    description,
    categoryId,
    unitOfMeasureId,
    unitPrice,
    costPrice,
    locationId,
    quantityOnHand,
    reorderLevel,
    maxLevel,
  }) => {
    const data = await createProduct({
      sku,
      name,
      description,
      categoryId,
      unitOfMeasureId,
      unitPrice,
      costPrice,
      locationId,
      quantityOnHand,
      reorderLevel,
      maxLevel,
    });
    if (data) {
      console.log('Product created successfully:', data);

      setFormData({
        productName: data.name,
        sku: data.sku,
        category: data.categoryId,
        description: data.description,
        status: data.isActive,
        costPrice: data.costPrice,
        sellingPrice: data.unitPrice,
        currentStock: data.inventories[0].quantityOnHand,
        minimumStock: data.inventories[0].reorderLevel,
        maximumStock: data.inventories[0].maxLevel,
        unitOfMeasurement: data.unitOfMeasureId,
        storageLocation: data.inventories[0].locationId,
      });
      const locations = data.inventories.map(i => {
        return { id: i.locationId, name: i.locationName };
      });
      setLocations(locations);
      setMode('update');
    }
  };
  const editProduct = async ({
    id,
    sku,
    name,
    description,
    categoryId,
    unitOfMeasureId,
    unitPrice,
    costPrice,
    locationId,
    quantityOnHand,
    reorderLevel,
    maxLevel,
  }) => {
    const data = await updateProduct(id, {
      sku,
      name,
      description,
      categoryId,
      unitOfMeasureId,
      unitPrice,
      costPrice,
      locationId,
      quantityOnHand,
      reorderLevel,
      maxLevel,
    });
    if (data) {
      console.log('Product updated successfully:', data);
      setFormData({
        productName: data.name,
        sku: data.sku,
        category: data.categoryId,
        description: data.description,
        status: data.isActive,
        costPrice: data.costPrice,
        sellingPrice: data.unitPrice,
        currentStock: data.inventories[0].quantityOnHand,
        minimumStock: data.inventories[0].reorderLevel,
        maximumStock: data.inventories[0].maxLevel,
        unitOfMeasurement: data.unitOfMeasureId,
        storageLocation: data.inventories[0].locationId,
      });
      const locations = data.inventories.map(i => {
        return { id: i.locationId, name: i.locationName };
      });
      setLocations(locations);
      setMode('update');
      console.log('Product updated successfully:', data);
    }
  };
  const saveProduct = () => {
    if (mode === 'add') {
      addNewProduct({
        sku: formData.sku,
        name: formData.productName,
        description: formData.description,
        categoryId: formData.category,
        unitOfMeasureId: formData.unitOfMeasurement,
        unitPrice: formData.sellingPrice,
        costPrice: formData.costPrice,
        locationId: formData.storageLocation,
        quantityOnHand: formData.currentStock,
        reorderLevel: formData.minimumStock,
        maxLevel: formData.maximumStock,
      });
    }
    if (mode === 'update') {
      console.log('data: ', formData);
      console.log('sku:', formData.sku);
      editProduct({
        id: id,
        sku: formData.sku,
        name: formData.productName,
        description: formData.description,
        categoryId: formData.category,
        unitOfMeasureId: formData.unitOfMeasurement,
        unitPrice: formData.sellingPrice,
        costPrice: formData.costPrice,
        locationId: formData.storageLocation,
        quantityOnHand: formData.currentStock,
        reorderLevel: formData.minimumStock,
        maxLevel: formData.maximumStock,
      });
    }
    setMode('update');
  };
  const handleSubmit = () => {
    saveProduct();
  };

  const handleCancel = () => {
    setFormData({
      productName: '',
      sku: '',
      category: 0,
      description: '',
      status: 'Active',
      costPrice: 0,
      sellingPrice: 0,
      currentStock: 0,
      minimumStock: 0,
      maximumStock: 0,
      unitOfMeasurement: 0,
      storageLocation: 0,
    });
    setActiveTab(0);
    if (onClose) {
      onClose();
    }
  };

  useEffect(() => {
    const fetchCategories = async () => {
      const data = await getProductCategories();
      if (data) {
        setCategories(data);
      }
    };
    const fetchUnitOfMeasures = async () => {
      const data = await GetUnitsNames();
      if (data) {
        console.log(data);
        setUnitOfMeasurement(data);
      }
    };

    fetchUnitOfMeasures();
    fetchCategories();
    if (id > 0) {
      const fetchProduct = async id => {
        const data = await getProductById(id);
        if (data) {
          setFormData({
            productName: data.name,
            sku: data.sku,
            category: data.categoryId,
            description: data.description,
            status: data.isActive,
            costPrice: data.costPrice,
            sellingPrice: data.unitPrice,

            unitOfMeasurement: data.unitOfMeasureId,
            currentStock: data.inventories[0].quantityOnHand,
            minimumStock: data.inventories[0].reorderLevel,
            maximumStock: data.inventories[0].maxLevel,
            storageLocation: data.inventories[0].locationId,
          });
          const locations = data.inventories.map(i => {
            return { id: i.locationId, name: i.locationName };
          });
          console.log(locations);
          setLocations(locations);
          setMode('update');
        }
      };

      fetchProduct(id);
    }
    if (mode === 'add') {
      const fetchLocations = async () => {
        const data = await getLocationsNames();
        if (data) {
          console.log(data);
          setLocations(data);
        }
      };
      fetchLocations();
    }

    console.log('mode is : ', mode);
  }, [mode, id]);

  if (!isOpen) return null;

  return (
    <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4'>
      <div className='bg-white rounded-lg w-full max-w-4xl max-h-[90vh] overflow-hidden'>
        {/* Header */}
        <div className='flex items-center justify-between p-6 border-b'>
          <div className='flex items-center gap-3'>
            <Package className='h-6 w-6 text-gray-600' />
            <h2 className='text-xl font-semibold'>
              {mode === 'add' ? 'Add New Product' : 'Edit Product'}
            </h2>
          </div>
          <button
            onClick={handleCancel}
            className='p-2 hover:bg-gray-100 rounded-lg transition-colors'
          >
            <X className='h-5 w-5' />
          </button>
        </div>

        {/* Tabs */}
        <div className='flex bg-gray-50 border-b'>
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
        <div className='p-6 overflow-y-auto max-h-[calc(90vh-200px)]'>
          {/* Basic Info Tab */}
          {activeTab === 0 && (
            <div>
              <div className='flex items-center gap-2 mb-6'>
                <Package className='h-5 w-5' />
                <h3 className='text-lg font-semibold'>Product Information</h3>
              </div>

              <div className='grid grid-cols-1 md:grid-cols-2 gap-6'>
                <div>
                  <label className='block text-sm font-medium mb-2'>
                    Product Name
                  </label>
                  <Input
                    placeholder='Enter product name'
                    value={formData.productName}
                    onChange={e =>
                      handleInputChange('productName', e.target.value)
                    }
                    className='h-12'
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium mb-2'>SKU</label>
                  <Input
                    placeholder='Enter SKU'
                    value={formData.sku}
                    onChange={e => handleInputChange('sku', e.target.value)}
                    className='h-12'
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium mb-2'>
                    Category
                  </label>
                  <select
                    value={formData.category}
                    onChange={e =>
                      handleInputChange('category', e.target.value)
                    }
                    className='w-full h-12 px-3 border border-gray-300 rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-600'
                  >
                    {categories.map(option => (
                      <option key={option.id} value={option.id}>
                        {option.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div className='md:col-span-2'>
                  <label className='block text-sm font-medium mb-2'>
                    Description
                  </label>
                  <textarea
                    placeholder='Enter product description'
                    value={formData.description}
                    onChange={e =>
                      handleInputChange('description', e.target.value)
                    }
                    className='w-full h-32 px-3 py-2 border border-gray-300 rounded-md resize-none focus:outline-none focus:ring-1 focus:ring-blue-600'
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium mb-2'>
                    Status
                  </label>
                  <select
                    value={formData.status}
                    onChange={e => handleInputChange('status', e.target.value)}
                    className='w-full h-12 px-3 border border-gray-300 rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-600'
                  >
                    {statusOptions.map(status => (
                      <option key={status} value={status}>
                        {status}
                      </option>
                    ))}
                  </select>
                </div>
              </div>
            </div>
          )}

          {/* Pricing Tab */}
          {activeTab === 1 && (
            <div>
              <div className='flex items-center gap-2 mb-6'>
                <DollarSign className='h-5 w-5' />
                <h3 className='text-lg font-semibold'>Pricing Information</h3>
              </div>

              <div className='grid grid-cols-1 md:grid-cols-2 gap-6 mb-8'>
                <div>
                  <label className='block text-sm font-medium mb-2'>
                    Cost Price
                  </label>
                  <Input
                    type='number'
                    placeholder='0'
                    value={formData.costPrice}
                    onChange={e =>
                      handleInputChange(
                        'costPrice',
                        parseFloat(e.target.value) || 0
                      )
                    }
                    className='h-12'
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium mb-2'>
                    Selling Price
                  </label>
                  <Input
                    type='number'
                    placeholder='0'
                    value={formData.sellingPrice}
                    onChange={e =>
                      handleInputChange(
                        'sellingPrice',
                        parseFloat(e.target.value) || 0
                      )
                    }
                    className='h-12'
                  />
                </div>
              </div>

              {/* Profit Calculations */}
              <div className='grid grid-cols-1 md:grid-cols-3 gap-4'>
                <div className='bg-blue-50 rounded-lg p-4'>
                  <div className='text-sm text-gray-600 mb-1'>
                    Profit Margin
                  </div>
                  <div className='text-2xl font-bold text-blue-600'>
                    {profitMargin}%
                  </div>
                </div>

                <div className='bg-green-50 rounded-lg p-4'>
                  <div className='text-sm text-gray-600 mb-1'>
                    Profit per Unit
                  </div>
                  <div className='text-2xl font-bold text-green-600'>
                    ${profitPerUnit.toFixed(2)}
                  </div>
                </div>

                <div className='bg-purple-50 rounded-lg p-4'>
                  <div className='text-sm text-gray-600 mb-1'>Markup</div>
                  <div className='text-2xl font-bold text-purple-600'>
                    {markup}%
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Inventory Tab */}
          {activeTab === 2 && (
            <div>
              <div className='flex items-center gap-2 mb-6'>
                <Archive className='h-5 w-5' />
                <h3 className='text-lg font-semibold'>Inventory Management</h3>
              </div>

              <div className='grid grid-cols-1 md:grid-cols-3 gap-6 mb-6'>
                <div>
                  <label className='block text-sm font-medium mb-2'>
                    Current Stock
                  </label>
                  <Input
                    type='number'
                    placeholder='0'
                    value={formData.currentStock}
                    onChange={e =>
                      handleInputChange(
                        'currentStock',
                        parseInt(e.target.value) || 0
                      )
                    }
                    className='h-12'
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium mb-2'>
                    Minimum Stock
                  </label>
                  <Input
                    type='number'
                    placeholder='0'
                    value={formData.minimumStock}
                    onChange={e =>
                      handleInputChange(
                        'minimumStock',
                        parseInt(e.target.value) || 0
                      )
                    }
                    className='h-12'
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium mb-2'>
                    Maximum Stock
                  </label>
                  <Input
                    type='number'
                    placeholder='0'
                    value={formData.maximumStock}
                    onChange={e =>
                      handleInputChange(
                        'maximumStock',
                        parseInt(e.target.value) || 0
                      )
                    }
                    className='h-12'
                  />
                </div>
              </div>

              <div className='grid grid-cols-1 md:grid-cols-2 gap-6'>
                <div>
                  <label className='block text-sm font-medium mb-2'>
                    Unit of Measurement
                  </label>
                  <select
                    value={formData.unitOfMeasurement}
                    onChange={e =>
                      handleInputChange('unitOfMeasurement', e.target.value)
                    }
                    className='w-full h-12 px-3 border border-gray-300 rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-600'
                  >
                    {unitOfMeasurement.map(unit => (
                      <option key={unit.id} value={unit.id}>
                        {unit.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className='block text-sm font-medium mb-2'>
                    Storage Location
                  </label>
                  <select
                    value={formData.storageLocation}
                    onChange={e =>
                      handleInputChange('storageLocation', e.target.value)
                    }
                    className='w-full h-12 px-3 border border-gray-300 rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-600'
                  >
                    {locations.map(location => (
                      <option key={location.id} value={location.id}>
                        {location.name}
                      </option>
                    ))}
                  </select>
                </div>
              </div>
            </div>
          )}

          {/* Details Tab */}
          {activeTab === 3 && (
            <div>
              <div className='flex items-center gap-2 mb-6'>
                <Package className='h-5 w-5' />
                <h3 className='text-lg font-semibold'>Additional Details</h3>
              </div>

              <div className='bg-gray-50 rounded-lg p-6'>
                <h4 className='font-medium mb-4'>Product Summary</h4>
                <div className='space-y-3'>
                  <div className='flex justify-between'>
                    <span className='text-gray-600'>Product Name:</span>
                    <span className='font-medium'>
                      {formData.productName || 'Not specified'}
                    </span>
                  </div>
                  <div className='flex justify-between'>
                    <span className='text-gray-600'>SKU:</span>
                    <span className='font-medium'>
                      {formData.sku || 'Not specified'}
                    </span>
                  </div>
                  <div className='flex justify-between'>
                    <span className='text-gray-600'>Category:</span>
                    <span className='font-medium'>
                      {formData.category || 'Not specified'}
                    </span>
                  </div>
                  <div className='flex justify-between'>
                    <span className='text-gray-600'>Selling Price:</span>
                    <span className='font-medium'>
                      ${formData.sellingPrice.toFixed(2)}
                    </span>
                  </div>
                  <div className='flex justify-between'>
                    <span className='text-gray-600'>Initial Stock:</span>
                    <span className='font-medium'>
                      {formData.currentStock} {formData.unitOfMeasurement}
                    </span>
                  </div>
                  <div className='flex justify-between'>
                    <span className='text-gray-600'>Status:</span>
                    <span
                      className={`font-medium ${formData.status === 'Active' ? 'text-green-600' : 'text-gray-600'}`}
                    >
                      {formData.status}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>

        {/* Footer */}
        <div className='flex items-center justify-end gap-3 p-6 border-t bg-gray-50'>
          <Button
            variant='secondary'
            onClick={handleCancel}
            disabled={isLoading}
            className='cursor-pointer'
          >
            Cancel
          </Button>
          <Button
            onClick={handleSubmit}
            // loading={isLoading}
            disabled={
              !formData.productName ||
              !formData.sku ||
              !formData.category ||
              !formData.unitOfMeasurement ||
              !formData.storageLocation
            }
            className='cursor-pointer'
          >
            {mode === 'add' ? 'Add Product' : 'Save Changes'}
          </Button>
        </div>
      </div>
    </div>
  );
};
export default AddProduct;
