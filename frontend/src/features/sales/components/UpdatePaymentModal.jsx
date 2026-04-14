import { useEffect, useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';
import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import { useUpdateOrderPayment } from '@features/sales/hooks/useOrders';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { formatDzdCurrency } from '@shared/utils/currencyFormatter';

const STATUS_VALUES = {
  Unpaid: 1,
  PartiallyPaid: 2,
  Paid: 3,
};

const STATUS_OPTIONS = [
  {
    value: STATUS_VALUES.Unpaid,
    labelKey: i18nKeyContainer.sales.orders.paymentStatus.unpaid,
  },
  {
    value: STATUS_VALUES.PartiallyPaid,
    labelKey: i18nKeyContainer.sales.orders.paymentStatus.partiallypaid,
  },
  {
    value: STATUS_VALUES.Paid,
    labelKey: i18nKeyContainer.sales.orders.paymentStatus.paid,
  },
];

function toFiniteNumber(value, fallback = 0) {
  const numberValue = Number(value);
  return Number.isFinite(numberValue) ? numberValue : fallback;
}

function normalizePaymentStatus(value) {
  const numericValue = Number(value);
  if ([1, 2, 3].includes(numericValue)) {
    return numericValue;
  }

  if (typeof value === 'string') {
    return STATUS_VALUES[value] ?? STATUS_VALUES.Unpaid;
  }

  return STATUS_VALUES.Unpaid;
}

function getExistingAmount(order, totalAmount) {
  const amountCandidates = [
    order?.amount,
    order?.paidAmount,
    order?.paymentAmount,
    order?.amountPaid,
  ];

  const firstFinite = amountCandidates.find(value => Number.isFinite(Number(value)));
  const normalizedAmount = Number.isFinite(Number(firstFinite))
    ? Number(firstFinite)
    : 0;

  if (normalizedAmount < 0) return 0;
  return Math.min(normalizedAmount, totalAmount);
}

export default function UpdatePaymentModal({ isOpen, order, onClose }) {
  const { t, i18n } = useTranslation();
  const updatePayment = useUpdateOrderPayment();
  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';

  const totalAmount = useMemo(
    () => toFiniteNumber(order?.totalAmount ?? order?.total, 0),
    [order]
  );

  const [paymentStatus, setPaymentStatus] = useState(STATUS_VALUES.Unpaid);
  const [amount, setAmount] = useState('0');

  useEffect(() => {
    if (!isOpen || !order) return;

    const initialStatus = normalizePaymentStatus(order.paymentStatus);
    const existingAmount = getExistingAmount(order, totalAmount);

    if (initialStatus === STATUS_VALUES.Paid) {
      setPaymentStatus(STATUS_VALUES.Paid);
      setAmount(String(totalAmount));
      return;
    }

    if (initialStatus === STATUS_VALUES.Unpaid) {
      setPaymentStatus(STATUS_VALUES.Unpaid);
      setAmount('0');
      return;
    }

    setPaymentStatus(STATUS_VALUES.PartiallyPaid);
    setAmount(existingAmount > 0 ? String(existingAmount) : '');
  }, [isOpen, order, totalAmount]);

  if (!isOpen || !order) {
    return null;
  }

  const isAmountLocked =
    paymentStatus === STATUS_VALUES.Paid || paymentStatus === STATUS_VALUES.Unpaid;

  const numericAmount = toFiniteNumber(amount, 0);

  const hasValidAmount = (() => {
    if (paymentStatus === STATUS_VALUES.Paid) {
      return numericAmount === totalAmount;
    }

    if (paymentStatus === STATUS_VALUES.Unpaid) {
      return numericAmount === 0;
    }

    return numericAmount > 0 && numericAmount < totalAmount;
  })();

  const validationMessage = (() => {
    if (paymentStatus === STATUS_VALUES.PartiallyPaid && totalAmount <= 0) {
      return t(i18nKeyContainer.sales.orders.updatePayment.partialAmountInvalid);
    }

    if (!hasValidAmount) {
      return t(i18nKeyContainer.sales.orders.updatePayment.partialAmountInvalid);
    }

    return '';
  })();

  const handleStatusChange = event => {
    const selectedStatus = normalizePaymentStatus(event.target.value);
    setPaymentStatus(selectedStatus);

    if (selectedStatus === STATUS_VALUES.Paid) {
      setAmount(String(totalAmount));
      return;
    }

    if (selectedStatus === STATUS_VALUES.Unpaid) {
      setAmount('0');
      return;
    }

    const currentAmount = toFiniteNumber(amount, 0);
    if (!(currentAmount > 0 && currentAmount < totalAmount)) {
      setAmount('');
    }
  };

  const handleSubmit = async event => {
    event.preventDefault();

    if (!hasValidAmount || updatePayment.isPending) {
      return;
    }

    const payload = {
      paymentStatus,
      amount: numericAmount,
    };

    try {
      const result = await updatePayment.mutateAsync({
        orderId: order.id,
        data: payload,
      });

      if (result.success) {
        onClose();
      }
    } catch {
      // Error handling is managed in useUpdateOrderPayment
    }
  };

  return (
    <div className='fixed inset-0 z-[9999] flex items-center justify-center bg-black/50 backdrop-blur-sm'>
      <div className='relative w-full max-w-md mx-4 bg-white rounded-xl shadow-2xl'>
        <div className='p-6 border-b border-gray-200'>
          <h2 className='text-xl font-semibold text-gray-900'>
            {t(i18nKeyContainer.sales.orders.updatePayment.title)}
          </h2>
          <p className='text-sm text-gray-600 mt-1'>
            {t(i18nKeyContainer.sales.orders.updatePayment.description)}
          </p>
        </div>

        <form onSubmit={handleSubmit} className='p-6 space-y-4'>
          <div>
            <p className='text-sm text-gray-500 mb-1'>
              {t(i18nKeyContainer.sales.orders.updatePayment.totalAmountLabel)}
            </p>
            <p className='text-base font-semibold text-gray-900'>
              {formatDzdCurrency(totalAmount, { locale: activeLocale })}
            </p>
          </div>

          <div>
            <label className='block text-sm font-medium text-gray-700 mb-1'>
              {t(i18nKeyContainer.sales.orders.updatePayment.paymentStatusLabel)}
            </label>
            <select
              value={paymentStatus}
              onChange={handleStatusChange}
              className='block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm'
            >
              {STATUS_OPTIONS.map(option => (
                <option key={option.value} value={option.value}>
                  {t(option.labelKey)}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className='block text-sm font-medium text-gray-700 mb-1'>
              {t(i18nKeyContainer.sales.orders.updatePayment.amountLabel)}
            </label>
            <Input
              type='number'
              min='0'
              step='0.01'
              value={amount}
              onChange={event => setAmount(event.target.value)}
              readOnly={isAmountLocked}
              placeholder={t(
                i18nKeyContainer.sales.orders.updatePayment.amountPlaceholder
              )}
              className='h-10'
            />
            {paymentStatus === STATUS_VALUES.PartiallyPaid && (
              <p className='mt-1 text-xs text-gray-500'>
                {t(i18nKeyContainer.sales.orders.updatePayment.partialAmountHint)}
              </p>
            )}
            {validationMessage && (
              <p className='mt-1 text-sm text-red-600'>{validationMessage}</p>
            )}
          </div>

          <div className='flex items-center justify-end gap-3 pt-3'>
            <Button
              type='button'
              variant='secondary'
              onClick={onClose}
              disabled={updatePayment.isPending}
            >
              {t(i18nKeyContainer.sales.shared.cancel)}
            </Button>
            <Button
              type='submit'
              disabled={!hasValidAmount || updatePayment.isPending}
              loading={updatePayment.isPending}
            >
              {t(i18nKeyContainer.sales.shared.saveChanges)}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
