import React, { useState } from 'react';
import { UserPlus } from 'lucide-react';
import Button from '@components/Buttons/Button';
import AddUpdateCustomer from './AddUpdateCustomer';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

/**
 * AddCustomerButton Component
 *
 * A reusable button that opens the AddUpdateCustomer dialog in "add" mode.
 *
 * Usage Example:
 * ```jsx
 * import { AddCustomerButton } from '@features/customers/components/customers';
 *
 * function MyPage() {
 *   const handleSuccess = () => {
 *     // Refresh your customer list or perform other actions
 *     console.log('Customer added successfully!');
 *   };
 *
 *   return <AddCustomerButton onSuccess={handleSuccess} />;
 * }
 * ```
 *
 * @param {Object} props - Component props
 * @param {function} props.onSuccess - Optional callback after successful customer creation
 */
const AddCustomerButton = ({ onSuccess }) => {
  const { t } = useTranslation();
  const [isOpen, setIsOpen] = useState(false);

  return (
    <>
      <Button
        onClick={() => setIsOpen(true)}
        LeftIcon={UserPlus}
        className='cursor-pointer'
      >
        {t(i18nKeyContainer.customers.addButton.label)}
      </Button>
      <AddUpdateCustomer
        isOpen={isOpen}
        onClose={() => setIsOpen(false)}
        customerId={0}
        onSuccess={() => {
          if (onSuccess) {
            onSuccess();
          }
        }}
      />
    </>
  );
};

export default AddCustomerButton;
