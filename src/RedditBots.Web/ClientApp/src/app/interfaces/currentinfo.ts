export interface Info {
  LPS: number;
  connectionStatus: connectionStatus;
}

export enum connectionStatus {
  connected,
  reconnecting,
  disconnected
}
