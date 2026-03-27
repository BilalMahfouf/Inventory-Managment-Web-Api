import React, { useState, useEffect } from 'react';
import { X, FolderTree } from 'lucide-react';
import Button from '@components/Buttons/Button';
import { Input } from '@components/ui/input';
import {
  createProductCategory,
  getProductCategoryById,
  updateProductCategory,
  getMainCategories,
} from '@/services/products/productCategoryService';
import { useToast } from '@/context/ToastContext';

/**
 * AddProductCategory Component
 *
 * A modal component for adding or editing product categories in the inventory system.
 * Supports both MainCategory and SubCategory types.
 * When SubCategory is selected, displays a select dropdown to choose parent category.
 *
 * @param {Object} props - Component props
 * @param {boolean} props.isOpen - Controls modal visibility
 * @param {function} props.onClose - Callback function when modal is closed
 * @param {number} props.categoryId - ID of the category to edit (0 for new category)
 * @param {function} props.onSuccess - Optional callback after successful save
 */
const AddProductCategory = ({ isOpen, onClose, categoryId = 0, onSuccess }) => {
  const { showSuccess, showError } = useToast();
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    type: 1, // 1 for MainCategory, 2 for SubCategory
    parentId: null,
  });
  const [mode, setMode] = useState('add'); // 'add' or 'update'
  const [id, setId] = useState(categoryId);
  const [isLoading, setIsLoading] = useState(false);
  const [errors, setErrors] = useState({});
  const [mainCategories, setMainCategories] = useState([]);

  const handleInputChange = (field, value) => {
    setFormData(prev => ({
      ...prev,
      [field]: value,
    }));
    // Clear error for this field when user starts typing
    if (errors[field]) {
      setErrors(prev => ({
        ...prev,
        [field]: '',
      }));
    }
  };

  const handleTypeChange = type => {
    setFormData(prev => ({
      ...prev,
      type: type,
      parentId: type === 1 ? null : prev.parentId,
    }));
    if (errors.type) {
      setErrors(prev => ({
        ...prev,
        type: '',
      }));
    }
  };

  const validateForm = () => {
    const newErrors = {};
    if (!formData.name.trim()) {
      newErrors.name = 'Category name is required';
    }
    if (formData.type === 2 && !formData.parentId) {
      newErrors.parentId = 'Parent category is required for subcategories';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const addNewCategory = async ({ name, description, type, parentId }) => {
    setIsLoading(true);
    const response = await createProductCategory({
      name,
      description: description || null,
      type,
      parentId: type === 2 ? parentId : null,
    });
    if (response.success) {
      showSuccess(
        'Product Category Created',
        `${name} has been added successfully.`
      );
      setId(response.data.id);
      if (onSuccess) {
        onSuccess();
      }
      onClose();
    } else {
      showError(
        'Category Creation Failed',
        `${name} could not be created. Error: ${response.message}`
      );
    }
    setIsLoading(false);
  };

  const editCategory = async (id, { name, description, type, parentId }) => {
    setIsLoading(true);
    const response = await updateProductCategory(id, {
      name,
      description: description || null,
      type,
      parentId: parentId,
    });

    if (response.success) {
      showSuccess(
        'Product Category Updated',
        `${name} has been updated successfully.`
      );
      setFormData({
        name: response.data.name,
        description: response.data.description || '',
        type: response.data.type,
        parentId: response.data.parentId || null,
      });
      if (onSuccess) {
        onSuccess();
      }
      onClose();
    } else {
      showError(
        'Category Update Failed',
        `Category could not be updated. Error: ${response.message}`
      );
    }
    setIsLoading(false);
  };

  const saveCategory = () => {
    if (!validateForm()) {
      return;
    }

    if (mode === 'add') {
      addNewCategory({
        name: formData.name,
        description: formData.description,
        type: formData.type,
        parentId: formData.parentId,
      });
      setMode('update');
    } else if (mode === 'update') {
      editCategory(id, {
        name: formData.name,
        description: formData.description,
        type: formData.type,
        parentId: formData.parentId,
      });
    }
  };

  const handleSubmit = e => {
    e.preventDefault();
    saveCategory();
  };

  const handleCancel = () => {
    setFormData({
      name: '',
      description: '',
      type: 1,
      parentId: null,
    });
    setErrors({});
    setMode('add');
    setId(0);
    if (onClose) {
      onClose();
    }
  };

  // Fetch main categories when component mounts or when type changes to SubCategory
  useEffect(() => {
    const fetchMainCategories = async () => {
      try {
        const data = await getMainCategories();
        if (data) {
          setMainCategories(data);
        }
      } catch (error) {
        console.error('Error fetching main categories:', error);
      }
    };

    if (isOpen) {
      fetchMainCategories();
    }
  }, [isOpen]);

  useEffect(() => {
    if (isOpen && id > 0) {
      const fetchCategory = async id => {
        try {
          setIsLoading(true);
          const data = await getProductCategoryById(id);
          if (data) {
            setFormData({
              name: data.name,
              description: data.description || '',
              type: data.type === 'MainCategory' ? 1 : 2,
              parentId: data.parentId || null,
            });
            setMode('update');
          }
        } catch (error) {
          console.error('Error fetching category:', error);
          showError(
            'Failed to Load Category',
            'Could not load product category data.'
          );
        } finally {
          setIsLoading(false);
        }
      };
      fetchCategory(id);
    } else if (isOpen && id === 0) {
      setMode('add');
      setFormData({
        name: '',
        description: '',
        type: 1,
        parentId: null,
      });
    }
  }, [id, isOpen, showError]);

  useEffect(() => {
    setId(categoryId);
  }, [categoryId]);

  if (!isOpen) return null;

  return (
    <div className='fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4'>
      <div className='bg-white rounded-lg w-full max-w-2xl max-h-[90vh] overflow-hidden flex flex-col'>
        {/* Header */}
        <div className='flex items-center justify-between p-6 border-b flex-shrink-0'>
          <div className='flex items-center gap-3'>
            <FolderTree className='h-6 w-6 text-gray-600' />
            <h2 className='text-xl font-semibold'>
              {mode === 'add'
                ? 'Add Product Category'
                : 'Edit Product Category'}
            </h2>
          </div>
          <button
            onClick={handleCancel}
            className='p-2 hover:bg-gray-100 rounded-lg transition-colors'
            disabled={isLoading}
          >
            <X className='h-5 w-5' />
          </button>
        </div>

        {/* Content */}
        <form
          onSubmit={handleSubmit}
          className='flex flex-col flex-1 overflow-hidden'
        >
          <div className='p-6 overflow-y-auto flex-1'>
            <div className='flex items-center gap-2 mb-6'>
              <FolderTree className='h-5 w-5' />
              <h3 className='text-lg font-semibold'>Category Information</h3>
            </div>

            <div className='space-y-6'>
              <div>
                <label className='block text-sm font-medium mb-2'>
                  Category Name <span className='text-red-500'>*</span>
                </label>
                <Input
                  placeholder='e.g., Electronics, Clothing, Food & Beverages'
                  value={formData.name}
                  onChange={e => handleInputChange('name', e.target.value)}
                  className={`h-12 ${errors.name ? 'border-red-500' : ''}`}
                  disabled={isLoading}
                  autoFocus
                />
                {errors.name && (
                  <p className='text-red-500 text-sm mt-1'>{errors.name}</p>
                )}
              </div>

              <div>
                <label className='block text-sm font-medium mb-2'>
                  Description
                </label>
                <textarea
                  placeholder='Optional description for this category...'
                  value={formData.description}
                  onChange={e =>
                    handleInputChange('description', e.target.value)
                  }
                  className='w-full h-32 px-3 py-2 border border-gray-300 rounded-md resize-none focus:outline-none focus:ring-1 focus:ring-blue-600'
                  disabled={isLoading}
                />
              </div>

              <div>
                <label className='block text-sm font-medium mb-3'>
                  Category Type <span className='text-red-500'>*</span>
                </label>
                <div className='space-y-3'>
                  {/* Main Category Radio */}
                  <label className='flex items-center p-4 border-2 rounded-lg cursor-pointer hover:bg-gray-50 transition-colors'>
                    <input
                      type='radio'
                      name='categoryType'
                      value={1}
                      checked={formData.type === 1}
                      onChange={() => handleTypeChange(1)}
                      disabled={isLoading}
                      className='w-4 h-4 text-blue-600 cursor-pointer'
                    />
                    <div className='ml-3'>
                      <span className='text-sm font-medium text-gray-900'>
                        Main Category
                      </span>
                      <p className='text-xs text-gray-500 mt-0.5'>
                        A top-level category that can contain subcategories
                      </p>
                    </div>
                  </label>

                  {/* Subcategory Radio */}
                  <label className='flex items-center p-4 border-2 rounded-lg cursor-pointer hover:bg-gray-50 transition-colors'>
                    <input
                      type='radio'
                      name='categoryType'
                      value={2}
                      checked={formData.type === 2}
                      onChange={() => handleTypeChange(2)}
                      disabled={isLoading}
                      className='w-4 h-4 text-blue-600 cursor-pointer'
                    />
                    <div className='ml-3'>
                      <span className='text-sm font-medium text-gray-900'>
                        Subcategory
                      </span>
                      <p className='text-xs text-gray-500 mt-0.5'>
                        A category that belongs to a main category
                      </p>
                    </div>
                  </label>
                </div>
              </div>

              {/* Parent Category Select - Only shown for SubCategory */}
              {formData.type === 2 && (
                <div>
                  <label className='block text-sm font-medium mb-2'>
                    Parent Category <span className='text-red-500'>*</span>
                  </label>
                  <select
                    value={formData.parentId || ''}
                    onChange={e =>
                      handleInputChange(
                        'parentId',
                        e.target.value ? parseInt(e.target.value) : null
                      )
                    }
                    className={`w-full h-12 px-3 border rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-600 ${
                      errors.parentId ? 'border-red-500' : 'border-gray-300'
                    }`}
                    disabled={isLoading}
                  >
                    <option value=''>Select a parent category</option>
                    {mainCategories.map(category => (
                      <option key={category.id} value={category.id}>
                        {category.name}
                      </option>
                    ))}
                  </select>
                  {errors.parentId && (
                    <p className='text-red-500 text-sm mt-1'>
                      {errors.parentId}
                    </p>
                  )}
                </div>
              )}
            </div>
          </div>

          {/* Footer */}
          <div className='flex items-center justify-end gap-3 p-6 border-t bg-gray-50 flex-shrink-0'>
            <Button
              type='button'
              variant='secondary'
              onClick={handleCancel}
              disabled={isLoading}
              className='cursor-pointer'
            >
              Cancel
            </Button>
            <Button
              type='submit'
              disabled={!formData.name.trim() || isLoading}
              className='cursor-pointer'
            >
              {isLoading
                ? mode === 'add'
                  ? 'Creating...'
                  : 'Saving...'
                : mode === 'add'
                  ? 'Create Category'
                  : 'Save Changes'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddProductCategory;
