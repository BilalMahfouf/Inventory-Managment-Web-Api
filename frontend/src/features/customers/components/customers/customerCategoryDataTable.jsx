import { useEffect, useMemo, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

import SimpleDataTable from '@components/DataTable/SimpleDataTable';
import ConfirmationDialog from '@components/ui/ConfirmationDialog';
import { useToast } from '@shared/context/ToastContext';
import { queryKeys } from '@shared/lib/queryKeys';
import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import {
  deleteCustomerCategory,
  getAllCustomerCategories,
} from '@features/customers/services/customerCategoryApi';

const normalizeCategory = category => ({
  id: category?.id ?? 0,
  name: category?.name ?? '',
  isIndividual: Boolean(category?.isIndividual),
  description: category?.description ?? '',
  createdOnUtc: category?.createdOnUtc ?? null,
});

const getColumns = (t, locale) => [
  {
    accessorKey: 'name',
    header: t(i18nKeyContainer.customers.categoryManagement.table.columns.name),
  },
  {
    id: 'type',
    accessorFn: row =>
      row.isIndividual
        ? t(i18nKeyContainer.customers.categoryManagement.table.type.individual)
        : t(i18nKeyContainer.customers.categoryManagement.table.type.business),
    header: t(i18nKeyContainer.customers.categoryManagement.table.columns.type),
    cell: ({ row }) => {
      const isIndividual = Boolean(row.original.isIndividual);

      return (
        <span
          className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
            isIndividual ? 'bg-emerald-100 text-emerald-800' : 'bg-blue-100 text-blue-800'
          }`}
        >
          {isIndividual
            ? t(i18nKeyContainer.customers.categoryManagement.table.type.individual)
            : t(i18nKeyContainer.customers.categoryManagement.table.type.business)}
        </span>
      );
    },
  },
  {
    accessorKey: 'description',
    header: t(i18nKeyContainer.customers.categoryManagement.table.columns.description),
    cell: ({ getValue }) => {
      const description = String(getValue() ?? '').trim();

      return description || t(i18nKeyContainer.customers.categoryManagement.table.fallback.noDescription);
    },
  },
  {
    accessorKey: 'createdOnUtc',
    header: t(i18nKeyContainer.customers.categoryManagement.table.columns.createdAt),
    cell: ({ getValue }) => {
      const value = getValue();

      if (!value) {
        return t(i18nKeyContainer.customers.shared.notAvailable);
      }

      const date = new Date(value);
      if (Number.isNaN(date.getTime())) {
        return t(i18nKeyContainer.customers.shared.notAvailable);
      }

      return date.toLocaleDateString(locale);
    },
  },
];

export default function CustomerCategoryDataTable() {
  const { t, i18n } = useTranslation();
  const { showSuccess, showError } = useToast();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [currentCategory, setCurrentCategory] = useState(null);

  const {
    data: categoriesResponse,
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: queryKeys.customers.customerCategories.all(),
    queryFn: async () => {
      const response = await getAllCustomerCategories();

      if (!response.success) {
        throw new Error(
          response.error ||
            t(
              i18nKeyContainer.customers.categoryManagement.table.toasts
                .loadFailedMessage
            )
        );
      }

      return response.data;
    },
  });

  useEffect(() => {
    if (!isError) {
      return;
    }

    showError(
      t(i18nKeyContainer.customers.categoryManagement.table.toasts.loadFailedTitle),
      error?.message ||
        t(i18nKeyContainer.customers.categoryManagement.table.toasts.loadFailedMessage)
    );
  }, [error?.message, isError, showError, t]);

  const deleteMutation = useMutation({
    mutationFn: deleteCustomerCategory,
    onSuccess: async response => {
      if (!response.success) {
        showError(
          t(i18nKeyContainer.customers.categoryManagement.table.toasts.deleteFailedTitle),
          response.error ||
            t(i18nKeyContainer.customers.categoryManagement.table.toasts.deleteFailedMessage)
        );
        setDeleteDialogOpen(false);
        return;
      }

      showSuccess(
        t(i18nKeyContainer.customers.categoryManagement.table.toasts.deleteSuccessTitle),
        t(i18nKeyContainer.customers.categoryManagement.table.toasts.deleteSuccessMessage)
      );
      setDeleteDialogOpen(false);
      await queryClient.invalidateQueries({
        queryKey: queryKeys.customers.customerCategories.all(),
      });
    },
  });

  const activeLocale = i18n.resolvedLanguage || i18n.language || 'en';
  const columns = useMemo(() => getColumns(t, activeLocale), [activeLocale, t]);
  const categories = useMemo(
    () =>
      Array.isArray(categoriesResponse)
        ? categoriesResponse.map(normalizeCategory)
        : [],
    [categoriesResponse]
  );

  return (
    <>
      <SimpleDataTable
        data={categories}
        columns={columns}
        loading={isLoading}
        searchPlaceholder={t(
          i18nKeyContainer.customers.categoryManagement.table.searchPlaceholder
        )}
        actionsColumnHeader={t(
          i18nKeyContainer.customers.categoryManagement.table.columns.actions
        )}
        onView={row => {
          navigate(`/customers/categories/${row.id}`);
        }}
        onEdit={row => {
          navigate(`/customers/categories/${row.id}/edit`);
        }}
        onDelete={row => {
          setCurrentCategory(row);
          setDeleteDialogOpen(true);
        }}
      />

      <ConfirmationDialog
        isOpen={deleteDialogOpen}
        onClose={() => {
          setDeleteDialogOpen(false);
        }}
        onConfirm={() => {
          if (currentCategory?.id) {
            deleteMutation.mutate(currentCategory.id);
          }
        }}
        loading={deleteMutation.isPending}
        type='delete'
        title={t(i18nKeyContainer.customers.categoryManagement.table.dialogs.deleteTitle)}
        itemName={currentCategory?.name}
        message={t(i18nKeyContainer.customers.categoryManagement.table.dialogs.deleteMessage)}
        confirmText={t(i18nKeyContainer.customers.categoryManagement.table.dialogs.confirmDelete)}
        cancelText={t(i18nKeyContainer.customers.categoryManagement.table.dialogs.cancelDelete)}
      />
    </>
  );
}
