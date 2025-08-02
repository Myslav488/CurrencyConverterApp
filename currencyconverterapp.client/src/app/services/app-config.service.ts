// src/app/services/app-config.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom, BehaviorSubject, Observable } from 'rxjs';

// Data models for configuration
export enum SortOrder {
  Asc = 'ASC',
  Desc = 'DESC'
}

export enum SortBy {
  Code = 'code',
  Name = 'name'
}

export interface AppSettings {
  api: {
    baseUrl: string;
  };
  currencySorting: {
    sortBy: SortBy;
    sortOrder: SortOrder;
  };
  logging: {
    logLevel: number;
  };
  dateFormat: string;
}

@Injectable({
  providedIn: 'root',
})
export class AppConfigService {
  // We use BehaviorSubject to store the configuration state
  private _configSubject = new BehaviorSubject<AppSettings | null>(null);

  constructor(private http: HttpClient) { }

  // Method for loading configuration from a backend endpoint
  async loadConfig(): Promise<void> {
    try {
      // The frontend now queries the backend controller
      const config = await firstValueFrom(this.http.get<AppSettings>('/api/AppConfig'));
      this._configSubject.next(config);
      console.log('App configuration loaded successfully from backend:', config);
    } catch (error) {
      console.error('Failed to load application configuration from backend!', error);
      throw error;
    }
  }

  // Public getter that returns an Observable which other services and components can subscribe to
  get config$(): Observable<AppSettings | null> {
    return this._configSubject.asObservable();
  }

  // We also provide access to the current configuration value
  get config(): AppSettings | null {
    return this._configSubject.value;
  }
}
