import { api, extractErrorMessage } from '@shared/services/api/api';

const BASE_URL = '/locations';

async function getLocationsNames() {
  try {
    const { data } = await api.get(`${BASE_URL}/names`);
    return data;
  } catch (error) {
    console.error('Error fetching locations:', error);
    return [];
  }
}
async function getLocationById(locationId) {
  try {
    const { data } = await api.get(`${BASE_URL}/${locationId}`);
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching location by ID:', error);
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to fetch location by ID'),
    };
  }
}

async function getLocations() {
  try {
    const { data } = await api.get(`${BASE_URL}`);
    return { success: true, data };
  } catch (error) {
    console.error('Error fetching locations:', error);
    return {
      success: false,
      error: extractErrorMessage(error, 'Failed to fetch locations'),
    };
  }
}

async function createLocation({ name, address, locationTypeId }) {
  try {
    const { data } = await api.post(`${BASE_URL}`, {
      name,
      address,
      locationTypeId,
    });
    return { success: true, data };
  } catch (error) {
    console.error('Error creating location:', error);
    return {
      success: false,
      message: extractErrorMessage(error, 'Failed to create location'),
    };
  }
}

async function updateLocation(id, { name, address, locationTypeId, isActive }) {
  try {
    const { data } = await api.put(`${BASE_URL}/${id}`, {
      id,
      name,
      address,
      locationTypeId,
      isActive,
    });
    return { success: true, data };
  } catch (error) {
    console.error('Error updating location:', error);
    return {
      success: false,
      message: extractErrorMessage(error, 'Failed to update location'),
    };
  }
}

async function deleteLocation(id) {
  try {
    await api.delete(`${BASE_URL}/${id}`);
    return { success: true };
  } catch (error) {
    console.error('Error deleting location:', error);
    return {
      success: false,
      message: extractErrorMessage(error, 'Failed to delete location'),
    };
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
