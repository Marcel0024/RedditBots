import { AfterViewInit, Component, ElementRef, NgZone, OnDestroy,  ViewChild } from "@angular/core";
import { Chart, registerables } from 'chart.js';
import ChartStreaming from 'chartjs-plugin-streaming';
import 'chartjs-adapter-luxon';
import { LogsService } from "../../../services/logs.service";
import { SignalrService } from "../../../services/signalr.service";

@Component({
  selector: "app-chartbar",
  templateUrl: "./chartbar.component.html",
  styleUrls: ["./chartbar.component.css"],
})
export class ChartbarComponent implements AfterViewInit, OnDestroy {
  datasets: any[] = [
    {
      backgroundColor: "#f0fff1",
      borderColor: "#aeff7b",
      data: [],
      label: "Logs processed",
    },
  ]

  LogCount: any[] = [];
  TotalLogs: number = 0;

  @ViewChild('incomminglogchart') chartCanvas: ElementRef | undefined;
  private chart: Chart | undefined;

  constructor(
    private signalrService: SignalrService,
    private logsService: LogsService,
    private ngZone: NgZone
  ) {
    this.subscribeToEvents();
    Chart.register(...registerables);
    Chart.register(ChartStreaming);
  }

  private subscribeToEvents(): void {
    this.signalrService.logReceived$.subscribe((incomingLog: any) => {
      this.ngZone.run(() => {
        if (incomingLog.Notify) {
          this.TotalLogs++;
        }

        this.LogCount.push(incomingLog);
      });
    });
  }

  ngAfterViewInit(): void {
    this.initChart();

    setInterval(() => {
      this.datasets[0].data.push({ x: Date.now(), y: this.TotalLogs  });
      this.logsService.setLPS(this.TotalLogs);
      this.TotalLogs = 0;
    }, 1000);
  }

  ngOnDestroy(): void {
    this.chart.destroy();
  }

  initChart(): void {
    if (!this.chartCanvas?.nativeElement) {
      return;
    }

    this.chart = new Chart(this.chartCanvas.nativeElement, {
      type: 'line',
      data: {
        datasets: this.datasets
      },
      options: {
        scales: {
          x: {
            type: 'realtime',
            title: {
              display: false,
            },
            realtime: {
              duration: 30000,
              refresh: 1000,
              delay: 1000,
              pause: false,
              ttl: undefined
            }
          },
          y: {
            beginAtZero: true,
            ticks: {
              stepSize: 5,
            },
            title: {
              display: false,
            }
          }
        },
        interaction: {
          intersect: false
        }
      }
    })
  }
}
