import { useState, useEffect } from 'react';
import { getSignalRConnectionAndStart } from './signalRService';

export default function useSignalR(methodName) {
  const [messages, setMessages] = useState(new Map());

  useEffect(() => {
    const start = async () => {
      try {
        const connection = await getSignalRConnectionAndStart();
        console.log('SignalR Connected.');

        connection.on(methodName, data => {
          setMessages(prevMessages => new Map(prevMessages).set(data.id, data));
          console.log('Received data:', messages);
        });
      } catch (err) {
        console.log('SignalR Connection Error: ', err);
        setTimeout(start, 5000);
      }
    };
    start();
  }, [methodName, messages]);
  return {
    messages: [...messages.values()],
    clear: () => setMessages(new Map()),
    remove: id =>
      setMessages(prev => {
        const newMessages = new Map(prev);
        newMessages.delete(id);
        return newMessages;
      }),
  };
}
