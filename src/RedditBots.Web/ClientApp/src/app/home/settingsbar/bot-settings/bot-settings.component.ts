import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { BotSetting } from '../../../interfaces/botsetting';

@Component({
  selector: 'app-bot-settings',
  templateUrl: './bot-settings.component.html',
  styleUrls: ['./bot-settings.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BotSettingsComponent {
  @Input() botSetting: BotSetting;
  @Output() onSettingChange = new EventEmitter<BotSetting>();

  constructor() { }

  toggleOnorOff(value: boolean): void {
    this.onSettingChange.emit(this.botSetting);
  }
}
