const BASE_URL = '/products';

import { api, extractErrorMessage } from '@shared/services/api/api';

async function getSummary() {
  try {
    const { data } = await api.get(`${BASE_URL}/summary`);
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function getAllProducts({
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
      return { item: [], totalCount: 0 };
    }
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function getStockMovementsHistory({
  page,
  pageSize,
  sortColumn,
  sortOrder,
  search,
}) {
  try {
    let params = new URLSearchParams();
    params.append('page', page ? page : 1);
    params.append('pageSize', pageSize ? pageSize : 10);
    if (sortColumn) params.append('sortColumn', sortColumn);
    if (sortOrder) params.append('sortOrder', sortOrder);
    if (search) params.append('search', search);
    const { data } = await api.get(
      `${BASE_URL}/stock-movements-history?${params.toString()}`
    );
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function createProduct({
  name,
  sku,
  description,
  categoryId,
  unitOfMeasureId,
  unitPrice,
  costPrice,
  locationId,
  quantityOnHand,
  reorderLevel,
  maxLevel,
}) {
  try {
    const { data } = await api.post(`${BASE_URL}`, {
      name,
      sku,
      description,
      categoryId,
      unitOfMeasureId,
      unitPrice,
      costPrice,
      locationId,
      quantityOnHand,
      reorderLevel,
      maxLevel,
    });
    return data?.data ?? data;
  } catch (error) {
    const errorMessage = extractErrorMessage(error, 'Failed to create product');
    console.error('Failed to create product:', errorMessage);
    throw new Error(errorMessage);
  }
}

async function getProductById(id) {
  try {
    const { data } = await api.get(`${BASE_URL}/${id}`);
    return data;
  } catch (error) {
    console.error('Failed to fetch product:', error);
    throw error;
  }
}

async function updateProduct(id, productData) {
  try {
    const { data } = await api.put(`${BASE_URL}/${id}`, productData);
    return data;
  } catch (error) {
    const errorMessage = extractErrorMessage(error, 'Failed to update product');
    console.error('Failed to update product:', errorMessage);
    throw new Error(errorMessage);
  }
}
async function deleteProduct(id) {
  try {
    await api.delete(`${BASE_URL}/${id}`);
  } catch (error) {
    const errorMessage = extractErrorMessage(error, 'Failed to delete product');
    console.error('Failed to delete product:', errorMessage);
    throw new Error(errorMessage);
  }
}

async function getInventoriesByProductId(productId) {
  try {
    const { data } = await api.get(`${BASE_URL}/${productId}/inventory`);
    return { success: true, data: data };
  } catch (error) {
    const errorMessage = extractErrorMessage(
      error,
      'Failed to fetch inventories for product'
    );
    console.error('Failed to fetch inventories for product:', errorMessage);
    return {
      success: false,
      error: errorMessage,
    };
  }
}

export {
  getSummary,
  getAllProducts,
  getStockMovementsHistory,
  createProduct,
  getProductById,
  updateProduct,
  deleteProduct,
  getInventoriesByProductId,
};
