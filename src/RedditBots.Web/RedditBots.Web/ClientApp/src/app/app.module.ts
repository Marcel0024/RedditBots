import { HttpClientModule } from "@angular/common/http";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { BrowserModule } from "@angular/platform-browser";
import { RouterModule } from "@angular/router";
import { AppComponent } from "./app.component";
import { HomeComponent } from "./home/home.component";
import { ChartbarComponent } from "./home/list-logs/chartbar/chartbar.component";
import { ListLogsComponent } from "./home/list-logs/list-logs.component";
import { LogComponent } from "./home/list-logs/logbar/log.component";
import { OverviewbarComponent } from "./home/overviewbar/overviewbar.component";
import { BotSettingsComponent } from "./home/settingsbar/bot-settings/bot-settings.component";
import { SettingsbarComponent } from "./home/settingsbar/settingsbar.component";

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ListLogsComponent,
    LogComponent,
    OverviewbarComponent,
    SettingsbarComponent,
    BotSettingsComponent,
    ChartbarComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(
      [{ path: "", component: HomeComponent, pathMatch: "full" }],
      { relativeLinkResolution: "legacy" }
    ),
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
