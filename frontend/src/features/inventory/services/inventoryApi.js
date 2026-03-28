import { api, extractErrorMessage } from '@shared/services/api/api';
const BASE_URL = '/inventory';

async function getAllInventory({
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

    const { data } = await api.get(`${BASE_URL}?${params.toString()}`);
    if (data === null || data === undefined) {
      return { success: false, data: [] };
    }
    return { success: true, data };
  } catch (error) {
    console.error(error);
    return { success: false, error: extractErrorMessage(error, 'Failed to fetch inventory') };
  }
}

async function createInventory({
  productId,
  locationId,
  quantityOnHand,
  reorderLevel,
  maxLevel,
}) {
  try {
    const { data } = await api.post(`${BASE_URL}`, {
      productId,
      locationId,
      quantityOnHand,
      reorderLevel,
      maxLevel,
    });
    return { success: true, data };
  } catch (error) {
    console.error(error);
    return { success: false, error: extractErrorMessage(error, 'Failed to create inventory') };
  }
}

async function updateInventory(
  inventoryId,
  { quantityOnHand, reorderLevel, maxLevel }
) {
  try {
    const { data } = await api.put(`${BASE_URL}/${inventoryId}`, {
      quantityOnHand,
      reorderLevel,
      maxLevel,
    });
    return { success: true, data };
  } catch (error) {
    console.error(error);
    return { success: false, error: extractErrorMessage(error, 'Failed to update inventory') };
  }
}

async function getInventoryById(id) {
  try {
    const { data } = await api.get(`${BASE_URL}/${id}`);
    return { success: true, data };
  } catch (error) {
    console.error(error);
    return { success: false, error: extractErrorMessage(error, 'Failed to fetch inventory by ID') };
  }
}

async function deleteInventoryById(id) {
  try {
    await api.delete(`${BASE_URL}/${id}`);
    return { success: true };
  } catch (error) {
    console.error(error);
    return { success: false, error: extractErrorMessage(error, 'Failed to delete inventory') };
  }
}
async function getInventorySummary() {
  try {
    const { data } = await api.get(`${BASE_URL}/summary`);
    return { success: true, data };
  } catch (error) {
    console.error(error);
    return { success: false, error: extractErrorMessage(error, 'Failed to fetch inventory summary') };
  }
}

export {
  getAllInventory,
  createInventory,
  updateInventory,
  getInventoryById,
  deleteInventoryById,
  getInventorySummary,
};
