import { Component, OnInit } from '@angular/core';
import { CurrencyService } from '../services/currency.service';
import { Currency } from '../models/currency.model';
import { ConvertedRate } from '../models/converted-rate.model';
import { CommonModule } from '@angular/common'; // import CommonModule
import { FormsModule } from '@angular/forms'; // import FormsModule
import { AppConfigService, SortBy, SortOrder } from '../services/app-config.service';
import { LoggingService } from '../services/logging.service';

@Component({
  selector: 'app-currency-converter',
  standalone: true,
  imports: [CommonModule, FormsModule], // import CommonModule i FormsModule
  templateUrl: './currency-converter.component.html',
  styleUrl: './currency-converter.component.css' // styles
})
export class CurrencyConverterComponent implements OnInit {
  availableCurrencies: Currency[] = [];
  selectedCurrency1: string = '';
  selectedCurrency2: string = '';
  startDate: string = '';
  endDate: string = '';
  convertedRates: ConvertedRate[] = [];
  isLoading: boolean = false;
  errorMessage: string = '';

  private sortBy: SortBy = SortBy.Code;
  private sortOrder: SortOrder = SortOrder.Asc;
  public dateFormat: string = "yyyy-MM-dd";

  constructor(
    private currencyService: CurrencyService,
    private logger: LoggingService,
    private appConfigService: AppConfigService
  ) {  }

  ngOnInit(): void {
    // get config via service
    this.sortBy = this.appConfigService.config?.currencySorting.sortBy ?? SortBy.Code;
    this.sortOrder = this.appConfigService.config?.currencySorting.sortOrder ?? SortOrder.Asc;
    this.dateFormat = this.appConfigService.config?.dateFormat ?? "yyyy-MM-dd";

    this.loadAvailableCurrencies();
  }

  // load a list of currencies fetched by api
  loadAvailableCurrencies(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.logger.info('Loading available currencies...');

    this.currencyService.getAvailableCurrencies().subscribe({
      next: (data) => {
        this.availableCurrencies = this.sortCurrencies(data);

        // set default values
        if (data.length > 0) {
          this.selectedCurrency1 = 'PLN';
          this.selectedCurrency2 = 'EUR';
        }
        this.isLoading = false;
        this.logger.info('Currency list loaded successfully.');
      },
      error: (err) => {
        this.errorMessage = `Could not load currencies list: ${err.message}`;
        this.isLoading = false;
        this.logger.error(this.errorMessage);
      }
    });
  }

  /**
   * Fetches the converted rates and displays them in a table.
   */
  onConvert(): void {
    this.errorMessage = '';
    this.convertedRates = []; // cleanup recent results


    if (!this.selectedCurrency1 || !this.selectedCurrency2 || !this.startDate || !this.endDate) {
      this.errorMessage = 'Please select both currencies and date range.';
      return;
    }

    if (this.selectedCurrency1 === this.selectedCurrency2) {
      this.errorMessage = 'Please select two different currencies.';
      return;
    }

    if (new Date(this.startDate) > new Date(this.endDate)) {
      this.errorMessage = 'The start date cannot be later than the end date.';
      return;
    }

    this.isLoading = true;
    this.logger.debug('The data to be converted is correct. Sending query...');

    this.currencyService.getConvertedRates(
      this.selectedCurrency1,
      this.selectedCurrency2,
      this.startDate,
      this.endDate
    ).subscribe({
      next: (data) => {
        this.convertedRates = data;
        this.isLoading = false;
        this.logger.info('Rates converted successfully.');
      },
      error: (err) => {
        this.errorMessage = `Error while converting rates: ${err.message}`;
        this.isLoading = false;
        this.logger.error(this.errorMessage);
      }
    });
  }

  // Sorting currencies based on setup in app-setting.json
  private sortCurrencies(currencies: Currency[]): Currency[] {
    return currencies.sort((a, b) => {
      let comparison = 0;
      if (this.sortBy === SortBy.Code) {
        comparison = a.code.localeCompare(b.code);
      } else if (this.sortBy === SortBy.Name) {
        comparison = a.name.localeCompare(b.name);
      }

      return this.sortOrder === SortOrder.Asc ? comparison : -comparison;
    });
  }
}
