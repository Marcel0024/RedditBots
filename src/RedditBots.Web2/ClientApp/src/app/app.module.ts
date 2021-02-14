import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { ListLogsComponent } from './list-logs/list-logs.component';
import { LogComponent } from './list-logs/log/log.component';
import { OverviewbarComponent } from './overviewbar/overviewbar.component';
import { SettingsbarComponent } from './settingsbar/settingsbar.component';
import { ChartbarComponent } from './list-logs/chartbar/chartbar.component'

import { ChartsModule, ThemeService } from 'ng2-charts';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    FetchDataComponent,
    ListLogsComponent,
    LogComponent,
    OverviewbarComponent,
    SettingsbarComponent,
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
