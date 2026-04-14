import { useQuery } from '@tanstack/react-query';
import Alert from './Alert';
import dashboardApi from '@features/dashboard/services/dashboardApi';
import { queryKeys } from '@shared/lib/queryKeys';

export default function Alerts() {
  const { data: alerts = [] } = useQuery({
    queryKey: queryKeys.dashboard.alerts(),
    queryFn: dashboardApi.getInventoryAlerts,
  });

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
