import { useState, useEffect, useCallback } from 'react';

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
  } = {}
) => {
  // Table state
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [pageIndex, setPageIndex] = useState(0);
  const [pageSize, setPageSize] = useState(initialPageSize);
  const [totalRows, setTotalRows] = useState(0);
  const [sorting, setSorting] = useState([]);
  const [globalFilter, setGlobalFilter] = useState('');
  const [error, setError] = useState(null);

  // Fetch function
  const fetchData = async (page, size, sortingArray = [], search = null) => {
    setLoading(true);
    setError(null);

    // Extract sorting information from TanStack Table format
    const sortColumn = sortingArray.length > 0 ? sortingArray[0].id : null;
    const sortOrder =
      sortingArray.length > 0 ? (sortingArray[0].desc ? 'desc' : 'asc') : null;

    const fetchFunctionParam = {
      page: page,
      pageSize: size,
      sortColumn: sortColumn,
      sortOrder: sortOrder,
      search: search,
    };

    try {
      const result = await fetchFunction(fetchFunctionParam);

      setData(result.item || []);
      setTotalRows(result.totalCount || 0);
    } catch (err) {
      setError(err);
      onError(err);
      setData([]);
      setTotalRows(0);
    } finally {
      setLoading(false);
    }
  };

  // Fetch data when dependencies change
  useEffect(() => {
    fetchData(pageIndex + 1, pageSize, sorting, globalFilter);
  }, [pageIndex, pageSize, sorting, globalFilter]);
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
    fetchData(pageIndex + 1, pageSize, sorting, globalFilter);
  }, [pageIndex, pageSize, sorting, globalFilter]);

  return {
    // State
    data,
    loading,
    pageIndex,
    pageSize,
    totalRows,
    sorting,
    globalFilter,
    error,

    // Handlers
    onPageChange: handlePageChange,
    onPageSizeChange: handlePageSizeChange,
    onSortingChange: handleSortingChange,
    onFilterChange: handleFilterChange,

    // Actions
    refresh,
  };
};
export default useServerSideDataTable;
