import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Tag, Calendar, User } from 'lucide-react';
import { getUnitOfMeasureById } from '@/services/products/UnitOfMeasureService';

/**
 * UnitOfMeasureView Component
 *
 * A dialog component to display detailed unit of measure information.
 * Shows unit information and audit details (created and updated information).
 *
 * @param {Object} props - Component props
 * @param {boolean} props.open - Controls dialog visibility
 * @param {Function} props.onOpenChange - Callback when dialog open state changes
 * @param {number} props.unitId - Unit of Measure ID to display
 *
 * @example
 * ```jsx
 * const [viewDialogOpen, setViewDialogOpen] = useState(false);
 * const [selectedUnitId, setSelectedUnitId] = useState(null);
 *
 * <UnitOfMeasureView
 *   open={viewDialogOpen}
 *   onOpenChange={setViewDialogOpen}
 *   unitId={selectedUnitId}
 * />
 * ```
 */
const UnitOfMeasureView = ({ open, onOpenChange, unitId }) => {
  const [unitData, setUnitData] = useState({
    id: 0,
    name: '',
    description: '',
    isActive: false,
    createdAt: null,
    createdByUserId: null,
    createdByUserName: null,
    updatedAt: null,
    updatedByUserId: null,
    updatedByUserName: null,
  });

  useEffect(() => {
    const fetchUnitOfMeasure = async () => {
      if (unitId) {
        try {
          const data = await getUnitOfMeasureById(unitId);
          if (data) {
            setUnitData(data);
          }
        } catch (error) {
          console.error('Error fetching unit of measure:', error);
        }
      }
    };
    fetchUnitOfMeasure();
  }, [unitId]);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className='max-w-3xl max-h-[90vh] overflow-hidden flex flex-col'>
        {/* Header */}
        <DialogHeader>
          <div className='flex items-center gap-2'>
            <Tag className='w-5 h-5' />
            <DialogTitle className='text-xl'>
              Unit of Measure Details
            </DialogTitle>
            <span className='text-sm text-gray-500 font-normal'>
              ID: {unitData.id}
            </span>
          </div>
        </DialogHeader>

        {/* Content */}
        <div className='flex-1 overflow-y-auto py-6 space-y-6'>
          {/* Unit Information Section */}
          <div>
            <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
              <Tag className='w-5 h-5' />
              Unit Information
            </h3>

            {/* Status Badge */}
            <div className='mb-6'>
              <span
                className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium ${
                  unitData.isActive
                    ? 'bg-green-100 text-green-800'
                    : 'bg-gray-100 text-gray-800'
                }`}
              >
                ✓ {unitData.isActive ? 'Active' : 'Inactive'}
              </span>
            </div>

            {/* Unit Details */}
            <div className='space-y-4'>
              <div>
                <label className='block text-sm font-medium text-gray-700 mb-1'>
                  Unit Name <span className='text-red-500'>*</span>
                </label>
                <p className='text-gray-900 text-lg font-medium'>
                  {unitData.name || '-'}
                </p>
              </div>

              <div>
                <label className='block text-sm font-medium text-gray-700 mb-1'>
                  Description
                </label>
                <p className='text-gray-600 text-sm bg-gray-50 p-3 rounded-md min-h-[60px]'>
                  {unitData.description || 'No description provided'}
                </p>
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
                    {unitData.createdAt
                      ? new Date(unitData.createdAt).toLocaleString('en-US', {
                          year: 'numeric',
                          month: 'long',
                          day: 'numeric',
                          hour: '2-digit',
                          minute: '2-digit',
                        })
                      : '-'}
                  </p>
                </div>
                <div>
                  <label className='text-sm font-medium text-blue-700 mb-1 flex items-center gap-1'>
                    <User className='w-3 h-3' />
                    User
                  </label>
                  <p className='text-blue-900 text-sm'>
                    {unitData.createdByUserName || 'Admin User'}
                  </p>
                </div>
              </div>

              {/* Updated Information */}
              <div className='bg-purple-50 p-4 rounded-lg space-y-3'>
                <h4 className='font-semibold text-purple-900 flex items-center gap-2'>
                  <Calendar className='w-4 h-4' />
                  Updated
                </h4>
                <div>
                  <label className='block text-sm font-medium text-purple-700 mb-1'>
                    Date & Time
                  </label>
                  <p className='text-purple-900 text-sm'>
                    {unitData.updatedAt
                      ? new Date(unitData.updatedAt).toLocaleString('en-US', {
                          year: 'numeric',
                          month: 'long',
                          day: 'numeric',
                          hour: '2-digit',
                          minute: '2-digit',
                        })
                      : 'Never updated'}
                  </p>
                </div>
                <div>
                  <label className='text-sm font-medium text-purple-700 mb-1 flex items-center gap-1'>
                    <User className='w-3 h-3' />
                    User
                  </label>
                  <p className='text-purple-900 text-sm'>
                    {unitData.updatedByUserName || '-'}
                  </p>
                </div>
              </div>
            </div>
          </div>
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

export default UnitOfMeasureView;
