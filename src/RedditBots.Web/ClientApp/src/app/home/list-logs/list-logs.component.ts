import { Component, NgZone } from '@angular/core';
import { Log } from '../../models/log';
import { DataService } from '../../services/data.service';
import { SettingsService } from '../../services/settings.service';
import { UserSettingsService } from '../../services/user-settings.service';

@Component({
  selector: 'app-list-logs',
  templateUrl: './list-logs.component.html',
  styleUrls: ['./list-logs.component.css']
})
export class ListLogsComponent {
  logs: Log[] = [];

  constructor(
    private _data: DataService,
    private _userSettings: UserSettingsService,
    private _settingsService: SettingsService
  ) {
    this.subscribeToEvents();
  }

  private subscribeToEvents(): void {
    this._settingsService.showDebugLogsChange.subscribe((botSetting) => {
      this._updateLogsDisplayed();
    });
    this._settingsService.botSettingChange.subscribe((botSetting) => {
      this._updateLogsDisplayed();
    });
    this._data.allLogsChange.subscribe((incomingLog: Log) => {
      this._updateLogsDisplayed();
    });
  }
  private _updateLogsDisplayed() {
    this.logs = [];

    //var logsInReverseOrder = this._data.logs.sort(function (a, b): any {
    //  return (a.date.getTime() - b.date.getTime());
    //});

    for (var i = 0; i < this._data.logs.length; i++) {
      if (this._data.logs[i].logLevel === 'Debug'
        && !this._userSettings.currentSettings.showDebugLogs) {
        continue;
      }

      var botSetting = this._userSettings.getBotSetting(this._data.logs[i].logName);
      if (!botSetting.isOn) {
        continue;
      }

      this.logs.push(this._data.logs[i]);
    }
  }
}