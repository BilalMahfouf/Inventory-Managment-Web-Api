import { fetchWithAuth } from '../auth/authService';

const BASE_URL = '/api/customers';
const CUSTOMER_CATEGORY_URL = '/api/customer-categories';

async function getCustomerSummary() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/summary`);
    if (!response.success) {
      const errorMessage = await response.error;
      return {
        success: false,
        error: errorMessage || 'Failed to fetch customer summary',
      };
    }
    const data = await response.response.json();
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching customer summary:', error);
    return { success: false, error };
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
    const response = await fetchWithAuth(`${BASE_URL}?${params.toString()}`);
    if (!response.success) {
      const errorMessage = await response.error;
      return {
        success: false,
        error: errorMessage || 'Failed to fetch customers',
      };
    }
    const data = await response.response.json();
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching customers:', error);
    return { success: false, error };
  }
}

async function getCustomerById(id) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${id}`);
    if (!response.success) {
      const errorMessage = await response.error;
      return {
        success: false,
        error: errorMessage || 'Failed to fetch customer',
      };
    }
    const data = await response.response.json();
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching customer:', error);
    return { success: false, error };
  }
}

async function createCustomer(customerData) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(customerData),
    });
    if (!response.success) {
      const errorMessage = await response.error;
      return {
        success: false,
        message: errorMessage || 'Failed to create customer',
      };
    }
    const data = await response.response.json();
    return { success: true, data };
  } catch (error) {
    console.error('Error creating customer:', error);
    return { success: false, message: error.message };
  }
}

async function updateCustomer(id, customerData) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(customerData),
    });
    if (!response.success) {
      const errorMessage = await response.error;
      return {
        success: false,
        message: errorMessage || 'Failed to update customer',
      };
    }
    const data = await response.response.json();
    return { success: true, data };
  } catch (error) {
    console.error('Error updating customer:', error);
    return { success: false, message: error.message };
  }
}

async function getCustomerCategories() {
  try {
    const response = await fetchWithAuth(`${CUSTOMER_CATEGORY_URL}`);
    if (!response.success) {
      const errorMessage = await response.error;
      return {
        success: false,
        error: errorMessage || 'Failed to fetch customer categories',
      };
    }
    const data = await response.response.json();
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching customer categories:', error);
    return { success: false, error };
  }
}

export {
  getCustomerSummary,
  getCustomers,
  getCustomerById,
  createCustomer,
  updateCustomer,
  getCustomerCategories,
};
