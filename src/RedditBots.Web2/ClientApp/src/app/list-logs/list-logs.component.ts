import { Component, NgZone } from '@angular/core';
import { Log } from '../models/log';
import { SignalrService } from '../services/signalr.service';

@Component({
  selector: 'app-list-logs',
  templateUrl: './list-logs.component.html',
  styleUrls: ['./list-logs.component.css']
})
export class ListLogsComponent {
  logs: Log[] = [];

  constructor(
    private signalR: SignalrService,
    private _ngZone: NgZone
  ) {
    this.subscribeToEvents();
  }

  private subscribeToEvents(): void {
    this.signalR.logReceived.subscribe((incomingLog: any) => {
      const log = {
        notify: incomingLog.Notify,
        logName: incomingLog.LogName,
        message: incomingLog.Message,
        logLevel: incomingLog.LogLevel
      }

      this._ngZone.run(() => {
        this.logs.push(log);

        if (this.logs.length >= 200) {
          for (var i = 0; i < this.logs.length - 200; i++) {
            this.logs.pop();
          }
        }
      });
    });
  }
}
