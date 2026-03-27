import { fetchWithAuth } from '../auth/authService';
const BASE_URL = 'api/locations';

async function getLocationsNames() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/names`);
    if (!response.success) {
      throw new Error('Failed to fetch locations');
    }
    return await response.response.json();
  } catch (error) {
    console.error('Error fetching locations:', error);
    return [];
  }
}
async function getLocationById(locationId) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${locationId}`);
    if (!response.success) {
      const errorMessage = await response.error;
      return { success: false, error: errorMessage };
    }
    const data = await response.response.json();
    return { success: true, data: data };
  } catch (error) {
    console.error('Error fetching location by ID:', error);
    return { success: false, error: error.message };
  }
}

async function getLocations() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}`);
    if (!response.success) {
      const errorMessage = await response.error;
      return { success: false, error: errorMessage };
    }
    const data = await response.response.json();
    return { success: true, data: data };
  } catch (error) {
    console.error('Error fetching locations:', error);
    return { success: false, error: error.message };
  }
}

async function createLocation({ name, address, locationTypeId }) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}`, {
      method: 'POST',
      body: JSON.stringify({ name, address, locationTypeId }),
      headers: { 'Content-Type': 'application/json' },
    });
    if (!response.success) {
      const errorMessage = await response.error;
      return { success: false, message: errorMessage };
    }
    const data = await response.response.json();
    return { success: true, data };
  } catch (error) {
    console.error('Error creating location:', error);
    return { success: false, message: error.message };
  }
}

async function updateLocation(id, { name, address, locationTypeId, isActive }) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${id}`, {
      method: 'PUT',
      body: JSON.stringify({ id, name, address, locationTypeId, isActive }),
      headers: { 'Content-Type': 'application/json' },
    });
    if (!response.success) {
      const errorMessage = await response.error;
      return { success: false, message: errorMessage };
    }
    const data = await response.response.json();
    return { success: true, data };
  } catch (error) {
    console.error('Error updating location:', error);
    return { success: false, message: error.message };
  }
}

async function deleteLocation(id) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${id}`, {
      method: 'DELETE',
    });
    if (!response.success) {
      const errorMessage = await response.error;
      return { success: false, message: errorMessage };
    }
    return { success: true };
  } catch (error) {
    console.error('Error deleting location:', error);
    return { success: false, message: error.message };
  }
}

export {
  getLocationsNames,
  getLocationById,
  getLocations,
  createLocation,
  updateLocation,
  deleteLocation,
};
