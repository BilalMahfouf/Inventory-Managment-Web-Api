import { fetchWithAuth } from '@services/auth/authService';
const BASE_URL = '/api/inventory';

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

    const response = await fetchWithAuth(`${BASE_URL}?${params.toString()}`);
    if (!response.success) {
      console.error('Failed to fetch inventory:', response.error);
      return { success: false, error: response.error };
    }
    const data = await response.response.json();
    if (data === null || data === undefined) {
      return { success: false, data: [] };
    }
    return { success: true, data: data };
  } catch (error) {
    console.error(error);
    return { success: false, error: error.message };
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
    const response = await fetchWithAuth(`${BASE_URL}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        productId,
        locationId,
        quantityOnHand,
        reorderLevel,
        maxLevel,
      }),
    });
    if (!response.success) {
      const errorMessage = await response.error;
      console.error('Failed to create inventory:', errorMessage);
      return { success: false, error: errorMessage };
    }
    const data = await response.response.json();
    return { success: true, data: data };
  } catch (error) {
    console.error(error);
    return { success: false, error: error.message };
  }
}

async function updateInventory(
  inventoryId,
  { quantityOnHand, reorderLevel, maxLevel }
) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${inventoryId}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        quantityOnHand,
        reorderLevel,
        maxLevel,
      }),
    });
    if (!response.success) {
      const errorMessage = await response.error;
      console.error('Failed to update inventory:', errorMessage);
      return { success: false, error: errorMessage };
    }
    const data = await response.response.json();
    return { success: true, data: data };
  } catch (error) {
    console.error(error);
    return { success: false, error: error.message };
  }
}

async function getInventoryById(id) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${id}`);
    if (!response.success) {
      const errorMessage = await response.error;
      console.error('Failed to fetch inventory by ID:', errorMessage);
      return { success: false, error: errorMessage };
    }
    const data = await response.response.json();
    return { success: true, data: data };
  } catch (error) {
    console.error(error);
    return { success: false, error: error.message };
  }
}

export { getAllInventory, createInventory, updateInventory, getInventoryById };
