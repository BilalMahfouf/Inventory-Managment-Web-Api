/* eslint-disable */
// Example: Integrating ConfirmationDialog into ProductDataTable for Delete Operations

import { useState } from 'react';
import ConfirmationDialog from '@/components/ui/ConfirmationDialog';
import { useToast } from '@shared/context/ToastContext';
import { deleteProduct } from '@features/products/services/productApi';
import { useTranslation } from 'react-i18next';

import i18nKeyContainer from '@shared/lib/i18n/keyContainer';

const ProductDataTable = () => {
  const { t } = useTranslation();
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [deleteLoading, setDeleteLoading] = useState(false);
  const { showSuccess, showError } = useToast();

  // Handle delete button click from DataTable actions
  const handleDeleteClick = product => {
    setSelectedProduct(product);
    setShowDeleteDialog(true);
  };

  // Handle confirm delete
  const handleConfirmDelete = async () => {
    setDeleteLoading(true);

    try {
      await deleteProduct(selectedProduct.id);

      // Show success toast
      showSuccess(
        t(i18nKeyContainer.products.demo.deleteExample.toastSuccessTitle),
        t(i18nKeyContainer.products.demo.deleteExample.toastSuccessMessage, {
          name: selectedProduct.name,
        })
      );

      // Close dialog
      setShowDeleteDialog(false);

      // Refresh the product list
      refreshProducts();
    } catch (error) {
      // Show error toast but keep dialog open
      showError(
        t(i18nKeyContainer.products.demo.deleteExample.toastErrorTitle),
        error.message ||
          t(i18nKeyContainer.products.demo.deleteExample.toastErrorMessage)
      );
    } finally {
      setDeleteLoading(false);
    }
  };

  // Handle dialog close/cancel
  const handleCloseDialog = () => {
    if (!deleteLoading) {
      setShowDeleteDialog(false);
      setSelectedProduct(null);
    }
  };

  return (
    <>
      {/* Your DataTable component */}
      <DataTable
        data={products}
        columns={columns}
        onDelete={handleDeleteClick}
        // ... other props
      />

      {/* Delete Confirmation Dialog */}
      <ConfirmationDialog
        isOpen={showDeleteDialog}
        onClose={handleCloseDialog}
        onConfirm={handleConfirmDelete}
        type='delete'
        title={t(i18nKeyContainer.products.demo.deleteExample.title)}
        itemName={selectedProduct?.name}
        message={t(i18nKeyContainer.products.demo.deleteExample.message)}
        loading={deleteLoading}
      />
    </>
  );
};

export default ProductDataTable;
