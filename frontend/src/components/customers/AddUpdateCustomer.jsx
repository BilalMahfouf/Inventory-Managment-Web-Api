import React, { useState, useEffect } from 'react';
import { X, User, Building2, FileText } from 'lucide-react';
import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import ViewCustomer from './ViewCustomer';
import {
  getCustomerById,
  createCustomer,
  updateCustomer,
  getCustomerCategories,
} from '@/services/customers/customerService';
import { useToast } from '@/context/ToastContext';
import { getCustomerCategoriesNames } from '@/services/customers/customerCategoryService';

/**
 * AddUpdateCustomer Component
 *
 * A comprehensive multi-step dialog for creating and editing customers.
 * Features three tabs: Basic Info, Business, and Summary (view-only, update mode only).
 *
 * @param {Object} props - Component props
 * @param {boolean} props.isOpen - Controls dialog visibility
 * @param {function} props.onClose - Callback function when dialog is closed
 * @param {number} props.customerId - ID of the customer to edit (0 for new customer)
 * @param {function} props.onSuccess - Optional callback after successful save
 */
const AddUpdateCustomer = ({ isOpen, onClose, customerId = 0, onSuccess }) => {
  const { showSuccess, showError } = useToast();
  const [activeTab, setActiveTab] = useState('basic');
  const [mode, setMode] = useState('add');
  const [id, setId] = useState(customerId);
  const [isLoading, setIsLoading] = useState(false);
  const [isFetchingData, setIsFetchingData] = useState(false);
  const [customerCategories, setCustomerCategories] = useState([]);
  const [errors, setErrors] = useState({});

  // Form state for all fields
  const [formData, setFormData] = useState({
    // Basic Info - Personal Details
    name: '',
    email: '',
    phone: '',
    customerCategoryId: '',
    // Basic Info - Address
    street: '',
    city: '',
    state: '',
    zipCode: '',
    // Business Details
    creditLimit: '5000.00',
    creditStatus: 0, // 0: Active, 1: On Hold, 2: Suspended
    paymentTerms: 'Net 30',
  });

  // Full customer data for view (Summary tab)
  const [customerData, setCustomerData] = useState(null);

  const tabs = [
    { id: 'basic', label: 'Basic Info', icon: User },
    { id: 'business', label: 'Business', icon: Building2 },
    ...(mode === 'update'
      ? [{ id: 'summary', label: 'Summary', icon: FileText }]
      : []),
  ];

  const handleInputChange = (field, value) => {
    setFormData(prev => ({
      ...prev,
      [field]: value,
    }));
    // Clear error for this field
    if (errors[field]) {
      setErrors(prev => ({
        ...prev,
        [field]: '',
      }));
    }
  };

  const validateBasicInfo = () => {
    const newErrors = {};
    if (!formData.name.trim()) {
      newErrors.name = 'Full name is required';
    }
    if (!formData.email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'Invalid email format';
    }
    if (!formData.phone.trim()) {
      newErrors.phone = 'Phone number is required';
    }
    if (!formData.customerCategoryId) {
      newErrors.customerCategoryId = 'Customer type is required';
    }
    if (!formData.street.trim()) {
      newErrors.street = 'Street address is required';
    }
    if (!formData.city.trim()) {
      newErrors.city = 'City is required';
    }
    if (!formData.state.trim()) {
      newErrors.state = 'State is required';
    }
    if (!formData.zipCode.trim()) {
      newErrors.zipCode = 'Zip code is required';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const validateBusinessInfo = () => {
    const newErrors = {};
    const creditLimit = parseFloat(formData.creditLimit);
    if (isNaN(creditLimit) || creditLimit < 0) {
      newErrors.creditLimit = 'Credit limit must be a positive number';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const saveCustomer = async () => {
    // Validate all fields
    const isBasicValid = validateBasicInfo();
    const isBusinessValid = validateBusinessInfo();

    if (!isBasicValid || !isBusinessValid) {
      showError(
        'Validation Error',
        'Please fill in all required fields correctly.'
      );
      setActiveTab('basic'); // Go back to first tab with errors
      return;
    }

    setIsLoading(true);

    const customerPayload = {
      name: formData.name.trim(),
      email: formData.email.trim(),
      phone: formData.phone.trim(),
      customerCategoryId: parseInt(formData.customerCategoryId),
      street: formData.street.trim(),
      city: formData.city.trim(),
      state: formData.state.trim(),
      zipCode: formData.zipCode.trim(),
      creditLimit: parseFloat(formData.creditLimit),
      creditStatus: parseInt(formData.creditStatus),
      paymentTerms: formData.paymentTerms.trim(),
    };

    let response;
    if (mode === 'add') {
      response = await createCustomer(customerPayload);
      if (response.success) {
        showSuccess(
          'Customer Created',
          `${formData.name} has been added successfully.`
        );
        setId(response.data.id);
        setMode('update');
        if (onSuccess) {
          onSuccess();
        }
        onClose();
      } else {
        showError(
          'Customer Creation Failed',
          response.message || 'Could not create customer.'
        );
      }
    } else if (mode === 'update') {
      response = await updateCustomer(id, customerPayload);
      if (response.success) {
        showSuccess(
          'Customer Updated',
          `Customer with ID ${customerId} has been updated successfully.`
        );
        if (onSuccess) {
          onSuccess();
        }
        onClose();
      } else {
        showError(
          'Customer Update Failed',
          response.message || 'Could not update customer.'
        );
      }
    }
    setIsLoading(false);
  };

  const handleSubmit = e => {
    e.preventDefault();
    saveCustomer();
  };

  const handleCancel = () => {
    setFormData({
      name: '',
      email: '',
      phone: '',
      customerCategoryId: '',
      street: '',
      city: '',
      state: '',
      zipCode: '',
      creditLimit: '5000.00',
      creditStatus: 0,
      paymentTerms: 'Net 30',
    });
    setErrors({});
    setMode('add');
    setId(0);
    setActiveTab('basic');
    setCustomerData(null);
    if (onClose) {
      onClose();
    }
  };

  const handleNextTab = () => {
    if (activeTab === 'basic') {
      if (validateBasicInfo()) {
        setActiveTab('business');
      }
    } else if (activeTab === 'business') {
      if (validateBusinessInfo()) {
        if (mode === 'update') {
          setActiveTab('summary');
        } else {
          saveCustomer(); // Save if in add mode
        }
      }
    }
  };

  const handlePreviousTab = () => {
    if (activeTab === 'business') {
      setActiveTab('basic');
    } else if (activeTab === 'summary') {
      setActiveTab('business');
    }
  };

  // Fetch customer categories
  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await getCustomerCategoriesNames();
        if (response.success) {
          console.log('customer categories: ', response.data);
          setCustomerCategories(response.data);
        }
      } catch (error) {
        console.error('Error fetching customer categories:', error);
      }
    };

    if (isOpen) {
      fetchCategories();
    }
  }, [isOpen]);

  // Fetch customer data for edit mode
  useEffect(() => {
    const fetchCustomer = async () => {
      if (!isOpen || id <= 0) return;

      setIsFetchingData(true);
      const response = await getCustomerById(id);
      if (response.success && response.data) {
        const data = response.data;
        setFormData({
          name: data.name || '',
          email: data.email || '',
          phone: data.phone || '',
          customerCategoryId: data.customerCategoryId || '',
          street: data.address?.street || data.street || '',
          city: data.address?.city || data.city || '',
          state: data.address?.state || data.state || '',
          zipCode: data.address?.zipCode || data.zipCode || '',
          creditLimit: data.creditLimit?.toString() || '5000.00',
          creditStatus: data.creditStatus || 0,
          paymentTerms: data.paymentTerms || 'Net 30',
        });
        setCustomerData(data);
        setMode('update');
      } else {
        showError('Failed to Load', 'Could not load customer data.');
      }
      setIsFetchingData(false);
    };

    fetchCustomer();
  }, [id, isOpen, showError]);

  // Reset when customerId prop changes
  useEffect(() => {
    setId(customerId);
    if (customerId === 0) {
      setMode('add');
      setActiveTab('basic');
      setCustomerData(null);
      setFormData({
        name: '',
        email: '',
        phone: '',
        customerCategoryId: '',
        street: '',
        city: '',
        state: '',
        zipCode: '',
        creditLimit: '5000.00',
        creditStatus: 0,
        paymentTerms: 'Net 30',
      });
    }
  }, [customerId]);

  if (!isOpen) return;

  return (
    <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4'>
      <div className='bg-white rounded-lg w-full max-w-3xl max-h-[90vh] overflow-hidden flex flex-col'>
        {/* Header */}
        <div className='flex items-center justify-between p-6 border-b flex-shrink-0'>
          <div>
            <h2 className='text-2xl font-bold text-gray-900'>
              {mode === 'add' ? 'Add New Customer' : 'Update Customer'}
            </h2>
            <p className='text-sm text-gray-500 mt-1'>
              {mode === 'add'
                ? 'Fill in the details below to add a new customer to the system.'
                : "Review the customer's information below."}
            </p>
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
        <div className='border-b flex-shrink-0'>
          <div className='flex px-6'>
            {tabs.map(tab => {
              const Icon = tab.icon;
              return (
                <button
                  key={tab.id}
                  onClick={() => setActiveTab(tab.id)}
                  className={`flex items-center gap-2 px-4 py-3 font-medium text-sm border-b-2 transition-colors ${
                    activeTab === tab.id
                      ? 'border-blue-600 text-blue-600'
                      : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                  }`}
                  type='button'
                >
                  <Icon className='h-4 w-4' />
                  {tab.label}
                </button>
              );
            })}
          </div>
        </div>

        {/* Content */}
        <form
          onSubmit={handleSubmit}
          className='flex flex-col flex-1 overflow-hidden'
        >
          <div className='p-6 overflow-y-auto flex-1'>
            {isFetchingData ? (
              <div className='flex items-center justify-center py-12'>
                <div className='animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600'></div>
              </div>
            ) : (
              <>
                {/* Basic Info Tab */}
                {activeTab === 'basic' && (
                  <div className='space-y-6'>
                    {/* Personal Details Section */}
                    <div>
                      <h3 className='text-lg font-semibold text-gray-900 mb-4'>
                        Personal Details
                      </h3>
                      <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                        <div>
                          <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                            Full name <span className='text-red-500'>*</span>
                          </label>
                          <Input
                            placeholder="Enter customer's full name"
                            value={formData.name}
                            onChange={e =>
                              handleInputChange('name', e.target.value)
                            }
                            className={`h-11 ${errors.name ? 'border-red-500' : ''}`}
                            disabled={isLoading}
                          />
                          {errors.name && (
                            <p className='text-red-500 text-sm mt-1'>
                              {errors.name}
                            </p>
                          )}
                        </div>

                        <div>
                          <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                            Email <span className='text-red-500'>*</span>
                          </label>
                          <Input
                            type='email'
                            placeholder='Enter email address'
                            value={formData.email}
                            onChange={e =>
                              handleInputChange('email', e.target.value)
                            }
                            className={`h-11 ${errors.email ? 'border-red-500' : ''}`}
                            disabled={isLoading}
                          />
                          {errors.email && (
                            <p className='text-red-500 text-sm mt-1'>
                              {errors.email}
                            </p>
                          )}
                        </div>

                        <div>
                          <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                            Phone <span className='text-red-500'>*</span>
                          </label>
                          <Input
                            type='tel'
                            placeholder='Enter phone number'
                            value={formData.phone}
                            onChange={e =>
                              handleInputChange('phone', e.target.value)
                            }
                            className={`h-11 ${errors.phone ? 'border-red-500' : ''}`}
                            disabled={isLoading}
                          />
                          {errors.phone && (
                            <p className='text-red-500 text-sm mt-1'>
                              {errors.phone}
                            </p>
                          )}
                        </div>

                        <div>
                          <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                            Customer Type{' '}
                            <span className='text-red-500'>*</span>
                          </label>
                          <select
                            value={formData.customerCategoryId}
                            onChange={e =>
                              handleInputChange(
                                'customerCategoryId',
                                e.target.value
                              )
                            }
                            className={`w-full h-11 px-3 border rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-600 ${
                              errors.customerCategoryId
                                ? 'border-red-500'
                                : 'border-gray-300'
                            }`}
                            disabled={isLoading}
                          >
                            <option value=''>Select customer type</option>
                            {customerCategories.map(category => (
                              <option key={category.id} value={category.id}>
                                {category.name}
                              </option>
                            ))}
                          </select>
                          {errors.customerCategoryId && (
                            <p className='text-red-500 text-sm mt-1'>
                              {errors.customerCategoryId}
                            </p>
                          )}
                        </div>
                      </div>
                    </div>

                    {/* Address Section */}
                    <div>
                      <h3 className='text-lg font-semibold text-gray-900 mb-4'>
                        Address
                      </h3>
                      <div className='space-y-4'>
                        <div>
                          <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                            Street <span className='text-red-500'>*</span>
                          </label>
                          <Input
                            placeholder='Enter street address'
                            value={formData.street}
                            onChange={e =>
                              handleInputChange('street', e.target.value)
                            }
                            className={`h-11 ${errors.street ? 'border-red-500' : ''}`}
                            disabled={isLoading}
                          />
                          {errors.street && (
                            <p className='text-red-500 text-sm mt-1'>
                              {errors.street}
                            </p>
                          )}
                        </div>

                        <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                          <div>
                            <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                              City <span className='text-red-500'>*</span>
                            </label>
                            <Input
                              placeholder='Enter city'
                              value={formData.city}
                              onChange={e =>
                                handleInputChange('city', e.target.value)
                              }
                              className={`h-11 ${errors.city ? 'border-red-500' : ''}`}
                              disabled={isLoading}
                            />
                            {errors.city && (
                              <p className='text-red-500 text-sm mt-1'>
                                {errors.city}
                              </p>
                            )}
                          </div>

                          <div>
                            <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                              State <span className='text-red-500'>*</span>
                            </label>
                            <Input
                              placeholder='Enter state'
                              value={formData.state}
                              onChange={e =>
                                handleInputChange('state', e.target.value)
                              }
                              className={`h-11 ${errors.state ? 'border-red-500' : ''}`}
                              disabled={isLoading}
                            />
                            {errors.state && (
                              <p className='text-red-500 text-sm mt-1'>
                                {errors.state}
                              </p>
                            )}
                          </div>
                        </div>

                        <div className='md:w-1/2 md:pr-2'>
                          <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                            Zip Code <span className='text-red-500'>*</span>
                          </label>
                          <Input
                            placeholder='Enter zip code'
                            value={formData.zipCode}
                            onChange={e =>
                              handleInputChange('zipCode', e.target.value)
                            }
                            className={`h-11 ${errors.zipCode ? 'border-red-500' : ''}`}
                            disabled={isLoading}
                          />
                          {errors.zipCode && (
                            <p className='text-red-500 text-sm mt-1'>
                              {errors.zipCode}
                            </p>
                          )}
                        </div>
                      </div>
                    </div>
                  </div>
                )}

                {/* Business Tab */}
                {activeTab === 'business' && (
                  <div className='space-y-6'>
                    <div>
                      <h3 className='text-lg font-semibold text-gray-900 mb-4'>
                        Business Details
                      </h3>
                      <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                        <div>
                          <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                            Credit Limit <span className='text-red-500'>*</span>
                          </label>
                          <div className='relative'>
                            <span className='absolute left-3 top-1/2 -translate-y-1/2 text-gray-500'>
                              $
                            </span>
                            <Input
                              type='number'
                              step='0.01'
                              placeholder='5000.00'
                              value={formData.creditLimit}
                              onChange={e =>
                                handleInputChange('creditLimit', e.target.value)
                              }
                              className={`h-11 pl-7 ${errors.creditLimit ? 'border-red-500' : ''}`}
                              disabled={isLoading}
                            />
                          </div>
                          {errors.creditLimit && (
                            <p className='text-red-500 text-sm mt-1'>
                              {errors.creditLimit}
                            </p>
                          )}
                        </div>

                        <div>
                          <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                            Credit Status
                          </label>
                          <select
                            value={formData.creditStatus}
                            onChange={e =>
                              handleInputChange('creditStatus', e.target.value)
                            }
                            className='w-full h-11 px-3 border border-gray-300 rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-600'
                            disabled={isLoading}
                          >
                            <option value={0}>Active</option>
                            <option value={1}>On Hold</option>
                            <option value={2}>Suspended</option>
                          </select>
                        </div>

                        <div className='md:col-span-2'>
                          <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                            Payment Terms
                          </label>
                          <Input
                            placeholder='e.g., Net 30'
                            value={formData.paymentTerms}
                            onChange={e =>
                              handleInputChange('paymentTerms', e.target.value)
                            }
                            className='h-11'
                            disabled={isLoading}
                          />
                        </div>
                      </div>
                    </div>
                  </div>
                )}

                {/* Summary Tab (Update mode only) */}
                {activeTab === 'summary' && mode === 'update' && (
                  <ViewCustomer
                    customer={customerData}
                    loading={isFetchingData}
                  />
                )}
              </>
            )}
          </div>

          {/* Footer */}
          <div className='flex items-center justify-end gap-3 p-6 border-t bg-gray-50 flex-shrink-0'>
            {activeTab !== 'basic' && (
              <Button
                type='button'
                variant='secondary'
                onClick={handlePreviousTab}
                disabled={isLoading}
                className='cursor-pointer'
              >
                Previous
              </Button>
            )}
            <Button
              type='button'
              variant='secondary'
              onClick={handleCancel}
              disabled={isLoading}
              className='cursor-pointer'
            >
              Cancel
            </Button>
            {activeTab === 'summary' ? (
              <Button
                type='button'
                onClick={handleCancel}
                disabled={isLoading}
                className='cursor-pointer'
              >
                Close
              </Button>
            ) : activeTab === 'business' && mode === 'add' ? (
              <Button
                type='button'
                onClick={handleNextTab}
                disabled={isLoading}
                loading={isLoading}
                className='cursor-pointer'
              >
                {isLoading ? 'Saving...' : 'Save Customer'}
              </Button>
            ) : activeTab === 'business' && mode === 'update' ? (
              <>
                <Button
                  type='button'
                  onClick={saveCustomer}
                  disabled={isLoading}
                  loading={isLoading}
                  className='cursor-pointer'
                >
                  {isLoading ? 'Saving...' : 'Save Changes'}
                </Button>
                <Button
                  type='button'
                  onClick={handleNextTab}
                  disabled={isLoading}
                  className='cursor-pointer'
                >
                  Next
                </Button>
              </>
            ) : (
              <Button
                type='button'
                onClick={handleNextTab}
                disabled={isLoading}
                className='cursor-pointer'
              >
                Next
              </Button>
            )}
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddUpdateCustomer;
