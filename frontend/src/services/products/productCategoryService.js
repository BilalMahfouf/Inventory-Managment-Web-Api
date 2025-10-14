import { fetchWithAuth } from '../auth/authService';

const BASE_URL = '/api/product-categories';

async function getProductCategories() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/names`);
    if (!response.success) {
      console.log('Failed to fetch product categories:', response.error);
      throw new Error('Failed to fetch product categories');
    }
    return await response.response.json();
  } catch (error) {
    console.error(error);
    throw error;
  }
}
async function getAllProductCategories() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}`);
    if (!response.success) {
      console.log('Failed to fetch all product categories:', response.error);
      throw new Error('Failed to fetch all product categories');
    }
    return await response.response.json();
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function getProductCategoryTree() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/tree`);
    if (!response.success) {
      console.log('Failed to fetch product category tree:', response.error);
      throw new Error('Failed to fetch product category tree');
    }
    return await response.response.json();
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function getProductCategoryById(id) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${id}`);
    if (!response.success) {
      console.log('Failed to fetch product category:', response.error);
      throw new Error('Failed to fetch product category');
    }
    return await response.response.json();
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function getMainCategories() {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/main-categories`);
    if (!response.success) {
      const errorMessage = await response.error;
      console.log('Failed to fetch main categories:', errorMessage);
      throw new Error('Failed to fetch main categories');
    }
    return await response.response.json();
  } catch (error) {
    console.error(error);
    throw error;
  }
}

async function createProductCategory({ name, description, type, parentId }) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      // Backend determines type based on parentId: null = MainCategory, not null = SubCategory
      body: JSON.stringify({
        name,
        description: description || null,
        parentId: type === 2 ? parentId : null,
      }),
    });

    if (!response.success) {
      const errorMessage = await response.error;
      return {
        success: false,
        message: errorMessage || 'Failed to create product category',
      };
    }

    const data = await response.response.json();
    return {
      success: true,
      data,
    };
  } catch (error) {
    console.error(error);
    return {
      success: false,
      message: error.message || 'An error occurred',
    };
  }
}

async function updateProductCategory(
  id,
  { name, description, type, parentId }
) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      // Backend determines type based on parentId: null = MainCategory, not null = SubCategory
      body: JSON.stringify({
        name,
        description: description || null,
        parentId: type === 2 ? parentId : null,
      }),
    });

    if (!response.success) {
      const errorMessage = await response.error;
      return {
        success: false,
        message: errorMessage || 'Failed to update product category',
      };
    }

    const data = await response.response.json();
    return {
      success: true,
      data,
    };
  } catch (error) {
    console.error(error);
    return {
      success: false,
      message: error.message || 'An error occurred',
    };
  }
}

async function deleteProductCategory(id) {
  try {
    const response = await fetchWithAuth(`${BASE_URL}/${id}`, {
      method: 'DELETE',
    });
    if (!response.success) {
      const errorMessage = await response.error;
      return {
        success: false,
        message: errorMessage || 'Failed to delete product category',
      };
    }
    return {
      success: true,
    };
  } catch (error) {
    console.error(error);
    return {
      success: false,
      message: error.message || 'An error occurred',
    };
  }
}

export {
  getProductCategories,
  getAllProductCategories,
  getProductCategoryTree,
  getProductCategoryById,
  getMainCategories,
  createProductCategory,
  updateProductCategory,
  deleteProductCategory,
};
