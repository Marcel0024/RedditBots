import { Component, NgZone } from '@angular/core';
import { Log } from '../../interfaces/log';
import { DataService } from '../../core/data.service';
import { SettingsService } from '../../core/settings.service';
import { UserSettingsService } from '../../core/user-settings.service';

@Component({
  selector: 'app-list-logs',
  templateUrl: './list-logs.component.html',
  styleUrls: ['./list-logs.component.css'],
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

    var logsInReverseOrder = this._data.logs.sort(function (a, b): any {
      return (b.date.getTime() - a.date.getTime());
    });

    for (var i = 0; i < logsInReverseOrder.length; i++) {
      if (logsInReverseOrder[i].logLevel === 'Debug'
        && !this._userSettings.currentSettings.showDebugLogs) {
        continue;
      }

      var botSetting = this._userSettings.getBotSetting(logsInReverseOrder[i].logName);
      if (!botSetting.isOn) {
        continue;
      }

      this.logs.push(logsInReverseOrder[i]);
    }
  }
}
