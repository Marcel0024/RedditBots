import { Injectable, NgZone } from '@angular/core';
import { Subject } from 'rxjs';
import { connectionStatus, Info } from '../interfaces/currentinfo';
import { Log } from '../interfaces/log';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  info: Info = {
    LPS: 0,
    connectionStatus: connectionStatus.disconnected
  };

  public logs: Log[] = [];

  public allLogsChange = new Subject<Log>();
  public LPSChange = new Subject<number>();
  public connectionStatusChange = new Subject<connectionStatus>();

  constructor(
    private _signalR: SignalrService,
    private _ngZone: NgZone
  ) {
    this.subscribeToEvents();
  }

  getActiveBotNames(): string[] {
    const allNames = this.logs.map((log) => log.logName)

    return allNames.filter((n, i) => allNames.indexOf(n) === i);
  }

  setLPS(lps: number) {
    this.LPSChange.next(lps);
  }

  setConnectionStatus(connectionStatus: connectionStatus) {
    this.connectionStatusChange.next(connectionStatus);
  }

  pushNewLog(log: Log) {
    this.logs.push(log);

    if (this.logs.length >= 2000) {
      for (var i = 0; i < this.logs.length - 2000; i++) {
        this.logs.pop();
      }
    }

    this.allLogsChange.next(log);
  }

  private subscribeToEvents(): void {
    this._signalR.logReceived.subscribe((incomingLog: any) => {
      const log = {
        notify: incomingLog.Notify,
        logName: this.getDisplayName(incomingLog.LogName),
        message: incomingLog.Message,
        logLevel: incomingLog.LogLevel,
        date: new Date(),
        url: this.getUrl(this.getDisplayName(incomingLog.LogName))
      }

      this._ngZone.run(() => {
        this.pushNewLog(log);
      });
    });

    this._signalR.connectionStatusChange.subscribe((cs: connectionStatus) => {
      this.connectionStatusChange.next(cs);
    });

  }

  private getDisplayName(logName: string): string {
    if (logName.indexOf('.') === -1) {
      return logName;
    }

    var namearray = logName.split('.');;
    var name = namearray[namearray.length - 1];

    if (name === 'PapiamentoRedditBot') {
      name = 'PapiamentoBot';
    }

    if (name.startsWith("Azure") || name.includes("Discord")) {
      return name;
    }

    return `/u/${name}`;
  }

  private getUrl(name: string): string {
    if (name.startsWith('/u/')) {
      return `https://www.reddit.com${name}`;
    }
    else {
      return null;
    }
  }
}
