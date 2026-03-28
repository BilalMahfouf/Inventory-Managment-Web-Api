import { fetchWithAuth } from '@shared/services/auth/authService';

const URL = '/api/sales-orders';

async function getOrdersDahsobardSummary() {
  try {
    const response = await fetchWithAuth(`${URL}/summary`);
    if (!response.success) {
      const errorMessage = await response.error;
      return { isSuccess: false, error: errorMessage };
    }
    return { isSuccess: true, data: response.response.json() };
  } catch (e) {
    return { isSuccess: false, exception: e };
  }
}

export { getOrdersDahsobardSummary };
