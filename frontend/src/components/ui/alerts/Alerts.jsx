import { useEffect, useState } from 'react';
import Alert from './alert';
import dashboardService from '@services/dashboardService';

export default function Alerts() {
  const [alerts, setAlerts] = useState([]);

  useEffect(() => {
    const fetchAlerts = async () => {
      try {
        const data = await dashboardService.getInventoryAlerts();
        setAlerts(data);
      } catch (error) {
        console.error('Error fetching alerts:', error);
      }
    };

    fetchAlerts();
  }, []);
  return (
    <div>
      <div className='flex flex-col gap-3'>
        {alerts.slice(0, 5).map((alert, index) => {
          return (
            <Alert
              key={index}
              productName={alert.productName}
              description={alert.description}
              status={alert.status}
            />
          );
        })}
      </div>
    </div>
  );
}
