import { useMemo } from 'react';
import { useQuery } from '@tanstack/react-query';
import { ArrowLeft, Calendar, Pencil, Shapes } from 'lucide-react';
import { useNavigate, useParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

import Button from '@components/Buttons/Button';
import PageHeader from '@components/ui/PageHeader';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';
import { divStyles } from '@shared/utils/uiVariables';
import { getCustomerCategoryById } from '@features/customers/services/customerCategoryApi';

const InfoRow = ({ label, value }) => (
  <div className='grid grid-cols-1 md:grid-cols-3 gap-2 py-3 border-b border-gray-100'>
    <span className='text-sm font-medium text-gray-500'>{label}</span>
    <span className='md:col-span-2 text-sm text-gray-900 break-words'>{value}</span>
  </div>
);

export default function ViewCustomerCategory() {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const { id: routeId } = useParams();

  const parsedId = Number.parseInt(routeId || '0', 10);
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
    enabled: isValidId,
  });

  const formattedDate = useMemo(() => {
    if (!data?.createdOnUtc) {
      return t(i18nKeyContainer.customers.shared.notAvailable);
    }

    const date = new Date(data.createdOnUtc);
    if (Number.isNaN(date.getTime())) {
      return t(i18nKeyContainer.customers.shared.notAvailable);
    }

    return date.toLocaleString(activeLocale);
  }, [activeLocale, data?.createdOnUtc, t]);

  if (!isValidId) {
    return (
      <div className='space-y-6'>
        <PageHeader
          title={t(i18nKeyContainer.customers.categoryManagement.view.title)}
          description={t(i18nKeyContainer.customers.categoryManagement.view.invalidId)}
        />
        <Button variant='secondary' onClick={() => navigate('/customers/categories')}>
          {t(i18nKeyContainer.customers.categoryManagement.view.actions.backToList)}
        </Button>
      </div>
    );
  }

  return (
    <div className='space-y-6'>
      <div className='flex flex-col md:flex-row md:items-center md:justify-between gap-4'>
        <PageHeader
          title={t(i18nKeyContainer.customers.categoryManagement.view.title)}
          description={t(i18nKeyContainer.customers.categoryManagement.view.subtitle, {
            id: parsedId,
          })}
        />
        <div className='flex flex-wrap gap-3'>
          <Button
            variant='secondary'
            LeftIcon={ArrowLeft}
            onClick={() => navigate('/customers/categories')}
          >
            {t(i18nKeyContainer.customers.categoryManagement.view.actions.backToList)}
          </Button>
          <Button
            LeftIcon={Pencil}
            onClick={() => navigate(`/customers/categories/${parsedId}/edit`)}
          >
            {t(i18nKeyContainer.customers.categoryManagement.view.actions.editCategory)}
          </Button>
        </div>
      </div>

      {isLoading && (
        <div className={divStyles + 'flex items-center justify-center min-h-52'}>
          <div className='animate-spin rounded-full h-10 w-10 border-b-2 border-blue-600'></div>
        </div>
      )}

      {isError && (
        <div className={divStyles}>
          <p className='text-red-600 font-medium'>
            {error?.message ||
              t(i18nKeyContainer.customers.categoryManagement.view.notFound)}
          </p>
        </div>
      )}

      {!isLoading && !isError && data && (
        <>
          <div className={divStyles + 'space-y-2'}>
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

          <div className={divStyles + 'space-y-2'}>
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
        </>
      )}
    </div>
  );
}
