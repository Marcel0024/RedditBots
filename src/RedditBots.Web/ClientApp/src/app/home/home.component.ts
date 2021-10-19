import { Component, OnInit } from "@angular/core";
import { BotSetting } from "../interfaces/botsetting";
import { Log } from "../interfaces/log";
import { SettingsState } from "../interfaces/settings-state";
import { LogsService } from "../services/logs.service";
import { UserSettingsService } from "../services/user-settings.service";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
})
export class HomeComponent implements OnInit {
  allLogs: Log[] = [];
  logsToDisplay: Log[] = [];
  state!: SettingsState;

  constructor(
    private logService: LogsService,
    private userSettingsService: UserSettingsService
  ) {
    this.state = userSettingsService.getOrCreateState();
  }

  ngOnInit(): void {
    this.logService.logStream$.subscribe((log) => {
      if (this.shouldDisplayLog(log)) {
        this.allLogs.unshift(log);
        this.updateLogsToDisplay();
      }
      if (this.state.desktopNotificationIsOn && log.notify) {
        this.notifyDesktop(log);
      }
      if (!this.state.botSettings.some(bs => bs.name === log.logName)) {
        this.userSettingsService.addBotSetting(log.logName)
      }
    });
    this.userSettingsService.stateChange$.subscribe((newState) => {
      this.state = newState;
      this.updateLogsToDisplay();
    });
  }

  updateLogsToDisplay(): void {
    this.logsToDisplay = this.allLogs.filter(log => this.shouldDisplayLog(log));
  }

  shouldDisplayLog(log: Log): boolean {
    let shouldDisplayLog = true;

    if (log.logLevel === "Debug" && !this.state.displayDebugLogs) {
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

  toggleDebugLogs(value: boolean): void {
    this.userSettingsService.toggleDebugLogs(value);
  }

  updateBotSetting(botSetting: BotSetting): void {
    this.userSettingsService.updateBotSetting(botSetting);
  }

  toggleSettingsMenu(value: boolean): void {
    this.userSettingsService.toggleSettingsMenu(value);
  }

  toggleReceiveDesktopNotififcations(value: boolean): void {
    let newValue = value;

    if (value) {
      if (Notification.permission !== "denied") {
        Notification.requestPermission().then(function (permission) {
          if (permission === "denied") {
            newValue = false;
            alert("Allow Notifications on this site to activate this function");
          }
        });
      } else if (Notification.permission === "denied") {
        newValue = false;
        alert("Allow Notifications on this site to activate this function");
      }
    }
    this.userSettingsService.setReceiveDesktopNotification(newValue);
  }

  private notifyDesktop(log: Log): void {
    new Notification(log.logName, {
      body: log.message,
      icon: "/bot.png",
      silent: true,
    });
  }
}
