import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  public showSettingsMenu: Subject<boolean> = new Subject<boolean>();

  constructor() { }

  toggleSettingsMenu(displaySettings: boolean) {
    this.showSettingsMenu.next(displaySettings);
  }
}
