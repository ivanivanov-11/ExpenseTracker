# ExpenseTracker

Personal web application for expense tracking using ASP.NET Core MVC. Register an account, enter your expenses, categorize them, set a budget and analyze how you spend your money.

## Main Features

- **Accounts** - register new account, log in/out, change your password, manage or delete personal data (using ASP.NET Core Identity).
- **Dashboard** - total expenses this month, total number of transactions, average transaction, your remaining budget (if you set a limit), your 10 latest expenses and expense breakdown by category.
- **Expenses Management** - create, view, edit and delete expenses (input: title, amount, date paid, category, payment method and optional notes).
- **Search, filters and pagination** - search by expense title or notes, filter by category and date range. 10 expenses per page.
- **Categories** - a predefined list of categories is provided (Housing, Food, Transport, Health, Subscriptions, Personal, Travel, Other) and users can create, edit and delete their own. Deleting a custom category transfers all expenses of the category to the default category 'Other'. Category 'Other' cannot be deleted.
- **Budget** - set a spending limit for the current month and compare it to your expenses. If you exceeded your budget, the remaining budget indicator on the Dashboard is colored red.
- **User data** - all your expenses, categories and budgets belong to the user logged in and cannot be seen by other users. If the user is not logged in, he or she is redirected to the Login page.

## Technologies Used

| Area | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) + Razor Pages (Identity UI) |
| Language | C# |
| Database | Microsoft SQL Server (Express / LocalDB / full) |
| ORM | Entity Framework Core 8 (Code-First migrations) |
| Authentication | ASP.NET Core Identity |
| Front end | Razor views, Bootstrap, jQuery + jQuery Validation, Font Awesome (CDN) |

## Requirements

- .NET 8 SDK
- Microsoft SQL Server installed on the computer. This project uses **SQL Server Express** (instance name `SQLEXPRESS`). It also works with LocalDB (installed with Visual Studio) or full SQL Server.
- Git
- Optional: Visual Studio 2022 (17.8+) or VS Code
- Internet connection when running the application (icons load from a CDN)

## Setup Instructions

1. **Clone the repository**

   ```bash
   git clone https://github.com/ivanivanov-11/ExpenseTracker
   cd ExpenseTracker
   ```

2. **Configure database connection** - open `ExpenseTracker/appsettings.Development.json` and set `Server=` to your SQL Server (see [Environment variables & configuration](#environment-variables--configuration) below). Default value will work for a default SQL Server instance.

3. **Restore packages and install the EF Core tool** (once)

   ```bash
   dotnet restore
   dotnet tool install --global dotnet-ef
   ```

4. **Create database by applying the migrations**

   ```bash
   dotnet ef database update --project ExpenseTracker.Data --startup-project ExpenseTracker
   ```

   Or open *Package Manager Console* in Visual Studio, set default project to `ExpenseTracker.Data` and run `Update-Database`.

5. **Run the application**

   ```bash
   dotnet run --project ExpenseTracker --launch-profile https
   ```

   The browser will open automatically at https://localhost:7066. If not, open it manually at https://localhost:7066. The application also listens on http://localhost:5297 (it is redirected to https://localhost:7066).

   If the browser complains about the developer certificate, run `dotnet dev-certs https --trust` once.

6. **Use the application** - press **Register**, enter any valid email and password meeting the Identity requirements (at least 6 characters, upper and lower case letters, digit and special character) and you will be logged in. Demo user accounts are not provided.

## Environment variables & configuration

This project requires only the following. **No API keys or any kind of secrets, no third-party credentials are required**, there are **no default accounts** in the application.

| Name | Where it is set | Required | Purpose | Default / example value |
|---|---|---|---|---|
| `ConnectionStrings:DefaultConnection` | `ExpenseTracker/appsettings.Development.json` (or env var `ConnectionStrings__DefaultConnection`) | **Yes** | SQL Server database connection string used by the application and migrations. | `Server=.;Database=ExpenseTrackerDB;Trusted_Connection=True;Encrypt=False` |
| `ASPNETCORE_ENVIRONMENT` | `ExpenseTracker/Properties/launchSettings.json` (or env var) | No | Specifies the current environment. `Development` enables detailed error pages and uses `appsettings.Development.json`. | `Development` (set by the launch profiles) |
| `ASPNETCORE_URLS` | `launchSettings.json` (`applicationUrl`) or env var | No | Application listen URL and port. | `https://localhost:7066;http://localhost:5297` |

### Connection string

Default value is:

```json
"DefaultConnection": "Server=.;Database=ExpenseTrackerDB;Trusted_Connection=True;Encrypt=False"
```

`Server=` part must match the computer. `Trusted_Connection=True` means Windows authentication will be used, **username and password are not required**. Use one of the following values depending on your SQL Server:

| SQL Server | `Server=` value |
|---|---|
| Default instance | `.` or `localhost` |
| SQL Server Express (used by the author) | `.\\SQLEXPRESS` or `PC-NAME\\SQLEXPRESS` |
| LocalDB (installed with Visual Studio) | `(localdb)\\MSSQLLocalDB` |

> Note double backslash in a json file. Single backslash is enough in env var or powershell command.

To know the name of your SQL Server instance, open SQL Server Management Studio and see the "Server name" field when connecting.

**Option A - edit `ExpenseTracker/appsettings.Development.json`** (example for SQL Server Express):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=ExpenseTrackerDB;Trusted_Connection=True;Encrypt=False"
  }
}
```

**Option B - .NET user secrets** (secrets are not stored in Git), for a server that requires user name and password:

```bash
cd ExpenseTracker
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\SQLEXPRESS;Database=ExpenseTrackerDB;User Id=your_user;Password=your_password;TrustServerCertificate=True"
```

**Option C - env var** (note the double underscore):

```powershell
# Windows PowerShell
$env:ConnectionStrings__DefaultConnection = "Server=.\SQLEXPRESS;Database=ExpenseTrackerDB;Trusted_Connection=True;Encrypt=False"
```

> `SQLExpressDevString` in `appsettings.Development.json` is not used by the application and may be ignored.