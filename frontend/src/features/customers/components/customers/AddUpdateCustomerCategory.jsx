import { useEffect, useMemo, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Save, Shapes, X } from 'lucide-react';
import { useTranslation } from 'react-i18next';

import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import { useToast } from '@shared/context/ToastContext';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';
import {
  createCustomerCategory,
  getCustomerCategoryById,
  updateCustomerCategory,
} from '@features/customers/services/customerCategoryApi';

const getInitialFormData = () => ({
  name: '',
  isIndividual: true,
  description: '',
});

export default function AddUpdateCustomerCategory({
  isOpen,
  onClose,
  categoryId = 0,
  onSuccess,
}) {
  const { t } = useTranslation();
  const { showError, showSuccess } = useToast();
  const queryClient = useQueryClient();

  const parsedId = Number.parseInt(String(categoryId || '0'), 10);
  const isEditMode = Number.isInteger(parsedId) && parsedId > 0;

  const [formData, setFormData] = useState(getInitialFormData);
  const [errors, setErrors] = useState({});

  const {
    data: categoryData,
    isFetching,
    isError,
    error,
  } = useQuery({
    queryKey: queryKeys.customers.customerCategories.detail(parsedId),
    queryFn: async () => {
      const response = await getCustomerCategoryById(parsedId);

      if (!response.success) {
        throw new Error(
          response.error ||
            t(i18nKeyContainer.customers.categoryManagement.form.toasts.loadFailedMessage)
        );
      }

      return response.data;
    },
    enabled: isOpen && isEditMode,
  });

  useEffect(() => {
    if (!isOpen) {
      return;
    }

    if (!isEditMode) {
      setFormData(getInitialFormData());
      setErrors({});
    }
  }, [isEditMode, isOpen]);

  useEffect(() => {
    if (isEditMode && categoryData) {
      setFormData({
        name: categoryData.name || '',
        isIndividual: Boolean(categoryData.isIndividual),
        description: categoryData.description || '',
      });
    }
  }, [categoryData, isEditMode]);

  const validateForm = () => {
    const nextErrors = {};
    const trimmedName = formData.name.trim();
    const trimmedDescription = formData.description.trim();

    if (!trimmedName) {
      nextErrors.name = t(
        i18nKeyContainer.customers.categoryManagement.form.validation.nameRequired
      );
    } else if (trimmedName.length > 100) {
      nextErrors.name = t(
        i18nKeyContainer.customers.categoryManagement.form.validation.nameTooLong
      );
    }

    if (trimmedDescription.length > 500) {
      nextErrors.description = t(
        i18nKeyContainer.customers.categoryManagement.form.validation
          .descriptionTooLong
      );
    }

    setErrors(nextErrors);
    return Object.keys(nextErrors).length === 0;
  };

  const createMutation = useMutation({
    mutationFn: createCustomerCategory,
    onSuccess: async response => {
      if (!response.success) {
        showError(
          t(i18nKeyContainer.customers.categoryManagement.form.toasts.createFailedTitle),
          response.message ||
            t(
              i18nKeyContainer.customers.categoryManagement.form.toasts
                .createFailedMessage
            )
        );
        return;
      }

      showSuccess(
        t(i18nKeyContainer.customers.categoryManagement.form.toasts.createSuccessTitle),
        t(i18nKeyContainer.customers.categoryManagement.form.toasts.createSuccessMessage, {
          name: formData.name.trim(),
        })
      );

      await queryClient.invalidateQueries({
        queryKey: queryKeys.customers.customerCategories.all(),
      });

      if (onSuccess) {
        onSuccess(response.data);
      }

      if (onClose) {
        onClose();
      }
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, payload }) => updateCustomerCategory(id, payload),
    onSuccess: async response => {
      if (!response.success) {
        showError(
          t(i18nKeyContainer.customers.categoryManagement.form.toasts.updateFailedTitle),
          response.message ||
            t(
              i18nKeyContainer.customers.categoryManagement.form.toasts
                .updateFailedMessage
            )
        );
        return;
      }

      showSuccess(
        t(i18nKeyContainer.customers.categoryManagement.form.toasts.updateSuccessTitle),
        t(i18nKeyContainer.customers.categoryManagement.form.toasts.updateSuccessMessage, {
          name: formData.name.trim(),
        })
      );

      await queryClient.invalidateQueries({
        queryKey: queryKeys.customers.customerCategories.all(),
      });
      await queryClient.invalidateQueries({
        queryKey: queryKeys.customers.customerCategories.detail(parsedId),
      });

      if (onSuccess) {
        onSuccess(response.data);
      }

      if (onClose) {
        onClose();
      }
    },
  });

  const isSaving = createMutation.isPending || updateMutation.isPending;

  const pageTitle = useMemo(
    () =>
      isEditMode
        ? t(i18nKeyContainer.customers.categoryManagement.form.title.edit)
        : t(i18nKeyContainer.customers.categoryManagement.form.title.add),
    [isEditMode, t]
  );

  const pageDescription = useMemo(
    () =>
      isEditMode
        ? t(i18nKeyContainer.customers.categoryManagement.form.subtitle.edit)
        : t(i18nKeyContainer.customers.categoryManagement.form.subtitle.add),
    [isEditMode, t]
  );

  const handleChange = (field, value) => {
    setFormData(prev => ({
      ...prev,
      [field]: value,
    }));

    if (errors[field]) {
      setErrors(prev => ({
        ...prev,
        [field]: '',
      }));
    }
  };

  const buildPayload = () => ({
    name: formData.name.trim(),
    isIndividual: Boolean(formData.isIndividual),
    description: formData.description.trim() || null,
  });

  const handleSubmit = event => {
    event.preventDefault();

    if (!validateForm()) {
      return;
    }

    const payload = buildPayload();

    if (isEditMode) {
      updateMutation.mutate({
        id: parsedId,
        payload,
      });
      return;
    }

    createMutation.mutate(payload);
  };

  if (!isOpen) return null;

  return (
    <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4'>
      <div className='bg-white rounded-lg w-full max-w-2xl max-h-[90vh] overflow-hidden flex flex-col'>
        <div className='flex items-center justify-between p-6 border-b flex-shrink-0'>
          <div>
            <h2 className='text-2xl font-bold text-gray-900'>{pageTitle}</h2>
            <p className='text-sm text-gray-500 mt-1'>{pageDescription}</p>
          </div>
          <button
            onClick={() => {
              if (onClose) {
                onClose();
              }
            }}
            className='p-2 hover:bg-gray-100 rounded-lg transition-colors'
            disabled={isSaving}
            type='button'
          >
            <X className='h-5 w-5' />
          </button>
        </div>

        <form onSubmit={handleSubmit} className='flex flex-col flex-1 overflow-hidden'>
          <div className='p-6 overflow-y-auto flex-1'>
            {isEditMode && isFetching && (
              <div className='flex items-center justify-center py-12'>
                <div className='animate-spin rounded-full h-10 w-10 border-b-2 border-blue-600'></div>
              </div>
            )}

            {isEditMode && isError && (
              <div className='rounded-lg border border-red-200 bg-red-50 p-4'>
                <p className='text-red-700'>
                  {error?.message ||
                    t(i18nKeyContainer.customers.categoryManagement.form.loadErrorTitle)}
                </p>
              </div>
            )}

            {(!isEditMode || (!isFetching && !isError)) && (
              <div className='space-y-6'>
                <div className='flex items-center gap-2'>
                  <Shapes className='h-5 w-5 text-gray-600' />
                  <h3 className='text-lg font-semibold'>
                    {t(i18nKeyContainer.customers.categoryManagement.form.sections.general)}
                  </h3>
                </div>

                <div className='grid grid-cols-1 md:grid-cols-2 gap-6'>
                  <div className='md:col-span-2'>
                    <label className='block text-sm font-medium mb-2'>
                      {t(i18nKeyContainer.customers.categoryManagement.form.fields.name)}{' '}
                      <span className='text-red-500'>
                        {t(i18nKeyContainer.customers.shared.required)}
                      </span>
                    </label>
                    <Input
                      value={formData.name}
                      onChange={event => handleChange('name', event.target.value)}
                      placeholder={t(
                        i18nKeyContainer.customers.categoryManagement.form.placeholders
                          .name
                      )}
                      className={errors.name ? 'border-red-500' : ''}
                      disabled={isSaving}
                    />
                    {errors.name && (
                      <p className='text-red-500 text-sm mt-1'>{errors.name}</p>
                    )}
                  </div>

                  <div className='md:col-span-2'>
                    <label className='block text-sm font-medium mb-3'>
                      {t(i18nKeyContainer.customers.categoryManagement.form.fields.type)}{' '}
                      <span className='text-red-500'>
                        {t(i18nKeyContainer.customers.shared.required)}
                      </span>
                    </label>
                    <div className='grid grid-cols-1 md:grid-cols-2 gap-3'>
                      <label className='flex items-center gap-2 border border-gray-300 rounded-lg p-3 cursor-pointer'>
                        <input
                          type='radio'
                          name='customer-category-type'
                          checked={formData.isIndividual === true}
                          onChange={() => handleChange('isIndividual', true)}
                          disabled={isSaving}
                        />
                        <span>
                          {t(
                            i18nKeyContainer.customers.categoryManagement.form.options
                              .individual
                          )}
                        </span>
                      </label>
                      <label className='flex items-center gap-2 border border-gray-300 rounded-lg p-3 cursor-pointer'>
                        <input
                          type='radio'
                          name='customer-category-type'
                          checked={formData.isIndividual === false}
                          onChange={() => handleChange('isIndividual', false)}
                          disabled={isSaving}
                        />
                        <span>
                          {t(
                            i18nKeyContainer.customers.categoryManagement.form.options
                              .business
                          )}
                        </span>
                      </label>
                    </div>
                  </div>

                  <div className='md:col-span-2'>
                    <label className='block text-sm font-medium mb-2'>
                      {t(i18nKeyContainer.customers.categoryManagement.form.fields.description)}
                    </label>
                    <textarea
                      value={formData.description}
                      onChange={event => handleChange('description', event.target.value)}
                      placeholder={t(
                        i18nKeyContainer.customers.categoryManagement.form.placeholders
                          .description
                      )}
                      rows={5}
                      className={`w-full px-3 py-2 border rounded-md resize-none focus:outline-none focus:ring-1 focus:ring-blue-600 ${
                        errors.description ? 'border-red-500' : 'border-gray-300'
                      }`}
                      disabled={isSaving}
                    />
                    {errors.description && (
                      <p className='text-red-500 text-sm mt-1'>{errors.description}</p>
                    )}
                  </div>
                </div>
              </div>
            )}
          </div>

          <div className='flex items-center justify-end gap-3 p-6 border-t bg-gray-50 flex-shrink-0'>
            <Button
              type='button'
              variant='secondary'
              onClick={() => {
                if (onClose) {
                  onClose();
                }
              }}
              disabled={isSaving}
            >
              {t(i18nKeyContainer.customers.categoryManagement.form.actions.cancel)}
            </Button>
            <Button
              type='submit'
              loading={isSaving}
              LeftIcon={Save}
              disabled={isEditMode && (isFetching || isError)}
            >
              {isSaving
                ? isEditMode
                  ? t(i18nKeyContainer.customers.categoryManagement.form.actions.saving)
                  : t(i18nKeyContainer.customers.categoryManagement.form.actions.creating)
                : isEditMode
                  ? t(i18nKeyContainer.customers.categoryManagement.form.actions.saveChanges)
                  : t(i18nKeyContainer.customers.categoryManagement.form.actions.create)}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
