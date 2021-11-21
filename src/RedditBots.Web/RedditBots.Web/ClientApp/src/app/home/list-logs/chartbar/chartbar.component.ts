import { AfterViewInit, Component, ElementRef, OnDestroy,  ViewChild } from "@angular/core";
import { Chart, registerables } from 'chart.js';
import ChartStreaming from 'chartjs-plugin-streaming';
import 'chartjs-adapter-luxon';
import { LogsService } from "../../../services/logs.service";

@Component({
  selector: "app-chartbar",
  templateUrl: "./chartbar.component.html",
  styleUrls: ["./chartbar.component.css"],
})
export class ChartbarComponent implements AfterViewInit, OnDestroy {
  @ViewChild('incomminglogchart') chartCanvas!: ElementRef;
  private chart: Chart | undefined;

  currentLPS: number = 0;

  constructor(logsService: LogsService) {
    Chart.register(...registerables);
    Chart.register(ChartStreaming);

    logsService.currentLPS$.subscribe(lps => this.currentLPS = lps)
  }

  ngAfterViewInit(): void {
    this.initChart();
  }

  ngOnDestroy(): void {
    this.chart.destroy();
  }

  initChart(): void {
    this.chart = new Chart(this.chartCanvas.nativeElement, {
      type: 'line',
      data: {
        datasets: [
          {
            backgroundColor: "#f0fff1",
            borderColor: "#aeff7b",
            data: [],
            label: "Total incoming logs",
          },
        ]
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
              ttl: undefined,
              onRefresh: chart => {
                chart.data.datasets[0].data.push({ x: Date.now(), y: this.currentLPS });
              }
            }
          },
          y: {
            beginAtZero: true,
            ticks: {
              precision: 0,
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
