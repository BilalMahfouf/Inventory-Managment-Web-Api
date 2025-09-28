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

async function getAllProducts({ page = 1, pageSize = 10 }) {
  try {
    const response = await fetchWithAuth(
      `${BASE_URL}?page=${page}&pageSize=${pageSize}`
    );
    if (!response.success) {
      console.log('Failed to fetch products:', response.error);
      throw new Error('Failed to fetch products');
    }
    const data = await response.response.json();
    if (data === null || data === undefined) {
      return [];
    }
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
}

export { getSummary, getAllProducts };
