import { Component, OnInit, Input } from '@angular/core';
import { BotSetting } from '../../interfaces/botsetting';
import { SettingsService } from '../../core/settings.service';
import { UserSettingsService } from '../../core/user-settings.service';

@Component({
  selector: 'app-settingsbar',
  templateUrl: './settingsbar.component.html',
  styleUrls: ['./settingsbar.component.css']
})
export class SettingsbarComponent implements OnInit {
  toggleSettings: boolean;

  receiveDesktopNotifs: boolean;
  showDebugLogs: boolean;

  botSettings: BotSetting[];

  constructor(
    private _settingsService: SettingsService,
    private _userSettings: UserSettingsService
  ) {
    _settingsService.showSettingsMenu.subscribe((toggle) => {
      this.toggleSettings = toggle;
    });
    _settingsService.botSettingAdded.subscribe((botS) => {
      this.botSettings.push(botS);
    });
  }

  toggleReceiveDesktopNotififcations(): void {
    this.promptAllowNotifications();

    this._settingsService.setDesktopNotifications(this.receiveDesktopNotifs);
  }

  private promptAllowNotifications(): void {
    if (this.receiveDesktopNotifs) {
      if (Notification.permission !== "denied") {
        Notification.requestPermission().then(function (permission) {
          if (permission === "denied") {
            alert("Allow Notifications on this site to activate this function");
            this.receiveDesktopNotifs = false;
          }
        });
      }
      else if (Notification.permission === "denied") {
        alert("Allow Notifications on this site to activate this function");
        this.receiveDesktopNotifs = false;
      }
    }
  }

  toggleDebugLogs(): void {
    this._settingsService.setDebugLogs(this.showDebugLogs);
  }

  updateBotSetting(botSetting: BotSetting): void {
    this._settingsService.setBotSetting(botSetting);
  }

  ngOnInit() {
    this.receiveDesktopNotifs = this._userSettings.currentSettings.recieveDesktopNotifications;
    this.showDebugLogs = this._userSettings.currentSettings.showDebugLogs;
    this.botSettings = this._userSettings.getBotsSettings();
    this.toggleSettings = this._userSettings.currentSettings.showSettingsMenu;
  }
}
