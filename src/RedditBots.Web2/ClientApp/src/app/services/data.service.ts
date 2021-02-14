import { Injectable } from '@angular/core';
import { Subject } from 'rxjs'

@Injectable({
  providedIn: 'root'
})
export class DataService {
  info: Info = { LPS: 0 };

  public LPSChange: Subject<Info> = new Subject<Info>();

  constructor() {
  }

  setLPS(lps: Info) {
    this.LPSChange.next(lps);
  }
}

export interface Info {
  LPS: number;
}
