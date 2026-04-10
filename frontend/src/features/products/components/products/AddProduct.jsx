import React, { useState, useEffect } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { X, Package, Wallet, Archive } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import { cn } from '@shared/lib/utils';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { getProductCategories } from '@features/products/services/productCategoryApi';
import { GetUnitsNames } from '@features/products/services/unitOfMeasureApi';
import { getLocationsNames } from '@features/inventory/services/locationApi';
import {
  createProduct,
  getProductById,
  updateProduct,
} from '@features/products/services/productApi';
import { useToast } from '@shared/context/ToastContext';
import { queryKeys } from '@shared/lib/queryKeys';
import { formatDzdCurrency } from '@shared/utils/currencyFormatter';

const getInitialFormData = () => ({
  // Basic Info
  productName: '',
  sku: '',
  category: 0,
  description: '',
  status: 'active',

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
/**
 * AddProduct Component
 *
 * A multi-step modal component for adding new products to the inventory system.
 * Features a tabbed interface with Basic Info, Pricing, Inventory, and Details sections.
 *
 * @param {Object} props - Component props
 * @param {boolean} props.isOpen - Controls modal visibility
 * @param {function} props.onClose - Callback function when modal is closed
 * @param {number} props.productId - ID of the product to edit (0 for new product)
 */
const AddProduct = ({ isOpen, onClose, productId = 0 }) => {
  const { t, i18n } = useTranslation();
  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';
  const [activeTab, setActiveTab] = useState(0);
  const { showSuccess, showError } = useToast();
  const queryClient = useQueryClient();
  const [formData, setFormData] = useState(getInitialFormData);
  const [mode, setMode] = useState('add'); // 'add' or 'update'
  const [productLocations, setProductLocations] = useState([]);
  const tabs = [
    {
      id: 0,
      label: t(i18nKeyContainer.products.addProductForm.tabs.basicInfo),
      icon: Package,
    },
    {
      id: 1,
      label: t(i18nKeyContainer.products.addProductForm.tabs.pricing),
      icon: Wallet,
    },
    {
      id: 2,
      label: t(i18nKeyContainer.products.addProductForm.tabs.inventory),
      icon: Archive,
    },
    {
      id: 3,
      label: t(i18nKeyContainer.products.addProductForm.tabs.details),
    },
  ];
  const statusOptions = ['active', 'inactive', 'draft'];
  const [id, setId] = useState(productId);
  const [isLoading, setIsLoading] = useState(false);

  const resetAddState = () => {
    setFormData(getInitialFormData());
    setProductLocations([]);
    setActiveTab(0);
    setId(0);
    setMode('add');
  };

  const { data: categories = [] } = useQuery({
    queryKey: queryKeys.products.categories(),
    queryFn: getProductCategories,
    enabled: isOpen,
  });

  const { data: unitOfMeasurement = [] } = useQuery({
    queryKey: queryKeys.products.unitOfMeasure(),
    queryFn: GetUnitsNames,
    enabled: isOpen,
  });

  const { data: baseLocations = [] } = useQuery({
    queryKey: [...queryKeys.inventory.locations('names')],
    queryFn: getLocationsNames,
    enabled: isOpen && mode === 'add',
  });

  const { data: productResponse } = useQuery({
    queryKey: queryKeys.products.detail(id),
    queryFn: () => getProductById(id),
    enabled: isOpen && id > 0,
  });

  const createProductMutation = useMutation({
    mutationFn: createProduct,
    onSuccess: async response => {
      const createdProduct = response?.data ?? response;

      if (createdProduct) {
        showSuccess(
          t(i18nKeyContainer.products.addProductForm.toasts.createSuccessTitle),
          t(
            i18nKeyContainer.products.addProductForm.toasts
              .createSuccessMessage,
            {
              name: createdProduct.name || formData.productName,
            }
          )
        );

        resetAddState();
        onClose();

        await queryClient.invalidateQueries({
          queryKey: queryKeys.products.all,
        });
        await queryClient.invalidateQueries({
          queryKey: queryKeys.inventory.all,
        });
      }
    },
    onError: error => {
      console.error('Product creation error:', error);
      showError(
        t(i18nKeyContainer.products.addProductForm.toasts.createErrorTitle),
        error.message ||
          t(i18nKeyContainer.products.addProductForm.toasts.createErrorMessage)
      );
    },
  });

  const updateProductMutation = useMutation({
    mutationFn: payload => updateProduct(payload.id, payload.data),
    onSuccess: async data => {
      if (data) {
        showSuccess(
          t(i18nKeyContainer.products.addProductForm.toasts.updateSuccessTitle),
          t(
            i18nKeyContainer.products.addProductForm.toasts
              .updateSuccessMessage,
            {
              name: data.name,
            }
          )
        );
        onClose();

        setFormData({
          productName: data.name,
          sku: data.sku,
          category: data.categoryId,
          description: data.description,
          status: data.isActive ? 'active' : 'inactive',
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
        setProductLocations(locations);
        setMode('update');

        await queryClient.invalidateQueries({
          queryKey: queryKeys.products.all,
        });
        await queryClient.invalidateQueries({
          queryKey: queryKeys.inventory.all,
        });
      } else {
        showError(
          t(i18nKeyContainer.products.addProductForm.toasts.updateErrorTitle),
          t(i18nKeyContainer.products.addProductForm.toasts.updateErrorMessage)
        );
      }
    },
    onError: error => {
      console.error('Product update error:', error);
      showError(
        t(i18nKeyContainer.products.addProductForm.toasts.updateErrorTitle),
        error.message ||
          t(i18nKeyContainer.products.addProductForm.toasts.updateErrorMessage)
      );
    },
  });

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

  const getStatusLabel = status => {
    switch (status) {
      case true:
      case 'active':
        return t(i18nKeyContainer.products.shared.status.active);
      case false:
      case 'inactive':
        return t(i18nKeyContainer.products.shared.status.inactive);
      case 'draft':
        return t(i18nKeyContainer.products.shared.status.draft);
      default:
        return status;
    }
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
    await createProductMutation.mutateAsync({
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
  };
  const editProduct = async ({
    id,
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
    await updateProductMutation.mutateAsync({
      id,
      data: {
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
      },
    });
  };
  const saveProduct = async () => {
    const normalizedPayload = {
      sku: formData.sku.trim(),
      name: formData.productName.trim(),
      description: formData.description.trim(),
      categoryId: Number(formData.category) || 0,
      unitOfMeasureId: Number(formData.unitOfMeasurement) || 0,
      unitPrice: Number(formData.sellingPrice) || 0,
      costPrice: Number(formData.costPrice) || 0,
      locationId: Number(formData.storageLocation) || 0,
      quantityOnHand: Number(formData.currentStock) || 0,
      reorderLevel: Number(formData.minimumStock) || 0,
      maxLevel: Number(formData.maximumStock) || 0,
    };

    if (mode === 'add') {
      await addNewProduct(normalizedPayload);
    }
    if (mode === 'update') {
      await editProduct({
        id: id,
        ...normalizedPayload,
      });
    }
  };
  const handleSubmit = async () => {
    setIsLoading(true);
    try {
      await saveProduct();
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancel = () => {
    if ((Number(productId) || 0) === 0) {
      resetAddState();
    } else {
      setActiveTab(0);
    }
    if (onClose) {
      onClose();
    }
  };

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    const nextProductId = Number(productId) || 0;
    setId(nextProductId);
    setActiveTab(0);

    if (nextProductId > 0) {
      setMode('update');
      return;
    }

    resetAddState();
  }, [isOpen, productId]);

  useEffect(() => {
    if (!productResponse || id <= 0) {
      return;
    }

    setFormData({
      productName: productResponse.name,
      sku: productResponse.sku,
      category: productResponse.categoryId,
      description: productResponse.description,
      status: productResponse.isActive ? 'active' : 'inactive',
      costPrice: productResponse.costPrice,
      sellingPrice: productResponse.unitPrice,
      unitOfMeasurement: productResponse.unitOfMeasureId,
      currentStock: productResponse.inventories[0].quantityOnHand,
      minimumStock: productResponse.inventories[0].reorderLevel,
      maximumStock: productResponse.inventories[0].maxLevel,
      storageLocation: productResponse.inventories[0].locationId,
    });
    const locations = productResponse.inventories.map(i => {
      return { id: i.locationId, name: i.locationName };
    });
    setProductLocations(locations);
    setMode('update');
  }, [id, productResponse]);

  const locations = mode === 'add' ? baseLocations : productLocations;
  const isSubmitDisabled =
    isLoading ||
    !formData.productName.trim() ||
    !formData.sku.trim() ||
    !formData.category ||
    !formData.unitOfMeasurement ||
    !formData.storageLocation;

  if (!isOpen) return null;

  return (
    <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4'>
      <div className='bg-white rounded-lg w-full max-w-4xl max-h-[90vh] overflow-hidden'>
        {/* Header */}
        <div className='flex items-center justify-between p-6 border-b'>
          <div className='flex items-center gap-3'>
            <Package className='h-6 w-6 text-gray-600' />
            <h2 className='text-xl font-semibold'>
              {mode === 'add'
                ? t(i18nKeyContainer.products.addProductForm.title.add)
                : t(i18nKeyContainer.products.addProductForm.title.edit)}
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
                <h3 className='text-lg font-semibold'>
                  {t(
                    i18nKeyContainer.products.addProductForm.sections
                      .productInformation
                  )}
                </h3>
              </div>

              <div className='grid grid-cols-1 md:grid-cols-2 gap-6'>
                <div>
                  <label className='block text-sm font-medium mb-2'>
                    {t(
                      i18nKeyContainer.products.addProductForm.fields
                        .productName
                    )}
                  </label>
                  <Input
                    placeholder={t(
                      i18nKeyContainer.products.addProductForm.placeholders
                        .productName
                    )}
                    value={formData.productName}
                    onChange={e =>
                      handleInputChange('productName', e.target.value)
                    }
                    className='h-12'
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium mb-2'>
                    {t(i18nKeyContainer.products.addProductForm.fields.sku)}
                  </label>
                  <Input
                    placeholder={t(
                      i18nKeyContainer.products.addProductForm.placeholders.sku
                    )}
                    value={formData.sku}
                    onChange={e => handleInputChange('sku', e.target.value)}
                    className='h-12'
                    disabled={mode === 'update'}
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium mb-2'>
                    {t(
                      i18nKeyContainer.products.addProductForm.fields.category
                    )}
                  </label>
                  <select
                    value={formData.category}
                    onChange={e =>
                      handleInputChange('category', Number(e.target.value) || 0)
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
                    {t(
                      i18nKeyContainer.products.addProductForm.fields
                        .description
                    )}
                  </label>
                  <textarea
                    placeholder={t(
                      i18nKeyContainer.products.addProductForm.placeholders
                        .description
                    )}
                    value={formData.description}
                    onChange={e =>
                      handleInputChange('description', e.target.value)
                    }
                    className='w-full h-32 px-3 py-2 border border-gray-300 rounded-md resize-none focus:outline-none focus:ring-1 focus:ring-blue-600'
                  />
                </div>

                <div>
                  <label className='block text-sm font-medium mb-2'>
                    {t(i18nKeyContainer.products.addProductForm.fields.status)}
                  </label>
                  <select
                    value={formData.status}
                    onChange={e => handleInputChange('status', e.target.value)}
                    className='w-full h-12 px-3 border border-gray-300 rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-600'
                  >
                    {statusOptions.map(status => (
                      <option key={status} value={status}>
                        {getStatusLabel(status)}
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
                <Wallet className='h-5 w-5' />
                <h3 className='text-lg font-semibold'>
                  {t(
                    i18nKeyContainer.products.addProductForm.sections
                      .pricingInformation
                  )}
                </h3>
              </div>

              <div className='grid grid-cols-1 md:grid-cols-2 gap-6 mb-8'>
                <div>
                  <label className='block text-sm font-medium mb-2'>
                    {t(
                      i18nKeyContainer.products.addProductForm.fields.costPrice
                    )}
                  </label>
                  <Input
                    type='number'
                    placeholder={t(
                      i18nKeyContainer.products.addProductForm.placeholders.zero
                    )}
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
                    {t(
                      i18nKeyContainer.products.addProductForm.fields
                        .sellingPrice
                    )}
                  </label>
                  <Input
                    type='number'
                    placeholder={t(
                      i18nKeyContainer.products.addProductForm.placeholders.zero
                    )}
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
                    {t(
                      i18nKeyContainer.products.addProductForm.metrics
                        .profitMargin
                    )}
                  </div>
                  <div className='text-2xl font-bold text-blue-600'>
                    {profitMargin}%
                  </div>
                </div>

                <div className='bg-green-50 rounded-lg p-4'>
                  <div className='text-sm text-gray-600 mb-1'>
                    {t(
                      i18nKeyContainer.products.addProductForm.metrics
                        .profitPerUnit
                    )}
                  </div>
                  <div className='text-2xl font-bold text-green-600'>
                    {formatDzdCurrency(profitPerUnit, { locale: activeLocale })}
                  </div>
                </div>

                <div className='bg-purple-50 rounded-lg p-4'>
                  <div className='text-sm text-gray-600 mb-1'>
                    {t(i18nKeyContainer.products.addProductForm.metrics.markup)}
                  </div>
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
                <h3 className='text-lg font-semibold'>
                  {t(
                    i18nKeyContainer.products.addProductForm.sections
                      .inventoryManagement
                  )}
                </h3>
              </div>

              <div className='grid grid-cols-1 md:grid-cols-3 gap-6 mb-6'>
                <div>
                  <label className='block text-sm font-medium mb-2'>
                    {t(
                      i18nKeyContainer.products.addProductForm.fields
                        .currentStock
                    )}
                  </label>
                  <Input
                    type='number'
                    placeholder={t(
                      i18nKeyContainer.products.addProductForm.placeholders.zero
                    )}
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
                    {t(
                      i18nKeyContainer.products.addProductForm.fields
                        .minimumStock
                    )}
                  </label>
                  <Input
                    type='number'
                    placeholder={t(
                      i18nKeyContainer.products.addProductForm.placeholders.zero
                    )}
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
                    {t(
                      i18nKeyContainer.products.addProductForm.fields
                        .maximumStock
                    )}
                  </label>
                  <Input
                    type='number'
                    placeholder={t(
                      i18nKeyContainer.products.addProductForm.placeholders.zero
                    )}
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
                    {t(
                      i18nKeyContainer.products.addProductForm.fields
                        .unitOfMeasurement
                    )}
                  </label>
                  <select
                    value={formData.unitOfMeasurement}
                    onChange={e =>
                      handleInputChange(
                        'unitOfMeasurement',
                        Number(e.target.value) || 0
                      )
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
                    {t(
                      i18nKeyContainer.products.addProductForm.fields
                        .storageLocation
                    )}
                  </label>
                  <select
                    value={formData.storageLocation}
                    onChange={e =>
                      handleInputChange(
                        'storageLocation',
                        Number(e.target.value) || 0
                      )
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
                <h3 className='text-lg font-semibold'>
                  {t(
                    i18nKeyContainer.products.addProductForm.sections
                      .additionalDetails
                  )}
                </h3>
              </div>

              <div className='bg-gray-50 rounded-lg p-6'>
                <h4 className='font-medium mb-4'>
                  {t(
                    i18nKeyContainer.products.addProductForm.sections
                      .productSummary
                  )}
                </h4>
                <div className='space-y-3'>
                  <div className='flex justify-between'>
                    <span className='text-gray-600'>
                      {t(
                        i18nKeyContainer.products.addProductForm.summary
                          .productName
                      )}
                    </span>
                    <span className='font-medium'>
                      {formData.productName ||
                        t(i18nKeyContainer.products.shared.notSpecified)}
                    </span>
                  </div>
                  <div className='flex justify-between'>
                    <span className='text-gray-600'>
                      {t(i18nKeyContainer.products.addProductForm.summary.sku)}
                    </span>
                    <span className='font-medium'>
                      {formData.sku ||
                        t(i18nKeyContainer.products.shared.notSpecified)}
                    </span>
                  </div>
                  <div className='flex justify-between'>
                    <span className='text-gray-600'>
                      {t(
                        i18nKeyContainer.products.addProductForm.summary
                          .category
                      )}
                    </span>
                    <span className='font-medium'>
                      {formData.category ||
                        t(i18nKeyContainer.products.shared.notSpecified)}
                    </span>
                  </div>
                  <div className='flex justify-between'>
                    <span className='text-gray-600'>
                      {t(
                        i18nKeyContainer.products.addProductForm.summary
                          .sellingPrice
                      )}
                    </span>
                    <span className='font-medium'>
                      {formatDzdCurrency(formData.sellingPrice, {
                        locale: activeLocale,
                      })}
                    </span>
                  </div>
                  <div className='flex justify-between'>
                    <span className='text-gray-600'>
                      {t(
                        i18nKeyContainer.products.addProductForm.summary
                          .initialStock
                      )}
                    </span>
                    <span className='font-medium'>
                      {formData.currentStock} {formData.unitOfMeasurement}
                    </span>
                  </div>
                  <div className='flex justify-between'>
                    <span className='text-gray-600'>
                      {t(
                        i18nKeyContainer.products.addProductForm.summary.status
                      )}
                    </span>
                    <span
                      className={`font-medium ${formData.status === 'active' ? 'text-green-600' : 'text-gray-600'}`}
                    >
                      {getStatusLabel(formData.status)}
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
            {t(i18nKeyContainer.products.addProductForm.actions.cancel)}
          </Button>
          <Button
            onClick={handleSubmit}
            // loading={isLoading}
            disabled={isSubmitDisabled}
            className='cursor-pointer'
          >
            {mode === 'add'
              ? t(i18nKeyContainer.products.addProductForm.actions.addProduct)
              : t(i18nKeyContainer.products.addProductForm.actions.saveChanges)}
          </Button>
        </div>
      </div>
    </div>
  );
};
export default AddProduct;
