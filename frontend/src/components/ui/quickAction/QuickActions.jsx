import Action from './Action';
import { useState } from 'react';
import { Package, Users2, ShoppingCart, MapPin } from 'lucide-react';
import { AddProduct } from '@/components/products';

export default function QuickActions({ className = '' }) {
  const [isAddProductOpen, setIsAddProductOpen] = useState(false);

  return (
    <div className={`grid grid-cols-2 gap-2 ${className}`}>
      <Action
        title='Add Product'
        icon={<Package />}
        onClick={() => setIsAddProductOpen(true)}
      />
      <Action title='Add Customer' icon={<Users2 />} theme='purple' />
      <Action title='New Sale Order' icon={<ShoppingCart />} theme='green' />
      <Action title='Add Location' icon={<MapPin />} theme='orange' />

      {isAddProductOpen && (
        <AddProduct
          isOpen={isAddProductOpen}
          onClose={() => setIsAddProductOpen(false)}
        />
      )}
    </div>
  );
}
