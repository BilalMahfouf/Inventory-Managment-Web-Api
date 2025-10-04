const BASE_URL = '/api/products';

import { fetchWithAuth } from '../auth/authService';

async function getSummary() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/summary`);
    if (!response.success) {
      console.log('Failed to fetch product summary:', response.error);
      throw new Error('Failed to fetch product summary');
    }
    return await response.response.json();
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function getAllProducts({
  page,
  pageSize,
  sortColumn,
  sortOrder,
  search,
}) {
  try {
    // Build query parameters, only include non-null values
    const params = new URLSearchParams();
    params.append('page', page ? page : 1);
    params.append('pageSize', pageSize ? pageSize : 10);

    if (sortColumn) params.append('sortColumn', sortColumn);
    if (sortOrder) params.append('sortOrder', sortOrder);
    if (search) params.append('search', search);

    const response = await fetchWithAuth(`${BASE_URL}?${params.toString()}`);
    if (!response.success) {
      console.log('Failed to fetch products:', response.error);
      throw new Error('Failed to fetch products');
    }
    const data = await response.response.json();
    if (data === null || data === undefined) {
      return { item: [], totalCount: 0 };
    }
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function getStockMovementsHistory({
  page,
  pageSize,
  sortColumn,
  sortOrder,
  search,
}) {
  try {
    let params = new URLSearchParams();
    params.append('page', page ? page : 1);
    params.append('pageSize', pageSize ? pageSize : 10);
    if (sortColumn) params.append('sortColumn', sortColumn);
    if (sortOrder) params.append('sortOrder', sortOrder);
    if (search) params.append('search', search);
    console.log('Fetching stock movements with params:', params.toString());
    const response = await fetchWithAuth(
      `${BASE_URL}/stock-movements-history?${params.toString()}`
    );
    if (!response.success) {
      console.log('Failed to fetch stock movements history:', response.error);
      throw new Error('Failed to fetch stock movements history');
    }
    return await response.response.json();
  } catch (error) {
    console.error(error);
    throw error;
  }
}

export { getSummary, getAllProducts, getStockMovementsHistory };
