import { fetchWithAuth } from '../auth/authService';
const BASE_URL = 'api/customer-categories';

async function getCustomerCategoriesNames() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/names`);
    if (!response.success) {
      const errorMessage = await response.error;
      return {
        success: false,
        error: errorMessage || 'Failed to fetch customer categories names',
      };
    }
    const data = await response.response.json();
    console.log(data);
    return {
      success: true,
      data: data,
    };
  } catch (error) {
    return {
      success: false,
      error: error.message || 'Failed to fetch customer categories names',
    };
  }
}

export { getCustomerCategoriesNames };
