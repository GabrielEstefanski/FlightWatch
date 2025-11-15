import * as signalR from '@microsoft/signalr';

const HUB_URL = process.env.NEXT_PUBLIC_HUB_URL || 'https://localhost:7185/hubs/flights';

export function createHubConnection(): signalR.HubConnection {
  return new signalR.HubConnectionBuilder()
    .withUrl(HUB_URL)
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();
}

