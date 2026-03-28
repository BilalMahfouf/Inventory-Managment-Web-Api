import { api, extractErrorMessage } from '@shared/services/api/api';

const BASE_URL = '/stock-transfers';

async function getAllStockTransfers({
  page,
  pageSize,
  sortColumn,
  sortOrder,
  search,
}) {
  try {
    const params = new URLSearchParams({
      page: page || 1,
      pageSize: pageSize || 10,
    });
    if (sortColumn) params.append('sortColumn', sortColumn);
    if (sortOrder) params.append('sortOrder', sortOrder);
    if (search) params.append('search', search);
    const { data } = await api.get(`${BASE_URL}?${params.toString()}`);
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching stock transfers:', error);
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to fetch stock transfers'),
    };
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
    const { data } = await api.post(BASE_URL, transferData);
    return { success: true, data };
  } catch (error) {
    console.error('Error creating stock transfer:', error);
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to create stock transfer'),
    };
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
    const { data } = await api.patch(`${BASE_URL}/${transferId}/status`, {
      status: newStatus,
    });
    return { success: true, data };
  } catch (error) {
    console.error('Error updating stock transfer status:', error);
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to update stock transfer status'),
    };
  }
}

/**
 * Get stock transfer by ID
 * @param {number} transferId - Transfer ID
 * @returns {Promise<{success: boolean, data?: any, error?: string}>}
 */
async function getStockTransferById(transferId) {
  try {
    const { data } = await api.get(`${BASE_URL}/${transferId}`);
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching stock transfer:', error);
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to fetch stock transfer'),
    };
  }
}

export {
  getAllStockTransfers,
  createStockTransfer,
  updateStockTransferStatus,
  getStockTransferById,
};
