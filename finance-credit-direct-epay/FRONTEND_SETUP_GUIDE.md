# Frontend Setup Guide - EPay Angular Application

Complete step-by-step guide to configure and run the Angular 17 frontend on your local machine.

---

## 📋 Prerequisites

### Required Software

1. **Node.js 18.x or 20.x** - [Download](https://nodejs.org/en/download/)
2. **npm** (comes with Node.js)
3. **Angular CLI 17** (will be installed)
4. **Visual Studio Code** (recommended) - [Download](https://code.visualstudio.com/)

### Backend Requirements

⚠️ **Important:** The backend API must be running before starting the frontend.

- Backend URL: `https://localhost:5001`
- See `BACKEND_SETUP_GUIDE.md` for backend setup

---

## 🚀 Step-by-Step Setup

### Step 1: Verify Node.js Installation

Open PowerShell or Command Prompt:

```powershell
node --version
```

**Expected Output:** `v18.x.x` or `v20.x.x`

```powershell
npm --version
```

**Expected Output:** `9.x.x` or `10.x.x`

If not installed, download from: https://nodejs.org/

---

### Step 2: Navigate to Frontend Directory

```powershell
cd C:\AugmentAI\finance-credit-direct-epay\frontend
```

---

### Step 3: Install Dependencies

```powershell
npm install
```

**Expected Output:**
```
added XXX packages in XXs
```

This will install:
- Angular 17 framework
- Angular Material UI components
- NgRx state management
- RxJS reactive extensions
- TypeScript compiler

**Note:** First installation may take 2-5 minutes.

---

### Step 4: Install Angular CLI (if not installed)

```powershell
npm install -g @angular/cli@17
```

Verify installation:

```powershell
ng version
```

**Expected Output:**
```
Angular CLI: 17.x.x
Node: 18.x.x or 20.x.x
Package Manager: npm 9.x.x or 10.x.x
```

---

### Step 5: Start the Development Server

```powershell
npm start
```

Or use Angular CLI directly:

```powershell
ng serve
```

**Expected Output:**
```
Initial chunk files | Names         | Raw size
vendor.js          | vendor        | X.XX MB
polyfills.js       | polyfills     | XX.XX kB
styles.css         | styles        | XX.XX kB
main.js            | main          | XX.XX kB

Application bundle generation complete.

Watch mode enabled. Watching for file changes...
  ➜  Local:   http://localhost:4200/
```

---

### Step 6: Access the Application

Open your browser and navigate to:

**http://localhost:4200**

You should see the EPay application with the Ashley Direct branding.

---

## 🌐 Environment Configuration

### Development Environment

**File:** `src/environments/environment.ts`

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api'
};
```

### Production Environment

**File:** `src/environments/environment.prod.ts`

Update the API URL for production deployment.

---

## 🔧 Visual Studio Code Setup (Recommended)

### Open Project in VS Code

```powershell
code C:\AugmentAI\finance-credit-direct-epay\frontend
```

### Recommended Extensions

1. **Angular Language Service** - Angular support
2. **ESLint** - Code linting
3. **Prettier** - Code formatting
4. **Material Icon Theme** - File icons

---

## ✅ Verify Installation

### Test 1: Application Loads

Navigate to http://localhost:4200

You should see:
- Ashley Direct header with orange branding
- Navigation tabs
- Invoice search form

### Test 2: API Connection

1. Open browser DevTools (F12)
2. Go to Network tab
3. Perform a search
4. Verify API calls to `https://localhost:5001/api/invoice/search`

---

## ❌ Troubleshooting

### Issue 1: "npm: command not found"

**Solution:** Install Node.js from https://nodejs.org/

### Issue 2: "ng: command not found"

**Solution:** Install Angular CLI globally:
```powershell
npm install -g @angular/cli@17
```

### Issue 3: "ENOENT: no such file or directory"

**Solution:** Run `npm install` first to install dependencies.

### Issue 4: Port 4200 Already in Use

**Solution:** Use a different port:
```powershell
ng serve --port 4300
```

### Issue 5: CORS Error

**Cause:** Backend not running or CORS not configured.

**Solution:**
1. Make sure backend is running on https://localhost:5001
2. Backend CORS is configured to allow http://localhost:4200

### Issue 6: SSL Certificate Error

**Solution:** Access the backend URL first and accept the certificate:
1. Open https://localhost:5001 in browser
2. Click "Advanced" → "Proceed to localhost"
3. Then access the frontend

---

## 📁 Project Structure

```
frontend/
├── src/
│   ├── app/
│   │   ├── features/
│   │   │   └── invoices/          # Invoice feature module
│   │   ├── shared/
│   │   │   └── components/        # Shared components
│   │   ├── app.component.ts       # Root component
│   │   ├── app.module.ts          # Root module
│   │   └── app-routing.module.ts  # Routing configuration
│   ├── environments/
│   │   ├── environment.ts         # Development config
│   │   └── environment.prod.ts    # Production config
│   ├── index.html                 # Main HTML file
│   ├── main.ts                    # Application entry point
│   └── styles.scss                # Global styles
├── angular.json                   # Angular CLI configuration
├── package.json                   # Dependencies
├── tsconfig.json                  # TypeScript configuration
└── tsconfig.app.json              # App-specific TS config
```

---

## 🛠️ Useful Commands

```powershell
# Start development server
npm start

# Build for production
npm run build

# Run tests
npm test

# Lint code
npm run lint

# Generate component
ng generate component features/my-feature

# Generate service
ng generate service services/my-service
```

---

## 🚀 Next Steps

After frontend is running:

1. ✅ Verify backend is running (https://localhost:5001)
2. ✅ Test invoice search functionality
3. ✅ Check browser console for errors
4. ✅ Test API integration

---

**Last Updated:** 2026-01-27  
**Version:** 1.0  
**Framework:** Angular 17

