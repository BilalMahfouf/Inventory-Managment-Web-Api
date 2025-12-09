// Example: Integrating ConfirmationDialog into ProductDataTable for Delete Operations

import { useState } from 'react';
import ConfirmationDialog from '@/components/ui/ConfirmationDialog';
import { useToast } from '@/context/ToastContext';
import { deleteProduct } from '@/services/products/productService';

const ProductDataTable = () => {
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
        'Product Deleted',
        `${selectedProduct.name} has been permanently removed.`
      );

      // Close dialog
      setShowDeleteDialog(false);

      // Refresh the product list
      refreshProducts();
    } catch (error) {
      // Show error toast but keep dialog open
      showError(
        'Delete Failed',
        error.message || 'Could not delete the product. Please try again.'
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
        title='Delete Product'
        itemName={selectedProduct?.name}
        message='This action cannot be undone and will permanently remove this product from the system.'
        loading={deleteLoading}
      />
    </>
  );
};

export default ProductDataTable;
