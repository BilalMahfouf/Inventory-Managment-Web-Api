import { useState, useEffect, useCallback } from 'react';
import { keepPreviousData, useQuery } from '@tanstack/react-query';

/**
 * Custom hook for server-side pagination, sorting, and filtering
 * @param {Function} fetchFunction - Function that fetches data from the server
 * @param {Object} options - Configuration options
 * @returns {Object} State and handlers for the DataTable
 */
const useServerSideDataTable = (
  fetchFunction,
  {
    initialPageSize = 10,
    onError = error => console.error('Table fetch error:', error),
    queryKey = ['table'],
  } = {}
) => {
  // Table state
  const [pageIndex, setPageIndex] = useState(0);
  const [pageSize, setPageSize] = useState(initialPageSize);
  const [sorting, setSorting] = useState([]);
  const [globalFilter, setGlobalFilter] = useState('');
  const sortColumn = sorting.length > 0 ? sorting[0].id : null;
  const sortOrder = sorting.length > 0 ? (sorting[0].desc ? 'desc' : 'asc') : null;

  const queryResult = useQuery({
    queryKey: [...queryKey, { page: pageIndex + 1, pageSize, sortColumn, sortOrder, search: globalFilter }],
    queryFn: () => {
      return fetchFunction({
        page: pageIndex + 1,
        pageSize,
        sortColumn,
        sortOrder,
        search: globalFilter,
      });
    },
    placeholderData: keepPreviousData,
  });

  useEffect(() => {
    if (queryResult.error) {
      onError(queryResult.error);
    }
  }, [queryResult.error, onError]);
  // Handlers
  const handlePageChange = useCallback(newPageIndex => {
    setPageIndex(newPageIndex);
  }, []);

  const handlePageSizeChange = useCallback(newPageSize => {
    setPageSize(newPageSize);
    setPageIndex(0); // Reset to first page
  }, []);

  const handleSortingChange = useCallback(newSorting => {
    setSorting(newSorting);
    // setPageIndex(0); // Reset to first page when sorting changes
  }, []);

  const handleFilterChange = useCallback(newFilter => {
    setGlobalFilter(newFilter);
    setPageIndex(0); // Reset to first page when filtering changes
  }, []);

  const refresh = useCallback(() => {
    return queryResult.refetch();
  }, [queryResult]);

  const result = queryResult.data || {};

  return {
    // State
    data: result.item || [],
    loading: queryResult.isLoading || queryResult.isFetching,
    pageIndex,
    pageSize,
    totalRows: result.totalCount || 0,
    sorting,
    globalFilter,
    error: queryResult.error,

    // Handlers
    onPageChange: handlePageChange,
    onPageSizeChange: handlePageSizeChange,
    onSortingChange: handleSortingChange,
    onFilterChange: handleFilterChange,

    // Actions
    refresh: refresh,
  };
};
export default useServerSideDataTable;
