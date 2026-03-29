import { useMemo } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Calendar, Pencil, Shapes, X } from 'lucide-react';
import { useTranslation } from 'react-i18next';

import Button from '@components/Buttons/Button';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';
import { formatAppDate } from '@shared/utils/dateFormatter';
import { getCustomerCategoryById } from '@features/customers/services/customerCategoryApi';

const InfoRow = ({ label, value }) => (
  <div className='grid grid-cols-1 md:grid-cols-3 gap-2 py-3 border-b border-gray-100'>
    <span className='text-sm font-medium text-gray-500'>{label}</span>
    <span className='md:col-span-2 text-sm text-gray-900 break-words'>{value}</span>
  </div>
);

export default function ViewCustomerCategory({
  isOpen,
  onClose,
  categoryId = 0,
  onEdit,
}) {
  const { t, i18n } = useTranslation();

  const parsedId = Number.parseInt(String(categoryId || '0'), 10);
  const isValidId = Number.isInteger(parsedId) && parsedId > 0;
  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';

  const { data, isLoading, isError, error } = useQuery({
    queryKey: queryKeys.customers.customerCategories.detail(parsedId),
    queryFn: async () => {
      const response = await getCustomerCategoryById(parsedId);

      if (!response.success) {
        throw new Error(
          response.error ||
            t(i18nKeyContainer.customers.categoryManagement.view.notFound)
        );
      }

      return response.data;
    },
    enabled: isOpen && isValidId,
  });

  const formattedDate = useMemo(() => {
    return formatAppDate(data?.createdOnUtc, {
      locale: activeLocale,
      withTime: true,
      fallback: t(i18nKeyContainer.customers.shared.notAvailable),
    });
  }, [activeLocale, data?.createdOnUtc, t]);

  if (!isOpen) return null;

  return (
    <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4'>
      <div className='bg-white rounded-lg w-full max-w-3xl max-h-[90vh] overflow-hidden flex flex-col'>
        <div className='flex items-center justify-between p-6 border-b flex-shrink-0'>
          <div>
            <h2 className='text-2xl font-bold text-gray-900'>
              {t(i18nKeyContainer.customers.categoryManagement.view.title)}
            </h2>
            <p className='text-sm text-gray-500 mt-1'>
              {isValidId
                ? t(i18nKeyContainer.customers.categoryManagement.view.subtitle, {
                    id: parsedId,
                  })
                : t(i18nKeyContainer.customers.categoryManagement.view.invalidId)}
            </p>
          </div>
          <button
            onClick={() => {
              if (onClose) {
                onClose();
              }
            }}
            className='p-2 hover:bg-gray-100 rounded-lg transition-colors'
            type='button'
          >
            <X className='h-5 w-5' />
          </button>
        </div>

        <div className='p-6 overflow-y-auto flex-1'>
          {!isValidId && (
            <div className='rounded-lg border border-red-200 bg-red-50 p-4'>
              <p className='text-red-700'>
                {t(i18nKeyContainer.customers.categoryManagement.view.invalidId)}
              </p>
            </div>
          )}

          {isValidId && isLoading && (
            <div className='flex items-center justify-center py-12'>
              <div className='animate-spin rounded-full h-10 w-10 border-b-2 border-blue-600'></div>
            </div>
          )}

          {isValidId && isError && (
            <div className='rounded-lg border border-red-200 bg-red-50 p-4'>
              <p className='text-red-700'>
                {error?.message ||
                  t(i18nKeyContainer.customers.categoryManagement.view.notFound)}
              </p>
            </div>
          )}

          {!isLoading && !isError && data && (
            <div className='space-y-6'>
              <div className='border border-gray-200 rounded-lg p-4 space-y-2'>
                <div className='flex items-center gap-2 mb-2'>
                  <Shapes className='h-5 w-5 text-gray-600' />
                  <h3 className='text-lg font-semibold'>
                    {t(i18nKeyContainer.customers.categoryManagement.view.sections.general)}
                  </h3>
                </div>

                <InfoRow
                  label={t(i18nKeyContainer.customers.categoryManagement.view.fields.id)}
                  value={data.id}
                />
                <InfoRow
                  label={t(i18nKeyContainer.customers.categoryManagement.view.fields.name)}
                  value={data.name}
                />
                <InfoRow
                  label={t(i18nKeyContainer.customers.categoryManagement.view.fields.type)}
                  value={
                    data.isIndividual
                      ? t(i18nKeyContainer.customers.categoryManagement.table.type.individual)
                      : t(i18nKeyContainer.customers.categoryManagement.table.type.business)
                  }
                />
                <InfoRow
                  label={t(
                    i18nKeyContainer.customers.categoryManagement.view.fields.description
                  )}
                  value={
                    data.description ||
                    t(i18nKeyContainer.customers.categoryManagement.view.fallback.description)
                  }
                />
              </div>

              <div className='border border-gray-200 rounded-lg p-4 space-y-2'>
                <div className='flex items-center gap-2 mb-2'>
                  <Calendar className='h-5 w-5 text-gray-600' />
                  <h3 className='text-lg font-semibold'>
                    {t(i18nKeyContainer.customers.categoryManagement.view.sections.system)}
                  </h3>
                </div>

                <InfoRow
                  label={t(
                    i18nKeyContainer.customers.categoryManagement.view.fields.createdAt
                  )}
                  value={formattedDate}
                />
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
          >
            {t(i18nKeyContainer.customers.categoryManagement.view.actions.backToList)}
          </Button>
          {isValidId && !isLoading && !isError && data && (
            <Button
              type='button'
              LeftIcon={Pencil}
              onClick={() => {
                if (onEdit) {
                  onEdit(parsedId);
                }
              }}
            >
              {t(i18nKeyContainer.customers.categoryManagement.view.actions.editCategory)}
            </Button>
          )}
        </div>
      </div>
    </div>
  );
}
