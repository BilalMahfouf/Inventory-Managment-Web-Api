import SimpleDataTable from '@components/DataTable/SimpleDataTable';
import React, { useState, useEffect, useCallback } from 'react';
import {
  getLocations,
  deleteLocation,
} from '@services/products/locationService';
import AddUpdateLocation from './AddUpdateLocation';
import ViewLocation from './ViewLocation';
import { useToast } from '@/context/ToastContext';
import ConfirmationDialog from '../ui/ConfirmationDialog';

export default function LocationDataTable() {
  const [data, setData] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [addEditDialogOpen, setAddEditDialogOpen] = useState(false);
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [currentLocationId, setCurrentLocationId] = useState(0);
  const { showError, showSuccess } = useToast();

  const fetchLocations = useCallback(async () => {
    setIsLoading(true);
    try {
      const response = await getLocations();
      if (!response.success) {
        setData(null);
        return;
      }
      setData(response.data);
    } catch (error) {
      console.error('Error fetching locations:', error);
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchLocations();
  }, [fetchLocations]);

  const handleAddNew = () => {
    setCurrentLocationId(0);
    setAddEditDialogOpen(true);
  };

  const handleView = row => {
    setCurrentLocationId(row.id);
    setViewDialogOpen(true);
  };

  const handleEdit = row => {
    setCurrentLocationId(row.id);
    setAddEditDialogOpen(true);
  };

  const handleCloseAddEdit = () => {
    setAddEditDialogOpen(false);
    setCurrentLocationId(0);
  };

  const handleSuccess = () => {
    fetchLocations(); // Refresh data after successful add/edit
    setAddEditDialogOpen(false);
    setViewDialogOpen(false);
    setDeleteDialogOpen(false);
  };
  const handleDelete = async () => {
    const response = await deleteLocation(currentLocationId);
    if (!response.success) {
      showError(
        'Delete Failed',
        response.message || 'Could not delete location.'
      );
      setDeleteDialogOpen(false);
      return;
    }
    showSuccess(
      'Deleted',
      `Location with ID ${currentLocationId} deleted successfully.`
    );
    setDeleteDialogOpen(false);
  };

  return (
    <>
      <SimpleDataTable
        data={data}
        columns={defaultColumns}
        loading={isLoading}
        enableActions={true}
        enablePagination={true}
        onView={handleView}
        onEdit={handleEdit}
        onDelete={row => {
          setCurrentLocationId(row.id);
          setDeleteDialogOpen(true);
        }}
        onAddNew={handleAddNew}
      />
      {addEditDialogOpen && (
        <AddUpdateLocation
          isOpen={addEditDialogOpen}
          onClose={handleCloseAddEdit}
          locationId={currentLocationId}
          onSuccess={handleSuccess}
        />
      )}
      {viewDialogOpen && (
        <ViewLocation
          open={viewDialogOpen}
          onOpenChange={setViewDialogOpen}
          locationId={currentLocationId}
        />
      )}
      {deleteDialogOpen && (
        <ConfirmationDialog
          isOpen={deleteDialogOpen}
          title='Delete Location'
          message={`Are you sure you want to delete location with ID ${currentLocationId}? This action cannot be undone.`}
          onConfirm={handleDelete}
          onCancel={() => setDeleteDialogOpen(false)}
          onClose={handleSuccess}
        />
      )}
    </>
  );
}

const defaultColumns = [
  {
    accessorKey: 'id',
    header: 'ID',
  },
  {
    accessorKey: 'name',
    header: 'Name',
  },
  {
    accessorKey: 'address',
    header: 'Address',
  },
  {
    accessorKey: 'locationTypeName',
    header: 'Type Name',
  },
  {
    accessorKey: 'createdAt',
    header: 'Created At',
    cell: ({ getValue }) => new Date(getValue()).toLocaleDateString(),
  },
  {
    accessorKey: 'createdByUserName',
    header: 'Created By',
  },
];
