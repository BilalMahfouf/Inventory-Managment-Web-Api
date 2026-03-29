import { Plus } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

import Button from '@components/Buttons/Button';
import PageHeader from '@components/ui/PageHeader';
import CustomerCategoryDataTable from '@features/customers/components/customers/customerCategoryDataTable';
import { divStyles } from '@shared/utils/uiVariables';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

export default function CustomerCategoriesPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();

  return (
    <div className='space-y-6'>
      <div className='flex flex-col md:flex-row md:items-center md:justify-between gap-4'>
        <PageHeader
          title={t(i18nKeyContainer.customers.categoryManagement.page.title)}
          description={t(i18nKeyContainer.customers.categoryManagement.page.description)}
        />
        <Button
          LeftIcon={Plus}
          onClick={() => {
            navigate('/customers/categories/new');
          }}
        >
          {t(i18nKeyContainer.customers.categoryManagement.page.addButton)}
        </Button>
      </div>

      <div className={divStyles + 'mt-2'}>
        <div className='mb-6'>
          <h3 className='text-2xl font-semibold leading-none tracking-tight'>
            {t(i18nKeyContainer.customers.categoryManagement.page.sectionTitle)}
          </h3>
        </div>
        <CustomerCategoryDataTable />
      </div>
    </div>
  );
}
