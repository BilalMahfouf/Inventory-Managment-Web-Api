import Button from '@/components/Buttons/Button';
import CustomerDataTable from '@/components/customers/CustomerDataTable';
import InfoCard from '@/components/ui/InfoCard';
import PageHeader from '@/components/ui/PageHeader';
import { getCustomerSummary } from '@/services/customers/customerService';
import { divStyles } from '@/util/uiVariables';
import { DollarSign, Plus, User2 } from 'lucide-react';
import { useState, useEffect } from 'react';
import { Tab, TabList, TabPanel, Tabs } from 'react-tabs';

export default function CustomersPage() {
  const [loading, setLoading] = useState(false);
  const [totalCustomers, setTotalCustomers] = useState(0);
  const [totalRevenue, setTotalRevenue] = useState(0);
  const [newCustomersLastMonth, setNewCustomersLastMonth] = useState(0);
  const [activeCustomers, setActiveCustomers] = useState(0);
  const [isAddModalOpen, setIsAddModalOpen] = useState(false);

  useEffect(() => {
    const fetchCustomerSummary = async () => {
      setLoading(true);
      const response = await getCustomerSummary();
      if (response.success) {
        setTotalCustomers(response.data.totalCustomers);
        setTotalRevenue(response.data.totalRevenue);
        setNewCustomersLastMonth(response.data.newCustomersLastMonth);
        setActiveCustomers(response.data.activeCustomers);
        setLoading(false);
        return;
      }
    };
    fetchCustomerSummary();
  }, []);

  return (
    <>
      <div className='flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4'>
        <PageHeader
          title='Inventory Management'
          description='Manage your product catalog and inventory.'
        />

        <Button LeftIcon={Plus} onClick={() => setIsAddModalOpen(true)}>
          Add Inventory
        </Button>
      </div>
      <div className='flex flex-col md:flex-row gap-6'>
        <InfoCard
          title='Total Customers'
          iconComponent={User2}
          number={loading ? '...' : totalCustomers.toLocaleString()}
          description={`${activeCustomers.toLocaleString()} customers are active.`}
          className='flex-1'
        />
        <InfoCard
          title='Total Revenue'
          iconComponent={DollarSign}
          number={loading ? '...' : `$${totalRevenue.toLocaleString()}`}
          description='Total revenue generated from all customers.'
          className='flex-1'
        />
        <InfoCard
          title='New Customers'
          iconComponent={User2}
          number={loading ? '...' : newCustomersLastMonth}
          description='New customers acquired in the last month.'
          className='flex-1'
        />
        <InfoCard
          title='Out of Stock Items'
          iconComponent={User2}
          number={loading ? '...' : 'nothing yet'}
          description='no thing yet.'
          className='flex-1'
        />
      </div>
      <div>
        <Tabs>
          <TabList className='flex bg-blue-50 rounded-lg p-1 mb-4 mt-6 gap-1 max-w-fit border-0'>
            <Tab
              className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
              selectedClassName='!bg-white !text-gray-900 shadow-sm'
            >
              Customers
            </Tab>
            <Tab
              className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
              selectedClassName='!bg-white !text-gray-900 shadow-sm'
            >
              Customer Categories
            </Tab>
            <Tab
              className='px-4 py-2 rounded-md cursor-pointer text-gray-600 transition-colors hover:text-gray-800'
              selectedClassName='!bg-white !text-gray-900 shadow-sm'
            >
              Customer Contacts
            </Tab>
          </TabList>
          <div className={divStyles + 'mt-6'}>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  Customer Catalog
                </h3>
              </div>
              <CustomerDataTable />
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  Customer Categories
                </h3>
              </div>
              <div>Customer Categories will be here !</div>
            </TabPanel>
            <TabPanel>
              <div className='mb-9 flex items-center justify-between'>
                <h3 className='text-2xl font-semibold leading-none tracking-tight'>
                  Customer Contacts
                </h3>
              </div>
              <div>Customer Contacts will be here !</div>
            </TabPanel>
          </div>
        </Tabs>
      </div>
    </>
  );
}
