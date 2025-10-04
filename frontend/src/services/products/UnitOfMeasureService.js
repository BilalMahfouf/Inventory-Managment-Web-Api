import { fetchWithAuth } from '../auth/authService';

const BASE_URL = '/api/unit-of-measures';

export const getUnitOfMeasures = async () => {
  const response = await fetchWithAuth(`${BASE_URL}`);
  if (!response.ok) {
    throw new Error('Failed to fetch unit of measures');
  }
  return response.json();
};

async function GetUnitsNames() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/names`);
    if (!response.success) {
      console.log('Failed to fetch unit of measures names:', response.error);
      throw new Error('Failed to fetch unit of measures names');
    }
    return await response.response.json();
  } catch (error) {
    console.error(error);
    throw error;
  }
}

export { GetUnitsNames };
