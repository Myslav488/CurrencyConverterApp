// src/app/app.component.ts
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CurrencyConverterComponent } from './currency-converter/currency-converter.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, AppComponent, CurrencyConverterComponent],
  template: `
    <main>
      <app-currency-converter></app-currency-converter>
    </main>
  `,
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'currency-frontend';
}
