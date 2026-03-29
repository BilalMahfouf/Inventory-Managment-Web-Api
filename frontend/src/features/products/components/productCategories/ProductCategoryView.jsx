import { useQuery } from '@tanstack/react-query';
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { FolderTree, Calendar, User, Folder, ChevronRight } from 'lucide-react';
import { useTranslation } from 'react-i18next';

import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { getProductCategoryById } from '@features/products/services/productCategoryApi';
import { queryKeys } from '@shared/lib/queryKeys';

/**
 * ProductCategoryView Component
 *
 * A dialog component to display detailed product category information.
 * Shows category information, audit details (created and updated information),
 * and either subcategories (for MainCategory) or parent category (for SubCategory).
 *
 * @param {Object} props - Component props
 * @param {boolean} props.open - Controls dialog visibility
 * @param {Function} props.onOpenChange - Callback when dialog open state changes
 * @param {number} props.categoryId - Product Category ID to display
 *
 * @example
 * ```jsx
 * const [viewDialogOpen, setViewDialogOpen] = useState(false);
 * const [selectedCategoryId, setSelectedCategoryId] = useState(null);
 *
 * <ProductCategoryView
 *   open={viewDialogOpen}
 *   onOpenChange={setViewDialogOpen}
 *   categoryId={selectedCategoryId}
 * />
 * ```
 */
const ProductCategoryView = ({ open, onOpenChange, categoryId }) => {
  const { t, i18n } = useTranslation();
  const defaultCategoryData = {
    id: 0,
    name: '',
    description: '',
    type: '',
    parentId: null,
    parentName: '',
    subCategories: [],
    createdAt: null,
    createdByUserId: null,
    createdByUserName: null,
    updatedByUserId: null,
    updatedByUserName: null,
  };

  const { data, isLoading } = useQuery({
    queryKey: [...queryKeys.products.categories(), 'detail', categoryId],
    queryFn: () => getProductCategoryById(categoryId),
    enabled: open && Boolean(categoryId),
  });

  const categoryData = data
    ? {
        id: data.id,
        name: data.name,
        description: data.description,
        type: data.type,
        parentId: data.parentId,
        parentName: data.parentName,
        subCategories: data.subCategories || [],
        createdAt: data.createdAt,
        createdByUserId: data.createdByUserId,
        createdByUserName: data.createdByUserName,
        updatedByUserId: data.updatedByUserId,
        updatedByUserName: data.updatedByUserName,
      }
    : defaultCategoryData;

  const isMainCategory = categoryData.type === 'MainCategory';
  const isSubCategory = categoryData.type === 'SubCategory';
  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className='max-w-4xl max-h-[90vh] overflow-hidden flex flex-col'>
        {/* Header */}
        <DialogHeader>
          <div className='flex items-center gap-2'>
            <FolderTree className='w-5 h-5' />
            <DialogTitle className='text-xl'>
              {t(i18nKeyContainer.products.categories.view.title)}
            </DialogTitle>
            <span className='text-sm text-gray-500 font-normal'>
              {t(i18nKeyContainer.products.categories.view.subtitleId, {
                id: categoryData.id,
              })}
            </span>
          </div>
        </DialogHeader>

        {/* Content */}
        <div className='flex-1 overflow-y-auto py-6 space-y-6'>
          {isLoading ? (
            <div className='flex justify-center items-center py-8'>
              <div className='text-gray-500'>
                {t(i18nKeyContainer.products.categories.view.loading)}
              </div>
            </div>
          ) : (
            <>
              {/* Category Information Section */}
              <div>
                <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                  <FolderTree className='w-5 h-5' />
                  {t(
                    i18nKeyContainer.products.categories.view.sections
                      .categoryInformation
                  )}
                </h3>

                {/* Type Badge */}
                <div className='mb-6'>
                  <span
                    className={`inline-flex items-center gap-1 px-3 py-1 rounded-full text-xs font-medium ${
                      isMainCategory
                        ? 'bg-blue-100 text-blue-800'
                        : 'bg-purple-100 text-purple-800'
                    }`}
                  >
                    <Folder className='w-3 h-3' />
                    {isMainCategory
                      ? t(
                          i18nKeyContainer.products.categories.view.badges
                            .mainCategory
                        )
                      : t(
                          i18nKeyContainer.products.categories.view.badges
                            .subCategory
                        )}
                  </span>
                </div>

                {/* Category Details */}
                <div className='space-y-4'>
                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(
                        i18nKeyContainer.products.categories.view.fields
                          .categoryName
                      )}{' '}
                      <span className='text-red-500'>
                        {t(i18nKeyContainer.products.shared.required)}
                      </span>
                    </label>
                    <p className='text-gray-900 text-lg font-medium'>
                      {categoryData.name ||
                        t(i18nKeyContainer.products.shared.hyphen)}
                    </p>
                  </div>

                  {/* Parent Category (for SubCategory) */}
                  {isSubCategory && (
                    <div>
                      <label className='block text-sm font-medium text-gray-700 mb-1'>
                        {t(
                          i18nKeyContainer.products.categories.view.fields
                            .parentCategory
                        )}
                      </label>
                      <div className='flex items-center gap-2 bg-gray-50 p-3 rounded-md'>
                        <Folder className='w-4 h-4 text-blue-600' />
                        <span className='text-gray-900 font-medium'>
                          {categoryData.parentName}
                        </span>
                      </div>
                    </div>
                  )}

                  <div>
                    <label className='block text-sm font-medium text-gray-700 mb-1'>
                      {t(
                        i18nKeyContainer.products.categories.view.fields
                          .description
                      )}
                    </label>
                    <p className='text-gray-600 text-sm bg-gray-50 p-3 rounded-md min-h-[60px]'>
                      {categoryData.description ||
                        t(
                          i18nKeyContainer.products.categories.view.placeholders
                            .noDescription
                        )}
                    </p>
                  </div>
                </div>
              </div>

              {/* Subcategories Section (for MainCategory) */}
              {isMainCategory && categoryData.subCategories.length > 0 && (
                <div className='border-t pt-6'>
                  <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                    <Folder className='w-5 h-5' />
                    {t(i18nKeyContainer.products.categories.view.sections.subcategories)}
                    {' '}
                    ({categoryData.subCategories.length})
                  </h3>
                  <div className='space-y-2'>
                    {categoryData.subCategories.map((subCategory, index) => (
                      <div
                        key={index}
                        className='flex items-center gap-3 bg-white border border-gray-200 p-3 rounded-lg hover:bg-gray-50 transition-colors'
                      >
                        <Folder className='w-4 h-4 text-purple-600' />
                        <div className='flex-1'>
                          <p className='text-gray-900 font-medium'>
                            {subCategory.subCategoryName}
                          </p>
                          <p className='text-xs text-gray-500'>
                            ID: {subCategory.subCategoryId}
                          </p>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              )}

              {/* No Subcategories Message */}
              {isMainCategory && categoryData.subCategories.length === 0 && (
                <div className='border-t pt-6'>
                  <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                    <Folder className='w-5 h-5' />
                    {t(i18nKeyContainer.products.categories.view.sections.subcategories)}
                    {' '}
                    (0)
                  </h3>
                  <div className='bg-gray-50 p-6 rounded-lg text-center'>
                    <Folder className='w-12 h-12 text-gray-300 mx-auto mb-2' />
                    <p className='text-gray-500 text-sm'>
                      {t(
                        i18nKeyContainer.products.categories.view.placeholders
                          .noSubcategories
                      )}
                    </p>
                  </div>
                </div>
              )}

              {/* Audit Information Section */}
              <div className='border-t pt-6'>
                <h3 className='text-lg font-semibold mb-4 flex items-center gap-2'>
                  <Calendar className='w-5 h-5' />
                  {t(
                    i18nKeyContainer.products.categories.view.sections
                      .auditInformation
                  )}
                </h3>

                <div className='grid grid-cols-1 md:grid-cols-2 gap-6'>
                  {/* Created Information */}
                  <div className='bg-blue-50 p-4 rounded-lg space-y-3'>
                    <h4 className='font-semibold text-blue-900 flex items-center gap-2'>
                      <Calendar className='w-4 h-4' />
                      {t(i18nKeyContainer.products.categories.view.sections.created)}
                    </h4>
                    <div>
                      <label className='block text-sm font-medium text-blue-700 mb-1'>
                        {t(i18nKeyContainer.products.categories.view.fields.dateTime)}
                      </label>
                      <p className='text-blue-900 text-sm'>
                        {categoryData.createdAt
                          ? new Date(categoryData.createdAt).toLocaleString(
                              activeLocale,
                              {
                                year: 'numeric',
                                month: 'long',
                                day: 'numeric',
                                hour: '2-digit',
                                minute: '2-digit',
                              }
                            )
                          : t(i18nKeyContainer.products.shared.hyphen)}
                      </p>
                    </div>
                    <div>
                      <label className='text-sm font-medium text-blue-700 mb-1 flex items-center gap-1'>
                        <User className='w-3 h-3' />
                        {t(i18nKeyContainer.products.categories.view.fields.user)}
                      </label>
                      <p className='text-blue-900 text-sm'>
                        {categoryData.createdByUserName ||
                          t(
                            i18nKeyContainer.products.categories.view.placeholders
                              .adminUser
                          )}
                      </p>
                    </div>
                  </div>

                  {/* Updated Information */}
                  <div className='bg-purple-50 p-4 rounded-lg space-y-3'>
                    <h4 className='font-semibold text-purple-900 flex items-center gap-2'>
                      <Calendar className='w-4 h-4' />
                      {t(i18nKeyContainer.products.categories.view.sections.updated)}
                    </h4>
                    <div>
                      <label className='block text-sm font-medium text-purple-700 mb-1'>
                        {t(i18nKeyContainer.products.categories.view.fields.dateTime)}
                      </label>
                      <p className='text-purple-900 text-sm'>
                        {categoryData.updatedByUserId
                          ? new Date(categoryData.updatedAt).toLocaleString(
                              activeLocale,
                              {
                                year: 'numeric',
                                month: 'long',
                                day: 'numeric',
                                hour: '2-digit',
                                minute: '2-digit',
                              }
                            )
                          : t(
                              i18nKeyContainer.products.categories.view
                                .placeholders.neverUpdated
                            )}
                      </p>
                    </div>
                    <div>
                      <label className='text-sm font-medium text-purple-700 mb-1 flex items-center gap-1'>
                        <User className='w-3 h-3' />
                        {t(i18nKeyContainer.products.categories.view.fields.user)}
                      </label>
                      <p className='text-purple-900 text-sm'>
                        {categoryData.updatedByUserName ||
                          t(i18nKeyContainer.products.shared.hyphen)}
                      </p>
                    </div>
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
            {t(i18nKeyContainer.products.shared.close)}
          </button>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default ProductCategoryView;
