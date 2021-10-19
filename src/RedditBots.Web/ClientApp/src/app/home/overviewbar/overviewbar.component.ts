import { Component, EventEmitter, Input, Output } from "@angular/core";
import { ConnectionStatus } from "../../interfaces/connection-status";
import { LogsService } from "../../services/logs.service";
import { SignalrService } from "../../services/signalr.service";

@Component({
  selector: "app-overviewbar",
  templateUrl: "./overviewbar.component.html",
  styleUrls: ["./overviewbar.component.css"],
})
export class OverviewbarComponent {
  connectionStatus = ConnectionStatus;

  totalViewers!: number;
  lps: number = 0;
  lastLog: string;
  currentConnectionStatus!: ConnectionStatus;

  @Input("display-settings-menu") displaySettingsMenu!: boolean;
  @Output("toggle-settings-menu-change") toggleSettingsMenuChange =
    new EventEmitter<boolean>();

  constructor(signalRService: SignalrService, logsService: LogsService) {
    signalRService.connectionStatusChange$.subscribe(
      (cs) => (this.currentConnectionStatus = cs)
    );
    signalRService.lastUpdate$.subscribe((lu) => (this.lastLog = lu));
    signalRService.totalViewers$.subscribe((tv) => (this.totalViewers = tv));
    logsService.currentLPS$.subscribe((lps) => (this.lps = lps));
  }

  toggleSettingsMenu(): void {
    this.toggleSettingsMenuChange.next(!this.displaySettingsMenu);
  }
}
