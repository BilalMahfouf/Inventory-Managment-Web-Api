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
        <p className='text-gray-500'>No customer data available</p>
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
          {value || 'N/A'}
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
      {isActive ? '● Active' : '● Inactive'}
    </span>
  );

  const CreditStatusBadge = ({ status }) => {
    const statusConfig = {
      0: { label: 'Active', color: 'bg-green-100 text-green-800' },
      1: { label: 'On Hold', color: 'bg-yellow-100 text-yellow-800' },
      2: { label: 'Suspended', color: 'bg-red-100 text-red-800' },
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
      <InfoSection title='General Information'>
        <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
          <InfoRow
            label='ID'
            value={`CUST-${String(customer.id).padStart(4, '0')}`}
          />
          <InfoRow label='Name' value={customer.name} icon={User} />
          <InfoRow
            label='Customer Category'
            value={
              customer.customerCategoryName ||
              customer.customerCategory?.name ||
              'Retail'
            }
          />
          <InfoRow label='Email' value={customer.email} icon={Mail} />
          <InfoRow label='Phone' value={customer.phone} icon={Phone} />
          <div className='flex items-start gap-3'>
            <div className='flex-1 min-w-0'>
              <p className='text-xs font-medium text-gray-500 uppercase tracking-wide'>
                Status
              </p>
              <div className='mt-1'>
                <StatusBadge isActive={customer.isActive} />
              </div>
            </div>
          </div>
        </div>
      </InfoSection>

      {/* Address */}
      <InfoSection title='Address'>
        <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
          <div className='md:col-span-2'>
            <InfoRow
              label='Street'
              value={customer.address?.street || customer.street}
              icon={MapPin}
            />
          </div>
          <InfoRow
            label='City'
            value={customer.address?.city || customer.city}
          />
          <InfoRow
            label='State'
            value={customer.address?.state || customer.state}
          />
          <InfoRow
            label='Zip Code'
            value={customer.address?.zipCode || customer.zipCode}
          />
        </div>
      </InfoSection>

      {/* Business Information */}
      <InfoSection title='Business Information'>
        <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
          <InfoRow
            label='Credit Limit'
            value={`$${(customer.creditLimit || 0).toFixed(2)}`}
            icon={DollarSign}
          />
          <InfoRow
            label='Payment Terms'
            value={customer.paymentTerms || 'Net 30'}
          />
          <div className='flex items-start gap-3'>
            <div className='mt-0.5'>
              <CreditCard className='h-4 w-4 text-gray-400' />
            </div>
            <div className='flex-1 min-w-0'>
              <p className='text-xs font-medium text-gray-500 uppercase tracking-wide'>
                Credit Status
              </p>
              <div className='mt-1'>
                <CreditStatusBadge status={customer.creditStatus} />
              </div>
            </div>
          </div>
        </div>
      </InfoSection>

      {/* System Information */}
      <InfoSection title='System Information'>
        <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
          <InfoRow
            label='Created At'
            value={
              customer.createdAt
                ? new Date(customer.createdAt).toLocaleString()
                : 'N/A'
            }
            icon={Calendar}
          />
          <InfoRow
            label='Created By'
            value={customer.createdByUserName || 'admin@system.com'}
          />
        </div>
      </InfoSection>
    </div>
  );
};

export default ViewCustomer;
