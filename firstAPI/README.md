# firstAPI

A .NET 9 Minimal API project showcasing small, focused endpoints for learning and quick experimentation. It includes calculators, text utilities, number games, dates, in-memory collections, temperature conversions, password utilities, simple validators, and unit conversions powered by UnitsNet.

## Tech Stack

- .NET 9 (ASP.NET Core Minimal API)
- UnitsNet (unit conversions)

## Project Structure

```
firstAPI/
├─ Program.cs                       # Minimal host + endpoint wiring
├─ Endpoints/
│  ├─ CalculatorEndpoints.cs        # /calculator
│  ├─ TextEndpoints.cs              # /text
│  ├─ NumberEndpoints.cs            # /numbers
│  ├─ DateEndpoints.cs              # /date
│  ├─ ColorsEndpoints.cs            # /colors 
│  ├─ TempEndpoints.cs              # /temp
│  ├─ PasswordEndpoints.cs          # /password
│  ├─ ValidateEndpoints.cs          # /validate
│  └─ ConvertEndPoints.cs           # /convert 
├─ Properties/
│  └─ launchSettings.json
├─ firstAPI.csproj
└─ README.md
```

## Getting Started

1. Clone and restore
   - `git clone <your-repo-url>`
   - `cd firstAPI`
   - `dotnet restore`

2. Run
   - `dotnet run`
   - Console shows the base URL, e.g. `http://localhost:5063`

## Endpoints Overview

Replace the port with what you see in your console output (e.g., `http://localhost:5063`).

### Calculator — `/calculator`
- `GET /calculator/add/{a}/{b}`
- `GET /calculator/subtract/{a}/{b}`
- `GET /calculator/multiply/{a}/{b}`
- `GET /calculator/divide/{a}/{b}` 

Example:
- `GET http://localhost:5063/calculator/add/2/3`

### Text — `/text`
- `GET /text/reverse/{text}`
- `GET /text/uppercase/{text}`
- `GET /text/lowercase/{text}`
- `GET /text/count/{text}` 
- `GET /text/palindrome/{text}` 

Example:
- `GET http://localhost:5063/text/reverse/hello`

### Numbers — `/numbers`
- `GET /numbers/fizzbuzz/{count}`
- `GET /numbers/prime/{number}`
- `GET /numbers/fibonacci/{count}`
- `GET /numbers/factors/{number}`

Example:
- `GET http://localhost:5063/numbers/prime/13`

### Date — `/date`
- `GET /date/today` (yyyy-MM-dd)
- `GET /date/age/{birthYear}`
- `GET /date/daysbetween/{date1}/{date2}`
- `GET /date/weekday/{date}`

Example:
- `GET http://localhost:5063/date/today`

### Colors — `/colors`
- `GET /colors` 
- `GET /colors/random` 
- `GET /colors/search/{letter}` 
- `POST /colors/add/{color}` 

Example:
- `GET http://localhost:5063/colors`

### Temperature — `/temp`
- `GET /temp/celsius-to-fahrenheit/{temp}`
- `GET /temp/fahrenheit-to-celsius/{temp}`
- `GET /temp/kelvin-to-celsius/{temp}`
- `GET /temp/compare/{temp1}/{unit1}/{temp2}/{unit2}` 

Example:
- `GET http://localhost:5063/temp/celsius-to-fahrenheit/25`

### Password — `/password`
- `GET /password/simple/{length}` 
- `GET /password/complex/{length}` 
- `GET /password/memorable/{words}` 
- `GET /password/strength/{password}` 

Example:
- `GET http://localhost:5063/password/strength/HelloWorld123!`

### Validate — `/validate`
- `GET /validate/email/{email}` 
- `GET /validate/phone/{phone}` 
- `GET /validate/creditcard/{number}` 
- `GET /validate/strongpassword/{password}` 

Example:
- `GET http://localhost:5063/validate/creditcard/4111%201111%201111%201111`

### Convert (UnitsNet) — `/convert`
- `GET /convert/length/{value}/{fromUnit}/{toUnit}`
- `GET /convert/weight/{value}/{fromUnit}/{toUnit}`
- `GET /convert/volume/{value}/{fromUnit}/{toUnit}`
- `GET /convert/list-units/{type}`

Examples:
- `GET http://localhost:5063/convert/length/2/ft/m`
- `GET http://localhost:5063/convert/weight/10/lb/kg`
- `GET http://localhost:5063/convert/volume/3/gal/l`
- `GET http://localhost:5063/convert/list-units/length`