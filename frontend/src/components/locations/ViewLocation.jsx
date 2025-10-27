import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { MapPin, Calendar, User, Building2 } from 'lucide-react';
import { getLocationById } from '@/services/products/locationService';

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
  const [locationData, setLocationData] = useState({
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
  });
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const fetchLocation = async () => {
      if (locationId) {
        setIsLoading(true);
        try {
          const response = await getLocationById(locationId);
          if (response.success && response.data) {
            setLocationData(response.data);
          }
        } catch (error) {
          console.error('Error fetching location:', error);
        } finally {
          setIsLoading(false);
        }
      }
    };
    fetchLocation();
  }, [locationId]);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className='max-w-3xl max-h-[90vh] overflow-hidden flex flex-col'>
        {/* Header */}
        <DialogHeader>
          <div className='flex items-center gap-2'>
            <MapPin className='w-5 h-5 text-blue-600' />
            <DialogTitle className='text-xl'>Location Details</DialogTitle>
            <span className='text-sm text-gray-500 font-normal'>
              ID: {locationData.id}
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
                  Location Information
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
                    {locationData.isActive ? '✓ Active' : '○ Inactive'}
                  </span>
                  {locationData.isDeleted && (
                    <span className='inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium bg-red-100 text-red-800 ml-2'>
                      🗑 Deleted
                    </span>
                  )}
                </div>

                {/* Location Details */}
                <div className='space-y-4'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Location Name
                    </label>
                    <p className='text-gray-900 text-lg font-medium'>
                      {locationData.name || '-'}
                    </p>
                  </div>

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Address
                    </label>
                    <p className='text-gray-600 bg-gray-50 p-3 rounded-md min-h-[60px]'>
                      {locationData.address || 'No address provided'}
                    </p>
                  </div>

                  <div className='grid grid-cols-2 gap-4'>
                    <div>
                      <label className='block text-sm font-medium text-gray-700 mb-1'>
                        Location Type
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
                        Location Type ID
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
                  Audit Information
                </h3>

                <div className='grid grid-cols-1 md:grid-cols-2 gap-6'>
                  {/* Created Information */}
                  <div className='bg-blue-50 p-4 rounded-lg space-y-3'>
                    <h4 className='font-semibold text-blue-900 flex items-center gap-2'>
                      <Calendar className='w-4 h-4' />
                      Created
                    </h4>
                    <div>
                      <label className='block text-sm font-medium text-blue-700 mb-1'>
                        Date & Time
                      </label>
                      <p className='text-blue-900 text-sm'>
                        {locationData.createdAt
                          ? new Date(locationData.createdAt).toLocaleString(
                              'en-US',
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
                        User
                      </label>
                      <p className='text-blue-900 text-sm'>
                        {locationData.createdByUserName || 'System'}
                      </p>
                    </div>
                    <div>
                      <label className='block text-sm font-medium text-blue-700 mb-1'>
                        User ID
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
                        Deleted
                      </h4>
                      <div>
                        <label className='block text-sm font-medium text-red-700 mb-1'>
                          Date & Time
                        </label>
                        <p className='text-red-900 text-sm'>
                          {locationData.deleteAt
                            ? new Date(locationData.deleteAt).toLocaleString(
                                'en-US',
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
                          User
                        </label>
                        <p className='text-red-900 text-sm'>
                          {locationData.deletedByUserName || '-'}
                        </p>
                      </div>
                      <div>
                        <label className='block text-sm font-medium text-red-700 mb-1'>
                          User ID
                        </label>
                        <p className='text-red-900 font-mono text-xs'>
                          {locationData.deletedByUserId || '-'}
                        </p>
                      </div>
                    </div>
                  ) : (
                    <div className='bg-gray-50 p-4 rounded-lg flex items-center justify-center'>
                      <p className='text-gray-500 text-sm'>
                        Location has not been deleted
                      </p>
                    </div>
                  )}
                </div>
              </div>

              {/* System Information Section */}
              <div className='border-t pt-6'>
                <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                  <Building2 className='w-5 h-5' />
                  System Information
                </h3>

                <div className='grid grid-cols-2 gap-4 bg-gray-50 p-4 rounded-lg'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Location ID
                    </label>
                    <p className='text-gray-900 font-mono text-xs'>
                      {locationData.id || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Location Type ID
                    </label>
                    <p className='text-gray-900 font-mono text-xs'>
                      {locationData.locationTypeId || '-'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Status
                    </label>
                    <p className='text-gray-900 text-sm'>
                      {locationData.isActive ? 'Active' : 'Inactive'}
                    </p>
                  </div>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      Is Deleted
                    </label>
                    <p className='text-gray-900 text-sm'>
                      {locationData.isDeleted ? 'Yes' : 'No'}
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
            Close
          </button>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default ViewLocation;
