import { Component, OnInit } from '@angular/core';
import { SettingsService } from '../services/settings.service';

@Component({
  selector: 'app-settingsbar',
  templateUrl: './settingsbar.component.html',
  styleUrls: ['./settingsbar.component.css']
})
export class SettingsbarComponent implements OnInit {
  showSettings: boolean;

  constructor(private _settings: SettingsService) {
    _settings.showSettingsMenu.subscribe((toggle) => {
      this.showSettings = toggle;
    });
  }

  ngOnInit() {
  }
}
