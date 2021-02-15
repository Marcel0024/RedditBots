import { Injectable, NgZone } from '@angular/core';
import { BotSetting } from '../models/botsetting';
import { DataService } from './data.service';
import { SettingsService } from './settings.service';

@Injectable({
  providedIn: 'root'
})
export class UserSettingsService {
  localStorageKey: string = 'usersettings3'

  public currentSettings: ISettings;

  constructor(
    private _settings: SettingsService,
    private _data: DataService,
    private _ngZone: NgZone
  ) {
    this.loadOrCreateLocalStorageSettings();

    this.subscribeValues();
  }

  getBotSetting(name: string): BotSetting {
    return this.currentSettings.botsettings.find(botS => botS.name === name);
  }

  private subscribeValues() {
    this._settings.receiveDesktopNotifications.subscribe((value) => {
      this.currentSettings.recieveDesktopNotifications = value;
      this.saveSettings();
    })
    this._settings.showDebugLogsChange.subscribe((value) => {
      this.currentSettings.showDebugLogs = value;
      this.saveSettings();
    })
    this._settings.showSettingsMenu.subscribe((toggle) => {
      this.currentSettings.showSettingsMenu = toggle;
      this.saveSettings();
    });
    this._settings.botSettingChange.subscribe((value) => {
      var botSettingToUpdate = this.currentSettings.botsettings.find(botS => botS.name === value.name);
      var index = this.currentSettings.botsettings.indexOf(botSettingToUpdate);
      this.currentSettings.botsettings[index].isOn = value.isOn;

      this.saveSettings();
    })
    this._data.allLogsChange.subscribe((log) => {
      const botSetting = this.currentSettings.botsettings.find(botS => botS.name === log.logName);

      if (!botSetting) {
        var newBotSetting = {
          name: log.logName,
          isOn: true
        };

        this.currentSettings.botsettings.push(newBotSetting);
        this._settings.addBotSetting(newBotSetting);
      }

      this.saveSettings();
    })
  }

  private loadOrCreateLocalStorageSettings(): void {
    var settings = localStorage.getItem(this.localStorageKey);

    if (settings === undefined || settings === null) {
      this.createDefaultSettings();
    } else {
      try {
        this.currentSettings = JSON.parse(settings);
      } catch {
        this.createDefaultSettings();
      }
    }
  }

  private createDefaultSettings() {
    this.currentSettings = {
      showSettingsMenu: false,
      recieveDesktopNotifications: false,
      showDebugLogs: true,
      botsettings: []
    };

    this.saveSettings();
  }

  private saveSettings(): void {
    this.addMissingBotsToSettings();

    localStorage.setItem(this.localStorageKey, JSON.stringify(this.currentSettings));
  }

  private addMissingBotsToSettings() {
    var bots = this._data.getActiveBotNames();
    var botSettings = this.currentSettings.botsettings.map(bot => bot.name);
    var missingBots = bots.filter((n, i) => botSettings.indexOf(n) === -1);

    for (var i = 0; i < missingBots.length; i++) {
      this.currentSettings.botsettings.push({
        name: missingBots[i],
        isOn: true
      });
    }
  }

  getBotsSettings(): import("../models/botsetting").BotSetting[] {
    return this.currentSettings.botsettings.map(bot => ({
      name: bot.name,
      isOn: bot.isOn
    }));
  }
}

interface ISettings {
  showSettingsMenu: boolean;
  recieveDesktopNotifications: boolean;
  showDebugLogs: boolean;
  botsettings: BotSetting[];
}
