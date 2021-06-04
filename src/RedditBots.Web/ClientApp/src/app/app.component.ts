import { ChangeDetectionStrategy, Component } from '@angular/core';
import { SignalrService } from './core/signalr.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {
  title = 'app';

  constructor(public signalRService: SignalrService) {
  }

  ngOnInit(): void {
    this.signalRService.connect();
  }
}
