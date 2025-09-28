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
  const fetchData = useCallback(
    async (page, size, sort = null, search = null) => {
      setLoading(true);
      setError(null);
      const fetchFunctionParam = {
        page,
        pageSize: size,
        // sorting: sort,
        // search,
      };
      try {
        console.log('Fetching data with params:', fetchFunctionParam);
        const result = await fetchFunction(fetchFunctionParam);
        console.log('Fetch result:', result);

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
    },
    [fetchFunction, onError]
  );

  // Fetch data when dependencies change
  useEffect(() => {
    fetchData(pageIndex + 1, pageSize, sorting, globalFilter);
  }, [pageIndex, pageSize]);
  // Handlers
  const handlePageChange = useCallback(newPageIndex => {
    setPageIndex(newPageIndex);
  }, []);

  const handlePageSizeChange = useCallback(newPageSize => {
    setPageSize(newPageSize);
    setPageIndex(0); // Reset to first page
  }, []);

  //   const handleSortingChange = useCallback(newSorting => {
  //     setSorting(newSorting);
  //     setPageIndex(0); // Reset to first page
  //   }, []);

  //   const handleFilterChange = useCallback(newFilter => {
  //     setGlobalFilter(newFilter);
  //     setPageIndex(0); // Reset to first page
  //   }, []);

  const refresh = useCallback(() => {
    fetchData(pageIndex, pageSize, sorting, globalFilter);
  }, [pageIndex, pageSize, sorting, globalFilter, fetchData]);

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
    // onSortingChange: handleSortingChange,
    // onFilterChange: handleFilterChange,

    // Actions
    refresh,
  };
};
export default useServerSideDataTable;
