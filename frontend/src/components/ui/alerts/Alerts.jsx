import Alert from './alert';

export default function Alerts() {
  return (
    <div>
      <div className='flex flex-col gap-4'>
        <Alert
          productName='Product 1'
          description='Description for product 1'
          time='2 hours ago'
        />
        <Alert
          productName='Product 2'
          description='Description for product 2'
          time='1 day ago'
        />
      </div>
    </div>
  );
}
