# IIS URL Path Fix - Subdirectory Deployment Support

## Problem

When deploying the application to IIS as a virtual directory (e.g., `/chatbotonbase/`), the frontend was using absolute paths like `/api/chatbot/query`, which resulted in 404 errors because the actual API endpoint was at `/chatbotonbase/api/chatbot/query`.

### Error Example:
```
GET http://localhost/api/chatbot/query
404 Not Found
```

**Actual endpoint should be:**
```
GET http://localhost/chatbotonbase/api/chatbot/query
```

## Solution

Changed the frontend to automatically detect the base path and construct relative URLs.

### Before (Broken):
```javascript
const API_URL = '/api/chatbot/query';  // Always uses root path

fetch('/api/chatbot/health');  // Fails in subdirectory
fetch('/api/chatbot/dbtest');  // Fails in subdirectory
fetch('/api/chatbot/query');   // Fails in subdirectory
```

### After (Fixed):
```javascript
// Automatically detect base path
const getBasePath = () => {
    const path = window.location.pathname;
    const basePath = path.substring(0, path.lastIndexOf('/') + 1);
    return basePath;
};

const BASE_PATH = getBasePath();
const API_URL = `${BASE_PATH}api/chatbot/query`;

fetch(`${BASE_PATH}api/chatbot/health`);  // Works everywhere
fetch(`${BASE_PATH}api/chatbot/dbtest`);  // Works everywhere
fetch(`${BASE_PATH}api/chatbot/query`);   // Works everywhere
```

## How It Works

### Root Deployment (`http://localhost/`)
- Current URL: `http://localhost/index.html`
- Detected Base Path: `/`
- API URL: `/api/chatbot/query`
- ✅ Works correctly

### Subdirectory Deployment (`http://localhost/chatbotonbase/`)
- Current URL: `http://localhost/chatbotonbase/index.html`
- Detected Base Path: `/chatbotonbase/`
- API URL: `/chatbotonbase/api/chatbot/query`
- ✅ Works correctly

### Deep Subdirectory (`http://localhost/apps/chatbot/`)
- Current URL: `http://localhost/apps/chatbot/index.html`
- Detected Base Path: `/apps/chatbot/`
- API URL: `/apps/chatbot/api/chatbot/query`
- ✅ Works correctly

## Testing

### Browser Console Output
After the fix, you should see:
```
=== API Connection Test ===
Base Path: /chatbotonbase/
API URL: /chatbotonbase/api/chatbot/query
Current Location: http://localhost/chatbotonbase/index.html
Testing health endpoint...
Health Status: 200
Testing database connection...
DB Test Status: 200
=== All Tests Passed ===
```

### Verify the Fix

1. **Open the application** in your browser
2. **Press F12** to open Developer Tools
3. **Go to Console tab**
4. **Look for "Base Path:"** in the diagnostic output
5. **Verify it matches your deployment path**

### Test All Endpoints

```javascript
// In browser console, verify these URLs are correct:
console.log('Base Path:', BASE_PATH);
console.log('API URL:', API_URL);
console.log('Health:', `${BASE_PATH}api/chatbot/health`);
console.log('DB Test:', `${BASE_PATH}api/chatbot/dbtest`);
```

## IIS Configuration

This fix works with any IIS configuration:

### Option 1: Root Application
- IIS Site: Default Web Site
- Application Path: `/`
- URL: `http://localhost/`
- ✅ Works

### Option 2: Virtual Directory
- IIS Site: Default Web Site
- Virtual Directory: `chatbotonbase`
- URL: `http://localhost/chatbotonbase/`
- ✅ Works

### Option 3: Application under Site
- IIS Site: Default Web Site
- Application: `chatbotonbase`
- URL: `http://localhost/chatbotonbase/`
- ✅ Works

## No Configuration Needed

The frontend now automatically adapts to any deployment path. You don't need to:
- ❌ Modify `index.html` for different deployments
- ❌ Set environment variables
- ❌ Configure base paths in settings
- ❌ Change API URLs manually

## Troubleshooting

### If you still get 404 errors:

1. **Check Browser Console:**
   ```
   Look for: "Base Path: /your-path/"
   Verify it matches your IIS application path
   ```

2. **Check Network Tab (F12):**
   ```
   Look at the actual request URL
   Should be: http://localhost/your-path/api/chatbot/query
   ```

3. **Verify IIS Application Path:**
   ```powershell
   # In IIS Manager, check:
   # - Site name
   # - Application/Virtual Directory name
   # - Physical path
   ```

4. **Test Endpoints Directly:**
   ```powershell
   # Replace 'chatbotonbase' with your actual path
   Invoke-WebRequest -Uri "http://localhost/chatbotonbase/api/chatbot/health"
   ```

## Benefits

✅ **Works in any deployment scenario**
✅ **No manual configuration required**
✅ **Automatic path detection**
✅ **Supports nested paths**
✅ **No code changes needed for different environments**
✅ **Backward compatible with root deployments**

## Files Changed

- ✅ `wwwroot/index.html` - Added `getBasePath()` function and updated all API calls

## Deployment Instructions

1. **Build the application:**
   ```powershell
   dotnet publish -c Release -o ./publish
   ```

2. **Copy to IIS:**
   ```powershell
   Copy-Item -Path ./publish/* -Destination "C:\inetpub\wwwroot\ChatbotOnbase\" -Recurse -Force
   ```

3. **Open in browser:**
   ```
   http://localhost/chatbotonbase/
   ```

4. **Verify in console:**
   - Check "Base Path" matches your deployment
   - All diagnostic tests should pass

That's it! The application will automatically detect and use the correct paths. 🎉

