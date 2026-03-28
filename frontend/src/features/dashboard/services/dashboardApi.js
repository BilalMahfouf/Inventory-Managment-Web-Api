import { api, extractErrorMessage } from '@shared/services/api/api';

const API_BASE_URL = '/dashboard';

async function _getInventoryAlerts() {
  try {
    const { data } = await api.get(`${API_BASE_URL}/inventory-alerts`);
    return data;
  } catch (error) {
    console.error('An error occurred while fetching inventory alerts:', error);
    throw error;
  }
}

async function _getTopSellingProducts(numberOfProducts = 5) {
  try {
    const { data } = await api.get(
      `${API_BASE_URL}/top-selling-products?numberOfProducts=${numberOfProducts}`
    );
    return data;
  } catch (error) {
    console.error(
      'An error occurred while fetching top selling products:',
      error
    );
    throw error;
  }
}

async function _getSummary() {
  try {
    const { data } = await api.get(`${API_BASE_URL}/summary`);
    return data;
  } catch (error) {
    const message = extractErrorMessage(error, 'Failed to fetch dashboard summary');
    console.error('Failed to fetch dashboard summary:', message);
    throw new Error(message);
  }
}

async function _getTodayPerformance() {
  try {
    const { data } = await api.get(`${API_BASE_URL}/today-performance`);
    return data;
  } catch (error) {
    const message = extractErrorMessage(
      error,
      "Failed to fetch today's performance data"
    );
    console.error("Failed to fetch today's performance data:", message);
    throw new Error(message);
  }
}

const dashboardApi = {
  getInventoryAlerts: async () => _getInventoryAlerts(),
  getTopSellingProducts: async numberOfProducts =>
    _getTopSellingProducts(numberOfProducts),
  getSummary: async () => _getSummary(),
  getTodayPerformance: async () => _getTodayPerformance(),
};

export default dashboardApi;
