import { Component, EventEmitter, Input, Output } from "@angular/core";
import { BotSetting } from "../../interfaces/botsetting";

@Component({
  selector: "app-settingsbar",
  templateUrl: "./settingsbar.component.html",
  styleUrls: ["./settingsbar.component.css"],
})
export class SettingsbarComponent {
  @Input("display-settings-menu") displaySettingsMenu!: boolean;
  @Input("display-debug-logs") displayDebugLogs!: boolean;
  @Input("receive-desktop-notification") receiveDesktopNotifs!: boolean;
  @Input("bot-settings") botSettings!: BotSetting[];

  @Output("receive-notification-change") onToggleReceiveDesktopNotififcations =
    new EventEmitter<boolean>();
  @Output("display-debug-logs-change") showDebugLogsChange =
    new EventEmitter<boolean>();
  @Output("bot-setting-change") botSettingChange =
    new EventEmitter<BotSetting>();

  constructor() { }

  toggleReceiveDesktopNotififcations(value: any): void {
    this.onToggleReceiveDesktopNotififcations.next(value.target.checked);
  }

  toggleDebugLogs(value: any): void {
    this.showDebugLogsChange.next(value.target.checked);
  }

  updateBotSetting(botSetting: any): void {
    this.botSettingChange.next(botSetting);
  }
}
