import { HttpClient } from "@angular/common/http";
import { EventEmitter, Injectable } from "@angular/core";
import { DateTime } from 'luxon';
import { Log } from "../interfaces/log";
import { SignalrService } from "./signalr.service";
import { UserSettingsService } from "./user-settings.service";

@Injectable({
  providedIn: "root",
})
export class LogsService {
  public TotalLogs: number = 0; // consumed by the chart

  allLogs: Log[] = [];

  logsStream$ = new EventEmitter<Log[]>();
  lastLogTime$ = new EventEmitter<string>();
  currentLPS$ = new EventEmitter<number>(); // Logs per second

  constructor(
    private signalR: SignalrService,
    private userSettingsService: UserSettingsService,
    http: HttpClient) {

    this.subscribeToEvents();

    http.get('/api/getlastlogs').subscribe((result: { logs: any[] }) => {
      this.allLogs = result.logs.map(l => this.processIncomingLog(l));
      this.updateScreenData();
    })

    setInterval(() => this.updateLastLogTimeAgo(), 1000);
  }

  setLPS(value: number): void {
    this.currentLPS$.emit(value);
  }

  private subscribeToEvents(): void {
    this.userSettingsService.stateChange$.subscribe(() => {
      this.updateScreenData();
    });
    this.signalR.logReceived$.subscribe((incomingLog: any) => {
      this.TotalLogs++;

      // fix camel case issues from MessagePack
      const fixedLog = {
        notify: incomingLog.Notify,
        logName: incomingLog.LogName,
        message: incomingLog.Message,
        logLevel: incomingLog.LogLevel,
        logDateTime: incomingLog.LogDateTime,
      }

      const log = this.processIncomingLog(fixedLog);

      this.allLogs.unshift(log);

      if (this.shouldDisplayLog(log)) {
        this.updateScreenData();

        if (this.userSettingsService.canDisplayDesktopNotifications() && log.notify) {
          this.notifyDesktop(log);
        }
      }

      if (!this.userSettingsService.hasBotSetting(log.logName)) {
        this.userSettingsService.addBotSetting(log.logName)
      }
    });
  }

  private processIncomingLog(incomingLog: any): Log {
    return {
      notify: incomingLog.notify,
      logName: this.getDisplayName(incomingLog.logName),
      message: incomingLog.message,
      logLevel: incomingLog.logLevel,
      logDateTimeISO: incomingLog.logDateTime,
      logDateTime: DateTime.fromISO(incomingLog.logDateTime).toLocaleString(DateTime.DATETIME_SHORT),
      url: this.getUrl(this.getDisplayName(incomingLog.logName)),
    }
  }

  units: Intl.RelativeTimeFormatUnit[] = [
    'year',
    'month',
    'week',
    'day',
    'hour',
    'minute',
    'second',
  ];

  private updateScreenData(): void {
    this.updateLastLogTimeAgo();
    this.logsStream$.emit(this.allLogs.filter(log => this.shouldDisplayLog(log)));
  }

  private updateLastLogTimeAgo(): void {
    if (this.allLogs.length > 0) {
      this.lastLogTime$.emit(this.getLastLogTimeAgo(this.allLogs[0].logDateTimeISO));
    }
  }

  private getLastLogTimeAgo(date: any): string {
    const dateTime = DateTime.fromISO(date);
    const diff = dateTime.diffNow().shiftTo(...this.units);
    const unit = this.units.find((unit) => diff.get(unit) !== 0);
    const relativeFormatter = new Intl.RelativeTimeFormat('en', {
      numeric: 'auto',
    });

    const time = relativeFormatter.format(Math.trunc(diff.as(unit)), unit);

    return time;
  }

  private shouldDisplayLog(log: Log): boolean {
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
