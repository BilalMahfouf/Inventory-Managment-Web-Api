import React, { useState, useEffect } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { X, MapPin } from 'lucide-react';
import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import {
  createLocation,
  getLocationById,
  updateLocation,
} from '@features/inventory/services/locationApi';
import { getAllLocationTypes } from '@features/inventory/services/locationTypeApi';
import { useToast } from '@shared/context/ToastContext';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';

/**
 * AddUpdateLocation Component
 *
 * A modal component for adding or editing locations in the inventory system.
 * Supports both add and update modes.
 *
 * @param {Object} props - Component props
 * @param {boolean} props.isOpen - Controls modal visibility
 * @param {function} props.onClose - Callback function when modal is closed
 * @param {number} props.locationId - ID of the location to edit (0 for new location)
 * @param {function} props.onSuccess - Optional callback after successful save
 */
const AddUpdateLocation = ({ isOpen, onClose, locationId = 0, onSuccess }) => {
  const { t } = useTranslation();
  const { showSuccess, showError } = useToast();
  const [formData, setFormData] = useState({
    name: '',
    address: '',
    locationTypeId: 0,
    isActive: true,
  });
  const [mode, setMode] = useState('add'); // 'add' or 'update'
  const [id, setId] = useState(locationId);
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState({});
  const queryClient = useQueryClient();

  const { data: locationTypesResponse, isLoading: loadingLocationTypes } =
    useQuery({
      queryKey: [...queryKeys.inventory.locations('list'), 'types'],
      queryFn: getAllLocationTypes,
      enabled: isOpen,
    });

  const locationTypes = locationTypesResponse?.success
    ? locationTypesResponse.data
    : [];

  const { data: locationResponse } = useQuery({
    queryKey: [...queryKeys.inventory.locations('list'), 'detail', id],
    queryFn: () => getLocationById(id),
    enabled: isOpen && id > 0,
  });

  const createMutation = useMutation({
    mutationFn: createLocation,
    onSuccess: async response => {
      if (response.success) {
        showSuccess(
          t(i18nKeyContainer.inventory.locations.form.toasts.createSuccessTitle),
          t(i18nKeyContainer.inventory.locations.form.toasts.createSuccessMessage, {
            name: response.data.name,
          })
        );
        setId(response.data.id);
        setMode('update');
        await queryClient.invalidateQueries({
          queryKey: queryKeys.inventory.locations('list'),
        });
        await queryClient.invalidateQueries({ queryKey: queryKeys.inventory.summary() });
        if (onSuccess) {
          onSuccess();
        }
        resetForm();
        onClose();
        return;
      }

      showError(
        t(i18nKeyContainer.inventory.locations.form.toasts.createFailedTitle),
        t(i18nKeyContainer.inventory.locations.form.toasts.createFailedMessage, {
          name: formData.name,
          error: response.message,
        })
      );
    },
  });

  const updateMutation = useMutation({
    mutationFn: payload => updateLocation(payload.id, payload.data),
    onSuccess: async response => {
      if (response.success) {
        showSuccess(
          t(i18nKeyContainer.inventory.locations.form.toasts.updateSuccessTitle),
          t(i18nKeyContainer.inventory.locations.form.toasts.updateSuccessMessage, {
            name: response.data.name,
          })
        );
        setFormData({
          name: response.data.name,
          address: response.data.address,
          locationTypeId: response.data.locationTypeId,
          isActive: response.data.isActive,
        });
        await queryClient.invalidateQueries({
          queryKey: queryKeys.inventory.locations('list'),
        });
        await queryClient.invalidateQueries({ queryKey: queryKeys.inventory.summary() });
        if (onSuccess) {
          onSuccess();
        }
        onClose();
        return;
      }

      showError(
        t(i18nKeyContainer.inventory.locations.form.toasts.updateFailedTitle),
        t(i18nKeyContainer.inventory.locations.form.toasts.updateFailedMessage, {
          error: response.message,
        })
      );
    },
  });

  const handleInputChange = (field, value) => {
    setFormData(prev => ({
      ...prev,
      [field]: value,
    }));
    // Clear error for this field when user starts typing
    if (errors[field]) {
      setErrors(prev => ({
        ...prev,
        [field]: '',
      }));
    }
  };

  const validateForm = () => {
    const newErrors = {};
    if (!formData.name.trim()) {
      newErrors.name = t(
        i18nKeyContainer.inventory.locations.form.validation.locationNameRequired
      );
    }
    if (!formData.address.trim()) {
      newErrors.address = t(
        i18nKeyContainer.inventory.locations.form.validation.addressRequired
      );
    }
    if (!formData.locationTypeId || formData.locationTypeId === 0) {
      newErrors.locationTypeId = t(
        i18nKeyContainer.inventory.locations.form.validation.locationTypeRequired
      );
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const saveLocation = () => {
    if (!validateForm()) {
      return;
    }

    if (mode === 'add') {
      createMutation.mutate({
        name: formData.name,
        address: formData.address,
        locationTypeId: formData.locationTypeId,
      });
    } else if (mode === 'update') {
      updateMutation.mutate({
        id,
        data: {
          name: formData.name,
          address: formData.address,
          locationTypeId: formData.locationTypeId,
          isActive: formData.isActive,
        },
      });
    }
  };

  const handleSubmit = e => {
    e.preventDefault();
    saveLocation();
    resetForm();
  };

  const resetForm = () => {
    setFormData({
      name: '',
      address: '',
      locationTypeId: 0,
      isActive: true,
    });
    setErrors({});
    setMode('add');
    setId(0);
    console.log('Form reset');
  };
  const handleCancel = () => {
    resetForm();
    if (onClose) {
      onClose();
    }
  };

  useEffect(() => {
    if (isOpen && id === 0) {
      setMode('add');
      setFormData({
        name: '',
        address: '',
        locationTypeId: 0,
        isActive: true,
      });
    }
  }, [id, isOpen]);

  useEffect(() => {
    if (locationResponse?.success && locationResponse?.data) {
      setFormData({
        name: locationResponse.data.name,
        address: locationResponse.data.address,
        locationTypeId: locationResponse.data.locationTypeId,
        isActive: locationResponse.data.isActive,
      });
      setMode('update');
    }
  }, [locationResponse]);

  useEffect(() => {
    setId(locationId);
  }, [locationId]);

  useEffect(() => {
    setIsLoading(createMutation.isPending || updateMutation.isPending);
  }, [createMutation.isPending, updateMutation.isPending]);

  if (!isOpen) return null;

  return (
    <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4'>
      <div className='bg-white rounded-lg w-full max-w-2xl overflow-hidden'>
        {/* Header */}
        <div className='flex items-center justify-between p-6 border-b'>
          <div className='flex items-center gap-3'>
            <MapPin className='h-6 w-6 text-blue-600' />
            <h2 className='text-xl font-semibold text-gray-900'>
              {mode === 'add'
                ? t(i18nKeyContainer.inventory.locations.form.title.add)
                : t(i18nKeyContainer.inventory.locations.form.title.edit)}
            </h2>
          </div>
          <button
            onClick={handleCancel}
            className='p-2 hover:bg-gray-100 rounded-lg transition-colors'
            disabled={isLoading}
          >
            <X className='h-5 w-5 text-gray-600' />
          </button>
        </div>

        {/* Content */}
        <form onSubmit={handleSubmit}>
          <div className='p-6'>
            <div className='flex items-center gap-2 mb-6'>
              <MapPin className='h-5 w-5 text-blue-600' />
              <h3 className='text-lg font-semibold text-gray-900'>
                {t(i18nKeyContainer.inventory.locations.form.sections.locationInformation)}
              </h3>
            </div>

            <div className='space-y-6'>
              {/* Location Name */}
              <div>
                <label className='block text-sm font-medium text-gray-700 mb-2'>
                  {t(i18nKeyContainer.inventory.locations.form.fields.locationName)}{' '}
                  <span className='text-red-500'>
                    {t(i18nKeyContainer.inventory.shared.required)}
                  </span>
                </label>
                <Input
                  placeholder={t(
                    i18nKeyContainer.inventory.locations.form.placeholders
                      .locationName
                  )}
                  value={formData.name}
                  onChange={e => handleInputChange('name', e.target.value)}
                  className={`h-12 ${errors.name ? 'border-red-500' : ''}`}
                  disabled={isLoading}
                  autoFocus
                />
                {errors.name && (
                  <p className='text-red-500 text-sm mt-1'>{errors.name}</p>
                )}
              </div>

              {/* Address */}
              <div>
                <label className='block text-sm font-medium text-gray-700 mb-2'>
                  {t(i18nKeyContainer.inventory.locations.form.fields.address)}{' '}
                  <span className='text-red-500'>
                    {t(i18nKeyContainer.inventory.shared.required)}
                  </span>
                </label>
                <textarea
                  placeholder={t(
                    i18nKeyContainer.inventory.locations.form.placeholders.address
                  )}
                  value={formData.address}
                  onChange={e => handleInputChange('address', e.target.value)}
                  className={`w-full h-24 px-3 py-2 border rounded-md resize-none focus:outline-none focus:ring-1 focus:ring-blue-600 ${
                    errors.address ? 'border-red-500' : 'border-gray-300'
                  }`}
                  disabled={isLoading}
                />
                {errors.address && (
                  <p className='text-red-500 text-sm mt-1'>{errors.address}</p>
                )}
              </div>

              {/* Location Type */}
              <div>
                <label className='block text-sm font-medium text-gray-700 mb-2'>
                  {t(i18nKeyContainer.inventory.locations.form.fields.locationType)}{' '}
                  <span className='text-red-500'>
                    {t(i18nKeyContainer.inventory.shared.required)}
                  </span>
                </label>
                <select
                  value={formData.locationTypeId}
                  onChange={e =>
                    handleInputChange(
                      'locationTypeId',
                      parseInt(e.target.value)
                    )
                  }
                  className={`w-full h-12 px-3 border rounded-md focus:outline-none focus:ring-1 focus:ring-blue-600 ${
                    errors.locationTypeId ? 'border-red-500' : 'border-gray-300'
                  }`}
                  disabled={isLoading || loadingLocationTypes}
                >
                  <option value={0}>
                    {loadingLocationTypes
                      ? t(i18nKeyContainer.inventory.shared.loading)
                      : t(
                          i18nKeyContainer.inventory.locations.form.placeholders
                            .selectLocationType
                        )}
                  </option>
                  {locationTypes.map(type => (
                    <option key={type.id} value={type.id}>
                      {type.name}
                      {type.description ? ` - ${type.description}` : ''}
                    </option>
                  ))}
                </select>
                {errors.locationTypeId && (
                  <p className='text-red-500 text-sm mt-1'>
                    {errors.locationTypeId}
                  </p>
                )}
              </div>

              {/* Active Status (Only in Update Mode) */}
              {mode === 'update' && (
                <div className='bg-blue-50 border border-blue-200 rounded-lg p-4'>
                  <div className='flex items-center justify-between'>
                    <div>
                      <label className='block text-sm font-medium text-gray-700 mb-1'>
                        {t(i18nKeyContainer.inventory.locations.form.fields.locationStatus)}
                      </label>
                      <p className='text-sm text-gray-600'>
                        {formData.isActive
                          ? t(
                              i18nKeyContainer.inventory.locations.form.hints
                                .locationActiveHint
                            )
                          : t(
                              i18nKeyContainer.inventory.locations.form.hints
                                .locationInactiveHint
                            )}
                      </p>
                    </div>
                    <label className='relative inline-flex items-center cursor-pointer'>
                      <input
                        type='checkbox'
                        checked={formData.isActive}
                        onChange={e =>
                          handleInputChange('isActive', e.target.checked)
                        }
                        className='sr-only peer'
                        disabled={isLoading}
                      />
                      <div className="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-2 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                    </label>
                  </div>
                </div>
              )}
            </div>
          </div>

          {/* Footer */}
          <div className='flex items-center justify-end gap-3 p-6 border-t bg-gray-50'>
            <Button
              type='button'
              variant='secondary'
              onClick={handleCancel}
              disabled={isLoading}
              className='cursor-pointer'
            >
              {t(i18nKeyContainer.inventory.locations.form.actions.cancel)}
            </Button>
            <Button
              type='submit'
              disabled={
                !formData.name.trim() ||
                !formData.address.trim() ||
                !formData.locationTypeId ||
                isLoading
              }
              className='cursor-pointer bg-blue-600 hover:bg-blue-700'
            >
              {isLoading
                ? mode === 'add'
                  ? t(i18nKeyContainer.inventory.locations.form.actions.creating)
                  : t(i18nKeyContainer.inventory.locations.form.actions.saving)
                : mode === 'add'
                  ? t(i18nKeyContainer.inventory.locations.form.actions.create)
                  : t(i18nKeyContainer.inventory.locations.form.actions.saveChanges)}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddUpdateLocation;
