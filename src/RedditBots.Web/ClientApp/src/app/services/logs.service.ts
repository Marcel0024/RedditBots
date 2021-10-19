import { EventEmitter, Injectable, NgZone } from "@angular/core";
import { Log } from "../interfaces/log";
import { SignalrService } from "./signalr.service";
import { UserSettingsService } from "./user-settings.service";

@Injectable({
  providedIn: "root",
})
export class LogsService {
  allLogs: Log[] = [];

  logsStream$ = new EventEmitter<Log[]>();
  currentLPS$ = new EventEmitter<number>(); // Logs per second

  constructor(private signalR: SignalrService, private userSettingsService: UserSettingsService, private ngZone: NgZone) {
    this.subscribeToEvents();

  }

  setLPS(value: number): void {
    this.currentLPS$.emit(value);
  }

  private subscribeToEvents(): void {
    this.userSettingsService.stateChange$.subscribe((newState) => {
      this.updateLogsToDisplay();
    });
    this.signalR.logReceived$.subscribe((incomingLog: any) => {
      const log = {
        notify: incomingLog.Notify,
        logName: this.getDisplayName(incomingLog.LogName),
        message: incomingLog.Message,
        logLevel: incomingLog.LogLevel,
        url: this.getUrl(this.getDisplayName(incomingLog.LogName)),
      };

      this.ngZone.run(() => {
        if (this.shouldDisplayLog(log)) {
          this.allLogs.unshift(log);
          this.updateLogsToDisplay();
        }
        if (this.userSettingsService.canDisplayDesktopNotifications() && log.notify) {
          this.notifyDesktop(log);
        }
        if (!this.userSettingsService.hasBotSetting(log.logName)) {
          this.userSettingsService.addBotSetting(log.logName)
        }
      });
    });
  }

  updateLogsToDisplay(): void {
    this.logsStream$.emit(this.allLogs.filter(log => this.shouldDisplayLog(log)));
  }

  shouldDisplayLog(log: Log): boolean {
    let shouldDisplayLog = true;

    if (log.logLevel === "Debug" && !this.userSettingsService.canDisplayDebugLogs()) {
      shouldDisplayLog = false;
    }

    var botSetting = this.userSettingsService.getBotSetting(log.logName);

    if (!botSetting) {
      return true;
    }

    if (!botSetting.isOn) {
      shouldDisplayLog = false;
    }

    return shouldDisplayLog;
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


  private notifyDesktop(log: Log): void {
    new Notification(log.logName, {
      body: log.message,
      icon: "/bot.png",
      silent: true,
    });
  }
}
