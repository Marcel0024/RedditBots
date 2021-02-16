import { Component, NgZone, OnInit } from '@angular/core';
import { DataService } from '../../../core/data.service';
import { SignalrService } from '../../../core/signalr.service';
import 'chartjs-plugin-streaming';

@Component({
  selector: 'app-chartbar',
  templateUrl: './chartbar.component.html',
  styleUrls: ['./chartbar.component.css']
})
export class ChartbarComponent implements OnInit {
  LogCount: any[] = [];
  TotalLogs: number = 0;

  constructor(
    private _data: DataService,
    private signalR: SignalrService,
    private _ngZone: NgZone
  ) {
    this.subscribeToEvents();
  }

  private subscribeToEvents(): void {
    this.signalR.logReceived.subscribe((incomingLog: any) => {
      this._ngZone.run(() => {
        if (incomingLog.Notify) {
          this.TotalLogs++;
        }

        this.LogCount.push(incomingLog);
      });
    });
  }

  ngOnInit() {
    setInterval(() => {
      this.datasets[0].data.push({ x: Date.now(), y: this.TotalLogs });
      this._data.setLPS(this.TotalLogs);
      this.TotalLogs = 0;
    }, 1000);
  }

  datasets: any[] = [{
    backgroundColor: '#f0fff1',
    borderColor: '#aeff7b',
    data: [],
    label: 'Logs processed'
  }];

  options: any = {
    scales: {
      xAxes: [{
        type: 'time',
        realtime: {
          duration: 30000,
          refresh: 1000,
          delay: 1000,
          pause: false,
          ttl: undefined,

          onRefresh: (chart) => {
            var data = this.LogCount.shift();

            if (data) {
              Array.prototype.push.apply(chart.data.datasets[0].data, [data]);
            }
          }
        }
      }],
      yAxes: [{
        ticks: {
          beginAtZero: true,
          stepSize: 5
        }
      }]
    },
    plugins: {
      streaming: {
        frameRate: 30
      }
    }
  };
}
