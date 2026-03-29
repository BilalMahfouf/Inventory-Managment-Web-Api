import { api, extractErrorMessage } from '@shared/services/api/api';

const BASE_URL = '/customer-categories';

async function getCustomerCategoriesNames() {
  try {
    const { data } = await api.get(`${BASE_URL}/names`);
    return {
      success: true,
      data,
    };
  } catch (error) {
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to fetch customer categories names'),
    };
  }
}

async function getAllCustomerCategories() {
  try {
    const { data } = await api.get(`${BASE_URL}`);
    return {
      success: true,
      data,
    };
  } catch (error) {
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to fetch customer categories'),
    };
  }
}

async function getCustomerCategoryById(id) {
  try {
    const { data } = await api.get(`${BASE_URL}/${id}`);
    return {
      success: true,
      data,
    };
  } catch (error) {
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to fetch customer category details'),
    };
  }
}

async function createCustomerCategory(payload) {
  try {
    const { data } = await api.post(`${BASE_URL}`, payload);
    return {
      success: true,
      data,
    };
  } catch (error) {
    return {
      success: false,
      message: extractErrorMessage(error, 'Failed to create customer category'),
    };
  }
}

async function updateCustomerCategory(id, payload) {
  try {
    const { data } = await api.put(`${BASE_URL}/${id}`, payload);
    return {
      success: true,
      data,
    };
  } catch (error) {
    return {
      success: false,
      message: extractErrorMessage(error, 'Failed to update customer category'),
    };
  }
}

async function deleteCustomerCategory(id) {
  try {
    await api.delete(`${BASE_URL}/${id}`);
    return {
      success: true,
    };
  } catch (error) {
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to delete customer category'),
    };
  }
}

export {
  getCustomerCategoriesNames,
  getAllCustomerCategories,
  getCustomerCategoryById,
  createCustomerCategory,
  updateCustomerCategory,
  deleteCustomerCategory,
};
