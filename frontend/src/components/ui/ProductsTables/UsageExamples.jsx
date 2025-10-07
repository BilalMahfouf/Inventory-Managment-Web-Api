/**
 * USAGE EXAMPLES FOR ProductViewDialog
 *
 * Copy and paste these examples as needed
 */

// ============================================
// EXAMPLE 1: Basic Usage (Already Implemented)
// ============================================

import { useState } from 'react';
import DataTable from '@components/DataTable/DataTable';
import ProductViewDialog from './ProductViewDialog';

export default function ProductDataTable() {
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);

  const handleView = row => {
    setSelectedProduct(row);
    setViewDialogOpen(true);
  };

  return (
    <>
      <DataTable
        data={products}
        columns={columns}
        onView={handleView}
        onEdit={row => console.log('Edit', row)}
        onDelete={row => console.log('Delete', row)}
      />

      <ProductViewDialog
        open={viewDialogOpen}
        onOpenChange={setViewDialogOpen}
        product={selectedProduct}
      />
    </>
  );
}

// ============================================
// EXAMPLE 2: With Inventory Data
// ============================================

export function ProductDataTableWithInventory() {
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);

  const handleView = row => {
    setSelectedProduct(row);
    setViewDialogOpen(true);
  };

  return (
    <>
      <DataTable data={products} columns={columns} onView={handleView} />

      <ProductViewDialog
        open={viewDialogOpen}
        onOpenChange={setViewDialogOpen}
        product={selectedProduct}
        inventory={{
          currentStock: selectedProduct?.currentStock || 0,
          minimumStock: selectedProduct?.minimumStock || 10,
          maximumStock: selectedProduct?.maximumStock || 500,
          storageLocation: selectedProduct?.storageLocation || 'N/A',
        }}
      />
    </>
  );
}

// ============================================
// EXAMPLE 3: With Duplicate Function
// ============================================

export function ProductDataTableWithDuplicate() {
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);
  const navigate = useNavigate(); // If using React Router

  const handleView = row => {
    setSelectedProduct(row);
    setViewDialogOpen(true);
  };

  const handleDuplicate = product => {
    // Option 1: Navigate to create page with pre-filled data
    navigate('/products/create', {
      state: {
        duplicateFrom: product,
      },
    });

    // Option 2: Call API to duplicate
    // await duplicateProduct(product.id);

    // Option 3: Open edit dialog with copied data
    // setEditProduct({ ...product, id: null, sku: `${product.sku}-COPY` });
  };

  return (
    <>
      <DataTable data={products} columns={columns} onView={handleView} />

      <ProductViewDialog
        open={viewDialogOpen}
        onOpenChange={setViewDialogOpen}
        product={selectedProduct}
        onDuplicate={handleDuplicate}
      />
    </>
  );
}

// ============================================
// EXAMPLE 4: Sample Product Data Structure
// ============================================

const sampleProduct = {
  // Basic Info
  id: '123e4567-e89b-12d3-a456-426614174000',
  sku: 'APP-2HD-GEN',
  name: 'AirPods Pro (2nd Gen)',
  description: 'Premium wireless earbuds with active noise cancellation',
  categoryId: '123e4567-e89b-12d3-a456-426614174001',
  categoryName: 'Audio',
  unitOfMeasureId: '123e4567-e89b-12d3-a456-426614174002',
  unitOfMeasureName: 'pcs',

  // Pricing
  costPrice: 180.0,
  unitPrice: 249.0,

  // Status
  isActive: true,

  // Audit Trail
  createdAt: '2024-01-01T10:30:00Z',
  createdByUserId: '123e4567-e89b-12d3-a456-426614174003',
  createdByUserName: 'admin',

  updatedAt: '2024-01-15T14:20:00Z',
  updatedByUserId: '123e4567-e89b-12d3-a456-426614174003',
  updatedByUserName: 'admin',

  // Deletion (if applicable)
  isDeleted: false,
  deleteAt: null,
  deletedByUserId: null,
  deletedByUserName: null,
};

const sampleInventory = {
  currentStock: 2,
  minimumStock: 5,
  maximumStock: 100,
  storageLocation: 'B1-C2-D1',
};

// ============================================
// EXAMPLE 5: Fetch Product Details Before View
// ============================================

export function ProductDataTableWithDetailsFetch() {
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleView = async row => {
    setLoading(true);
    setViewDialogOpen(true);

    try {
      // Fetch complete product details with inventory
      const response = await fetch(`/api/products/${row.id}`);
      const data = await response.json();

      setSelectedProduct(data);
    } catch (error) {
      console.error('Error fetching product details:', error);
      // Fallback to row data
      setSelectedProduct(row);
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <DataTable data={products} columns={columns} onView={handleView} />

      {loading ? (
        <Dialog open={viewDialogOpen} onOpenChange={setViewDialogOpen}>
          <DialogContent>
            <div className='flex items-center justify-center p-8'>
              <div className='animate-spin rounded-full h-8 w-8 border-2 border-blue-500 border-t-transparent' />
              <span className='ml-2'>Loading product details...</span>
            </div>
          </DialogContent>
        </Dialog>
      ) : (
        <ProductViewDialog
          open={viewDialogOpen}
          onOpenChange={setViewDialogOpen}
          product={selectedProduct}
        />
      )}
    </>
  );
}

// ============================================
// EXAMPLE 6: Integration with Edit & Delete
// ============================================

export function ProductDataTableComplete() {
  const [viewDialogOpen, setViewDialogOpen] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);

  const handleView = row => {
    setSelectedProduct(row);
    setViewDialogOpen(true);
  };

  const handleEdit = row => {
    setSelectedProduct(row);
    setEditDialogOpen(true);
  };

  const handleDelete = row => {
    setSelectedProduct(row);
    setDeleteDialogOpen(true);
  };

  const handleDuplicate = product => {
    // Close view dialog and open edit with copied data
    setViewDialogOpen(false);
    setSelectedProduct({
      ...product,
      id: null,
      sku: `${product.sku}-COPY`,
      name: `${product.name} (Copy)`,
    });
    setEditDialogOpen(true);
  };

  return (
    <>
      <DataTable
        data={products}
        columns={columns}
        onView={handleView}
        onEdit={handleEdit}
        onDelete={handleDelete}
      />

      {/* View Dialog */}
      <ProductViewDialog
        open={viewDialogOpen}
        onOpenChange={setViewDialogOpen}
        product={selectedProduct}
        onDuplicate={handleDuplicate}
      />

      {/* Edit Dialog */}
      {/* <ProductEditDialog
        open={editDialogOpen}
        onOpenChange={setEditDialogOpen}
        product={selectedProduct}
      /> */}

      {/* Delete Confirmation Dialog */}
      {/* <ProductDeleteDialog
        open={deleteDialogOpen}
        onOpenChange={setDeleteDialogOpen}
        product={selectedProduct}
      /> */}
    </>
  );
}

// ============================================
// EXAMPLE 7: Standalone Usage (Outside DataTable)
// ============================================

export function ProductDetailsPage() {
  const { productId } = useParams();
  const [product, setProduct] = useState(null);
  const [dialogOpen, setDialogOpen] = useState(false);

  useEffect(() => {
    fetchProduct(productId).then(setProduct);
  }, [productId]);

  return (
    <div>
      <button onClick={() => setDialogOpen(true)}>View Full Details</button>

      <ProductViewDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        product={product}
      />
    </div>
  );
}

// ============================================
// TIPS
// ============================================

/**
 * 1. Always pass the complete product object with all fields
 * 2. Inventory data is optional - dialog will handle missing data gracefully
 * 3. Use onDuplicate callback for duplicate functionality
 * 4. Dialog auto-calculates profit metrics from costPrice and unitPrice
 * 5. Stock status is auto-determined from inventory data
 * 6. All dates are auto-formatted to user's locale
 * 7. Component is fully null-safe - missing fields show '-'
 */
