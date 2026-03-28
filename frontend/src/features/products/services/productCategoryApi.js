import { api, extractErrorMessage } from '@shared/services/api/api';

const BASE_URL = '/product-categories';

async function getProductCategories() {
  try {
    const { data } = await api.get(`${BASE_URL}/names`);
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
}
async function getAllProductCategories() {
  try {
    const { data } = await api.get(`${BASE_URL}`);
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function getProductCategoryTree() {
  try {
    const { data } = await api.get(`${BASE_URL}/tree`);
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function getProductCategoryById(id) {
  try {
    const { data } = await api.get(`${BASE_URL}/${id}`);
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function getMainCategories() {
  try {
    const { data } = await api.get(`${BASE_URL}/main-categories`);
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function createProductCategory({ name, description, type, parentId }) {
  try {
    const { data } = await api.post(`${BASE_URL}`, {
      name,
      description: description || null,
      parentId: type === 2 ? parentId : null,
    });
    return {
      success: true,
      data,
    };
  } catch (error) {
    console.error(error);
    return {
      success: false,
      message: extractErrorMessage(error, 'Failed to create product category'),
    };
  }
}

async function updateProductCategory(
  id,
  { name, description, type, parentId }
) {
  try {
    const { data } = await api.put(`${BASE_URL}/${id}`, {
      name,
      description: description || null,
      parentId: type === 2 ? parentId : null,
    });
    return {
      success: true,
      data,
    };
  } catch (error) {
    console.error(error);
    return {
      success: false,
      message: extractErrorMessage(error, 'Failed to update product category'),
    };
  }
}

async function deleteProductCategory(id) {
  try {
    await api.delete(`${BASE_URL}/${id}`);
    return {
      success: true,
    };
  } catch (error) {
    console.error(error);
    return {
      success: false,
      message: extractErrorMessage(error, 'Failed to delete product category'),
    };
  }
}

export {
  getProductCategories,
  getAllProductCategories,
  getProductCategoryTree,
  getProductCategoryById,
  getMainCategories,
  createProductCategory,
  updateProductCategory,
  deleteProductCategory,
};
