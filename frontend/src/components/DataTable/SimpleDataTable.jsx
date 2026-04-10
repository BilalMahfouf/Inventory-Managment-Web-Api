import { useState, useMemo, useRef, useCallback, useEffect } from 'react';
import {
  useReactTable,
  getCoreRowModel,
  getSortedRowModel,
  getFilteredRowModel,
  getPaginationRowModel,
  flexRender,
} from '@tanstack/react-table';
import { Eye, Pencil, Trash2, MoreHorizontal } from 'lucide-react';

/**
 * SimpleDataTable Component
 *
 * A client-side data table with built-in pagination, sorting, and filtering.
 * Unlike DataTable which handles server-side operations, this component
 * processes all data operations on the client side.
 *
 * Use this when:
 * - You have all data available at once
 * - The dataset is relatively small (< 1000 rows)
 * - The API returns the complete dataset in one request
 *
 * @param {Array} data - Complete array of data to display
 * @param {Array} columns - Column definitions for the table
 * @param {boolean} enableSorting - Enable column sorting (default: true)
 * @param {boolean} enableFiltering - Enable search/filtering (default: true)
 * @param {boolean} enablePagination - Enable pagination (default: true)
 * @param {string} searchPlaceholder - Placeholder text for search input
 * @param {string} className - Additional CSS classes for container
 * @param {boolean} loading - Show loading state
 * @param {Array} pageSizes - Available page size options
 * @param {number} initialPageSize - Initial number of rows per page
 * @param {boolean} enableActions - Show actions column (default: true)
 * @param {Function} onView - Handler for view action
 * @param {Function} onEdit - Handler for edit action
 * @param {Function} onDelete - Handler for delete action
 * @param {string} actionsColumnHeader - Header text for actions column
 */
const SimpleDataTable = ({
  data = [],
  columns = [],
  // UI control props
  enableSorting = true,
  enableFiltering = true,
  enablePagination = true,
  searchPlaceholder = 'Search...',
  className = '',
  loading = false,
  // Available page sizes
  pageSizes = [10, 20, 30, 40, 50],
  initialPageSize = 10,
  // Actions props
  enableActions = true,
  onView = null,
  onEdit = null,
  onDelete = null,
  actionsColumnHeader = 'Actions',
}) => {
  const [sorting, setSorting] = useState([]);
  const [globalFilter, setGlobalFilter] = useState('');
  const [openDropdown, setOpenDropdown] = useState(null);
  const [dropdownPosition, setDropdownPosition] = useState({
    top: 0,
    left: 0,
    maxHeight: 220,
  });
  const triggerRefs = useRef(new Map());
  const dropdownRef = useRef(null);

  const DROPDOWN_VIEWPORT_MARGIN = 8;
  const DROPDOWN_GAP = 8;
  const DROPDOWN_MIN_HEIGHT = 120;
  const DROPDOWN_DEFAULT_HEIGHT = 220;
  const ACTION_MENU_WIDTH = 160;

  const setTriggerRef = useCallback((rowId, node) => {
    if (node) {
      triggerRefs.current.set(rowId, node);
      return;
    }

    triggerRefs.current.delete(rowId);
  }, []);

  const updateDropdownPosition = useCallback(
    (rowId, { attemptScroll = false } = {}) => {
      const triggerNode = triggerRefs.current.get(rowId);

      if (!triggerNode) {
        return;
      }

      const rect = triggerNode.getBoundingClientRect();
      const viewportHeight = window.innerHeight;
      const viewportWidth = window.innerWidth;
      const spaceBelow =
        viewportHeight - rect.bottom - DROPDOWN_VIEWPORT_MARGIN;
      const spaceAbove = rect.top - DROPDOWN_VIEWPORT_MARGIN;
      const shouldOpenAbove =
        spaceBelow < DROPDOWN_MIN_HEIGHT && spaceAbove > spaceBelow;
      const availableSpace = shouldOpenAbove ? spaceAbove : spaceBelow;
      const maxHeight = Math.max(
        DROPDOWN_MIN_HEIGHT,
        Math.floor(availableSpace - DROPDOWN_GAP)
      );
      const measuredMenuHeight =
        dropdownRef.current?.offsetHeight ?? DROPDOWN_DEFAULT_HEIGHT;
      const menuHeight = Math.min(measuredMenuHeight, maxHeight);

      let left = rect.right - ACTION_MENU_WIDTH;
      left = Math.max(
        DROPDOWN_VIEWPORT_MARGIN,
        Math.min(
          left,
          viewportWidth - ACTION_MENU_WIDTH - DROPDOWN_VIEWPORT_MARGIN
        )
      );

      let top = shouldOpenAbove
        ? rect.top - menuHeight - DROPDOWN_GAP
        : rect.bottom + DROPDOWN_GAP;
      const maxTop = viewportHeight - menuHeight - DROPDOWN_VIEWPORT_MARGIN;
      top = Math.min(
        Math.max(top, DROPDOWN_VIEWPORT_MARGIN),
        Math.max(DROPDOWN_VIEWPORT_MARGIN, maxTop)
      );

      setDropdownPosition(prev => {
        if (
          prev.top === top &&
          prev.left === left &&
          prev.maxHeight === maxHeight
        ) {
          return prev;
        }

        return {
          top,
          left,
          maxHeight,
        };
      });

      if (!attemptScroll) {
        return;
      }

      const bottomOverflow =
        top + menuHeight - (viewportHeight - DROPDOWN_VIEWPORT_MARGIN);
      const topOverflow = DROPDOWN_VIEWPORT_MARGIN - top;

      if (bottomOverflow > 0 || topOverflow > 0) {
        triggerNode.scrollIntoView({ block: 'nearest', inline: 'nearest' });
        window.requestAnimationFrame(() => {
          updateDropdownPosition(rowId, { attemptScroll: false });
        });
      }
    },
    []
  );

  useEffect(() => {
    if (openDropdown === null) {
      return undefined;
    }

    const handleOutsideClick = event => {
      const triggerNode = triggerRefs.current.get(openDropdown);

      if (
        dropdownRef.current?.contains(event.target) ||
        triggerNode?.contains(event.target)
      ) {
        return;
      }

      setOpenDropdown(null);
    };

    const handleWindowChange = () => {
      updateDropdownPosition(openDropdown);
    };

    const handleKeyDown = event => {
      if (event.key === 'Escape') {
        setOpenDropdown(null);
      }
    };

    const frameId = window.requestAnimationFrame(() => {
      updateDropdownPosition(openDropdown, { attemptScroll: true });
    });

    document.addEventListener('mousedown', handleOutsideClick);
    document.addEventListener('touchstart', handleOutsideClick);
    document.addEventListener('keydown', handleKeyDown);
    window.addEventListener('resize', handleWindowChange);
    window.addEventListener('scroll', handleWindowChange, true);

    return () => {
      window.cancelAnimationFrame(frameId);
      document.removeEventListener('mousedown', handleOutsideClick);
      document.removeEventListener('touchstart', handleOutsideClick);
      document.removeEventListener('keydown', handleKeyDown);
      window.removeEventListener('resize', handleWindowChange);
      window.removeEventListener('scroll', handleWindowChange, true);
    };
  }, [openDropdown, updateDropdownPosition]);

  useEffect(() => {
    setOpenDropdown(null);
  }, [data]);

  // Actions column definition
  const actionsColumn = useMemo(
    () => ({
      id: 'actions',
      header: actionsColumnHeader,
      enableSorting: false,
      enableColumnFilter: false,
      cell: ({ row }) => (
        <div className='relative'>
          <button
            ref={node => setTriggerRef(row.id, node)}
            onClick={event => {
              event.stopPropagation();
              setOpenDropdown(openDropdown === row.id ? null : row.id);
            }}
            className='p-1 hover:bg-gray-100 rounded-md transition-colors'
            aria-label='Actions menu'
            aria-expanded={openDropdown === row.id}
          >
            <MoreHorizontal className='w-5 h-5 text-gray-600' />
          </button>

          {openDropdown === row.id && (
            <div
              ref={dropdownRef}
              className='fixed w-40 bg-white rounded-md shadow-lg border border-gray-200 z-40 overflow-y-auto'
              style={{
                top: `${dropdownPosition.top}px`,
                left: `${dropdownPosition.left}px`,
                maxHeight: `${dropdownPosition.maxHeight}px`,
              }}
            >
              {onView && (
                <button
                  onClick={() => {
                    onView(row.original);
                    setOpenDropdown(null);
                  }}
                  className='w-full px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-100 flex items-center gap-2 transition-colors'
                >
                  <Eye className='w-4 h-4' />
                  View
                </button>
              )}
              {onEdit && (
                <button
                  onClick={() => {
                    onEdit(row.original);
                    setOpenDropdown(null);
                  }}
                  className='w-full px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-100 flex items-center gap-2 transition-colors'
                >
                  <Pencil className='w-4 h-4' />
                  Edit
                </button>
              )}
              {onDelete && (
                <button
                  onClick={() => {
                    onDelete(row.original);
                    setOpenDropdown(null);
                  }}
                  className='w-full px-4 py-2 text-left text-sm text-red-600 hover:bg-red-50 flex items-center gap-2 transition-colors'
                >
                  <Trash2 className='w-4 h-4' />
                  Delete
                </button>
              )}
            </div>
          )}
        </div>
      ),
    }),
    [
      onView,
      onEdit,
      onDelete,
      actionsColumnHeader,
      openDropdown,
      dropdownPosition,
      setTriggerRef,
    ]
  );

  // Combine columns with actions column
  const tableColumns = useMemo(() => {
    // Ensure columns is an array
    const safeColumns = Array.isArray(columns) ? columns : [];

    if (enableActions && (onView || onEdit || onDelete)) {
      return [...safeColumns, actionsColumn];
    }
    return safeColumns;
  }, [columns, actionsColumn, enableActions, onView, onEdit, onDelete]);

  // Ensure data is always an array to prevent null/undefined errors
  const safeData = useMemo(() => {
    return Array.isArray(data) ? data : [];
  }, [data]);

  const table = useReactTable({
    data: safeData,
    columns: tableColumns,
    state: {
      sorting,
      globalFilter,
    },
    initialState: {
      pagination: {
        pageSize: initialPageSize,
      },
    },
    enableSorting,
    enableFilters: enableFiltering,
    onSortingChange: setSorting,
    onGlobalFilterChange: setGlobalFilter,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: enableSorting ? getSortedRowModel() : undefined,
    getFilteredRowModel: enableFiltering ? getFilteredRowModel() : undefined,
    getPaginationRowModel: enablePagination
      ? getPaginationRowModel()
      : undefined,
    // Global filter function - searches across all columns
    globalFilterFn: 'includesString',
  });

  const totalRows = table.getFilteredRowModel()?.rows?.length || 0;
  const pageIndex = table.getState().pagination.pageIndex;
  const pageSize = table.getState().pagination.pageSize;
  const pageCount = table.getPageCount();

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
              onChange={e => setGlobalFilter(e.target.value)}
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
                <td
                  colSpan={tableColumns.length}
                  className='px-6 py-12 text-center'
                >
                  <div className='flex items-center justify-center'>
                    <div className='animate-spin rounded-full h-8 w-8 border-2 border-blue-500 border-t-transparent'></div>
                    <span className='ml-2 text-gray-500'>Loading...</span>
                  </div>
                </td>
              </tr>
            ) : table.getRowModel().rows.length === 0 ? (
              <tr>
                <td
                  colSpan={tableColumns.length}
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
      {enablePagination && totalRows > 0 && (
        <div className='flex items-center justify-between mt-4'>
          <div className='flex items-center gap-2'>
            <span className='text-sm text-gray-700'>
              Showing {Math.min(pageIndex * pageSize + 1, totalRows)} to{' '}
              {Math.min((pageIndex + 1) * pageSize, totalRows)} of {totalRows}{' '}
              entries
            </span>
          </div>

          <div className='flex items-center gap-2'>
            <button
              onClick={() => table.setPageIndex(0)}
              disabled={!table.getCanPreviousPage() || loading}
              className='px-3 py-1 text-sm border border-gray-300 rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50'
            >
              {'<<'}
            </button>
            <button
              onClick={() => table.previousPage()}
              disabled={!table.getCanPreviousPage() || loading}
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
              onClick={() => table.nextPage()}
              disabled={!table.getCanNextPage() || loading}
              className='px-3 py-1 text-sm border border-gray-300 rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50'
            >
              Next
            </button>
            <button
              onClick={() => table.setPageIndex(pageCount - 1)}
              disabled={!table.getCanNextPage() || loading}
              className='px-3 py-1 text-sm border border-gray-300 rounded disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50'
            >
              {'>>'}
            </button>

            <select
              value={pageSize}
              onChange={e => table.setPageSize(Number(e.target.value))}
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

export default SimpleDataTable;
