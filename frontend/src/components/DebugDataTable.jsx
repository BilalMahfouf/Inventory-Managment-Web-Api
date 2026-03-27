import React from 'react';
import useServerSideDataTable from '../hooks/useServerSideDataTable';

// Mock fetch function for testing
const mockFetchFunction = async ({
  Page,
  PageSize,
  SortColumn,
  SortOrder,
  search,
}) => {
  console.log('📦 Mock API called with:', {
    Page,
    PageSize,
    SortColumn,
    SortOrder,
    search,
  });

  // Simulate network delay
  await new Promise(resolve => setTimeout(resolve, 300));

  // Generate mock data based on page
  const totalItems = 50; // Total items in "database"
  const startIndex = (Page - 1) * PageSize;

  const mockItems = [];
  for (let i = 0; i < PageSize; i++) {
    const itemIndex = startIndex + i;
    if (itemIndex < totalItems) {
      mockItems.push({
        id: itemIndex + 1,
        name: `Item ${itemIndex + 1}`,
        value: Math.floor(Math.random() * 1000),
        category: `Category ${Math.floor(itemIndex / 10) + 1}`,
      });
    }
  }

  return {
    item: mockItems,
    totalCount: totalItems,
  };
};

const DebugDataTable = () => {
  const {
    data,
    loading,
    pageIndex,
    pageSize,
    totalRows,
    onPageChange,
    onPageSizeChange,
    onSortingChange,
    onFilterChange,
    error,
  } = useServerSideDataTable(mockFetchFunction, {
    initialPageSize: 5,
    onError: error => console.error('❌ Hook Error:', error),
  });

  console.log('🎯 Component render - Current state:', {
    pageIndex,
    pageSize,
    totalRows,
    dataLength: data.length,
    loading,
  });

  return (
    <div className='p-6 max-w-4xl mx-auto'>
      <h1 className='text-2xl font-bold mb-4'>Debug DataTable Pagination</h1>

      {/* Debug Info */}
      <div className='mb-4 p-4 bg-gray-100 rounded'>
        <h3 className='font-semibold mb-2'>Current State:</h3>
        <div className='text-sm space-y-1'>
          <div>
            Page Index: <strong>{pageIndex}</strong> (0-based)
          </div>
          <div>
            Page Size: <strong>{pageSize}</strong>
          </div>
          <div>
            Total Rows: <strong>{totalRows}</strong>
          </div>
          <div>
            Current Page: <strong>{pageIndex + 1}</strong>
          </div>
          <div>
            Total Pages: <strong>{Math.ceil(totalRows / pageSize)}</strong>
          </div>
          <div>
            Loading: <strong>{loading ? 'Yes' : 'No'}</strong>
          </div>
          <div>
            Data Items: <strong>{data.length}</strong>
          </div>
        </div>
      </div>

      {/* Controls */}
      <div className='mb-4 space-x-2'>
        <button
          onClick={() => {
            console.log('🔄 Previous page clicked');
            onPageChange(pageIndex - 1);
          }}
          disabled={pageIndex === 0}
          className='px-3 py-1 bg-blue-500 text-white rounded disabled:bg-gray-300'
        >
          Previous Page
        </button>

        <button
          onClick={() => {
            console.log('🔄 Next page clicked');
            onPageChange(pageIndex + 1);
          }}
          disabled={pageIndex >= Math.ceil(totalRows / pageSize) - 1}
          className='px-3 py-1 bg-blue-500 text-white rounded disabled:bg-gray-300'
        >
          Next Page
        </button>

        <select
          value={pageSize}
          onChange={e => {
            console.log('📏 Page size change clicked');
            onPageSizeChange(Number(e.target.value));
          }}
          className='px-2 py-1 border rounded'
        >
          <option value={5}>5 per page</option>
          <option value={10}>10 per page</option>
          <option value={20}>20 per page</option>
        </select>
      </div>

      {/* Data Table */}
      {loading ? (
        <div className='text-center py-8'>Loading...</div>
      ) : (
        <div className='border rounded'>
          <table className='w-full'>
            <thead className='bg-gray-50'>
              <tr>
                <th className='p-3 text-left'>ID</th>
                <th className='p-3 text-left'>Name</th>
                <th className='p-3 text-left'>Value</th>
                <th className='p-3 text-left'>Category</th>
              </tr>
            </thead>
            <tbody>
              {data.map(item => (
                <tr key={item.id} className='border-t'>
                  <td className='p-3'>{item.id}</td>
                  <td className='p-3'>{item.name}</td>
                  <td className='p-3'>{item.value}</td>
                  <td className='p-3'>{item.category}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {error && (
        <div className='mt-4 p-4 bg-red-100 text-red-700 rounded'>
          Error: {error.message}
        </div>
      )}

      <div className='mt-4 p-4 bg-blue-50 rounded'>
        <h3 className='font-semibold mb-2'>Instructions:</h3>
        <ol className='list-decimal list-inside text-sm space-y-1'>
          <li>Open your browser's console (F12)</li>
          <li>Click "Next Page" button</li>
          <li>Watch the console logs to see if parameters are changing</li>
          <li>If the Page parameter stays the same, the issue is confirmed</li>
        </ol>
      </div>
    </div>
  );
};

export default DebugDataTable;
