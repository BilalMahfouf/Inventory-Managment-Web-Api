import React, { useState, useEffect, useCallback } from 'react';
import { X, MapPin } from 'lucide-react';
import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import {
  createLocation,
  getLocationById,
  updateLocation,
} from '@/services/products/locationService';
import { getAllLocationTypes } from '@/services/products/locationTypeService';
import { useToast } from '@/context/ToastContext';

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
  const [locationTypes, setLocationTypes] = useState([]);
  const [loadingLocationTypes, setLoadingLocationTypes] = useState(false);

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
      newErrors.name = 'Location name is required';
    }
    if (!formData.address.trim()) {
      newErrors.address = 'Address is required';
    }
    if (!formData.locationTypeId || formData.locationTypeId === 0) {
      newErrors.locationTypeId = 'Location type is required';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const fetchLocationTypes = useCallback(async () => {
    setLoadingLocationTypes(true);
    try {
      const response = await getAllLocationTypes();
      if (response.success) {
        setLocationTypes(response.data);
      } else {
        showError('Failed to Load', 'Could not load location types.');
      }
    } catch (error) {
      console.error('Error fetching location types:', error);
      showError('Error', 'Failed to fetch location types.');
    } finally {
      setLoadingLocationTypes(false);
    }
  }, [showError]);

  const addNewLocation = async ({ name, address, locationTypeId }) => {
    setIsLoading(true);
    console.log('Creating location with:', { name, address, locationTypeId });
    const response = await createLocation({
      name,
      address,
      locationTypeId,
    });
    if (response.success) {
      showSuccess('Location Created', `${name} has been added successfully.`);
      setId(response.data.id);
      setMode('update');
      if (onSuccess) {
        onSuccess();
      }
      resetForm();
      onClose();
    } else {
      showError(
        'Location Creation Failed',
        `${name} could not be created. Error: ${response.message}`
      );
    }
    setIsLoading(false);
  };

  const editLocation = async (
    id,
    { name, address, locationTypeId, isActive }
  ) => {
    setIsLoading(true);
    const response = await updateLocation(id, {
      name,
      address,
      locationTypeId,
      isActive,
    });

    if (response.success) {
      showSuccess('Location Updated', `${name} has been updated successfully.`);
      setFormData({
        name: response.data.name,
        address: response.data.address,
        locationTypeId: response.data.locationTypeId,
        isActive: response.data.isActive,
      });
      if (onSuccess) {
        onSuccess();
      }
      onClose();
    } else {
      showError(
        'Location Update Failed',
        `Location could not be updated. Error: ${response.message}`
      );
    }
    setIsLoading(false);
  };

  const saveLocation = () => {
    if (!validateForm()) {
      return;
    }

    if (mode === 'add') {
      addNewLocation({
        name: formData.name,
        address: formData.address,
        locationTypeId: formData.locationTypeId,
      });
    } else if (mode === 'update') {
      editLocation(id, {
        name: formData.name,
        address: formData.address,
        locationTypeId: formData.locationTypeId,
        isActive: formData.isActive,
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
    if (isOpen) {
      fetchLocationTypes();
    }
  }, [isOpen, fetchLocationTypes]);

  useEffect(() => {
    if (isOpen && id > 0) {
      const fetchLocation = async id => {
        try {
          setIsLoading(true);
          const response = await getLocationById(id);
          if (response.success && response.data) {
            setFormData({
              name: response.data.name,
              address: response.data.address,
              locationTypeId: response.data.locationTypeId,
              isActive: response.data.isActive,
            });
            setMode('update');
          } else {
            showError(
              'Failed to Load Location',
              'Could not load location data.'
            );
          }
        } catch (error) {
          console.error('Error fetching location:', error);
          showError('Failed to Load Location', 'Could not load location data.');
        } finally {
          setIsLoading(false);
        }
      };
      fetchLocation(id);
    } else if (isOpen && id === 0) {
      setMode('add');
      setFormData({
        name: '',
        address: '',
        locationTypeId: 0,
        isActive: true,
      });
    }
  }, [id, isOpen, showError]);

  useEffect(() => {
    setId(locationId);
  }, [locationId]);

  if (!isOpen) return null;

  return (
    <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4'>
      <div className='bg-white rounded-lg w-full max-w-2xl overflow-hidden'>
        {/* Header */}
        <div className='flex items-center justify-between p-6 border-b'>
          <div className='flex items-center gap-3'>
            <MapPin className='h-6 w-6 text-blue-600' />
            <h2 className='text-xl font-semibold text-gray-900'>
              {mode === 'add' ? 'Add Location' : 'Edit Location'}
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
                Location Information
              </h3>
            </div>

            <div className='space-y-6'>
              {/* Location Name */}
              <div>
                <label className='block text-sm font-medium text-gray-700 mb-2'>
                  Location Name <span className='text-red-500'>*</span>
                </label>
                <Input
                  placeholder='e.g., Main Warehouse, Store A, Distribution Center'
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
                  Address <span className='text-red-500'>*</span>
                </label>
                <textarea
                  placeholder='Enter the full address of this location...'
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
                  Location Type <span className='text-red-500'>*</span>
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
                      ? 'Loading...'
                      : 'Select a location type'}
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
                        Location Status
                      </label>
                      <p className='text-sm text-gray-600'>
                        {formData.isActive
                          ? 'This location is currently active and available for use'
                          : 'This location is currently inactive'}
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
              Cancel
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
                  ? 'Creating...'
                  : 'Saving...'
                : mode === 'add'
                  ? 'Create Location'
                  : 'Save Changes'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddUpdateLocation;
