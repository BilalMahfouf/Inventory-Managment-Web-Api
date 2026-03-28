import React from 'react';
import {
  User,
  Mail,
  Phone,
  MapPin,
  CreditCard,
  Calendar,
  DollarSign,
} from 'lucide-react';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

/**
 * ViewCustomer Component
 *
 * Displays a comprehensive read-only view of customer information organized into sections.
 * Used in the Summary tab of the AddUpdateCustomer dialog.
 *
 * @param {Object} props - Component props
 * @param {Object} props.customer - Customer data object
 * @param {boolean} props.loading - Loading state
 */
const ViewCustomer = ({ customer, loading }) => {
  const { t, i18n } = useTranslation();
  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';

  if (loading) {
    return (
      <div className='flex items-center justify-center py-12'>
        <div className='animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600'></div>
      </div>
    );
  }

  if (!customer) {
    return (
      <div className='flex items-center justify-center py-12'>
        <p className='text-gray-500'>
          {t(i18nKeyContainer.customers.view.noCustomerData)}
        </p>
      </div>
    );
  }

  const InfoSection = ({ title, children }) => (
    <div className='bg-white border border-gray-200 rounded-lg p-6'>
      <h3 className='text-base font-semibold text-gray-900 mb-4'>{title}</h3>
      <div className='space-y-3'>{children}</div>
    </div>
  );

  const InfoRow = ({ label, value, icon: Icon }) => (
    <div className='flex items-start gap-3'>
      {Icon && (
        <div className='mt-0.5'>
          <Icon className='h-4 w-4 text-gray-400' />
        </div>
      )}
      <div className='flex-1 min-w-0'>
        <p className='text-xs font-medium text-gray-500 uppercase tracking-wide'>
          {label}
        </p>
        <p className='text-sm text-gray-900 mt-1 break-words'>
          {value || t(i18nKeyContainer.customers.shared.notAvailable)}
        </p>
      </div>
    </div>
  );

  const StatusBadge = ({ isActive }) => (
    <span
      className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
        isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
      }`}
    >
      {isActive
        ? `● ${t(i18nKeyContainer.customers.shared.status.active)}`
        : `● ${t(i18nKeyContainer.customers.shared.status.inactive)}`}
    </span>
  );

  const CreditStatusBadge = ({ status }) => {
    const statusConfig = {
      0: {
        label: t(i18nKeyContainer.customers.shared.creditStatus.active),
        color: 'bg-green-100 text-green-800',
      },
      1: {
        label: t(i18nKeyContainer.customers.shared.creditStatus.onHold),
        color: 'bg-yellow-100 text-yellow-800',
      },
      2: {
        label: t(i18nKeyContainer.customers.shared.creditStatus.suspended),
        color: 'bg-red-100 text-red-800',
      },
    };

    const config = statusConfig[status] || statusConfig[0];

    return (
      <span
        className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${config.color}`}
      >
        {config.label}
      </span>
    );
  };

  return (
    <div className='space-y-6'>
      {/* General Information */}
      <InfoSection title={t(i18nKeyContainer.customers.view.sections.generalInformation)}>
        <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
          <InfoRow
            label={t(i18nKeyContainer.customers.view.fields.id)}
            value={t(i18nKeyContainer.customers.view.labels.customerCode, {
              id: String(customer.id).padStart(4, '0'),
            })}
          />
          <InfoRow
            label={t(i18nKeyContainer.customers.view.fields.name)}
            value={customer.name}
            icon={User}
          />
          <InfoRow
            label={t(i18nKeyContainer.customers.view.fields.customerCategory)}
            value={
              customer.customerCategoryName ||
              customer.customerCategory?.name ||
              t(i18nKeyContainer.customers.shared.defaults.customerCategory)
            }
          />
          <InfoRow
            label={t(i18nKeyContainer.customers.view.fields.email)}
            value={customer.email}
            icon={Mail}
          />
          <InfoRow
            label={t(i18nKeyContainer.customers.view.fields.phone)}
            value={customer.phone}
            icon={Phone}
          />
          <div className='flex items-start gap-3'>
            <div className='flex-1 min-w-0'>
              <p className='text-xs font-medium text-gray-500 uppercase tracking-wide'>
                {t(i18nKeyContainer.customers.view.fields.status)}
              </p>
              <div className='mt-1'>
                <StatusBadge isActive={customer.isActive} />
              </div>
            </div>
          </div>
        </div>
      </InfoSection>

      {/* Address */}
      <InfoSection title={t(i18nKeyContainer.customers.view.sections.address)}>
        <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
          <div className='md:col-span-2'>
            <InfoRow
              label={t(i18nKeyContainer.customers.view.fields.street)}
              value={customer.address?.street || customer.street}
              icon={MapPin}
            />
          </div>
          <InfoRow
            label={t(i18nKeyContainer.customers.view.fields.city)}
            value={customer.address?.city || customer.city}
          />
          <InfoRow
            label={t(i18nKeyContainer.customers.view.fields.state)}
            value={customer.address?.state || customer.state}
          />
          <InfoRow
            label={t(i18nKeyContainer.customers.view.fields.zipCode)}
            value={customer.address?.zipCode || customer.zipCode}
          />
        </div>
      </InfoSection>

      {/* Business Information */}
      <InfoSection title={t(i18nKeyContainer.customers.view.sections.businessInformation)}>
        <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
          <InfoRow
            label={t(i18nKeyContainer.customers.view.fields.creditLimit)}
            value={`${t(i18nKeyContainer.customers.shared.currencySymbol)}${Number(customer.creditLimit || 0).toLocaleString(activeLocale, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`}
            icon={DollarSign}
          />
          <InfoRow
            label={t(i18nKeyContainer.customers.view.fields.paymentTerms)}
            value={
              customer.paymentTerms ||
              t(i18nKeyContainer.customers.shared.defaults.paymentTerms)
            }
          />
          <div className='flex items-start gap-3'>
            <div className='mt-0.5'>
              <CreditCard className='h-4 w-4 text-gray-400' />
            </div>
            <div className='flex-1 min-w-0'>
              <p className='text-xs font-medium text-gray-500 uppercase tracking-wide'>
                {t(i18nKeyContainer.customers.view.fields.creditStatus)}
              </p>
              <div className='mt-1'>
                <CreditStatusBadge status={customer.creditStatus} />
              </div>
            </div>
          </div>
        </div>
      </InfoSection>

      {/* System Information */}
      <InfoSection title={t(i18nKeyContainer.customers.view.sections.systemInformation)}>
        <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
          <InfoRow
            label={t(i18nKeyContainer.customers.view.fields.createdAt)}
            value={
              customer.createdAt
                ? new Date(customer.createdAt).toLocaleString(activeLocale)
                : t(i18nKeyContainer.customers.shared.notAvailable)
            }
            icon={Calendar}
          />
          <InfoRow
            label={t(i18nKeyContainer.customers.view.fields.createdBy)}
            value={
              customer.createdByUserName ||
              t(i18nKeyContainer.customers.shared.defaults.createdBy)
            }
          />
        </div>
      </InfoSection>
    </div>
  );
};

export default ViewCustomer;
