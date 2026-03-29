import React, { useState, useEffect } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { X, Tag } from 'lucide-react';
import { useTranslation } from 'react-i18next';

import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import {
  createUnitOfMeasure,
  getUnitOfMeasureById,
  updateUnitOfMeasure,
} from '@features/products/services/unitOfMeasureApi';
import { useToast } from '@shared/context/ToastContext';
import { queryKeys } from '@shared/lib/queryKeys';

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
  const { t } = useTranslation();
  const { showSuccess, showError } = useToast();
  const [formData, setFormData] = useState({
    name: '',
    description: '',
  });
  const [mode, setMode] = useState('add'); // 'add' or 'update'
  const [id, setId] = useState(unitId);
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState({});
  const queryClient = useQueryClient();

  const { data: unitData } = useQuery({
    queryKey: [...queryKeys.products.unitOfMeasure(), 'detail', id],
    queryFn: () => getUnitOfMeasureById(id),
    enabled: isOpen && id > 0,
  });

  const createMutation = useMutation({
    mutationFn: createUnitOfMeasure,
    onSuccess: async response => {
      showSuccess(
        t(i18nKeyContainer.products.units.form.toasts.createSuccessTitle),
        t(i18nKeyContainer.products.units.form.toasts.createSuccessMessage, {
          name: response.data.name,
        })
      );
      setId(response.data.id);
      setMode('update');
      await queryClient.invalidateQueries({
        queryKey: queryKeys.products.unitOfMeasure(),
      });
      if (onSuccess) {
        onSuccess();
      }
      onClose();
    },
    onError: error => {
      showError(
        t(i18nKeyContainer.products.units.form.toasts.createErrorTitle),
        error?.message ||
          t(i18nKeyContainer.products.units.form.toasts.createErrorMessage, {
            name: formData.name,
            error: t(i18nKeyContainer.products.shared.notSpecified),
          })
      );
    },
  });

  const updateMutation = useMutation({
    mutationFn: payload => updateUnitOfMeasure(payload.id, payload.data),
    onSuccess: async response => {
      showSuccess(
        t(i18nKeyContainer.products.units.form.toasts.updateSuccessTitle),
        t(i18nKeyContainer.products.units.form.toasts.updateSuccessMessage, {
          name: response.data.name,
        })
      );
      setFormData({
        name: response.data.name,
        description: response.data.description || '',
      });
      await queryClient.invalidateQueries({
        queryKey: queryKeys.products.unitOfMeasure(),
      });
      if (onSuccess) {
        onSuccess();
      }
      onClose();
    },
    onError: error => {
      showError(
        t(i18nKeyContainer.products.units.form.toasts.updateErrorTitle),
        error?.message ||
          t(i18nKeyContainer.products.units.form.toasts.updateErrorMessage, {
            error: t(i18nKeyContainer.products.shared.notSpecified),
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
        i18nKeyContainer.products.units.form.validation.unitNameRequired
      );
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const saveUnit = () => {
    if (!validateForm()) {
      return;
    }

    if (mode === 'add') {
      createMutation.mutate({
        name: formData.name,
        description: formData.description,
      });
    } else if (mode === 'update') {
      updateMutation.mutate({
        id,
        data: {
          name: formData.name,
          description: formData.description,
        },
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
    if (isOpen && id === 0) {
      setMode('add');
      setFormData({
        name: '',
        description: '',
      });
    }
  }, [id, isOpen]);

  useEffect(() => {
    if (unitData) {
      setFormData({
        name: unitData.name,
        description: unitData.description || '',
      });
      setMode('update');
    }
  }, [unitData]);

  useEffect(() => {
    setId(unitId);
  }, [unitId]);

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
            <Tag className='h-6 w-6 text-gray-600' />
            <h2 className='text-xl font-semibold'>
              {mode === 'add'
                ? t(i18nKeyContainer.products.units.form.title.add)
                : t(i18nKeyContainer.products.units.form.title.edit)}
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
              <h3 className='text-lg font-semibold'>
                {t(i18nKeyContainer.products.units.form.sectionInfo)}
              </h3>
            </div>

            <div className='space-y-6'>
              <div>
                <label className='block text-sm font-medium mb-2'>
                  {t(i18nKeyContainer.products.units.form.fields.unitName)}{' '}
                  <span className='text-red-500'>
                    {t(i18nKeyContainer.products.shared.required)}
                  </span>
                </label>
                <Input
                  placeholder={t(
                    i18nKeyContainer.products.units.form.placeholders.unitName
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

              <div>
                <label className='block text-sm font-medium mb-2'>
                  {t(i18nKeyContainer.products.units.form.fields.description)}
                </label>
                <textarea
                  placeholder={t(
                    i18nKeyContainer.products.units.form.placeholders.description
                  )}
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
              {t(i18nKeyContainer.products.units.form.actions.cancel)}
            </Button>
            <Button
              type='submit'
              disabled={!formData.name.trim() || isLoading}
              className='cursor-pointer'
            >
              {isLoading
                ? mode === 'add'
                  ? t(i18nKeyContainer.products.units.form.actions.creating)
                  : t(i18nKeyContainer.products.units.form.actions.saving)
                : mode === 'add'
                  ? t(i18nKeyContainer.products.units.form.actions.create)
                  : t(i18nKeyContainer.products.units.form.actions.saveChanges)}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddUnitOfMeasure;
