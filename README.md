# EPAM Automated Testing in .NET - Test Automation Framework

A layered **C# / .NET test automation framework** covering both **UI (Selenium WebDriver)** and **REST API (RestSharp)** testing, with **BDD scenarios (Reqnroll/Gherkin)** and a **GitHub Actions CI pipeline**.
Built as part of the EPAM *Automated Testing in .NET* program; UI tests target the public [epam.com](https://www.epam.com/) website, API tests target [JSONPlaceholder](https://jsonplaceholder.typicode.com).

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4) ![NUnit](https://img.shields.io/badge/NUnit-4-green) ![Selenium](https://img.shields.io/badge/Selenium-4-43B02A) ![RestSharp](https://img.shields.io/badge/RestSharp-114-blue) ![Reqnroll](https://img.shields.io/badge/BDD-Reqnroll-orange)

---

## Highlights

- **3-layer architecture** - `Core` (reusable infrastructure) → `Business` (Page Objects & models) → `Tests` (test cases only).
- **Page Object Model** with a fluent API - test methods read like user scenarios.
- **Cross-browser** - Chrome, Firefox, Edge via a `BrowserFactory`; headless mode supported.
- **Thread-safe WebDriver** management (`ThreadLocal<IWebDriver>`) ready for parallel execution.
- **API testing layer** - `ApiClient` wrapper over RestSharp + fluent `RequestBuilder`, typed response models (System.Text.Json).
- **BDD** - Gherkin feature files with Reqnroll step definitions.
- **Environment-based configuration** - `appsettings.{env}.json` + environment-variable overrides (`TAF_ENV`, `TAF_*`).
- **Diagnostics** - NLog logging of every step and request, automatic **screenshot on failure**.
- **CI/CD** - GitHub Actions: runs on PRs, nightly schedule and manually with browser selection; publishes `.trx` results and screenshots as artifacts.
- Explicit waits only (implicit wait disabled), stale-element retry handling, cookie-banner handling.

## Architecture

```
TestProject1.sln
├── Core/                       # Framework layer (no knowledge of the app under test)
│   ├── Api/                    # ApiClient (RestSharp wrapper), RequestBuilder
│   ├── Browser/                # BrowserFactory, BrowserType, DriverManager (ThreadLocal)
│   ├── Configuration/          # ConfigProvider, TafSettings (appsettings + env vars)
│   ├── Logging/                # TafLogger (NLog)
│   ├── Pages/                  # BasePage: waits, clicks, scrolling helpers
│   ├── Tests/                  # BaseTest: setup/teardown, screenshot on failure
│   └── Utilities/              # ScreenshotHelper
├── Business/                   # Application layer
│   ├── Pages/                  # HomePage, CareersPage, InsightsPage, ServicesPage, ArticlePage…
│   └── Models/                 # UserModel (API DTOs)
├── TestProject1/               # Test layer
│   ├── Api/UserApiTests.cs     # REST API tests
│   ├── Features/               # Gherkin .feature files
│   ├── StepDefinitions/        # Reqnroll step bindings
│   ├── *Tests.cs               # UI tests
│   └── appsettings*.json       # qa / staging configs, NLog.config
└── .github/workflows/          # CI pipeline
```

## Test coverage

| Area | Type | What is verified |
|---|---|---|
| Careers search | UI, data-driven | Search by keyword + country + "Remote" filter returns a matching job card |
| Global search | UI, data-driven | Every search result contains the searched keyword |
| Insights carousel | UI | Slide title matches the title of the opened article |
| File download | UI | Code of Conduct PDF is downloaded and non-empty |
| Services navigation | UI, BDD | Navigating to a service category opens the correct page with "Related Offerings" |
| Users API | API | Status codes (200/201/404), headers (`Content-Type`), schema & content (10 unique users), POST creation |

Example - a fluent UI test:

```csharp
string jobCardText = homePage.Open(ConfigProvider.Settings.BaseUrl)
                             .NavigateToCareers()
                             .ClickStartSearch()
                             .EnterKeyword("Java")
                             .SelectCountry("Poland")
                             .SelectRemote()
                             .ClickSearch()
                             .ExpandLatestJobCardContaining("Java");
```

Example - an API test:

```csharp
var request  = new RequestBuilder("/users", Method.Get).Build();
var response = await _apiClient.ExecuteAsync<List<UserModel>>(request);

Assert.That((int)response.StatusCode, Is.EqualTo(200));
Assert.That(response.Data, Has.Count.EqualTo(10));
```

## Getting started

**Prerequisites:** .NET 9 SDK, Chrome / Firefox / Edge installed (drivers are resolved automatically by Selenium Manager).

```bash
git clone https://github.com/Hanzoze/Epam-Automated-Testing-in-.NET.git
cd Epam-Automated-Testing-in-.NET
dotnet restore
dotnet build

# all tests
dotnet test

# API tests only
dotnet test --filter TestCategory=API

# UI tests only
dotnet test --filter TestCategory!=API

# BDD regression scenarios
dotnet test --filter Category=regression
```

### Configuration

Settings live in `TestProject1/appsettings.json` and are overridden per environment (`appsettings.qa.json`, `appsettings.staging.json`):

```json
{
  "TafSettings": {
    "Browser": "Chrome",
    "Headless": false,
    "BaseUrl": "https://www.epam.com/",
    "MinLogLevel": "Info",
    "DefaultWaitTimeoutSeconds": 10
  }
}
```

Select an environment or override any value with environment variables:

```powershell
$env:TAF_ENV = "staging"
$env:TAF_TafSettings__Browser = "Firefox"
dotnet test
```

## CI/CD

`.github/workflows/test-pipeline.yml`:

- **Triggers:** pull requests to `main`, nightly run (02:00 UTC), manual run with browser choice (Chrome / Firefox / Edge).
- **Steps:** restore → build → API tests → UI tests (run even if API tests fail) → upload `.trx` reports and failure screenshots as artifacts (kept 7 days).

