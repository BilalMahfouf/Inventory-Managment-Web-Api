import { api, extractErrorMessage } from '@shared/services/api/api';

const BASE_URL = '/customer-categories';

async function getCustomerCategoriesNames() {
  try {
    const { data } = await api.get(`${BASE_URL}/names`);
    console.log(data);
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

export { getCustomerCategoriesNames };
