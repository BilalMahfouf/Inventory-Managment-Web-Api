import { useState, useEffect } from 'react';
import { getSignalRConnectionAndStart } from './signalRService';

export default function useSignalR(methodName) {
  const [messages, setMessages] = useState([]);

  useEffect(() => {
    const start = async () => {
      try {
        const connection = await getSignalRConnectionAndStart();
        console.log('SignalR Connected.');
        connection.on(methodName, data => {
          setMessages(prevMessages => [...prevMessages, data]);
          console.log('Received data:', data);
        });
      } catch (err) {
        console.log('SignalR Connection Error: ', err);
        setTimeout(start, 5000);
      }
    };
    start();
  }, [methodName]);
  return {
    messages,
    clear: () => setMessages([]),
    remove: id => setMessages(prev => prev.filter(msg => msg.id !== id)),
  };
}
