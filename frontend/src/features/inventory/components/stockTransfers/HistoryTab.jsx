import React from 'react';
import { History, Clock, User, TrendingUp } from 'lucide-react';
import Button from '@components/Buttons/Button';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

/**
 * HistoryTab Component
 *
 * Read-only tab displaying stock transfer history and status information.
 * Shows transfer status, timestamps, and user information.
 *
 * @param {Object} props - Component props
 * @param {Object} props.transferData - Stock transfer data object
 * @param {string} props.transferData.status - Current transfer status
 * @param {string} props.transferData.createdAt - Creation timestamp
 * @param {string} props.transferData.createdByUserName - Name of user who created transfer
 * @param {number} props.transferData.createdByUserId - ID of user who created transfer
 * @param {number} props.transferData.id - Transfer ID
 * @param {function} props.onStatusChange - Callback when status change is requested
 * @param {boolean} props.canChangeStatus - Whether status can be changed
 * @param {string} props.mode - Component mode ('add' or 'view')
 */
const HistoryTab = ({
  transferData = null,
  onStatusChange,
  canChangeStatus = false,
  mode = 'add',
}) => {
  const { t, i18n } = useTranslation();
  // Get status color class
  const getStatusColor = status => {
    switch (status) {
      case 'Pending':
        return 'bg-yellow-100 text-yellow-800 border-yellow-300';
      case 'Approved':
        return 'bg-blue-100 text-blue-800 border-blue-300';
      case 'InTransit':
        return 'bg-purple-100 text-purple-800 border-purple-300';
      case 'Completed':
        return 'bg-green-100 text-green-800 border-green-300';
      case 'Cancelled':
        return 'bg-gray-100 text-gray-800 border-gray-300';
      case 'Rejected':
        return 'bg-red-100 text-red-800 border-red-300';
      case 'Failed':
        return 'bg-red-100 text-red-800 border-red-300';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-300';
    }
  };

  // Format status display name
  const formatStatus = status => {
    if (!status) {
      return t(i18nKeyContainer.inventory.shared.notSpecified);
    }

    const statusMap = {
      Pending: t(i18nKeyContainer.inventory.shared.status.pending),
      Approved: t(i18nKeyContainer.inventory.shared.status.approved),
      InTransit: t(i18nKeyContainer.inventory.shared.status.inTransit),
      Completed: t(i18nKeyContainer.inventory.shared.status.completed),
      Cancelled: t(i18nKeyContainer.inventory.shared.status.cancelled),
      Rejected: t(i18nKeyContainer.inventory.shared.status.rejected),
      Failed: t(i18nKeyContainer.inventory.shared.status.failed),
    };

    if (statusMap[status]) {
      return statusMap[status];
    }

    return status.replace(/([A-Z])/g, ' $1').trim();
  };

  // Format date
  const formatDate = dateString => {
    if (!dateString) return t(i18nKeyContainer.inventory.shared.notAvailable);
    const date = new Date(dateString);
    return date.toLocaleString(i18n.resolvedLanguage || i18n.language || 'en', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  // If in add mode, show informational message
  if (mode === 'add' || !transferData) {
    return (
      <div>
        <div className='flex items-center gap-2 mb-6'>
          <History className='h-5 w-5' />
          <h3 className='text-lg font-semibold'>
            {t(i18nKeyContainer.inventory.stockTransfers.form.sections.transferHistory)}
          </h3>
        </div>

        <div className='text-center py-12 text-gray-500'>
          <History className='h-16 w-16 mx-auto mb-4 text-gray-400' />
          <h4 className='text-lg font-medium mb-2'>
            {t(
              i18nKeyContainer.inventory.stockTransfers.form.sections
                .noHistoryAvailable
            )}
          </h4>
          <p className='text-sm'>
            {t(
              i18nKeyContainer.inventory.stockTransfers.form.placeholders
                .noHistoryMessage
            )}
          </p>
        </div>
      </div>
    );
  }

  return (
    <div>
      <div className='flex items-center gap-2 mb-6'>
        <History className='h-5 w-5' />
        <h3 className='text-lg font-semibold'>
          {t(i18nKeyContainer.inventory.stockTransfers.form.sections.transferHistory)}
        </h3>
      </div>

      {/* Transfer Status Section */}
      <div className='bg-white border border-gray-200 rounded-lg p-6 mb-6'>
        <div className='flex items-center justify-between mb-4'>
          <h4 className='font-semibold text-gray-900'>
            {t(i18nKeyContainer.inventory.stockTransfers.form.sections.currentStatus)}
          </h4>
          {canChangeStatus && (
            <Button
              variant='secondary'
              onClick={onStatusChange}
              className='text-sm'
            >
              <TrendingUp className='h-4 w-4 mr-2' />
              {t(i18nKeyContainer.inventory.stockTransfers.form.actions.changeStatus)}
            </Button>
          )}
        </div>

        <div className='flex items-center gap-3'>
          <span
            className={`px-4 py-2 rounded-lg text-sm font-medium border ${getStatusColor(
              transferData.status
            )}`}
          >
            {formatStatus(transferData.status)}
          </span>
        </div>
      </div>

      {/* Transfer Timeline */}
      <div className='bg-gray-50 rounded-lg p-6 border border-gray-200 mb-6'>
        <h4 className='font-semibold text-gray-900 mb-4'>
          {t(i18nKeyContainer.inventory.stockTransfers.form.sections.transferTimeline)}
        </h4>

        <div className='space-y-4'>
          {/* Creation Event */}
          <div className='flex gap-4'>
            <div className='flex-shrink-0'>
              <div className='w-10 h-10 rounded-full bg-blue-100 flex items-center justify-center'>
                <Clock className='h-5 w-5 text-blue-600' />
              </div>
            </div>
            <div className='flex-1'>
              <div className='flex items-center justify-between mb-1'>
                <h5 className='font-medium text-gray-900'>
                  {t(
                    i18nKeyContainer.inventory.stockTransfers.form.sections
                      .transferCreated
                  )}
                </h5>
                <span className='text-sm text-gray-500'>
                  {formatDate(transferData.createdAt)}
                </span>
              </div>
              <div className='flex items-center gap-2 text-sm text-gray-600'>
                <User className='h-4 w-4' />
                <span>
                  {transferData.createdByUserName ||
                    t(i18nKeyContainer.inventory.shared.unknownUser)}
                </span>
              </div>
            </div>
          </div>

          {/* Current Status Event */}
          {transferData.status !== 'Pending' && (
            <div className='flex gap-4'>
              <div className='flex-shrink-0'>
                <div
                  className={`w-10 h-10 rounded-full flex items-center justify-center ${
                    transferData.status === 'Completed'
                      ? 'bg-green-100'
                      : transferData.status === 'Cancelled' ||
                          transferData.status === 'Rejected' ||
                          transferData.status === 'Failed'
                        ? 'bg-red-100'
                        : 'bg-yellow-100'
                  }`}
                >
                  <TrendingUp
                    className={`h-5 w-5 ${
                      transferData.status === 'Completed'
                        ? 'text-green-600'
                        : transferData.status === 'Cancelled' ||
                            transferData.status === 'Rejected' ||
                            transferData.status === 'Failed'
                          ? 'text-red-600'
                          : 'text-yellow-600'
                    }`}
                  />
                </div>
              </div>
              <div className='flex-1'>
                <div className='flex items-center justify-between mb-1'>
                  <h5 className='font-medium text-gray-900'>
                    {t(
                      i18nKeyContainer.inventory.stockTransfers.form.labels
                        .statusWithValue,
                      {
                        status: formatStatus(transferData.status),
                      }
                    )}
                  </h5>
                  <span className='text-sm text-gray-500'>
                    {formatDate(transferData.createdAt)}
                  </span>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* Transfer Information */}
      <div className='bg-white border border-gray-200 rounded-lg p-6'>
        <h4 className='font-semibold text-gray-900 mb-4'>
          {t(i18nKeyContainer.inventory.stockTransfers.form.sections.transferInformation)}
        </h4>

        <div className='grid grid-cols-1 md:grid-cols-2 gap-4'>
          <div>
            <p className='text-sm text-gray-600 mb-1'>
              {t(i18nKeyContainer.inventory.stockTransfers.form.fields.transferId)}
            </p>
            <p className='font-medium text-gray-900'>#{transferData.id}</p>
          </div>
          <div>
            <p className='text-sm text-gray-600 mb-1'>
              {t(i18nKeyContainer.inventory.stockTransfers.form.fields.createdBy)}
            </p>
            <p className='font-medium text-gray-900'>
              {transferData.createdByUserName ||
                t(i18nKeyContainer.inventory.shared.notSpecified)}
            </p>
          </div>
          <div>
            <p className='text-sm text-gray-600 mb-1'>
              {t(i18nKeyContainer.inventory.stockTransfers.form.fields.userId)}
            </p>
            <p className='font-medium text-gray-900'>
              #{transferData.createdByUserId || t(i18nKeyContainer.inventory.shared.notAvailable)}
            </p>
          </div>
          <div>
            <p className='text-sm text-gray-600 mb-1'>
              {t(i18nKeyContainer.inventory.stockTransfers.form.fields.createdDate)}
            </p>
            <p className='font-medium text-gray-900'>
              {formatDate(transferData.createdAt)}
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default HistoryTab;
