import { EventEmitter, Injectable } from "@angular/core";
import {
    HubConnection,
    HubConnectionBuilder,
    LogLevel
} from "@microsoft/signalr";
import { MessagePackHubProtocol } from "@microsoft/signalr-protocol-msgpack";
import { ConnectionStatus } from "../interfaces/connection-status";
import { Log } from "../interfaces/log";

@Injectable({
  providedIn: "root",
})
export class SignalrService {
  public logReceived$ = new EventEmitter<Log>();
  public totalViewers$ = new EventEmitter<number>();
  public connectionStatusChange$ = new EventEmitter<ConnectionStatus>();

  private hubConnection: HubConnection;

  constructor() { }

  public connect = () => {
    this.startConnection();
    this.addListeners();
  };

  private getConnection(): HubConnection {
    return new HubConnectionBuilder()
      .withUrl("/loghub")
      .withHubProtocol(new MessagePackHubProtocol())
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();
  }
  private startConnection() {
    this.hubConnection = this.getConnection();

    this.hubConnection
      .start()
      .then(() => this.connectionStatusChange$.emit(ConnectionStatus.connected))
      .catch(() =>
        this.connectionStatusChange$.emit(ConnectionStatus.disconnected)
      );

    this.hubConnection.onreconnecting(() => {
      this.connectionStatusChange$.emit(ConnectionStatus.reconnecting);
    });

    this.hubConnection.onreconnected(() => {
      this.connectionStatusChange$.emit(ConnectionStatus.connected);
    });

    this.hubConnection.onclose(() => {
      this.connectionStatusChange$.emit(ConnectionStatus.disconnected);

      setTimeout(() => {
        location.reload();
      }, 5000);
    });
  }

  private addListeners() {
    this.hubConnection.on("UpdateViewers", (data: number) => {
      console.log(`Total viewers: ${data}`);
      this.totalViewers$.emit(data);
    });
    this.hubConnection.on("Log", (data: Log) => {
      this.logReceived$.emit(data);
    });
  }
}
