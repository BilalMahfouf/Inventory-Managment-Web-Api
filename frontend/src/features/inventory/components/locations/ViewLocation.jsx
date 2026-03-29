import { useQuery } from '@tanstack/react-query';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { MapPin, Calendar, User, Building2 } from 'lucide-react';
import { getLocationById } from '@features/inventory/services/locationApi';
import { useTranslation } from 'react-i18next';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { queryKeys } from '@shared/lib/queryKeys';

/**
 * ViewLocation Component
 *
 * A dialog component to display detailed location information.
 * Shows location details, location type info, and audit details (created and updated information).
 *
 * @param {Object} props - Component props
 * @param {boolean} props.open - Controls dialog visibility
 * @param {Function} props.onOpenChange - Callback when dialog open state changes
 * @param {number} props.locationId - Location ID to display
 *
 * @example
 * ```jsx
 * const [viewDialogOpen, setViewDialogOpen] = useState(false);
 * const [selectedLocationId, setSelectedLocationId] = useState(null);
 *
 * <ViewLocation
 *   open={viewDialogOpen}
 *   onOpenChange={setViewDialogOpen}
 *   locationId={selectedLocationId}
 * />
 * ```
 */
const ViewLocation = ({ open, onOpenChange, locationId }) => {
  const { t, i18n } = useTranslation();
  const defaultLocationData = {
    id: 0,
    name: '',
    address: '',
    isActive: false,
    locationTypeId: 0,
    locationTypeName: '',
    createdAt: null,
    createdByUserId: null,
    createdByUserName: null,
    isDeleted: false,
    deleteAt: null,
    deletedByUserId: null,
    deletedByUserName: null,
  };

  const { data: locationResponse, isLoading } = useQuery({
    queryKey: [...queryKeys.inventory.locations('list'), 'detail', locationId],
    queryFn: () => getLocationById(locationId),
    enabled: open && Boolean(locationId),
  });

  const locationData =
    locationResponse?.success && locationResponse?.data
      ? locationResponse.data
      : defaultLocationData;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className='max-w-3xl max-h-[90vh] overflow-hidden flex flex-col'>
        {/* Header */}
        <DialogHeader>
          <div className='flex items-center gap-2'>
            <MapPin className='w-5 h-5 text-blue-600' />
            <DialogTitle className='text-xl'>
              {t(i18nKeyContainer.inventory.locations.view.title)}
            </DialogTitle>
            <span className='text-sm text-gray-500 font-normal'>
              {t(i18nKeyContainer.inventory.locations.view.subtitleId, {
                id: locationData.id,
              })}
            </span>
          </div>
        </DialogHeader>

        {/* Content */}
        <div className='flex-1 overflow-y-auto py-6 space-y-6'>
          {isLoading ? (
            <div className='flex items-center justify-center py-12'>
              <div className='animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600'></div>
            </div>
          ) : (
            <>
              {/* Location Information Section */}
              <div>
                <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                  <MapPin className='w-5 h-5 text-blue-600' />
                  {t(i18nKeyContainer.inventory.locations.view.sections.locationInformation)}
                </h3>

                {/* Status Badge */}
                <div className='mb-6'>
                  <span
                    className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium ${
                      locationData.isActive
                        ? 'bg-green-100 text-green-800'
                        : 'bg-gray-100 text-gray-800'
                    }`}
                  >
                    {locationData.isActive
                      ? t(i18nKeyContainer.inventory.shared.status.active)
                      : t(i18nKeyContainer.inventory.shared.status.inactive)}
                  </span>
                  {locationData.isDeleted && (
                    <span className='inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium bg-red-100 text-red-800 ml-2'>
                      {t(i18nKeyContainer.inventory.shared.status.deleted)}
                    </span>
                  )}
                </div>

                {/* Location Details */}
                <div className='space-y-4'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.inventory.locations.view.fields.locationName)}
                    </label>
                    <p className='text-gray-900 text-lg font-medium'>
                      {locationData.name || '-'}
                    </p>
                  </div>

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.inventory.locations.view.fields.address)}
                    </label>
                    <p className='text-gray-600 bg-gray-50 p-3 rounded-md min-h-[60px]'>
                      {locationData.address ||
                        t(
                          i18nKeyContainer.inventory.locations.view.placeholders
                            .noAddressProvided
                        )}
                    </p>
                  </div>

                  <div className='grid grid-cols-2 gap-4'>
                    <div>
                      <label className='block text-sm font-medium text-gray-700 mb-1'>
                        {t(i18nKeyContainer.inventory.locations.view.fields.locationType)}
                      </label>
                      <div className='flex items-center gap-2 bg-blue-50 p-3 rounded-md'>
                        <Building2 className='w-4 h-4 text-blue-600' />
                        <p className='text-blue-900 font-medium'>
                          {locationData.locationTypeName || '-'}
                        </p>
                      </div>
                    </div>

                    <div>
                      <label className='block text-sm font-medium text-gray-700 mb-1'>
                        {t(i18nKeyContainer.inventory.locations.view.fields.locationTypeId)}
                      </label>
                      <p className='text-gray-900 font-mono text-sm bg-gray-50 p-3 rounded-md'>
                        {locationData.locationTypeId || '-'}
                      </p>
                    </div>
                  </div>
                </div>
              </div>

              {/* Audit Information Section */}
              <div className='border-t pt-6'>
                <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                  <Calendar className='w-5 h-5' />
                  {t(i18nKeyContainer.inventory.locations.view.sections.auditInformation)}
                </h3>

                <div className='grid grid-cols-1 md:grid-cols-2 gap-6'>
                  {/* Created Information */}
                  <div className='bg-blue-50 p-4 rounded-lg space-y-3'>
                    <h4 className='font-semibold text-blue-900 flex items-center gap-2'>
                      <Calendar className='w-4 h-4' />
                      {t(i18nKeyContainer.inventory.locations.view.sections.created)}
                    </h4>
                    <div>
                      <label className='block text-sm font-medium text-blue-700 mb-1'>
                        {t(i18nKeyContainer.inventory.locations.view.fields.dateTime)}
                      </label>
                      <p className='text-blue-900 text-sm'>
                        {locationData.createdAt
                          ? new Date(locationData.createdAt).toLocaleString(
                              i18n.resolvedLanguage || i18n.language || 'en',
                              {
                                year: 'numeric',
                                month: 'long',
                                day: 'numeric',
                                hour: '2-digit',
                                minute: '2-digit',
                              }
                            )
                          : '-'}
                      </p>
                    </div>
                    <div>
                      <label className='text-sm font-medium text-blue-700 mb-1 flex items-center gap-1'>
                        <User className='w-3 h-3' />
                        {t(i18nKeyContainer.inventory.locations.view.fields.user)}
                      </label>
                      <p className='text-blue-900 text-sm'>
                        {locationData.createdByUserName ||
                          t(i18nKeyContainer.inventory.shared.systemUser)}
                      </p>
                    </div>
                    <div>
                      <label className='block text-sm font-medium text-blue-700 mb-1'>
                        {t(i18nKeyContainer.inventory.locations.view.fields.userId)}
                      </label>
                      <p className='text-blue-900 font-mono text-xs'>
                        {locationData.createdByUserId || '-'}
                      </p>
                    </div>
                  </div>

                  {/* Deleted Information (if applicable) */}
                  {locationData.isDeleted ? (
                    <div className='bg-red-50 p-4 rounded-lg space-y-3'>
                      <h4 className='font-semibold text-red-900 flex items-center gap-2'>
                        <Calendar className='w-4 h-4' />
                        {t(i18nKeyContainer.inventory.locations.view.sections.deleted)}
                      </h4>
                      <div>
                        <label className='block text-sm font-medium text-red-700 mb-1'>
                          {t(i18nKeyContainer.inventory.locations.view.fields.dateTime)}
                        </label>
                        <p className='text-red-900 text-sm'>
                          {locationData.deleteAt
                            ? new Date(locationData.deleteAt).toLocaleString(
                                i18n.resolvedLanguage || i18n.language || 'en',
                                {
                                  year: 'numeric',
                                  month: 'long',
                                  day: 'numeric',
                                  hour: '2-digit',
                                  minute: '2-digit',
                                }
                              )
                            : '-'}
                        </p>
                      </div>
                      <div>
                        <label className='text-sm font-medium text-red-700 mb-1 flex items-center gap-1'>
                          <User className='w-3 h-3' />
                          {t(i18nKeyContainer.inventory.locations.view.fields.user)}
                        </label>
                        <p className='text-red-900 text-sm'>
                          {locationData.deletedByUserName || '-'}
                        </p>
                      </div>
                      <div>
                        <label className='block text-sm font-medium text-red-700 mb-1'>
                          {t(i18nKeyContainer.inventory.locations.view.fields.userId)}
                        </label>
                        <p className='text-red-900 font-mono text-xs'>
                          {locationData.deletedByUserId || '-'}
                        </p>
                      </div>
                    </div>
                  ) : (
                    <div className='bg-gray-50 p-4 rounded-lg flex items-center justify-center'>
                      <p className='text-gray-500 text-sm'>
                        {t(
                          i18nKeyContainer.inventory.locations.view.placeholders
                            .notDeleted
                        )}
                      </p>
                    </div>
                  )}
                </div>
              </div>

              {/* System Information Section */}
              <div className='border-t pt-6'>
                <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                  <Building2 className='w-5 h-5' />
                  {t(i18nKeyContainer.inventory.locations.view.sections.systemInformation)}
                </h3>

                <div className='grid grid-cols-2 gap-4 bg-gray-50 p-4 rounded-lg'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.inventory.locations.view.fields.locationId)}
                    </label>
                    <p className='text-gray-900 font-mono text-xs'>
                      {locationData.id || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.inventory.locations.view.fields.locationTypeId)}
                    </label>
                    <p className='text-gray-900 font-mono text-xs'>
                      {locationData.locationTypeId || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.inventory.locations.view.fields.status)}
                    </label>
                    <p className='text-gray-900 text-sm'>
                      {locationData.isActive
                        ? t(i18nKeyContainer.inventory.shared.status.active)
                        : t(i18nKeyContainer.inventory.shared.status.inactive)}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(i18nKeyContainer.inventory.locations.view.fields.isDeleted)}
                    </label>
                    <p className='text-gray-900 text-sm'>
                      {locationData.isDeleted
                        ? t(i18nKeyContainer.inventory.shared.yes)
                        : t(i18nKeyContainer.inventory.shared.no)}
                    </p>
                  </div>
                </div>
              </div>
            </>
          )}
        </div>

        {/* Footer Actions */}
        <div className='flex justify-end gap-2 border-t pt-4'>
          <button
            onClick={() => onOpenChange(false)}
            className='px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 transition-colors'
          >
            {t(i18nKeyContainer.inventory.shared.close)}
          </button>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default ViewLocation;
