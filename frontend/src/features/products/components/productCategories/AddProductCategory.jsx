import React, { useState, useEffect } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { X, FolderTree } from 'lucide-react';
import { useTranslation } from 'react-i18next';

import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import {
  createProductCategory,
  getProductCategoryById,
  updateProductCategory,
  getMainCategories,
} from '@features/products/services/productCategoryApi';
import { useToast } from '@shared/context/ToastContext';
import { queryKeys } from '@shared/lib/queryKeys';

/**
 * AddProductCategory Component
 *
 * A modal component for adding or editing product categories in the inventory system.
 * Supports both MainCategory and SubCategory types.
 * When SubCategory is selected, displays a select dropdown to choose parent category.
 *
 * @param {Object} props - Component props
 * @param {boolean} props.isOpen - Controls modal visibility
 * @param {function} props.onClose - Callback function when modal is closed
 * @param {number} props.categoryId - ID of the category to edit (0 for new category)
 * @param {function} props.onSuccess - Optional callback after successful save
 */
const AddProductCategory = ({ isOpen, onClose, categoryId = 0, onSuccess }) => {
  const { t } = useTranslation();
  const { showSuccess, showError } = useToast();
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    type: 1, // 1 for MainCategory, 2 for SubCategory
    parentId: null,
  });
  const [mode, setMode] = useState('add'); // 'add' or 'update'
  const [id, setId] = useState(categoryId);
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState({});
  const queryClient = useQueryClient();

  const { data: mainCategories = [] } = useQuery({
    queryKey: [...queryKeys.products.categories(), 'main-categories'],
    queryFn: getMainCategories,
    enabled: isOpen,
  });

  const { data: categoryData } = useQuery({
    queryKey: [...queryKeys.products.categories(), 'detail', id],
    queryFn: () => getProductCategoryById(id),
    enabled: isOpen && id > 0,
  });

  const createMutation = useMutation({
    mutationFn: createProductCategory,
    onSuccess: async response => {
      if (!response.success) {
        showError(
          t(i18nKeyContainer.products.categories.form.toasts.createErrorTitle),
          t(i18nKeyContainer.products.categories.form.toasts.createErrorMessage, {
            name: formData.name,
            error: response.message,
          })
        );
        return;
      }

      showSuccess(
        t(i18nKeyContainer.products.categories.form.toasts.createSuccessTitle),
        t(i18nKeyContainer.products.categories.form.toasts.createSuccessMessage, {
          name: formData.name,
        })
      );
      setId(response.data.id);
      await queryClient.invalidateQueries({ queryKey: queryKeys.products.categories() });
      await queryClient.invalidateQueries({ queryKey: queryKeys.products.summary() });
      if (onSuccess) {
        onSuccess();
      }
      onClose();
    },
  });

  const updateMutation = useMutation({
    mutationFn: payload => updateProductCategory(payload.id, payload.data),
    onSuccess: async response => {
      if (!response.success) {
        showError(
          t(i18nKeyContainer.products.categories.form.toasts.updateErrorTitle),
          t(i18nKeyContainer.products.categories.form.toasts.updateErrorMessage, {
            error: response.message,
          })
        );
        return;
      }

      showSuccess(
        t(i18nKeyContainer.products.categories.form.toasts.updateSuccessTitle),
        t(i18nKeyContainer.products.categories.form.toasts.updateSuccessMessage, {
          name: formData.name,
        })
      );
      setFormData({
        name: response.data.name,
        description: response.data.description || '',
        type: response.data.type,
        parentId: response.data.parentId || null,
      });
      await queryClient.invalidateQueries({ queryKey: queryKeys.products.categories() });
      await queryClient.invalidateQueries({ queryKey: queryKeys.products.summary() });
      if (onSuccess) {
        onSuccess();
      }
      onClose();
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

  const handleTypeChange = type => {
    setFormData(prev => ({
      ...prev,
      type: type,
      parentId: type === 1 ? null : prev.parentId,
    }));
    if (errors.type) {
      setErrors(prev => ({
        ...prev,
        type: '',
      }));
    }
  };

  const validateForm = () => {
    const newErrors = {};
    if (!formData.name.trim()) {
      newErrors.name = t(
        i18nKeyContainer.products.categories.form.validation
          .categoryNameRequired
      );
    }
    if (formData.type === 2 && !formData.parentId) {
      newErrors.parentId = t(
        i18nKeyContainer.products.categories.form.validation
          .parentCategoryRequired
      );
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const saveCategory = () => {
    if (!validateForm()) {
      return;
    }

    if (mode === 'add') {
      createMutation.mutate({
        name: formData.name,
        description: formData.description,
        type: formData.type,
        parentId: formData.parentId,
      });
      setMode('update');
    } else if (mode === 'update') {
      updateMutation.mutate({
        id,
        data: {
          name: formData.name,
          description: formData.description,
          type: formData.type,
          parentId: formData.parentId,
        },
      });
    }
  };

  const handleSubmit = e => {
    e.preventDefault();
    saveCategory();
  };

  const handleCancel = () => {
    setFormData({
      name: '',
      description: '',
      type: 1,
      parentId: null,
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
        type: 1,
        parentId: null,
      });
    }
  }, [id, isOpen]);

  useEffect(() => {
    if (categoryData) {
      setFormData({
        name: categoryData.name,
        description: categoryData.description || '',
        type: categoryData.type === 'MainCategory' ? 1 : 2,
        parentId: categoryData.parentId || null,
      });
      setMode('update');
    }
  }, [categoryData]);

  useEffect(() => {
    setId(categoryId);
  }, [categoryId]);

  useEffect(() => {
    setIsLoading(createMutation.isPending || updateMutation.isPending);
  }, [createMutation.isPending, updateMutation.isPending]);

  if (!isOpen) return null;

  return (
    <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4'>
      <div className='bg-white rounded-lg w-full max-w-2xl max-h-[90vh] overflow-hidden flex flex-col'>
        {/* Header */}
        <div className='flex items-center justify-between p-6 border-b flex-shrink-0'>
          <div className='flex items-center gap-3'>
            <FolderTree className='h-6 w-6 text-gray-600' />
            <h2 className='text-xl font-semibold'>
              {mode === 'add'
                ? t(i18nKeyContainer.products.categories.form.title.add)
                : t(i18nKeyContainer.products.categories.form.title.edit)}
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
        <form
          onSubmit={handleSubmit}
          className='flex flex-col flex-1 overflow-hidden'
        >
          <div className='p-6 overflow-y-auto flex-1'>
            <div className='flex items-center gap-2 mb-6'>
              <FolderTree className='h-5 w-5' />
              <h3 className='text-lg font-semibold'>
                {t(i18nKeyContainer.products.categories.form.sectionInfo)}
              </h3>
            </div>

            <div className='space-y-6'>
              <div>
                <label className='block text-sm font-medium mb-2'>
                  {t(i18nKeyContainer.products.categories.form.fields.categoryName)}{' '}
                  <span className='text-red-500'>
                    {t(i18nKeyContainer.products.shared.required)}
                  </span>
                </label>
                <Input
                  placeholder={t(
                    i18nKeyContainer.products.categories.form.placeholders
                      .categoryName
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
                  {t(i18nKeyContainer.products.categories.form.fields.description)}
                </label>
                <textarea
                  placeholder={t(
                    i18nKeyContainer.products.categories.form.placeholders
                      .description
                  )}
                  value={formData.description}
                  onChange={e =>
                    handleInputChange('description', e.target.value)
                  }
                  className='w-full h-32 px-3 py-2 border border-gray-300 rounded-md resize-none focus:outline-none focus:ring-1 focus:ring-blue-600'
                  disabled={isLoading}
                />
              </div>

              <div>
                <label className='block text-sm font-medium mb-3'>
                  {t(i18nKeyContainer.products.categories.form.fields.categoryType)}{' '}
                  <span className='text-red-500'>
                    {t(i18nKeyContainer.products.shared.required)}
                  </span>
                </label>
                <div className='space-y-3'>
                  {/* Main Category Radio */}
                  <label className='flex items-center p-4 border-2 rounded-lg cursor-pointer hover:bg-gray-50 transition-colors'>
                    <input
                      type='radio'
                      name='categoryType'
                      value={1}
                      checked={formData.type === 1}
                      onChange={() => handleTypeChange(1)}
                      disabled={isLoading}
                      className='w-4 h-4 text-blue-600 cursor-pointer'
                    />
                    <div className='ml-3'>
                      <span className='text-sm font-medium text-gray-900'>
                        {t(i18nKeyContainer.products.categories.form.fields.mainCategory)}
                      </span>
                      <p className='text-xs text-gray-500 mt-0.5'>
                        {t(
                          i18nKeyContainer.products.categories.form.fields
                            .mainCategoryDescription
                        )}
                      </p>
                    </div>
                  </label>

                  {/* Subcategory Radio */}
                  <label className='flex items-center p-4 border-2 rounded-lg cursor-pointer hover:bg-gray-50 transition-colors'>
                    <input
                      type='radio'
                      name='categoryType'
                      value={2}
                      checked={formData.type === 2}
                      onChange={() => handleTypeChange(2)}
                      disabled={isLoading}
                      className='w-4 h-4 text-blue-600 cursor-pointer'
                    />
                    <div className='ml-3'>
                      <span className='text-sm font-medium text-gray-900'>
                        {t(i18nKeyContainer.products.categories.form.fields.subcategory)}
                      </span>
                      <p className='text-xs text-gray-500 mt-0.5'>
                        {t(
                          i18nKeyContainer.products.categories.form.fields
                            .subcategoryDescription
                        )}
                      </p>
                    </div>
                  </label>
                </div>
              </div>

              {/* Parent Category Select - Only shown for SubCategory */}
              {formData.type === 2 && (
                <div>
                  <label className='block text-sm font-medium mb-2'>
                    {t(i18nKeyContainer.products.categories.form.fields.parentCategory)}{' '}
                    <span className='text-red-500'>
                      {t(i18nKeyContainer.products.shared.required)}
                    </span>
                  </label>
                  <select
                    value={formData.parentId || ''}
                    onChange={e =>
                      handleInputChange(
                        'parentId',
                        e.target.value ? parseInt(e.target.value) : null
                      )
                    }
                    className={`w-full h-12 px-3 border rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-600 ${
                      errors.parentId ? 'border-red-500' : 'border-gray-300'
                    }`}
                    disabled={isLoading}
                  >
                    <option value=''>
                      {t(
                        i18nKeyContainer.products.categories.form.placeholders
                          .parentCategory
                      )}
                    </option>
                    {mainCategories.map(category => (
                      <option key={category.id} value={category.id}>
                        {category.name}
                      </option>
                    ))}
                  </select>
                  {errors.parentId && (
                    <p className='text-red-500 text-sm mt-1'>
                      {errors.parentId}
                    </p>
                  )}
                </div>
              )}
            </div>
          </div>

          {/* Footer */}
          <div className='flex items-center justify-end gap-3 p-6 border-t bg-gray-50 flex-shrink-0'>
            <Button
              type='button'
              variant='secondary'
              onClick={handleCancel}
              disabled={isLoading}
              className='cursor-pointer'
            >
              {t(i18nKeyContainer.products.categories.form.actions.cancel)}
            </Button>
            <Button
              type='submit'
              disabled={!formData.name.trim() || isLoading}
              className='cursor-pointer'
            >
              {isLoading
                ? mode === 'add'
                  ? t(i18nKeyContainer.products.categories.form.actions.creating)
                  : t(i18nKeyContainer.products.categories.form.actions.saving)
                : mode === 'add'
                  ? t(i18nKeyContainer.products.categories.form.actions.create)
                  : t(i18nKeyContainer.products.categories.form.actions.saveChanges)}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddProductCategory;
