import { useQuery } from '@tanstack/react-query';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Tag, Calendar, User } from 'lucide-react';
import { useTranslation } from 'react-i18next';

import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { getUnitOfMeasureById } from '@features/products/services/unitOfMeasureApi';
import { queryKeys } from '@shared/lib/queryKeys';
import { formatAppDate } from '@shared/utils/dateFormatter';

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
  const { t, i18n } = useTranslation();
  const defaultUnitData = {
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
  };

  const { data: unitData = defaultUnitData } = useQuery({
    queryKey: [...queryKeys.products.unitOfMeasure(), 'detail', unitId],
    queryFn: () => getUnitOfMeasureById(unitId),
    enabled: open && Boolean(unitId),
  });

  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className='max-w-3xl max-h-[90vh] overflow-hidden flex flex-col'>
        {/* Header */}
        <DialogHeader>
          <div className='flex items-center gap-2'>
            <Tag className='w-5 h-5' />
            <DialogTitle className='text-xl'>
              {t(i18nKeyContainer.products.units.view.title)}
            </DialogTitle>
            <span className='text-sm text-gray-500 font-normal'>
              {t(i18nKeyContainer.products.units.view.subtitleId, {
                id: unitData.id,
              })}
            </span>
          </div>
        </DialogHeader>

        {/* Content */}
        <div className='flex-1 overflow-y-auto py-6 space-y-6'>
          {/* Unit Information Section */}
          <div>
            <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
              <Tag className='w-5 h-5' />
              {t(i18nKeyContainer.products.units.view.sections.unitInformation)}
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
                ✓{' '}
                {unitData.isActive
                  ? t(i18nKeyContainer.products.shared.status.active)
                  : t(i18nKeyContainer.products.shared.status.inactive)}
              </span>
            </div>

            {/* Unit Details */}
            <div className='space-y-4'>
              <div>
                <label className='block text-sm font-medium text-gray-700 mb-1'>
                  {t(i18nKeyContainer.products.units.view.fields.unitName)}{' '}
                  <span className='text-red-500'>
                    {t(i18nKeyContainer.products.shared.required)}
                  </span>
                </label>
                <p className='text-gray-900 text-lg font-medium'>
                  {unitData.name || t(i18nKeyContainer.products.shared.hyphen)}
                </p>
              </div>

              <div>
                <label className='block text-sm font-medium text-gray-700 mb-1'>
                  {t(i18nKeyContainer.products.units.view.fields.description)}
                </label>
                <p className='text-gray-600 text-sm bg-gray-50 p-3 rounded-md min-h-[60px]'>
                  {unitData.description ||
                    t(i18nKeyContainer.products.units.view.placeholders.noDescription)}
                </p>
              </div>
            </div>
          </div>

          {/* Audit Information Section */}
          <div className='border-t pt-6'>
            <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
              <Calendar className='w-5 h-5' />
              {t(i18nKeyContainer.products.units.view.sections.auditInformation)}
            </h3>

            <div className='grid grid-cols-1 md:grid-cols-2 gap-6'>
              {/* Created Information */}
              <div className='bg-blue-50 p-4 rounded-lg space-y-3'>
                <h4 className='font-semibold text-blue-900 flex items-center gap-2'>
                  <Calendar className='w-4 h-4' />
                  {t(i18nKeyContainer.products.units.view.sections.created)}
                </h4>
                <div>
                  <label className='block text-sm font-medium text-blue-700 mb-1'>
                    {t(i18nKeyContainer.products.units.view.fields.dateTime)}
                  </label>
                  <p className='text-blue-900 text-sm'>
                    {formatAppDate(unitData.createdAt, {
                      locale: activeLocale,
                      withTime: true,
                      fallback: t(i18nKeyContainer.products.shared.hyphen),
                    })}
                  </p>
                </div>
                <div>
                  <label className='text-sm font-medium text-blue-700 mb-1 flex items-center gap-1'>
                    <User className='w-3 h-3' />
                    {t(i18nKeyContainer.products.units.view.fields.user)}
                  </label>
                  <p className='text-blue-900 text-sm'>
                    {unitData.createdByUserName ||
                      t(i18nKeyContainer.products.units.view.placeholders.adminUser)}
                  </p>
                </div>
              </div>

              {/* Updated Information */}
              <div className='bg-purple-50 p-4 rounded-lg space-y-3'>
                <h4 className='font-semibold text-purple-900 flex items-center gap-2'>
                  <Calendar className='w-4 h-4' />
                  {t(i18nKeyContainer.products.units.view.sections.updated)}
                </h4>
                <div>
                  <label className='block text-sm font-medium text-purple-700 mb-1'>
                    {t(i18nKeyContainer.products.units.view.fields.dateTime)}
                  </label>
                  <p className='text-purple-900 text-sm'>
                    {formatAppDate(unitData.updatedAt, {
                      locale: activeLocale,
                      withTime: true,
                      fallback: t(i18nKeyContainer.products.units.view.placeholders.neverUpdated),
                    })}
                  </p>
                </div>
                <div>
                  <label className='text-sm font-medium text-purple-700 mb-1 flex items-center gap-1'>
                    <User className='w-3 h-3' />
                    {t(i18nKeyContainer.products.units.view.fields.user)}
                  </label>
                  <p className='text-purple-900 text-sm'>
                    {unitData.updatedByUserName ||
                      t(i18nKeyContainer.products.shared.hyphen)}
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
            {t(i18nKeyContainer.products.shared.close)}
          </button>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default UnitOfMeasureView;
