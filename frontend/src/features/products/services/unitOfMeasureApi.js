import { api, extractErrorMessage } from '@shared/services/api/api';

const BASE_URL = '/unit-of-measures';

export const getUnitOfMeasures = async () => {
  try {
    const { data } = await api.get(`${BASE_URL}`);
    return data;
  } catch (error) {
    console.error('Error fetching unit of measures:', error);
    throw error;
  }
};

async function GetUnitsNames() {
  try {
    const { data } = await api.get(`${BASE_URL}/names`);
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function getUnitOfMeasureById(id) {
  try {
    const { data } = await api.get(`${BASE_URL}/${id}`);
    return data;
  } catch (error) {
    console.error('Error fetching unit of measure:', error);
    throw error;
  }
}

async function createUnitOfMeasure({ name, description }) {
  try {
    const { data } = await api.post(`${BASE_URL}`, { name, description });
    return { success: true, data };
  } catch (error) {
    console.error('Error creating unit of measure:', error);
    throw error;
  }
}

async function updateUnitOfMeasure(id, { name, description }) {
  try {
    const { data } = await api.put(`${BASE_URL}/${id}`, { name, description });
    return { success: true, data };
  } catch (error) {
    console.error('Error updating unit of measure:', error);
    throw error;
  }
}

async function deleteUnitOfMeasure(id) {
  try {
    await api.delete(`${BASE_URL}/${id}`);
    return true;
  } catch (error) {
    const errorText = extractErrorMessage(error, 'Failed to delete unit of measure');
    console.error('Error deleting unit of measure:', errorText);
    throw new Error(errorText);
  }
}

export {
  GetUnitsNames,
  getUnitOfMeasureById,
  createUnitOfMeasure,
  updateUnitOfMeasure,
  deleteUnitOfMeasure,
};
