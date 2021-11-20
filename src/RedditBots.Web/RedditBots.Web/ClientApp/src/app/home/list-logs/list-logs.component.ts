import { Component, Input } from '@angular/core';
import { Log } from '../../interfaces/log';

@Component({
  selector: 'app-list-logs',
  templateUrl: './list-logs.component.html',
  styleUrls: ['./list-logs.component.css'],
})
export class ListLogsComponent {
  @Input("logs") logs!: Log[];
}
