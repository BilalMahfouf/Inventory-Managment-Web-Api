import { useEffect, useMemo, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { ArrowLeft, Save, Shapes } from 'lucide-react';
import { useNavigate, useParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

import Button from '@components/Buttons/Button';
import PageHeader from '@components/ui/PageHeader';
import { Input } from '@components/ui/input';
import { useToast } from '@shared/context/ToastContext';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';
import { divStyles } from '@shared/utils/uiVariables';
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

export default function AddUpdateCustomerCategory() {
  const { t } = useTranslation();
  const { showError, showSuccess } = useToast();
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const { id: routeId } = useParams();

  const parsedId = Number.parseInt(routeId || '0', 10);
  const isEditMode = Number.isInteger(parsedId) && parsedId > 0;
  const isInvalidEditRoute = Boolean(routeId) && !isEditMode;

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
    enabled: isEditMode,
  });

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

      navigate('/customers/categories');
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

      navigate('/customers/categories');
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

  if (isInvalidEditRoute) {
    return (
      <div className='space-y-6'>
        <PageHeader
          title={t(i18nKeyContainer.customers.categoryManagement.form.invalidRouteTitle)}
          description={t(
            i18nKeyContainer.customers.categoryManagement.form.invalidRouteDescription
          )}
        />
        <Button variant='secondary' onClick={() => navigate('/customers/categories')}>
          {t(i18nKeyContainer.customers.categoryManagement.form.actions.backToList)}
        </Button>
      </div>
    );
  }

  if (isEditMode && isFetching) {
    return (
      <div className='space-y-6'>
        <PageHeader title={pageTitle} description={pageDescription} />
        <div className={divStyles + 'flex items-center justify-center min-h-52'}>
          <div className='animate-spin rounded-full h-10 w-10 border-b-2 border-blue-600'></div>
        </div>
      </div>
    );
  }

  if (isEditMode && isError) {
    return (
      <div className='space-y-6'>
        <PageHeader
          title={t(i18nKeyContainer.customers.categoryManagement.form.loadErrorTitle)}
          description={error?.message}
        />
        <Button variant='secondary' onClick={() => navigate('/customers/categories')}>
          {t(i18nKeyContainer.customers.categoryManagement.form.actions.backToList)}
        </Button>
      </div>
    );
  }

  return (
    <div className='space-y-6'>
      <div className='flex flex-col md:flex-row md:items-center md:justify-between gap-4'>
        <PageHeader title={pageTitle} description={pageDescription} />
        <Button
          variant='secondary'
          LeftIcon={ArrowLeft}
          onClick={() => navigate('/customers/categories')}
        >
          {t(i18nKeyContainer.customers.categoryManagement.form.actions.backToList)}
        </Button>
      </div>

      <form className={divStyles + 'space-y-6'} onSubmit={handleSubmit}>
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
                i18nKeyContainer.customers.categoryManagement.form.placeholders.name
              )}
              className={errors.name ? 'border-red-500' : ''}
              disabled={isSaving}
            />
            {errors.name && <p className='text-red-500 text-sm mt-1'>{errors.name}</p>}
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
                  {t(i18nKeyContainer.customers.categoryManagement.form.options.individual)}
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
                  {t(i18nKeyContainer.customers.categoryManagement.form.options.business)}
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

        <div className='flex flex-col-reverse sm:flex-row sm:justify-end gap-3'>
          <Button
            type='button'
            variant='secondary'
            onClick={() => navigate('/customers/categories')}
            disabled={isSaving}
          >
            {t(i18nKeyContainer.customers.categoryManagement.form.actions.cancel)}
          </Button>
          <Button type='submit' loading={isSaving} LeftIcon={Save}>
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
  );
}
