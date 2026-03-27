import { fetchWithAuth } from '../auth/authService';
const BASE_URL = 'api/location-type';

async function getAllLocationTypes() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}`);
    if (!response.success) {
      const errorMessage = await response.error;
      return { success: false, error: errorMessage };
    }
    const data = await response.response.json();
    return { success: true, data: data };
  } catch (error) {
    console.error('Error fetching location types:', error);
    return { success: false, error: error.message };
  }
}

async function getLocationTypeById(locationTypeId) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${locationTypeId}`);
    if (!response.success) {
      const errorMessage = await response.error;
      return { success: false, error: errorMessage };
    }
    const data = await response.response.json();
    return { success: true, data: data };
  } catch (error) {
    console.error('Error fetching location type by ID:', error);
    return { success: false, error: error.message };
  }
}

async function createLocationType({ name, description }) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}`, {
      method: 'POST',
      body: JSON.stringify({ name, description }),
    });
    if (!response.success) {
      const errorMessage = await response.error;
      return { success: false, message: errorMessage };
    }
    const data = await response.response.json();
    return { success: true, data };
  } catch (error) {
    console.error('Error creating location type:', error);
    return { success: false, message: error.message };
  }
}

export { getAllLocationTypes, getLocationTypeById, createLocationType };
