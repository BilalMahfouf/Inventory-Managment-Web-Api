import DataTable from '@components/DataTable/DataTable';
import { useState, useEffect } from 'react';
import { getAllProducts } from '@services/products/productService';

export default function ProductDataTable() {
  const [products, setProducts] = useState([]);
  const [columns, setColumns] = useState(defaultColumns);
  const [loading, setLoading] = useState(true);
  useEffect(() => {
    const fetchProducts = async () => {
      setLoading(true);
      const data = await getAllProducts();
      if (data && data.length > 0) {
        setProducts(data);
        setLoading(false);
      }
    };
    fetchProducts();
  }, []);

  return (
    <>
      {loading ? (
        <div>Loading products...</div>
      ) : (
        <DataTable data={products} columns={columns} />
      )}
    </>
  );
}

const defaultColumns = [
  {
    accessorKey: 'sku',
    header: 'SKU',
  },
  {
    accessorKey: 'name',
    header: 'Product',
    cell: ({ row }) => (
      <div>
        <div className='font-medium'>{row.original.name}</div>
        <div className='text-sm text-gray-500'>{row.original.categoryName}</div>
      </div>
    ),
  },
  {
    accessorKey: 'unitPrice',
    header: 'Price',
    cell: ({ getValue }) => `$${getValue().toFixed(2)}`,
  },
  {
    accessorKey: 'costPrice',
    header: 'Cost',
    cell: ({ getValue }) => `$${getValue().toFixed(2)}`,
  },

  {
    accessorKey: 'isActive',
    header: 'Status',
    cell: ({ getValue }) => (
      <span className='inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800'>
        ✓ {getValue()}
      </span>
    ),
  },
  {
    accessorKey: 'createdAt',
    header: 'Created At',
    cell: ({ getValue }) => new Date(getValue()).toLocaleDateString(),
  },
  {
    id: 'actions',
    header: 'Actions',
    cell: () => (
      <button className='text-gray-400 hover:text-gray-600'>•••</button>
    ),
  },
];
