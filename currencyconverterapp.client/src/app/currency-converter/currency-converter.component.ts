import { Component, OnInit } from '@angular/core';
import { CurrencyService } from '../services/currency.service';
import { Currency } from '../models/currency.model';
import { ConvertedRate } from '../models/converted-rate.model';
import { CommonModule } from '@angular/common'; // import CommonModule
import { FormsModule } from '@angular/forms'; // import FormsModule

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

  constructor(private currencyService: CurrencyService) { }

  ngOnInit(): void {
    this.loadAvailableCurrencies();
  }

  // load a list of currencies fetched by api
  loadAvailableCurrencies(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.currencyService.getAvailableCurrencies().subscribe({
      next: (data) => {
        this.availableCurrencies = data;
        // set default values
        if (data.length > 0) {
          this.selectedCurrency1 = 'PLN';
          this.selectedCurrency2 = 'EUR';
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = `Could not load currencies list: ${err.message}`;
        this.isLoading = false;
        console.error('Error loading currencies:', err);
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
    this.currencyService.getConvertedRates(
      this.selectedCurrency1,
      this.selectedCurrency2,
      this.startDate,
      this.endDate
    ).subscribe({
      next: (data) => {
        this.convertedRates = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = `Error while converting rates: ${err.message}`;
        this.isLoading = false;
        console.error('Error converting rates:', err);
      }
    });
  }
}
