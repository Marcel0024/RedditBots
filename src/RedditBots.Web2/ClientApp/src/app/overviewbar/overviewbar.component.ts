import { Component, NgZone, OnInit } from '@angular/core';
import { DataService, Info } from '../services/data.service';
import { SettingsService } from '../services/settings.service';
import { SignalrService } from '../services/signalr.service';

@Component({
  selector: 'app-overviewbar',
  templateUrl: './overviewbar.component.html',
  styleUrls: ['./overviewbar.component.css']
})
export class OverviewbarComponent implements OnInit {
  totalViewers: number;
  lps: number = 0;
  lastLog: string;

  public displaySettings: boolean;

  constructor(
    private _signalR: SignalrService,
    private _ngZone: NgZone,
    private _data: DataService,
    private _settings: SettingsService
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
    this._data.LPSChange.subscribe((info: Info) => {
        this.lps = info.LPS;
    });
  }

  ngOnInit() {
  }
}
