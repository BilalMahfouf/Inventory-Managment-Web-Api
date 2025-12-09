import React, { useState, useEffect } from 'react';
import { X, Tag } from 'lucide-react';
import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import {
  createUnitOfMeasure,
  getUnitOfMeasureById,
  updateUnitOfMeasure,
} from '@/services/products/UnitOfMeasureService';
import { useToast } from '@/context/ToastContext';

/**
 * AddUnitOfMeasure Component
 *
 * A modal component for adding or editing units of measure in the inventory system.
 * Supports both add and update modes.
 *
 * @param {Object} props - Component props
 * @param {boolean} props.isOpen - Controls modal visibility
 * @param {function} props.onClose - Callback function when modal is closed
 * @param {number} props.unitId - ID of the unit to edit (0 for new unit)
 * @param {function} props.onSuccess - Optional callback after successful save
 */
const AddUnitOfMeasure = ({ isOpen, onClose, unitId = 0, onSuccess }) => {
  const { showSuccess, showError } = useToast();
  const [formData, setFormData] = useState({
    name: '',
    description: '',
  });
  const [mode, setMode] = useState('add'); // 'add' or 'update'
  const [id, setId] = useState(unitId);
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState({});

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
      newErrors.name = 'Unit name is required';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const addNewUnit = async ({ name, description }) => {
    setIsLoading(true);
    const response = await createUnitOfMeasure({
      name,
      description,
    });
    if (response.success) {
      showSuccess(
        'Unit of Measure Created',
        `${name} has been added successfully.`
      );
      setId(response.data.id);
      setMode('update');
      if (onSuccess) {
        onSuccess();
      }
      onClose();
    } else if (!response.success) {
      showError(
        'Unit Creation Failed',
        `${name} could not be created. error: ${response.message}`
      );
    }
    setIsLoading(false);
  };

  const editUnit = async (id, { name, description }) => {
    setIsLoading(true);
    const response = await updateUnitOfMeasure(id, {
      name,
      description,
    });

    if (response.success) {
      showSuccess(
        'Unit of Measure Updated',
        `${name} has been updated successfully.`
      );
      setFormData({
        name: response.data.name,
        description: response.data.description || '',
      });
      if (onSuccess) {
        onSuccess();
      }
      onClose();
    } else {
      showError(
        'Unit Update Failed',
        `Unit could not be updated. Error: ${response.message}`
      );
    }
    setIsLoading(false);
  };

  const saveUnit = () => {
    if (!validateForm()) {
      return;
    }

    if (mode === 'add') {
      addNewUnit({
        name: formData.name,
        description: formData.description,
      });
    } else if (mode === 'update') {
      editUnit(id, {
        name: formData.name,
        description: formData.description,
      });
    }
  };

  const handleSubmit = e => {
    e.preventDefault();
    saveUnit();
  };

  const handleCancel = () => {
    setFormData({
      name: '',
      description: '',
    });
    setErrors({});
    setMode('add');
    setId(0);
    if (onClose) {
      onClose();
    }
  };

  useEffect(() => {
    if (isOpen && id > 0) {
      const fetchUnit = async id => {
        try {
          setIsLoading(true);
          const data = await getUnitOfMeasureById(id);
          if (data) {
            setFormData({
              name: data.name,
              description: data.description || '',
            });
            setMode('update');
          }
        } catch (error) {
          console.error('Error fetching unit:', error);
          showError(
            'Failed to Load Unit',
            'Could not load unit of measure data.'
          );
        } finally {
          setIsLoading(false);
        }
      };
      fetchUnit(id);
    } else if (isOpen && id === 0) {
      setMode('add');
      setFormData({
        name: '',
        description: '',
      });
    }
  }, [id, isOpen, showError]);

  useEffect(() => {
    setId(unitId);
  }, [unitId]);

  if (!isOpen) return null;

  return (
    <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4'>
      <div className='bg-white rounded-lg w-full max-w-2xl overflow-hidden'>
        {/* Header */}
        <div className='flex items-center justify-between p-6 border-b'>
          <div className='flex items-center gap-3'>
            <Tag className='h-6 w-6 text-gray-600' />
            <h2 className='text-xl font-semibold'>
              {mode === 'add' ? 'Add Unit of Measure' : 'Edit Unit of Measure'}
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

        {/* Content */}
        <form onSubmit={handleSubmit}>
          <div className='p-6'>
            <div className='flex items-center gap-2 mb-6'>
              <Tag className='h-5 w-5' />
              <h3 className='text-lg font-semibold'>Unit Information</h3>
            </div>

            <div className='space-y-6'>
              <div>
                <label className='block text-sm font-medium mb-2'>
                  Unit Name <span className='text-red-500'>*</span>
                </label>
                <Input
                  placeholder='e.g., Pieces, Kilograms, Meters, Liters'
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

              <div>
                <label className='block text-sm font-medium mb-2'>
                  Description
                </label>
                <textarea
                  placeholder='Optional description for this unit of measure...'
                  value={formData.description}
                  onChange={e =>
                    handleInputChange('description', e.target.value)
                  }
                  className='w-full h-32 px-3 py-2 border border-gray-300 rounded-md resize-none focus:outline-none focus:ring-1 focus:ring-blue-600'
                  disabled={isLoading}
                />
              </div>
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
              disabled={!formData.name.trim() || isLoading}
              className='cursor-pointer'
            >
              {isLoading
                ? mode === 'add'
                  ? 'Creating...'
                  : 'Saving...'
                : mode === 'add'
                  ? 'Create Unit'
                  : 'Save Changes'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddUnitOfMeasure;
