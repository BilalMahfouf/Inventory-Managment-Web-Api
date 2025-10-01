import { useState, useEffect } from 'react';
import {
  useReactTable,
  getCoreRowModel,
  flexRender,
} from '@tanstack/react-table';

const DataTable = ({
  data = [],
  columns = [],
  // Server-side pagination props
  totalRows = 0,
  pageIndex = 0,
  pageSize = 10,
  onPageChange = () => {},
  onPageSizeChange = () => {},
  onSortingChange = () => {},
  onFilterChange = () => {},
  // UI control props
  enableSorting = true,
  enableFiltering = true,
  enablePagination = true,
  searchPlaceholder = 'Search...',
  className = '',
  loading = false,
  // Debounce delay for search (ms)
  searchDebounceMs = 500,
  // Available page sizes
  pageSizes = [10, 20, 30, 40, 50],
}) => {
  const [sorting, setSorting] = useState([]);
  const [globalFilter, setGlobalFilter] = useState('');
  const [searchTimeout, setSearchTimeout] = useState(null);

  // Handle sorting changes - reset page to initial value when sorting
  const handleSortingChange = updater => {
    const newSorting =
      typeof updater === 'function' ? updater(sorting) : updater;
    setSorting(newSorting);
    // Call the parent's sorting handler which should reset the page
    onSortingChange(newSorting);
  };

  // Handle search with debouncing - reset page to initial value when filtering
  const handleFilterChange = value => {
    setGlobalFilter(value);

    // Clear existing timeout
    if (searchTimeout) {
      clearTimeout(searchTimeout);
    }

    // Set new timeout for debounced search
    const timeout = setTimeout(() => {
      // Call the parent's filter handler which should reset the page
      onFilterChange(value);
    }, searchDebounceMs);

    setSearchTimeout(timeout);
  };

  // Cleanup timeout on unmount
  useEffect(() => {
    return () => {
      if (searchTimeout) {
        clearTimeout(searchTimeout);
      }
    };
  }, [searchTimeout]);

  // Calculate page count
  const pageCount = Math.ceil(totalRows / pageSize);

  const table = useReactTable({
    data,
    columns,
    pageCount,
    state: {
      sorting,
      globalFilter,
      pagination: {
        pageIndex,
        pageSize,
      },
    },
    manualPagination: true,
    manualSorting: enableSorting,
    manualFiltering: enableFiltering,
    onSortingChange: handleSortingChange,
    onGlobalFilterChange: setGlobalFilter,
    getCoreRowModel: getCoreRowModel(),
    // Don't use client-side sorting/filtering since we're doing server-side
    getSortedRowModel: undefined,
    getFilteredRowModel: undefined,
  });

  return (
    <div className={`w-full ${className}`}>
      {/* Search Input */}
      {enableFiltering && (
        <div className='mb-4'>
          <div className='relative'>
            <input
              type='text'
              placeholder={searchPlaceholder}
              value={globalFilter ?? ''}
              onChange={e => handleFilterChange(e.target.value)}
              className='w-full max-w-sm px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent'
              disabled={loading}
            />
            {loading && (
              <div className='absolute right-3 top-1/2 transform -translate-y-1/2'>
                <div className='animate-spin rounded-full h-4 w-4 border-2 border-blue-500 border-t-transparent'></div>
              </div>
            )}
          </div>
        </div>
      )}

      {/* Table */}
      <div className='overflow-x-auto border border-gray-200 rounded-lg'>
        <table className='w-full bg-white'>
          <thead className='bg-gray-50 border-b border-gray-200'>
            {table.getHeaderGroups().map(headerGroup => (
              <tr key={headerGroup.id}>
                {headerGroup.headers.map(header => (
                  <th
                    key={header.id}
                    className='px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider'
                  >
                    {header.isPlaceholder ? null : (
                      <div
                        {...{
                          className: header.column.getCanSort()
                            ? 'cursor-pointer select-none flex items-center gap-2'
                            : '',
                          onClick: header.column.getToggleSortingHandler(),
                        }}
                      >
                        {flexRender(
                          header.column.columnDef.header,
                          header.getContext()
                        )}
                        {enableSorting && (
                          <span className='text-gray-400'>
                            {{
                              asc: '↑',
                              desc: '↓',
                            }[header.column.getIsSorted()] ?? '↕'}
                          </span>
                        )}
                      </div>
                    )}
                  </th>
                ))}
              </tr>
            ))}
          </thead>
          <tbody className='bg-white divide-y divide-gray-200'>
            {loading ? (
              <tr>
                <td colSpan={columns.length} className='px-6 py-12 text-center'>
                  <div className='flex items-center justify-center'>
                    <div className='animate-spin rounded-full h-8 w-8 border-2 border-blue-500 border-t-transparent'></div>
                    <span className='ml-2 text-gray-500'>Loading...</span>
                  </div>
                </td>
              </tr>
            ) : table.getRowModel().rows.length === 0 ? (
              <tr>
                <td
                  colSpan={columns.length}
                  className='px-6 py-12 text-center text-gray-500'
                >
                  No data available
                </td>
              </tr>
            ) : (
              table.getRowModel().rows.map(row => (
                <tr key={row.id} className='hover:bg-gray-50'>
                  {row.getVisibleCells().map(cell => (
                    <td
                      key={cell.id}
                      className='px-6 py-4 whitespace-nowrap text-sm text-gray-900'
                    >
                      {flexRender(
                        cell.column.columnDef.cell,
                        cell.getContext()
                      )}
                    </td>
                  ))}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* Pagination */}
      {enablePagination && (
        <div className='flex items-center justify-between mt-4'>
          <div className='flex items-center gap-2'>
            <span className='text-sm text-gray-700'>
              Showing {pageIndex * pageSize + 1} to{' '}
              {Math.min((pageIndex + 1) * pageSize, totalRows)} of {totalRows}{' '}
              entries
            </span>
          </div>

          <div className='flex items-center gap-2'>
            <button
              onClick={() => onPageChange(0)}
              disabled={pageIndex === 0 || loading}
              className='px-3 py-1 text-sm border border-gray-300 rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50'
            >
              {'<<'}
            </button>
            <button
              onClick={() => onPageChange(pageIndex - 1)}
              disabled={pageIndex === 0 || loading}
              className='px-3 py-1 text-sm border border-gray-300 rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50'
            >
              Previous
            </button>

            <span className='flex items-center gap-1 text-sm'>
              <span>Page</span>
              <strong>
                {pageIndex + 1} of {pageCount || 1}
              </strong>
            </span>

            <button
              onClick={() => onPageChange(pageIndex + 1)}
              disabled={pageIndex >= pageCount - 1 || loading}
              className='px-3 py-1 text-sm border border-gray-300 rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50'
            >
              Next
            </button>
            <button
              onClick={() => onPageChange(pageCount - 1)}
              disabled={pageIndex >= pageCount - 1 || loading}
              className='px-3 py-1 text-sm border border-gray-300 rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50'
            >
              {'>>'}
            </button>

            <select
              value={pageSize}
              onChange={e => onPageSizeChange(Number(e.target.value))}
              disabled={loading}
              className='px-2 py-1 text-sm border border-gray-300 rounded disabled:opacity-50'
            >
              {pageSizes.map(size => (
                <option key={size} value={size}>
                  Show {size}
                </option>
              ))}
            </select>
          </div>
        </div>
      )}
    </div>
  );
};

export default DataTable;
