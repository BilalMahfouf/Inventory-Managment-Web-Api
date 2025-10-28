import { fetchWithAuth } from '../auth/authService';

const BASE_URL = '/api/customers';
async function getCustomerSummary() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/summary`);
    if (!response.success) {
      const errorMessage = await response.error;
      return {
        success: false,
        error: errorMessage || 'Failed to fetch customer summary',
      };
    }
    const data = await response.response.json();
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching customer summary:', error);
    return { success: false, error };
  }
}
async function getCustomers({ page, pageSize, sortColumn, sortOrder, search }) {
  try {
    const params = new URLSearchParams();
    if (page) params.append('page', page);
    if (pageSize) params.append('pageSize', pageSize);
    if (sortColumn) params.append('sortColumn', sortColumn);
    if (sortOrder) params.append('sortOrder', sortOrder);
    if (search) params.append('search', search);
    const response = await fetchWithAuth(`${BASE_URL}?${params.toString()}`);
    if (!response.success) {
      const errorMessage = await response.error;
      return {
        success: false,
        error: errorMessage || 'Failed to fetch customers',
      };
    }
    const data = await response.response.json();
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching customers:', error);
    return { success: false, error };
  }
}

export { getCustomerSummary, getCustomers };
