import Action from './Action';
import { useState } from 'react';
import { Package, Users2, ShoppingCart, MapPin } from 'lucide-react';
import { useTranslation } from 'react-i18next';

import i18nKeyContainer from '@shared/lib/i18n/keyContainer';
import { AddProduct } from '@features/products/components/products';

export default function QuickActions({ className = '' }) {
  const { t } = useTranslation();
  const [isAddProductOpen, setIsAddProductOpen] = useState(false);

  return (
    <div className={`grid grid-cols-2 gap-2 ${className}`}>
      <Action
        title={t(i18nKeyContainer.dashboard.quickActions.addProduct)}
        icon={<Package />}
        onClick={() => setIsAddProductOpen(true)}
      />
      <Action
        title={t(i18nKeyContainer.dashboard.quickActions.addCustomer)}
        icon={<Users2 />}
        theme='purple'
      />
      <Action
        title={t(i18nKeyContainer.dashboard.quickActions.newSaleOrder)}
        icon={<ShoppingCart />}
        theme='green'
      />
      <Action
        title={t(i18nKeyContainer.dashboard.quickActions.addLocation)}
        icon={<MapPin />}
        theme='orange'
      />

      {isAddProductOpen && (
        <AddProduct
          isOpen={isAddProductOpen}
          onClose={() => setIsAddProductOpen(false)}
        />
      )}
    </div>
  );
}
