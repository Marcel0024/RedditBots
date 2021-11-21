import { Component, Input, OnInit } from "@angular/core";
import { Log } from "../../../interfaces/log";

@Component({
  selector: "app-log",
  templateUrl: "./log.component.html",
  styleUrls: ["./log.component.css"],
})
export class LogComponent implements OnInit {
  @Input() log!: Log;

  showNEWStyling: boolean = false;

  constructor() { }

  ngOnInit() {
    if (this.log.notify) {
      this.showNEWStyling = true;

      setTimeout(() => {
        this.showNEWStyling = false;
      }, 2000);
    }
  }
}
