import { BotSetting } from "./botsetting";

export interface SettingsState {
  displaySettingsMenu: boolean;
  displayDebugLogs: boolean;
  desktopNotificationIsOn: boolean;
  isDarkMode: boolean;
  botSettings: BotSetting[];
}
