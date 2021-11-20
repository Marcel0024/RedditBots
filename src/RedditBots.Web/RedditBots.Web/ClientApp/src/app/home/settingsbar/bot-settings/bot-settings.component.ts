import { Component, EventEmitter, Input, Output } from "@angular/core";
import { BotSetting } from "../../../interfaces/botsetting";

@Component({
  selector: "app-bot-settings",
  templateUrl: "./bot-settings.component.html",
  styleUrls: ["./bot-settings.component.css"],
})
export class BotSettingsComponent {
  @Input() botSetting: BotSetting;
  @Output() onSettingChange = new EventEmitter<BotSetting>();

  constructor() { }

  toggleOnorOff(value: any): void {
    this.onSettingChange.emit({
      name: this.botSetting.name,
      isOn: value.target.checked,
    });
  }
}
