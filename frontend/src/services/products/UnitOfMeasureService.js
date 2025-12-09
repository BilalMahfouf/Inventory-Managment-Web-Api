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

async function getUnitOfMeasureById(id) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${id}`);
    if (!response.success) {
      const errorText = await response.response.text();
      console.error('Failed to fetch unit of measure:', errorText);
      throw new Error('Failed to fetch unit of measure');
    }
    return response.response.json();
  } catch (error) {
    console.error('Error fetching unit of measure:', error);
    throw error;
  }
}

async function createUnitOfMeasure({ name, description }) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ name, description }),
    });
    if (!response.success) {
      const errorText = await response.error;
      console.error('Failed to create unit of measure:', errorText);
      return { success: false, message: errorText };
    }
    return { success: true, data: await response.response.json() };
  } catch (error) {
    console.error('Error creating unit of measure:', error);
    throw error;
  }
}

async function updateUnitOfMeasure(id, { name, description }) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ name, description }),
    });
    if (!response.success) {
      const errorText = await response.error;
      console.error('Failed to update unit of measure:', errorText);
      return { success: false, message: errorText };
    }
    return { success: true, data: await response.response.json() };
  } catch (error) {
    console.error('Error updating unit of measure:', error);
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

export {
  GetUnitsNames,
  getUnitOfMeasureById,
  createUnitOfMeasure,
  updateUnitOfMeasure,
  deleteUnitOfMeasure,
};
