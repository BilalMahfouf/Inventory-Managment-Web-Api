import { fetchWithAuth } from './auth/authService';

const API_BASE_URL = '/api/dashboard';

async function _getInventoryAlerts() {
  try {
    const response = await fetchWithAuth(`${API_BASE_URL}/inventory-alerts`);
    if (!response.success) {
      console.error('Failed to fetch inventory alerts:', response.error);
      throw new Error(response.error || 'Failed to fetch inventory alerts');
    }
    return await response.response.json();
  } catch (error) {
    console.error('An error occurred while fetching inventory alerts:', error);
    throw error;
  }
}

async function _getTopSellingProducts(numberOfProducts = 5) {
  try {
    const response = await fetchWithAuth(
      `${API_BASE_URL}/top-selling-products?numberOfProducts=${numberOfProducts}`
    );
    if (!response.success) {
      console.error('Failed to fetch top selling products:', response.error);
      throw new Error(response.error || 'Failed to fetch top selling products');
    }
    return await response.response.json();
  } catch (error) {
    console.error(
      'An error occurred while fetching top selling products:',
      error
    );
    throw error;
  }
}

const dashboardService = {
  getInventoryAlerts: async () => _getInventoryAlerts(),
  getTopSellingProducts: async numberOfProducts =>
    _getTopSellingProducts(numberOfProducts),
};

export default dashboardService;
