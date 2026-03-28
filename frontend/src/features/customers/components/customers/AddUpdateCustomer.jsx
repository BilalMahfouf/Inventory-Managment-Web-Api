import React, { useState, useEffect } from 'react';
import { X, User, Building2, FileText } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import ViewCustomer from './ViewCustomer';
import {
  getCustomerById,
  createCustomer,
  updateCustomer,
} from '@features/customers/services/customerApi';
import { useToast } from '@shared/context/ToastContext';
import { getCustomerCategoriesNames } from '@features/customers/services/customerCategoryApi';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

const getInitialFormData = defaultPaymentTerms => ({
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
  paymentTerms: defaultPaymentTerms,
});

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
  const { t } = useTranslation();
  const { showSuccess, showError } = useToast();
  const defaultPaymentTerms = t(
    i18nKeyContainer.customers.shared.defaults.paymentTerms
  );
  const [activeTab, setActiveTab] = useState('basic');
  const [mode, setMode] = useState('add');
  const [id, setId] = useState(customerId);
  const [isLoading, setIsLoading] = useState(false);
  const [isFetchingData, setIsFetchingData] = useState(false);
  const [customerCategories, setCustomerCategories] = useState([]);
  const [errors, setErrors] = useState({});

  // Form state for all fields
  const [formData, setFormData] = useState(() =>
    getInitialFormData(defaultPaymentTerms)
  );

  // Full customer data for view (Summary tab)
  const [customerData, setCustomerData] = useState(null);

  const tabs = [
    {
      id: 'basic',
      label: t(i18nKeyContainer.customers.form.tabs.basicInfo),
      icon: User,
    },
    {
      id: 'business',
      label: t(i18nKeyContainer.customers.form.tabs.business),
      icon: Building2,
    },
    ...(mode === 'update'
      ? [
          {
            id: 'summary',
            label: t(i18nKeyContainer.customers.form.tabs.summary),
            icon: FileText,
          },
        ]
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
      newErrors.name = t(i18nKeyContainer.customers.form.validation.fullNameRequired);
    }
    if (!formData.email.trim()) {
      newErrors.email = t(i18nKeyContainer.customers.form.validation.emailRequired);
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = t(i18nKeyContainer.customers.form.validation.invalidEmail);
    }
    if (!formData.phone.trim()) {
      newErrors.phone = t(i18nKeyContainer.customers.form.validation.phoneRequired);
    }
    if (!formData.customerCategoryId) {
      newErrors.customerCategoryId = t(
        i18nKeyContainer.customers.form.validation.customerTypeRequired
      );
    }
    if (!formData.street.trim()) {
      newErrors.street = t(i18nKeyContainer.customers.form.validation.streetRequired);
    }
    if (!formData.city.trim()) {
      newErrors.city = t(i18nKeyContainer.customers.form.validation.cityRequired);
    }
    if (!formData.state.trim()) {
      newErrors.state = t(i18nKeyContainer.customers.form.validation.stateRequired);
    }
    if (!formData.zipCode.trim()) {
      newErrors.zipCode = t(i18nKeyContainer.customers.form.validation.zipCodeRequired);
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const validateBusinessInfo = () => {
    const newErrors = {};
    const creditLimit = parseFloat(formData.creditLimit);
    if (isNaN(creditLimit) || creditLimit < 0) {
      newErrors.creditLimit = t(
        i18nKeyContainer.customers.form.validation.creditLimitPositive
      );
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
        t(i18nKeyContainer.customers.form.toasts.validationErrorTitle),
        t(i18nKeyContainer.customers.form.toasts.validationErrorMessage)
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
      paymentTerms: formData.paymentTerms.trim() || defaultPaymentTerms,
    };

    let response;
    if (mode === 'add') {
      response = await createCustomer(customerPayload);
      if (response.success) {
        showSuccess(
          t(i18nKeyContainer.customers.form.toasts.createSuccessTitle),
          t(i18nKeyContainer.customers.form.toasts.createSuccessMessage, {
            name: formData.name,
          })
        );
        setId(response.data.id);
        setMode('update');
        if (onSuccess) {
          onSuccess();
        }
        onClose();
      } else {
        showError(
          t(i18nKeyContainer.customers.form.toasts.createFailedTitle),
          response.message ||
            t(i18nKeyContainer.customers.form.toasts.createFailedMessage)
        );
      }
    } else if (mode === 'update') {
      response = await updateCustomer(id, customerPayload);
      if (response.success) {
        showSuccess(
          t(i18nKeyContainer.customers.form.toasts.updateSuccessTitle),
          t(i18nKeyContainer.customers.form.toasts.updateSuccessMessage, {
            id,
          })
        );
        if (onSuccess) {
          onSuccess();
        }
        onClose();
      } else {
        showError(
          t(i18nKeyContainer.customers.form.toasts.updateFailedTitle),
          response.message ||
            t(i18nKeyContainer.customers.form.toasts.updateFailedMessage)
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
    setFormData(getInitialFormData(defaultPaymentTerms));
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
          paymentTerms: data.paymentTerms || defaultPaymentTerms,
        });
        setCustomerData(data);
        setMode('update');
      } else {
        showError(
          t(i18nKeyContainer.customers.form.toasts.loadFailedTitle),
          t(i18nKeyContainer.customers.form.toasts.loadFailedMessage)
        );
      }
      setIsFetchingData(false);
    };

    fetchCustomer();
  }, [id, isOpen, showError, t, defaultPaymentTerms]);

  // Reset when customerId prop changes
  useEffect(() => {
    setId(customerId);
    if (customerId === 0) {
      setMode('add');
      setActiveTab('basic');
      setCustomerData(null);
      setFormData(getInitialFormData(defaultPaymentTerms));
    }
  }, [customerId, defaultPaymentTerms]);

  if (!isOpen) return null;

  return (
    <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4'>
      <div className='bg-white rounded-lg w-full max-w-3xl max-h-[90vh] overflow-hidden flex flex-col'>
        {/* Header */}
        <div className='flex items-center justify-between p-6 border-b flex-shrink-0'>
          <div>
            <h2 className='text-2xl font-bold text-gray-900'>
              {mode === 'add'
                ? t(i18nKeyContainer.customers.form.title.add)
                : t(i18nKeyContainer.customers.form.title.edit)}
            </h2>
            <p className='text-sm text-gray-500 mt-1'>
              {mode === 'add'
                ? t(i18nKeyContainer.customers.form.subtitle.add)
                : t(i18nKeyContainer.customers.form.subtitle.edit)}
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
                        {t(i18nKeyContainer.customers.form.sections.personalDetails)}
                      </h3>
                      <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                        <div>
                          <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                            {t(i18nKeyContainer.customers.form.fields.fullName)}{' '}
                            <span className='text-red-500'>
                              {t(i18nKeyContainer.customers.shared.required)}
                            </span>
                          </label>
                          <Input
                            placeholder={t(
                              i18nKeyContainer.customers.form.placeholders.fullName
                            )}
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
                            {t(i18nKeyContainer.customers.form.fields.email)}{' '}
                            <span className='text-red-500'>
                              {t(i18nKeyContainer.customers.shared.required)}
                            </span>
                          </label>
                          <Input
                            type='email'
                            placeholder={t(
                              i18nKeyContainer.customers.form.placeholders.email
                            )}
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
                            {t(i18nKeyContainer.customers.form.fields.phone)}{' '}
                            <span className='text-red-500'>
                              {t(i18nKeyContainer.customers.shared.required)}
                            </span>
                          </label>
                          <Input
                            type='tel'
                            placeholder={t(
                              i18nKeyContainer.customers.form.placeholders.phone
                            )}
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
                            {t(i18nKeyContainer.customers.form.fields.customerType)}{' '}
                            <span className='text-red-500'>
                              {t(i18nKeyContainer.customers.shared.required)}
                            </span>
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
                            <option value=''>
                              {t(i18nKeyContainer.customers.form.placeholders.customerType)}
                            </option>
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
                        {t(i18nKeyContainer.customers.form.sections.address)}
                      </h3>
                      <div className='space-y-4'>
                        <div>
                          <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                            {t(i18nKeyContainer.customers.form.fields.street)}{' '}
                            <span className='text-red-500'>
                              {t(i18nKeyContainer.customers.shared.required)}
                            </span>
                          </label>
                          <Input
                            placeholder={t(
                              i18nKeyContainer.customers.form.placeholders.street
                            )}
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
                              {t(i18nKeyContainer.customers.form.fields.city)}{' '}
                              <span className='text-red-500'>
                                {t(i18nKeyContainer.customers.shared.required)}
                              </span>
                            </label>
                            <Input
                              placeholder={t(
                                i18nKeyContainer.customers.form.placeholders.city
                              )}
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
                              {t(i18nKeyContainer.customers.form.fields.state)}{' '}
                              <span className='text-red-500'>
                                {t(i18nKeyContainer.customers.shared.required)}
                              </span>
                            </label>
                            <Input
                              placeholder={t(
                                i18nKeyContainer.customers.form.placeholders.state
                              )}
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
                            {t(i18nKeyContainer.customers.form.fields.zipCode)}{' '}
                            <span className='text-red-500'>
                              {t(i18nKeyContainer.customers.shared.required)}
                            </span>
                          </label>
                          <Input
                            placeholder={t(
                              i18nKeyContainer.customers.form.placeholders.zipCode
                            )}
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
                        {t(i18nKeyContainer.customers.form.sections.businessDetails)}
                      </h3>
                      <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
                        <div>
                          <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                            {t(i18nKeyContainer.customers.form.fields.creditLimit)}{' '}
                            <span className='text-red-500'>
                              {t(i18nKeyContainer.customers.shared.required)}
                            </span>
                          </label>
                          <div className='relative'>
                            <span className='absolute left-3 top-1/2 -translate-y-1/2 text-gray-500'>
                              {t(i18nKeyContainer.customers.shared.currencySymbol)}
                            </span>
                            <Input
                              type='number'
                              step='0.01'
                              placeholder={t(
                                i18nKeyContainer.customers.form.placeholders.creditLimit
                              )}
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
                            {t(i18nKeyContainer.customers.form.fields.creditStatus)}
                          </label>
                          <select
                            value={formData.creditStatus}
                            onChange={e =>
                              handleInputChange('creditStatus', e.target.value)
                            }
                            className='w-full h-11 px-3 border border-gray-300 rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-600'
                            disabled={isLoading}
                          >
                            <option value={0}>
                              {t(i18nKeyContainer.customers.shared.creditStatus.active)}
                            </option>
                            <option value={1}>
                              {t(i18nKeyContainer.customers.shared.creditStatus.onHold)}
                            </option>
                            <option value={2}>
                              {t(i18nKeyContainer.customers.shared.creditStatus.suspended)}
                            </option>
                          </select>
                        </div>

                        <div className='md:col-span-2'>
                          <label className='block text-sm font-medium text-gray-700 mb-1.5'>
                            {t(i18nKeyContainer.customers.form.fields.paymentTerms)}
                          </label>
                          <Input
                            placeholder={t(
                              i18nKeyContainer.customers.form.placeholders.paymentTerms
                            )}
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
                {t(i18nKeyContainer.customers.form.actions.previous)}
              </Button>
            )}
            <Button
              type='button'
              variant='secondary'
              onClick={handleCancel}
              disabled={isLoading}
              className='cursor-pointer'
            >
              {t(i18nKeyContainer.customers.form.actions.cancel)}
            </Button>
            {activeTab === 'summary' ? (
              <Button
                type='button'
                onClick={handleCancel}
                disabled={isLoading}
                className='cursor-pointer'
              >
                {t(i18nKeyContainer.customers.form.actions.close)}
              </Button>
            ) : activeTab === 'business' && mode === 'add' ? (
              <Button
                type='button'
                onClick={handleNextTab}
                disabled={isLoading}
                loading={isLoading}
                className='cursor-pointer'
              >
                {isLoading
                  ? t(i18nKeyContainer.customers.form.actions.saving)
                  : t(i18nKeyContainer.customers.form.actions.saveCustomer)}
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
                  {isLoading
                    ? t(i18nKeyContainer.customers.form.actions.saving)
                    : t(i18nKeyContainer.customers.form.actions.saveChanges)}
                </Button>
                <Button
                  type='button'
                  onClick={handleNextTab}
                  disabled={isLoading}
                  className='cursor-pointer'
                >
                  {t(i18nKeyContainer.customers.form.actions.next)}
                </Button>
              </>
            ) : (
              <Button
                type='button'
                onClick={handleNextTab}
                disabled={isLoading}
                className='cursor-pointer'
              >
                {t(i18nKeyContainer.customers.form.actions.next)}
              </Button>
            )}
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddUpdateCustomer;
