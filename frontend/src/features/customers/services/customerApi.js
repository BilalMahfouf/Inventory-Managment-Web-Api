import { api, extractErrorMessage } from '@shared/services/api/api';

const BASE_URL = '/customers';
const CUSTOMER_CATEGORY_URL = '/customer-categories';

async function getCustomerSummary() {
  try {
    const { data } = await api.get(`${BASE_URL}/summary`);
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching customer summary:', error);
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to fetch customer summary'),
    };
  }
}

async function getCustomers({ page, pageSize, sortColumn, sortOrder, search }) {
  try {
    const params = new URLSearchParams();
    if (page) params.append('page', page);
    if (pageSize) params.append('pageSize', pageSize);
    if (sortColumn) params.append('sortColumn', sortColumn);
    if (sortOrder) params.append('sortOrder', sortOrder);
    if (search) params.append('search', search);
    const { data } = await api.get(`${BASE_URL}?${params.toString()}`);
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching customers:', error);
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to fetch customers'),
    };
  }
}

async function getCustomerById(id) {
  try {
    const { data } = await api.get(`${BASE_URL}/${id}`);
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching customer:', error);
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to fetch customer'),
    };
  }
}

async function createCustomer(customerData) {
  try {
    const { data } = await api.post(`${BASE_URL}`, customerData);
    return { success: true, data };
  } catch (error) {
    console.error('Error creating customer:', error);
    return {
      success: false,
      message: extractErrorMessage(error, 'Failed to create customer'),
    };
  }
}

async function updateCustomer(id, customerData) {
  try {
    const { data } = await api.put(`${BASE_URL}/${id}`, customerData);
    return { success: true, data };
  } catch (error) {
    console.error('Error updating customer:', error);
    return {
      success: false,
      message: extractErrorMessage(error, 'Failed to update customer'),
    };
  }
}

async function getCustomerCategories() {
  try {
    const { data } = await api.get(`${CUSTOMER_CATEGORY_URL}`);
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching customer categories:', error);
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to fetch customer categories'),
    };
  }
}

async function deleteCustomerById(id) {
  try {
    await api.delete(`${BASE_URL}/${id}`);
    return { success: true };
  } catch (error) {
    console.error('Error deleting customer:', error);
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to delete customer'),
    };
  }
}

export {
  getCustomerSummary,
  getCustomers,
  getCustomerById,
  createCustomer,
  updateCustomer,
  getCustomerCategories,
  deleteCustomerById,
};
