import { Component, OnInit } from "@angular/core";
import { BotSetting } from "../interfaces/botsetting";
import { Log } from "../interfaces/log";
import { SettingsState } from "../interfaces/settings-state";
import { LogsService } from "../services/logs.service";
import { UserSettingsService } from "../services/user-settings.service";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
})
export class HomeComponent implements OnInit {
  logs: Log[] = []
  state!: SettingsState;
  lastLogTime!: string;

  constructor(
    private logService: LogsService,
    private userSettingsService: UserSettingsService,
  ) {
    this.state = this.userSettingsService.getOrCreateState();
  }

  ngOnInit(): void {
    this.logService.logsStream$.subscribe((logs) => 
      this.logs = logs
    );
    this.logService.lastLogTime$.subscribe((logs) =>
      this.lastLogTime = logs
    );
  }

  toggleDebugLogs(value: boolean): void {
    this.userSettingsService.toggleDebugLogs(value);
  }

  updateBotSetting(botSetting: BotSetting): void {
    this.userSettingsService.updateBotSetting(botSetting);
  }

  toggleSettingsMenu(value: boolean): void {
    this.userSettingsService.toggleSettingsMenu(value);
  }

  toggleReceiveDesktopNotififcations(value: boolean): void {
    this.userSettingsService.setReceiveDesktopNotification(value);
  }
}
