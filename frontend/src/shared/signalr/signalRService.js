import {
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr';

const DEFAULT_BACKEND_ORIGIN = 'https://localhost:7443';

function normalizeBackendOrigin(origin) {
  if (typeof origin !== 'string') {
    return DEFAULT_BACKEND_ORIGIN;
  }

  const trimmed = origin.trim();
  if (!trimmed) {
    return DEFAULT_BACKEND_ORIGIN;
  }

  return trimmed.replace(/\/+$/, '');
}

const BACKEND_ORIGIN = normalizeBackendOrigin(
  import.meta.env.VITE_API_ORIGIN
);

let connection = null;

async function getSignalRConnectionAndStart() {
  if (connection !== null) {
    console.log('Reusing existing SignalR connection.', connection);
    return connection;
  }
  connection = new HubConnectionBuilder()
    .withUrl(`${BACKEND_ORIGIN}/hubs/notification`, { withCredentials: true })
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
