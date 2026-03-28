import { api, extractErrorMessage } from '@shared/services/api/api';

const BASE_URL = '/location-type';

async function getAllLocationTypes() {
  try {
    const { data } = await api.get(`${BASE_URL}`);
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching location types:', error);
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to fetch location types'),
    };
  }
}

async function getLocationTypeById(locationTypeId) {
  try {
    const { data } = await api.get(`${BASE_URL}/${locationTypeId}`);
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching location type by ID:', error);
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to fetch location type by ID'),
    };
  }
}

async function createLocationType({ name, description }) {
  try {
    const { data } = await api.post(`${BASE_URL}`, { name, description });
    return { success: true, data };
  } catch (error) {
    console.error('Error creating location type:', error);
    return {
      success: false,
      message: extractErrorMessage(error, 'Failed to create location type'),
    };
  }
}

export { getAllLocationTypes, getLocationTypeById, createLocationType };
