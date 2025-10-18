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
export { getLocationsNames, getLocationById };
