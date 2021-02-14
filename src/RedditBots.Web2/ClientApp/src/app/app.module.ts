import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { ListLogsComponent } from './home/list-logs/list-logs.component';
import { LogComponent } from './/home/list-logs/log/log.component';
import { OverviewbarComponent } from './home/overviewbar/overviewbar.component';
import { SettingsbarComponent } from './home/settingsbar/settingsbar.component';
import { BotSettingsComponent } from './home/settingsbar/bot-settings/bot-settings.component';
import { ChartbarComponent } from './home/list-logs/chartbar/chartbar.component';

import { ChartsModule, ThemeService } from 'ng2-charts';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ListLogsComponent,
    LogComponent,
    OverviewbarComponent,
    SettingsbarComponent,
    BotSettingsComponent,
    ChartbarComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ChartsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
    ])
  ],
  providers: [
    ThemeService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
