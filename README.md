Here’s an extended **README.md** that includes both **UI (Playwright BDD)** and **API testing** instructions. You can use this as your project documentation:

---

# PlaywrightBDDFramework - Sauce Demo Automation

## 📌 Overview

This project is an automated testing framework for the **Sauce Demo web application** using:

* **C# .NET 8**
* **Playwright** (UI browser automation)
* **SpecFlow** (BDD-style feature definitions)
* **NUnit** (for assertions)
* **ExtentReports** (HTML reporting)
* **REST API Testing** (using `HttpClient` / SpecFlow steps)

It covers **login, product management, cart, checkout, logout**, and **API CRUD operations** for testing endpoints.

---

## 🛠 Setup Instructions

### 1. Prerequisites

* Install [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
* Install a modern browser (Chromium, Firefox, Webkit for Playwright)
* Optional: Visual Studio 2022 or VS Code

### 2. Clone the repository

```bash
git clone <your-repo-url>
cd PlaywrightBDDFramework
```

### 3. Restore dependencies

```bash
dotnet restore
```

---

## 🔹 UI Test Execution (Playwright + SpecFlow)

### Run tests

* **Headless mode (default)**

```bash
dotnet test
```

* **Headful mode (to see the browser UI)**
  Update `TestHooks.cs`:

```csharp
Headless = false
```

### View reports

After execution, HTML reports are generated in:

```text
<project-root>/Reports/extent-report-<timestamp>.html
```

Open the `.html` file in any browser.

---

## 🔹 API Test Execution

### API Base Setup

API tests interact with the **Petstore or Sauce Demo API endpoints** (replace with your environment).

* Base URL: `https://petstore.swagger.io/v2`
* Methods used: `GET`, `POST`, `PUT`, `DELETE`
* Test scenarios include:

  * Create a new pet (`POST /pet`)
  * Retrieve a pet by ID (`GET /pet/{id}`)
  * Update a pet (`PUT /pet`)
  * Delete a pet (`DELETE /pet/{id}`)
  * Invalid payload tests (negative scenarios)

### Run API tests

```bash
dotnet test --filter Category=API
```

> **Tip:** You can categorize tests using `[Category("API")]` and `[Category("UI")]` in your step definition classes for easier filtering.

---

## 🧩 Project Structure

```text
PlaywrightBDDFramework/
│
├─ PlaywrightBDDFramework.csproj        ← Project file
├─ Hooks/
│   └─ TestHooks.cs                     ← Playwright & ExtentReports setup
├─ StepDefinitions/
│   ├─ SauceDemoSteps.cs                ← UI step definitions
│   └─ PetApiSteps.cs                   ← API step definitions
├─ Pages/
│   └─ LoginPage.cs, InventoryPage.cs, CartPage.cs, etc.
├─ Features/
│   ├─ SauceDemo.feature                ← UI BDD scenarios
│   └─ PetApi.feature                    ← API BDD scenarios
├─ Reports/                             ← Generated HTML reports
├─ README.md                            ← This documentation
```

---

## 🔧 Configuration & Environment

* **UI tests**

  * URL: `https://www.saucedemo.com/`
  * Browser options (Chromium, Firefox, Webkit)
  * Headless vs headful mode configurable in `TestHooks.cs`

* **API tests**

  * Base URL and authentication can be configured in `AppSettings.json` or in `ScenarioContext`
  * Optional: Use environment variables for API keys

---

## 📝 Test Scenarios

### UI Positive Scenarios

1. Successful login and inventory visibility
2. Add single/multiple items to cart
3. Complete checkout successfully
4. Logout and verify redirect to login page

### UI Negative Scenarios

1. Invalid login credentials
2. Checkout with empty cart
3. Remove items from cart

### API Positive Scenarios

1. Create a pet with valid payload
2. Retrieve pet details by ID
3. Update pet successfully
4. Delete pet successfully

### API Negative Scenarios

1. Create pet with invalid payload
2. Retrieve non-existing pet
3. Update pet with missing fields
4. Delete non-existing pet

---

## 📐 Test Design Rationale

* **Scenario coverage:** Focus on real user and API flows
* **BDD approach:** SpecFlow features make tests readable for technical and non-technical stakeholders
* **Page Object Model (POM):** Improves maintainability and readability for UI tests
* **API layer abstraction:** Step definitions encapsulate HTTP requests for cleaner tests

