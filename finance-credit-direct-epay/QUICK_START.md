# Quick Start Guide - EPay Backend

**5-Minute Setup for Local Development**

---

## ⚡ Quick Setup (5 Steps)

### 1️⃣ Install .NET 8.0 SDK

Download: https://dotnet.microsoft.com/download/dotnet/8.0

Verify:
```powershell
dotnet --version
# Should show: 8.0.x
```

---

### 2️⃣ Configure Database Connection

Edit: `backend/appsettings.Development.json`

Replace `YOUR_SERVER_NAME` with your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "AFI_Batch": "Server=localhost;Database=Datawhse;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

**Common server names:**
- `localhost` - Local SQL Server
- `(localdb)\MSSQLLocalDB` - LocalDB
- `YOUR_PC_NAME\SQLEXPRESS` - SQL Express

---

### 3️⃣ Navigate to Backend Directory

```powershell
cd C:\AugmentAI\finance-credit-direct-epay\backend
```

---

### 4️⃣ Restore & Build

```powershell
dotnet restore
dotnet build
```

---

### 5️⃣ Run the Application

```powershell
dotnet run
```

**Expected Output:**
```
Now listening on: https://localhost:5001
Now listening on: http://localhost:5000
```

---

## 🌐 Access Swagger UI

Open browser: **https://localhost:5001**

You should see the API documentation with 3 endpoints.

---

## ✅ Test the API

In Swagger UI:

1. Click **GET /api/invoice/default-date-span**
2. Click **Try it out**
3. Click **Execute**

**Expected:** Returns a number (e.g., 90)

---

## 🔧 Visual Studio 2022 (Alternative)

1. Open Visual Studio 2022
2. File → Open → Project/Solution
3. Select: `backend/EPay.Api.sln`
4. Press **F5**

---

## ❌ Common Issues

### "Connection string not found"
→ Add `AFI_Batch` to `appsettings.Development.json`

### "Cannot connect to SQL Server"
→ Check SQL Server is running: `services.msc`

### "Port already in use"
→ Stop other applications using port 5001

### "SSL certificate error"
→ Run: `dotnet dev-certs https --trust`

---

## 📁 Key Files

| File | Purpose |
|------|---------|
| `appsettings.Development.json` | Database connection strings |
| `Program.cs` | Application startup |
| `Controllers/InvoiceController.cs` | API endpoints |
| `EPay.Api.csproj` | Project configuration |

---

## 🚀 Next Steps

After backend is running:

1. ✅ Test endpoints in Swagger
2. ✅ Set up Angular frontend
3. ✅ Test full application

---

**Need detailed help?** See `BACKEND_SETUP_GUIDE.md`

