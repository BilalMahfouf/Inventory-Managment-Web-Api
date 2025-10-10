import { fetchWithAuth } from '../auth/authService';
const BASE_URL = 'api/locations';

export async function getLocationsNames() {
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
