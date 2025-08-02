import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Currency } from '../models/currency.model';
import { ConvertedRate } from '../models/converted-rate.model';
import { AppConfigService } from './app-config.service';
import { LoggingService } from './logging.service';

@Injectable({
  providedIn: 'root'
})
export class CurrencyService {
  private apiUrl: string = "";

  constructor(private http: HttpClient, private logger: LoggingService, private appConfigService: AppConfigService) {
    this.apiUrl = this.appConfigService.config?.api.baseUrl ?? 'https://localhost:7220/api';
  }

  getAvailableCurrencies(): Observable<Currency[]> {
    this.logger.info('Downloading a list of currencies from the selected API...');
    return this.http.get<Currency[]>(`${this.apiUrl}/Currency/available`).pipe(
      catchError(this.handleError)
    );
  }

  getConvertedRates(
    currency1Code: string,
    currency2Code: string,
    startDate: string,
    endDate: string
  ): Observable<ConvertedRate[]> {
    this.logger.info(`Fetching rates for ${currency1Code} to ${currency2Code} from ${startDate} to ${endDate}...`);

    let params = new HttpParams()
      .set('currency1Code', currency1Code)
      .set('currency2Code', currency2Code)
      .set('startDate', startDate)
      .set('endDate', endDate);

    // 'convert' endpoint to be implemented
    return this.http.get<ConvertedRate[]>(`${this.apiUrl}/Currency/convert`, { params }).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: any): Observable<never> {
    let errorMessage = 'Unknown error occured!';
    if (error.error instanceof ErrorEvent) {
      // client error
      errorMessage = `Error message: ${error.error.message}`;
    } else {
      // server error
      errorMessage = `Error code: ${error.status}\nError message: ${error.message}`;
      if (error.error && typeof error.error === 'string') {
        errorMessage = `Error code: ${error.status}\nError message: ${error.error}`;
      }
    }
    this.logger.error(error);
    return throwError(() => new Error(errorMessage));
  }
}
