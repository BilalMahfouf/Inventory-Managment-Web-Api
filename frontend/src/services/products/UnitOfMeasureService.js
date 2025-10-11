import { fetchWithAuth } from '../auth/authService';

const BASE_URL = '/api/unit-of-measures';

export const getUnitOfMeasures = async () => {
  try {
    const response = await fetchWithAuth(`${BASE_URL}`);
    if (!response.success) {
      const errorText = await response.response.text();
      console.error('Failed to fetch unit of measures:', errorText);
      throw new Error('Failed to fetch unit of measures');
    }
    return response.response.json();
  } catch (error) {
    console.error('Error fetching unit of measures:', error);
    throw error;
  }
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

async function deleteUnitOfMeasure(id) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${id}`, {
      method: 'DELETE',
    });
    if (response.success) {
      return true;
    }
    const errorText = await response.response.text();
    console.error('Failed to delete unit of measure:', errorText);
    throw new Error('Failed to delete unit of measure');
  } catch (error) {
    console.error('Error deleting unit of measure:', error);
    throw error;
  }
}

export { GetUnitsNames, deleteUnitOfMeasure };
