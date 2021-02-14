import { Component, Input, Output, EventEmitter } from '@angular/core';
import { BotSetting } from '../../../models/botsetting';

@Component({
  selector: 'app-bot-settings',
  templateUrl: './bot-settings.component.html',
  styleUrls: ['./bot-settings.component.css']
})
export class BotSettingsComponent {
  @Input() botSetting: BotSetting;
  @Output() onSettingChange = new EventEmitter<BotSetting>();

  constructor() { }

  toggleOnorOff(value: boolean): void {
    this.onSettingChange.emit(this.botSetting);
  }
}
