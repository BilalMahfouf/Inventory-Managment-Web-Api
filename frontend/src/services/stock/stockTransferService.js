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

/**
 * Create a new stock transfer
 * @param {Object} transferData - Transfer data
 * @param {number} transferData.productId - Product ID
 * @param {number} transferData.fromLocationId - From location ID
 * @param {number} transferData.toLocationId - To location ID
 * @param {number} transferData.quantity - Transfer quantity
 * @returns {Promise<{success: boolean, data?: any, error?: string}>}
 */
async function createStockTransfer(transferData) {
  try {
    const response = await fetchWithAuth(BASE_URL, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(transferData),
    });

    if (!response.success) {
      const error = await response.error;
      return { success: false, error: error };
    }

    const data = await response.response.json();
    return { success: true, data: data };
  } catch (error) {
    console.error('Error creating stock transfer:', error);
    return { success: false, error: error.message };
  }
}

/**
 * Update stock transfer status
 * @param {number} transferId - Transfer ID
 * @param {string} newStatus - New transfer status
 * @returns {Promise<{success: boolean, data?: any, error?: string}>}
 */
async function updateStockTransferStatus(transferId, newStatus) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${transferId}/status`, {
      method: 'PATCH',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ status: newStatus }),
    });

    if (!response.success) {
      const error = await response.error;
      return { success: false, error: error };
    }

    const data = await response.response.json();
    return { success: true, data: data };
  } catch (error) {
    console.error('Error updating stock transfer status:', error);
    return { success: false, error: error.message };
  }
}

/**
 * Get stock transfer by ID
 * @param {number} transferId - Transfer ID
 * @returns {Promise<{success: boolean, data?: any, error?: string}>}
 */
async function getStockTransferById(transferId) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${transferId}`);

    if (!response.success) {
      const error = await response.error;
      return { success: false, error: error };
    }

    const data = await response.response.json();
    return { success: true, data: data };
  } catch (error) {
    console.error('Error fetching stock transfer:', error);
    return { success: false, error: error.message };
  }
}

export {
  getAllStockTransfers,
  createStockTransfer,
  updateStockTransferStatus,
  getStockTransferById,
};
