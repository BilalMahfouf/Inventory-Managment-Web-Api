# Server-Side DataTable Usage Guide

This guide explains how to use the updated DataTable component with server-side pagination, sorting, and filtering.

## Backend API Requirements

Your backend should accept the following parameters:

```csharp
public class DataTableRequest
{
    public int PageSize { get; init; }
    public int Page { get; init; }
    public string? search { get; init; } = null;
    public string? SortColumn { get; init; } = null;
    public string? SortOrder { get; init; } = null; // "asc" or "desc"
}
```

## Frontend Implementation

### 1. Create a Service Function

Create a service function that calls your backend API:

```javascript
// services/yourService.js
export const fetchYourData = async ({
  Page,
  PageSize,
  SortColumn,
  SortOrder,
  search,
}) => {
  const response = await api.get('/your-endpoint', {
    params: {
      Page,
      PageSize,
      SortColumn,
      SortOrder,
      search,
    },
  });

  return {
    item: response.data.items, // Array of data
    totalCount: response.data.totalCount, // Total number of records
  };
};
```

### 2. Use the useServerSideDataTable Hook

```javascript
import useServerSideDataTable from '@hooks/useServerSideDataTable';
import { fetchYourData } from '@services/yourService';

const YourComponent = () => {
  const tableProps = useServerSideDataTable(fetchYourData, {
    initialPageSize: 10,
    onError: error => {
      console.error('Data fetch error:', error);
      // Handle error display
    },
  });

  // Your component logic...
};
```

### 3. Configure Your DataTable

```javascript
import DataTable from '@components/DataTable/DataTable';

const columns = [
  {
    accessorKey: 'id',
    header: 'ID',
    enableSorting: true,
  },
  {
    accessorKey: 'name',
    header: 'Name',
    enableSorting: true,
  },
  // Add more columns...
];

return (
  <DataTable
    data={tableProps.data}
    columns={columns}
    totalRows={tableProps.totalRows}
    pageIndex={tableProps.pageIndex}
    pageSize={tableProps.pageSize}
    onPageChange={tableProps.onPageChange}
    onPageSizeChange={tableProps.onPageSizeChange}
    onSortingChange={tableProps.onSortingChange}
    onFilterChange={tableProps.onFilterChange}
    loading={tableProps.loading}
    enableSorting={true}
    enableFiltering={true}
    enablePagination={true}
    searchPlaceholder='Search...'
  />
);
```

## Key Features

### 1. Server-Side Sorting

- Click column headers to sort
- Automatically resets to page 1 when sorting changes
- Sends `SortColumn` and `SortOrder` to backend

### 2. Server-Side Filtering

- Global search input with debouncing (500ms default)
- Automatically resets to page 1 when search changes
- Sends `search` parameter to backend

### 3. Server-Side Pagination

- Page size selector
- Page navigation buttons
- Maintains current page when only data changes

## Hook Properties

The `useServerSideDataTable` hook returns:

```javascript
{
  // State
  data: [], // Current page data
  loading: false, // Loading state
  pageIndex: 0, // Current page index (0-based)
  pageSize: 10, // Current page size
  totalRows: 0, // Total number of records
  sorting: [], // Current sorting state
  globalFilter: '', // Current search filter
  error: null, // Error state

  // Handlers
  onPageChange: (newPageIndex) => {}, // Change page
  onPageSizeChange: (newPageSize) => {}, // Change page size
  onSortingChange: (newSorting) => {}, // Change sorting
  onFilterChange: (newFilter) => {}, // Change filter

  // Actions
  refresh: () => {}, // Refresh current data
}
```

## DataTable Props

| Prop                | Type     | Default                | Description                    |
| ------------------- | -------- | ---------------------- | ------------------------------ |
| `data`              | Array    | `[]`                   | Array of data for current page |
| `columns`           | Array    | `[]`                   | Column definitions             |
| `totalRows`         | Number   | `0`                    | Total number of records        |
| `pageIndex`         | Number   | `0`                    | Current page index (0-based)   |
| `pageSize`          | Number   | `10`                   | Current page size              |
| `onPageChange`      | Function | `() => {}`             | Page change handler            |
| `onPageSizeChange`  | Function | `() => {}`             | Page size change handler       |
| `onSortingChange`   | Function | `() => {}`             | Sorting change handler         |
| `onFilterChange`    | Function | `() => {}`             | Filter change handler          |
| `loading`           | Boolean  | `false`                | Loading state                  |
| `enableSorting`     | Boolean  | `true`                 | Enable sorting                 |
| `enableFiltering`   | Boolean  | `true`                 | Enable filtering               |
| `enablePagination`  | Boolean  | `true`                 | Enable pagination              |
| `searchPlaceholder` | String   | `'Search...'`          | Search input placeholder       |
| `searchDebounceMs`  | Number   | `500`                  | Search debounce delay          |
| `pageSizes`         | Array    | `[10, 20, 30, 40, 50]` | Available page sizes           |

## Page Reset Behavior

The following actions automatically reset to page 1:

- Changing sort column or sort order
- Changing search filter
- Changing page size

This ensures users don't end up on empty pages when filtering or sorting reduces the total number of results.

## Error Handling

The hook includes error handling:

```javascript
const tableProps = useServerSideDataTable(fetchData, {
  onError: error => {
    // Show error notification
    toast.error('Failed to load data');
    console.error(error);
  },
});

// Display error in UI
{
  tableProps.error && (
    <div className='error-message'>Error: {tableProps.error.message}</div>
  );
}
```
