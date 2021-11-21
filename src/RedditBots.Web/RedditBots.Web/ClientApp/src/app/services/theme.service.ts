import { EventEmitter, Injectable } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class ThemeService {
  darkModeOn: boolean = true;
  onToggleDarkMode$ = new EventEmitter<boolean>();

  toggleDarkMode(darkModeOn: boolean) {
    this.onToggleDarkMode$.emit(darkModeOn);
    this.darkModeOn = darkModeOn;

    if (darkModeOn === true) {
      document.documentElement.style.setProperty('--background-color', '#303030');
      document.documentElement.style.setProperty('--logs-background', '#424242');
      document.documentElement.style.setProperty('--log-backgroundcolor', '#606060');
      document.documentElement.style.setProperty('--font-color', 'white');
      document.documentElement.style.setProperty('--char-background-color', '#999999');
      document.documentElement.style.setProperty('--link-color', '#9fceff');    

    } else {
      document.documentElement.style.setProperty('--background-color', '#f8f9fa');
      document.documentElement.style.setProperty('--logs-background', 'white');
      document.documentElement.style.setProperty('--log-backgroundcolor', 'white');
      document.documentElement.style.setProperty('--font-color', 'black');     
      document.documentElement.style.setProperty('--char-background-color', 'white');
      document.documentElement.style.setProperty('--link-color', '#007bff');
    }
  }
}
