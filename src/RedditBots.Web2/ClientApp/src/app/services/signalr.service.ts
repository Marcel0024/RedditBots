import { HttpClient } from '@angular/common/http';
import { EventEmitter, Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { MessagePackHubProtocol } from '@microsoft/signalr-protocol-msgpack'
import { Log } from '../models/log';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  public logReceived = new EventEmitter<Log>();
  public lastUpdate = new EventEmitter<string>();
  public totalViewers = new EventEmitter<number>();

  private hubConnection: HubConnection

  constructor() { }

  public connect = () => {
    this.startConnection();
    this.addListeners();
  }

  private getConnection(): HubConnection {
    return new HubConnectionBuilder()
      .withUrl('/loghub')
      .withHubProtocol(new MessagePackHubProtocol())
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();
  }
  private startConnection() {
    this.hubConnection = this.getConnection();

    this.hubConnection.start()
      .then(() => console.log('Connected'))
      .catch((err) => console.log('error while establishing signalr connection: ' + err))
  }

  private addListeners() {
    this.hubConnection.on("UpdateLastDateTime", (data: string) => {
      this.lastUpdate.emit(data);
    })
    this.hubConnection.on("UpdateViewers", (data: number) => {
      this.totalViewers.emit(data);
    })
    this.hubConnection.on("Log", (data: Log) => {
      this.logReceived.emit(data);
    })
  }
}
