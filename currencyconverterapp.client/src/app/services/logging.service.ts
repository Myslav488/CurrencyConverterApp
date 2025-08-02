// src/app/services/logging.service.ts
import { Injectable } from '@angular/core';
import { AppConfigService } from './app-config.service';

enum LogLevel {
  Error,
  Warn,
  Info,
  Debug
}

@Injectable({
  providedIn: 'root'
})

export class LoggingService {
  constructor(private appConfigService: AppConfigService) { }

  log(message: any, level: LogLevel = LogLevel.Info): void {

    if (level <= (this.appConfigService.config?.logging.logLevel ?? LogLevel.Info)) {
      const timestamp = new Date().toISOString();
      const prefix = `[${timestamp}] [${LogLevel[level]}]`;

      switch (level) {
        case LogLevel.Error:
          console.error(prefix, message);
          break;
        case LogLevel.Warn:
          console.warn(prefix, message);
          break;
        case LogLevel.Info:
          console.info(prefix, message);
          break;
        case LogLevel.Debug:
          console.log(prefix, message);
          break;
      }
    }
  }

  error(message: any): void { this.log(message, LogLevel.Error); }
  warn(message: any): void { this.log(message, LogLevel.Warn); }
  info(message: any): void { this.log(message, LogLevel.Info); }
  debug(message: any): void { this.log(message, LogLevel.Debug); }
}
