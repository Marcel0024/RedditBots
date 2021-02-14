import { Component, Input, OnInit } from '@angular/core';
import { Log } from '../../../models/log';
import { UserSettingsService } from '../../../services/user-settings.service';

@Component({
  selector: 'app-log',
  templateUrl: './log.component.html',
  styleUrls: ['./log.component.css']
})
export class LogComponent implements OnInit {
  @Input() log: Log;

  showNEWStyling: boolean = false;

  constructor(private _userSettings: UserSettingsService) { }

  ngOnInit() {
    if (this.log.notify) {
      this.showNEWStyling = true;

      if (this._userSettings.currentSettings.recieveDesktopNotifications) {
        this.notifyDesktop();
      }
    }

    this.log.notify = false;

    setTimeout(() => {
      this.showNEWStyling = false;
    }, 1500);
  }

  private notifyDesktop(): void {
    new Notification(this.log.logName, {
      body: this.log.message,
      icon: '/bot.png',
      silent: true
    });
  }
}
