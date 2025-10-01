# Generic DataTable with Server-Side Pagination

A highly customizable, generic DataTable component built with TanStack Table that supports server-side pagination, sorting, filtering, and loading states.

## 📁 Files Structure

```
DataTable/
├── DataTable.jsx           # Main DataTable component
├── useServerSideTable.js   # Custom hook for server-side logic
├── index.js               # Exports
├── ServerSideExample.jsx  # Full manual example
├── HookExample.jsx        # Example using the hook
└── README.md             # This file
```

## 🚀 Features

- ✅ **Server-Side Pagination** - Efficient handling of large datasets
- ✅ **Sorting** - Click column headers to sort
- ✅ **Global Search** - Search across all columns with debouncing
- ✅ **Loading States** - Visual feedback during API calls
- ✅ **Error Handling** - Built-in error states
- ✅ **Customizable** - Highly configurable props
- ✅ **Responsive** - Works on all screen sizes
- ✅ **TypeScript Ready** - (can be easily converted)

## 📦 Installation

The component uses `@tanstack/react-table` which is already installed.

## 🎯 Usage

### Method 1: Using the Hook (Recommended)

```jsx
import React from 'react';
import DataTable, { useServerSideTable } from '../components/DataTable';

const MyComponent = () => {
  // Define your API fetch function
  const fetchData = async ({ page, pageSize, sorting, search }) => {
    const params = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
      search: search || '',
      ...(sorting.length > 0 && {
        sortBy: sorting[0].id,
        sortOrder: sorting[0].desc ? 'desc' : 'asc',
      }),
    });

    const response = await fetch(`/api/your-endpoint?${params}`);
    return await response.json();
  };

  // Use the hook
  const tableProps = useServerSideTable(fetchData);

  // Define columns
  const columns = [
    { accessorKey: 'id', header: 'ID' },
    { accessorKey: 'name', header: 'Name' },
    { accessorKey: 'email', header: 'Email' },
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
    />
  );
};
```

### Method 2: Manual State Management

```jsx
import React, { useState, useEffect } from 'react';
import DataTable from '../components/DataTable';

const MyComponent = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [pageIndex, setPageIndex] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [totalRows, setTotalRows] = useState(0);

  // Fetch function
  const fetchData = async () => {
    setLoading(true);
    try {
      const response = await fetch(
        `/api/data?page=${pageIndex}&size=${pageSize}`
      );
      const result = await response.json();
      setData(result.data);
      setTotalRows(result.totalRows);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [pageIndex, pageSize]);

  const columns = [
    { accessorKey: 'id', header: 'ID' },
    { accessorKey: 'name', header: 'Name' },
  ];

  return (
    <DataTable
      data={data}
      columns={columns}
      totalRows={totalRows}
      pageIndex={pageIndex}
      pageSize={pageSize}
      onPageChange={setPageIndex}
      onPageSizeChange={size => {
        setPageSize(size);
        setPageIndex(0);
      }}
      loading={loading}
    />
  );
};
```

## 🔧 Props

### DataTable Props

| Prop                | Type     | Default            | Description                                |
| ------------------- | -------- | ------------------ | ------------------------------------------ |
| `data`              | Array    | `[]`               | Array of data objects to display           |
| `columns`           | Array    | `[]`               | Column definitions (TanStack Table format) |
| `totalRows`         | Number   | `0`                | Total number of rows across all pages      |
| `pageIndex`         | Number   | `0`                | Current page index (0-based)               |
| `pageSize`          | Number   | `10`               | Number of rows per page                    |
| `onPageChange`      | Function | `() => {}`         | Called when page changes                   |
| `onPageSizeChange`  | Function | `() => {}`         | Called when page size changes              |
| `onSortingChange`   | Function | `() => {}`         | Called when sorting changes                |
| `onFilterChange`    | Function | `() => {}`         | Called when search filter changes          |
| `loading`           | Boolean  | `false`            | Shows loading spinner                      |
| `enableSorting`     | Boolean  | `true`             | Enable column sorting                      |
| `enableFiltering`   | Boolean  | `true`             | Enable global search                       |
| `enablePagination`  | Boolean  | `true`             | Enable pagination controls                 |
| `searchPlaceholder` | String   | `"Search..."`      | Placeholder for search input               |
| `searchDebounceMs`  | Number   | `500`              | Debounce delay for search (ms)             |
| `pageSizes`         | Array    | `[10,20,30,40,50]` | Available page size options                |
| `className`         | String   | `""`               | Additional CSS classes                     |

### useServerSideTable Options

| Option            | Type     | Default         | Description                  |
| ----------------- | -------- | --------------- | ---------------------------- |
| `initialPageSize` | Number   | `10`            | Initial page size            |
| `debounceMs`      | Number   | `500`           | Debounce delay for API calls |
| `onError`         | Function | `console.error` | Error handler function       |

## 🌐 Backend API Requirements

Your backend API should:

### Accept these query parameters:

- `page` - Page index (0-based)
- `pageSize` - Number of items per page
- `search` - Global search term (optional)
- `sortBy` - Field to sort by (optional)
- `sortOrder` - `asc` or `desc` (optional)

### Return this format:

```json
{
  "data": [...],        // Array of items for current page
  "totalRows": 150,     // Total number of items across all pages
  "page": 0,           // Current page index
  "pageSize": 10       // Items per page
}
```

## 🎨 Column Definitions

Columns follow TanStack Table format:

```jsx
const columns = [
  {
    accessorKey: 'name', // Data property key
    header: 'Full Name', // Column header text
    cell: ({ getValue }) => (
      <strong>{getValue()}</strong> // Custom cell rendering
    ),
  },
  {
    accessorKey: 'status',
    header: 'Status',
    cell: ({ getValue }) => (
      <span className={`badge ${getValue().toLowerCase()}`}>{getValue()}</span>
    ),
  },
  {
    id: 'actions', // Custom column (no data binding)
    header: 'Actions',
    cell: ({ row }) => (
      <button onClick={() => handleEdit(row.original.id)}>Edit</button>
    ),
  },
];
```

## 🎯 Best Practices

1. **Always reset page to 0** when changing filters or sorting
2. **Use debouncing** for search to avoid excessive API calls
3. **Handle loading states** to provide user feedback
4. **Implement error handling** for network failures
5. **Use the hook** for simpler state management
6. **Keep column definitions stable** (use `useMemo` if needed)

## 🔄 Advanced Usage

### Custom Loading Spinner

```jsx
<DataTable
  {...props}
  loading={loading}
  // Custom loading will be shown in table body
/>
```

### Error Handling

```jsx
const tableProps = useServerSideTable(fetchData, {
  onError: error => {
    toast.error('Failed to load data: ' + error.message);
  },
});

// Display error in UI
{
  tableProps.error && <Alert variant='error'>{tableProps.error.message}</Alert>;
}
```

### Refresh Data

```jsx
const tableProps = useServerSideTable(fetchData);

return (
  <div>
    <button onClick={tableProps.refresh}>Refresh Data</button>
    <DataTable {...tableProps} />
  </div>
);
```

## 🎉 That's it!

You now have a fully functional, generic DataTable with server-side pagination that can be used throughout your application. Simply customize the columns and API endpoint for each use case.
