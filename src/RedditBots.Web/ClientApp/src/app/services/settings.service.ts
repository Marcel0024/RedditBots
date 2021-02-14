import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { BotSetting } from '../models/botsetting';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  public showSettingsMenu: Subject<boolean> = new Subject<boolean>();

  public receiveDesktopNotifications = new Subject<boolean>();
  public showDebugLogsChange = new Subject<boolean>();
  public botSettingChange = new Subject<BotSetting>();
  public botSettingAdded = new Subject<BotSetting>();

  constructor() { }

  toggleSettingsMenu(displaySettings: boolean) {
    this.showSettingsMenu.next(displaySettings);
  }

  setDesktopNotifications(value: boolean) {
    this.receiveDesktopNotifications.next(value);
  }

  addBotSetting(value: BotSetting) {
    this.botSettingAdded.next(value);
  }

  setDebugLogs(value: boolean) {
    this.showDebugLogsChange.next(value);
  }

  setBotSetting(value: BotSetting) {
    this.botSettingChange.next(value);
  }
}
