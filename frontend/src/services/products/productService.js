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

export { getSummary };
