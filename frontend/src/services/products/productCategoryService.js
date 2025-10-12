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
async function getAllProductCategories() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}`);
    if (!response.success) {
      console.log('Failed to fetch all product categories:', response.error);
      throw new Error('Failed to fetch all product categories');
    }
    return await response.response.json();
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function getProductCategoryTree() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/tree`);
    if (!response.success) {
      console.log('Failed to fetch product category tree:', response.error);
      throw new Error('Failed to fetch product category tree');
    }
    return await response.response.json();
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function getProductCategoryById(id) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${id}`);
    if (!response.success) {
      console.log('Failed to fetch product category:', response.error);
      throw new Error('Failed to fetch product category');
    }
    return await response.response.json();
  } catch (error) {
    console.error(error);
    throw error;
  }
}

export {
  getProductCategories,
  getAllProductCategories,
  getProductCategoryTree,
  getProductCategoryById,
};
