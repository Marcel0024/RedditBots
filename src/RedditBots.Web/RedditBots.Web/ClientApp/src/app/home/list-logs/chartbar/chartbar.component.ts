import { AfterViewInit, Component, ElementRef, OnDestroy, ViewChild } from "@angular/core";
import { Chart, registerables } from 'chart.js';
import ChartStreaming from 'chartjs-plugin-streaming';
import 'chartjs-adapter-luxon';
import { LogsService } from "../../../services/logs.service";
import { ThemeService } from "../../../services/theme.service";

@Component({
  selector: "app-chartbar",
  templateUrl: "./chartbar.component.html",
  styleUrls: ["./chartbar.component.css"],
})
export class ChartbarComponent implements AfterViewInit, OnDestroy {
  @ViewChild('incomminglogchart') chartCanvas!: ElementRef;
  private chart: Chart | undefined;

  currentLPS: number = 0;

  constructor(logsService: LogsService, private themeService: ThemeService) {
    Chart.register(...registerables);
    Chart.register(ChartStreaming);

    logsService.currentLPS$.subscribe(lps => this.currentLPS = lps)
    themeService.onToggleDarkMode$.subscribe(darkModeOn => {
      this.chart.update();
    })
  }

  ngAfterViewInit(): void {
    this.initChart();
  }

  ngOnDestroy(): void {
    this.chart.destroy();
  }

  getColor(): string {
    return this.themeService.darkModeOn ? 'white' : '#999999';
  }

  initChart(): void {
    this.chart = new Chart(this.chartCanvas.nativeElement, {
      type: 'line',
      data: {
        datasets: [
          {
            backgroundColor: "white",
            borderColor: "#aeff7b",
            data: [],
            label: "Total incoming logs",
          },
        ]
      },
      options: {
        plugins: {
          legend: {
            labels: {
              color: this.getColor()
            }
          },
        },
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
              ttl: undefined,
              onRefresh: chart => {
                chart.data.datasets[0].data.push({ x: Date.now(), y: this.currentLPS });
              },
            },
            grid: {
              color: () => this.getColor(),
              tickColor: () => this.getColor(),
            },
            ticks: {
              color: () => this.getColor(),
              textStrokeColor: () => this.getColor(),
            }
          },
          y: {
            beginAtZero: true,
            ticks: {
              precision: 0,
              stepSize: 5,
              color: () => this.getColor(),
              textStrokeColor: () => this.getColor(),
            },
            title: {
              display: false,
            },
            grid: {
              color: () => this.getColor(),
              tickColor: () => this.getColor(),
            },
          }
        },
        interaction: {
          intersect: false
        }
      }
    })
  }
}
