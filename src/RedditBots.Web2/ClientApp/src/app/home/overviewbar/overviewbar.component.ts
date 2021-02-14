import { Component, NgZone, OnInit } from '@angular/core';
import { connectionStatus } from '../../models/currentinfo';
import { DataService } from '../../services/data.service';
import { SettingsService } from '../../services/settings.service';
import { SignalrService } from '../../services/signalr.service';
import { UserSettingsService } from '../../services/user-settings.service';

@Component({
  selector: 'app-overviewbar',
  templateUrl: './overviewbar.component.html',
  styleUrls: ['./overviewbar.component.css']
})
export class OverviewbarComponent implements OnInit {
  totalViewers: number;
  lps: number = 0;
  lastLog: string;
  connectionStatus: connectionStatus;

  public displaySettings: boolean;

  constructor(
    private _signalR: SignalrService,
    private _ngZone: NgZone,
    private _data: DataService,
    private _settings: SettingsService,
    private _userSettings: UserSettingsService
  ) {
    this.subscribeToEvents();
  }

  toggleSettings(): void {
    this.displaySettings = !this.displaySettings;
    this._settings.toggleSettingsMenu(this.displaySettings);
  }

  private subscribeToEvents() {
    this._signalR.totalViewers.subscribe((totalViewers: number) => {
      this._ngZone.run(() => {
        this.totalViewers = totalViewers;
      });
    });
    this._signalR.lastUpdate.subscribe((lastUpdate: string) => {
      this._ngZone.run(() => {
        this.lastLog = lastUpdate;
      });
    });
    this._data.LPSChange.subscribe((lps: number) => {
        this.lps = lps;
    });
    this._data.connectionStatusChange.subscribe((cs: connectionStatus) => {
      console.log(cs);
      this.connectionStatus = cs;
    });
  }

  ngOnInit() {
    this.displaySettings = this._userSettings.currentSettings.showSettingsMenu;
  }
}
