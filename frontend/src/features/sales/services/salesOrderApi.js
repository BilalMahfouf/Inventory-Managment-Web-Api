import { api, extractErrorMessage } from '@shared/services/api/api';

const URL = '/sales-orders';

async function getOrdersDahsobardSummary() {
  try {
    const { data } = await api.get(`${URL}/summary`);
    return { isSuccess: true, data };
  } catch (e) {
    return {
      isSuccess: false,
      exception: extractErrorMessage(e, 'Failed to fetch sales summary'),
    };
  }
}

export { getOrdersDahsobardSummary };
