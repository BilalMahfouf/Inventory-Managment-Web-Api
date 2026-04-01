export const queryKeys = {
    dashboard: {
        all: ['dashboard'],
        summary: () => [...queryKeys.dashboard.all, 'summary'],
        alerts: () => [...queryKeys.dashboard.all, 'alerts'],
        topSellingProducts: count => [...queryKeys.dashboard.all, 'top-selling-products', count],
        todayPerformance: () => [...queryKeys.dashboard.all, 'today-performance'],
    },
    products: {
        all: ['products'],
        table: params => [...queryKeys.products.all, 'table', params],
        summary: () => [...queryKeys.products.all, 'summary'],
        stockMovements: params => [...queryKeys.products.all, 'stock-movements', params],
        detail: productId => [...queryKeys.products.all, 'detail', productId],
        categories: () => [...queryKeys.products.all, 'categories'],
        unitOfMeasure: () => [...queryKeys.products.all, 'unit-of-measure'],
    },
    inventory: {
        all: ['inventory'],
        table: params => [...queryKeys.inventory.all, 'table', params],
        summary: () => [...queryKeys.inventory.all, 'summary'],
        detail: inventoryId => [...queryKeys.inventory.all, 'detail', inventoryId],
        stockTransfers: params => [...queryKeys.inventory.all, 'stock-transfers', params],
        locations: params => [...queryKeys.inventory.all, 'locations', params],
    },
    customers: {
        all: ['customers'],
        table: params => [...queryKeys.customers.all, 'table', params],
        summary: () => [...queryKeys.customers.all, 'summary'],
        detail: customerId => [...queryKeys.customers.all, 'detail', customerId],
        categories: () => [...queryKeys.customers.all, 'categories'],
        customerCategories: {
            all: () => [...queryKeys.customers.all, 'customer-categories'],
            table: params => [...queryKeys.customers.customerCategories.all(), 'table', params],
            detail: categoryId => [...queryKeys.customers.customerCategories.all(), 'detail', categoryId],
            names: () => [...queryKeys.customers.customerCategories.all(), 'names'],
        },
    },
    sales: {
        all: ['sales'],
        summary: () => [...queryKeys.sales.all, 'summary'],
        orders: params => [...queryKeys.sales.all, 'orders', params],
        detail: orderId => [...queryKeys.sales.all, 'detail', orderId],
    },
    auth: {
        login: () => ['auth', 'login'],
        logout: () => ['auth', 'logout'],
    },
};
