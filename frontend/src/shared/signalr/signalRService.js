import {
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr';

const API_URL = 'https://localhost:7230';

let connection = null;

async function getSignalRConnectionAndStart() {
  if (connection !== null) {
    console.log('Reusing existing SignalR connection.', connection);
    return connection;
  }
  connection = new HubConnectionBuilder()
    .withUrl(`${API_URL}/hubs/notification`)
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build();
  if (connection.state === HubConnectionState.Disconnected) {
    connection.stop();
  }

  await connection.start();

  console.log('SignalR connection created.', connection);

  return connection;
}

export { getSignalRConnectionAndStart };
