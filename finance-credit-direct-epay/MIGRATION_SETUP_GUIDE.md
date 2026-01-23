# EPay Migration - Setup and Run Guide

This guide explains how to set up and run the migrated EPay application (Angular + ASP.NET Core).

---

## 📋 Prerequisites

### Backend Requirements
- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** (2019+ recommended)
- **Visual Studio 2022** or **VS Code** with C# extension

### Frontend Requirements
- **Node.js 18+** - [Download](https://nodejs.org/)
- **npm 9+** (comes with Node.js)
- **Angular CLI 17+** - Install globally: `npm install -g @angular/cli`

---

## 🚀 Backend Setup (ASP.NET Core)

### 1. Navigate to Backend Directory
```bash
cd finance-credit-direct-epay/backend
```

### 2. Update Connection Strings
Edit `appsettings.Development.json` and update the connection strings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SQL_SERVER;Database=Datawhse;Integrated Security=true;TrustServerCertificate=true;",
    "AshleyDatabase": "Server=YOUR_SQL_SERVER;Database=Ashley;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

Replace `YOUR_SQL_SERVER` with your actual SQL Server instance name.

### 3. Restore NuGet Packages
```bash
dotnet restore
```

### 4. Build the Project
```bash
dotnet build
```

### 5. Run the Backend
```bash
dotnet run
```

The backend API will start at:
- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000
- **Swagger UI**: https://localhost:5001 (opens automatically)

---

## 🎨 Frontend Setup (Angular)

### 1. Navigate to Frontend Directory
```bash
cd finance-credit-direct-epay/frontend
```

### 2. Install Dependencies
```bash
npm install
```

This will install all required packages including:
- Angular 17
- Angular Material
- NgRx (Store, Effects, DevTools)
- RxJS

### 3. Update API URL (if needed)
Edit `src/environments/environment.ts` if your backend runs on a different port:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api'  // Update if needed
};
```

### 4. Run the Frontend
```bash
npm start
```

Or using Angular CLI:
```bash
ng serve
```

The frontend will start at:
- **URL**: http://localhost:4200

---

## 🧪 Testing the Application

### 1. Open Browser
Navigate to: http://localhost:4200

### 2. Test Invoice Search
- Enter search criteria (Invoice #, Credit #, PO #, or Date range)
- Click "Search" button
- Verify results are displayed in the table
- Test pagination, sorting, and selection

### 3. Verify API
Open Swagger UI at: https://localhost:5001
- Test `/api/invoice/search` endpoint
- Test `/api/invoice/default-date-span` endpoint

---

## 📁 Project Structure

```
finance-credit-direct-epay/
├── backend/                          # ASP.NET Core 8.0 Web API
│   ├── Controllers/                  # API Controllers
│   │   └── InvoiceController.cs
│   ├── Models/                       # DTOs and Models
│   │   └── DTOs/
│   ├── Repositories/                 # Data Access Layer
│   │   ├── IInvoiceRepository.cs
│   │   └── InvoiceRepository.cs
│   ├── Services/                     # Business Logic Layer
│   │   ├── IInvoiceService.cs
│   │   └── InvoiceService.cs
│   ├── Properties/
│   │   └── launchSettings.json       # Development server config
│   ├── Program.cs                    # Application entry point
│   ├── appsettings.json              # Configuration
│   ├── appsettings.Development.json  # Development config
│   ├── EPay.Api.csproj               # Project file
│   └── EPay.Api.sln                  # Solution file
│
└── frontend/                         # Angular 17 Application
    ├── src/
    │   ├── app/
    │   │   ├── features/
    │   │   │   └── invoices/         # Invoice feature module
    │   │   │       ├── components/   # UI Components
    │   │   │       ├── models/       # TypeScript interfaces
    │   │   │       ├── services/     # HTTP services
    │   │   │       ├── store/        # NgRx state management
    │   │   │       └── invoices.module.ts
    │   │   ├── shared/
    │   │   │   └── components/       # Shared components
    │   │   ├── app.component.ts
    │   │   ├── app.module.ts
    │   │   └── app-routing.module.ts
    │   ├── environments/             # Environment configs
    │   ├── index.html
    │   ├── main.ts
    │   └── styles.scss
    ├── angular.json                  # Angular CLI config
    ├── package.json                  # NPM dependencies
    └── tsconfig.json                 # TypeScript config
```

---

## 🔧 Development Commands

### Backend
```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run

# Run with watch (auto-reload)
dotnet watch run

# Clean
dotnet clean
```

### Frontend
```bash
# Install dependencies
npm install

# Start dev server
npm start

# Build for production
npm run build

# Run tests
npm test

# Lint code
npm run lint
```

---

## 🐛 Troubleshooting

### Backend Issues

**Issue**: Cannot connect to database
- **Solution**: Verify SQL Server is running and connection string is correct
- Check Windows Authentication or SQL Server Authentication settings

**Issue**: Port 5001 already in use
- **Solution**: Change port in `Properties/launchSettings.json`

### Frontend Issues

**Issue**: `npm install` fails
- **Solution**: Delete `node_modules` and `package-lock.json`, then run `npm install` again

**Issue**: CORS errors
- **Solution**: Verify backend CORS policy includes `http://localhost:4200`

**Issue**: API calls fail
- **Solution**: Verify backend is running and `environment.ts` has correct API URL

---

## 📝 Next Steps

1. **Configure Database**: Update connection strings with production values
2. **Add Authentication**: Implement JWT authentication
3. **Write Tests**: Add unit tests and E2E tests
4. **Deploy**: Follow deployment guide in `docs/DEPLOYMENT.md`
5. **Migrate Other Pages**: History, Confirmation, Analyst Report, etc.

---

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Angular Documentation](https://angular.io/docs)
- [NgRx Documentation](https://ngrx.io/docs)
- [Angular Material](https://material.angular.io/)

---

**Migration Status**: ✅ Main Page Complete  
**Ready to Run**: ✅ Yes  
**Last Updated**: 2026-01-23

