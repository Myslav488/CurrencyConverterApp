# CurrencyconverterappClient

The main objective of this application is to provide users with a currency conversion tool that not only displays real-time rates but also offers historical data. It allows users to track the exchange rate between two selected currencies over a specific period, also including the average, minimum, and maximum rates, along with the dates on which these extreme values occurred.

## Instructions for Use
Select Currencies: Choose the source currency and the target currency from the dropdown menus.

Choose Dates: Use the date pickers to select a start date and an end date for the conversion period. The default dates are yesterday and today, respectively.

Convert: Click the "Convert" button to fetch the data.

View Results: The application will display a table with daily exchange rates and a summary of statistics (average, minimum, and maximum rates) for the selected period.

Error Handling: If the date range is invalid or exceeds 93 days, same currencies were choosen or end date is before starting date, an error message will be shown.

This project was created using Angular version 19.2.15 and .NET 8.0.

## Download and use of an application

To get an aplication run:
 
```bash
git clone https://github.com/Myslav488/CurrencyConverterApp.git
```

Then setup an environment:
```bash
cd CurrencyConverterApp
npm install
```

To start a local development server, run:

```bash
cd CurrencyConverterApp.Server
dotnet run
```

Once the server is running, open your browser and navigate to `http://localhost:51684/`. The application will be ready to use.
