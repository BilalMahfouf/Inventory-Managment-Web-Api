import { fetchWithAuth } from '@services/auth/authService';

const BASE_URL = '/api/stock-transfers';

async function getAllStockTransfers({
  page,
  pageSize,
  sortColumn,
  sortOrder,
  search,
}) {
  try {
    const params = new URLSearchParams({
      page: page,
      pageSize: pageSize,
    });
    if (sortColumn) params.append('sortColumn', sortColumn);
    if (sortOrder) params.append('sortOrder', sortOrder);
    if (search) params.append('search', search);
    const response = await fetchWithAuth(`${BASE_URL}?${params.toString()}`);
    if (!response.success) {
      const error = await response.error;
      return { success: false, error: error };
    }
    return { success: true, data: await response.response.json() };
  } catch (error) {
    console.error('Error fetching stock transfers:', error);
    return { success: false, error: error.message };
  }
}

export { getAllStockTransfers };
