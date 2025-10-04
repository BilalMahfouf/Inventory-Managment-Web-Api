import { fetchWithAuth } from '../auth/authService';

const BASE_URL = '/api/product-categories';

async function getProductCategories() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/names`);
    if (!response.success) {
      console.log('Failed to fetch product categories:', response.error);
      throw new Error('Failed to fetch product categories');
    }
    return await response.response.json();
  } catch (error) {
    console.error(error);
    throw error;
  }
}

export { getProductCategories };
