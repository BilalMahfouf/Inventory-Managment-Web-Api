import { useState, useEffect, useMemo, useRef, useCallback } from 'react';
import {
  useReactTable,
  getCoreRowModel,
  flexRender,
} from '@tanstack/react-table';
import { Eye, Pencil, Trash2, MoreHorizontal } from 'lucide-react';

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
  // Actions props
  enableActions = true,
  onView = null,
  onEdit = null,
  onDelete = null,
  customActions = null,
  actionsColumnHeader = 'Actions',
}) => {
  const [sorting, setSorting] = useState([]);
  const [globalFilter, setGlobalFilter] = useState('');
  const [searchTimeout, setSearchTimeout] = useState(null);
  const [openDropdown, setOpenDropdown] = useState(null);
  const [dropdownPosition, setDropdownPosition] = useState({
    top: 0,
    left: 0,
    maxHeight: 240,
  });
  const triggerRefs = useRef(new Map());
  const dropdownRef = useRef(null);

  const DROPDOWN_VIEWPORT_MARGIN = 8;
  const DROPDOWN_GAP = 8;
  const DROPDOWN_MIN_HEIGHT = 140;
  const DROPDOWN_DEFAULT_HEIGHT = 240;
  const ACTION_MENU_WIDTH = 208;

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
  }, [pageIndex, pageSize, data]);

  // Actions column definition
  const actionsColumn = useMemo(
    () => ({
      id: 'actions',
      header: actionsColumnHeader,
      cell: ({ row }) => {
        const rowActions =
          typeof customActions === 'function'
            ? customActions(row.original)
            : Array.isArray(customActions)
              ? customActions
              : [];

        return (
          <div className='relative'>
            <button
              ref={node => setTriggerRef(row.id, node)}
              onClick={event => {
                event.stopPropagation();
                setOpenDropdown(openDropdown === row.id ? null : row.id);
              }}
              className='p-1 hover:bg-gray-100 rounded-md transition-colors cursor-pointer'
              aria-label='Actions menu'
              aria-expanded={openDropdown === row.id}
            >
              <MoreHorizontal className='w-5 h-5 text-gray-600' />
            </button>

            {openDropdown === row.id && (
              <div
                ref={dropdownRef}
                className='fixed w-52 bg-white rounded-md shadow-lg border border-gray-200 z-40 overflow-y-auto'
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
                    className='w-full px-4 py-2 cursor-pointer text-left text-sm text-gray-700 hover:bg-gray-100 flex items-center gap-2 transition-colors'
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
                    className='w-full px-4 py-2 text-left cursor-pointer text-sm text-gray-700 hover:bg-gray-100 flex items-center gap-2 transition-colors'
                  >
                    <Pencil className='w-4 h-4' />
                    Edit
                  </button>
                )}
                {rowActions.map((action, index) => {
                  if (!action?.label || typeof action?.onClick !== 'function') {
                    return null;
                  }

                  if (action.hidden) {
                    return null;
                  }

                  const Icon = action.icon;
                  const isDestructive = action.variant === 'destructive';

                  return (
                    <button
                      key={action.key || `${action.label}-${index}`}
                      onClick={() => {
                        if (action.disabled) return;
                        action.onClick(row.original);
                        setOpenDropdown(null);
                      }}
                      disabled={Boolean(action.disabled)}
                      className={`w-full px-4 py-2 text-left cursor-pointer text-sm flex items-center gap-2 transition-colors ${
                        isDestructive
                          ? 'text-red-600 hover:bg-red-50'
                          : 'text-gray-700 hover:bg-gray-100'
                      } ${action.disabled ? 'opacity-50 cursor-not-allowed' : ''}`}
                    >
                      {Icon && <Icon className='w-4 h-4' />}
                      {action.label}
                    </button>
                  );
                })}
                {onDelete && (
                  <button
                    onClick={() => {
                      onDelete(row.original);
                      setOpenDropdown(null);
                    }}
                    className='w-full px-4 cursor-pointer py-2 text-left text-sm text-red-600 hover:bg-red-50 flex items-center gap-2 transition-colors'
                  >
                    <Trash2 className='w-4 h-4' />
                    Delete
                  </button>
                )}
              </div>
            )}
          </div>
        );
      },
    }),
    [
      onView,
      onEdit,
      onDelete,
      customActions,
      actionsColumnHeader,
      openDropdown,
      dropdownPosition,
      setTriggerRef,
    ]
  );

  // Combine columns with actions column
  const tableColumns = useMemo(() => {
    if (enableActions && (onView || onEdit || onDelete || customActions)) {
      return [...columns, actionsColumn];
    }
    return columns;
  }, [
    columns,
    actionsColumn,
    enableActions,
    onView,
    onEdit,
    onDelete,
    customActions,
  ]);

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
    columns: tableColumns,
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
