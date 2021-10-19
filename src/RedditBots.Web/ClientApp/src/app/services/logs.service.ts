import { EventEmitter, Injectable, NgZone } from "@angular/core";
import { Log } from "../interfaces/log";
import { SignalrService } from "./signalr.service";

@Injectable({
  providedIn: "root",
})
export class LogsService {
  public logStream$ = new EventEmitter<Log>();
  public currentLPS$ = new EventEmitter<number>(); // Logs per second

  constructor(private signalR: SignalrService, private ngZone: NgZone) {
    this.subscribeToEvents();
  }

  setLPS(value: number): void {
    this.currentLPS$.emit(value);
  }

  private subscribeToEvents(): void {
    this.signalR.logReceived$.subscribe((incomingLog: any) => {
      const log = {
        notify: incomingLog.Notify,
        logName: this.getDisplayName(incomingLog.LogName),
        message: incomingLog.Message,
        logLevel: incomingLog.LogLevel,
        url: this.getUrl(this.getDisplayName(incomingLog.LogName)),
      };

      this.ngZone.run(() => {
        this.logStream$.next(log);
      });
    });
  }

  private getDisplayName(logName: string): string {
    if (logName.indexOf(".") === -1) {
      return logName;
    }

    var namearray = logName.split(".");
    var name = namearray[namearray.length - 1];

    if (name === "PapiamentoRedditBot") {
      name = "PapiamentoBot";
    }

    if (name.startsWith("Azure") || name.includes("Discord")) {
      return name;
    }

    return `/u/${name}`;
  }

  private getUrl(name: string): string {
    if (name.startsWith("/u/")) {
      return `https://www.reddit.com${name}`;
    } else {
      return null;
    }
  }
}
