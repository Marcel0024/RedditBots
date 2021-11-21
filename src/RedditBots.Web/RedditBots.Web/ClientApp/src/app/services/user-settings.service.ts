import { EventEmitter, Injectable } from "@angular/core";
import { BotSetting } from "../interfaces/botsetting";
import { SettingsState } from "../interfaces/settings-state";

@Injectable({
  providedIn: "root",
})
export class UserSettingsService {
  localStorageKey: string = "usersettings4";

  private state!: SettingsState;
  public stateChange$ = new EventEmitter<SettingsState>();

  constructor() { }

  setReceiveDesktopNotification(value: boolean): void {
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
    this.state.desktopNotificationIsOn = newValue;
    this.saveSettingsAndBroadcast();
  }

  addBotSetting(name: string): void {
    this.state.botSettings.push({ isOn: true, name: name });
    this.saveSettingsAndBroadcast();
  }

  updateBotSetting(setting: BotSetting): void {
    this.state.botSettings.forEach((botSetting) => {
      if (botSetting.name === setting.name) {
        botSetting.isOn = setting.isOn;
      }
    });
    this.saveSettingsAndBroadcast();
  }

  toggleSettingsMenu(value: boolean): void {
    this.state.displaySettingsMenu = value;
    this.saveSettingsAndBroadcast();
  }

  toggleDebugLogs(value: boolean): void {
    this.state.displayDebugLogs = value;
    this.saveSettingsAndBroadcast();
  }

  toggleDarkMode(value: boolean): void {
    this.state.isDarkMode = value;
    this.saveSettingsAndBroadcast();
  }

  getBotSetting(name: string): BotSetting {
    return this.state.botSettings.find((botSetting) => botSetting.name === name);
  }

  canDisplayDesktopNotifications(): boolean {
    return this.state.desktopNotificationIsOn;
  }

  canDisplayDebugLogs(): boolean {
    return this.state.displayDebugLogs;
  }

  canDisplaySettingsMenu(): boolean {
    return this.state.displaySettingsMenu;
  }

  hasBotSetting(name: string): boolean {
    return this.state.botSettings.some(bs => bs.name === name);
  }

  getOrCreateState(): SettingsState {
    var settings = localStorage.getItem(this.localStorageKey);

    if (settings === undefined || settings === null) {
      this.createDefaultSettings();
    } else {
      try {
        this.state = JSON.parse(settings);
      } catch {
        this.createDefaultSettings();
      }
    }

    return this.state;
  }

  private createDefaultSettings(): void {
    this.state = {
      desktopNotificationIsOn: false,
      displaySettingsMenu: false,
      displayDebugLogs: false,
      isDarkMode: true,
      botSettings: [],
    };

    this.saveSettingsAndBroadcast();
  }

  private saveSettingsAndBroadcast(): void {
    localStorage.setItem(this.localStorageKey, JSON.stringify(this.state));

    this.stateChange$.emit(this.state);
  }
}
