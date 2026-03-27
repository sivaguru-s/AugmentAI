# Authentication Setup Guide

This guide explains how to enable authentication for the EPay API.

---

## Current Status

✅ **Authentication is currently DISABLED for development**

The `[Authorize]` attribute is commented out in `InvoiceController.cs` to allow testing without authentication.

---

## Why Authentication is Disabled

The original code expects:
- JWT (JSON Web Token) authentication
- User claims (CustomerNumber, ShipToNumber, etc.)
- Role-based authorization (EPAYANLYST role)

However, authentication is not yet configured in `Program.cs`.

---

## Option 1: Keep Authentication Disabled (Current Setup)

**Best for:** Local development and testing

**Status:** ✅ Already configured

The `[Authorize]` attribute is commented out, so you can test the API without authentication.

**Note:** You'll need to manually provide customer data in API requests since there are no user claims.

---

## Option 2: Enable Windows Authentication

**Best for:** Internal corporate network with Active Directory

### Step 1: Update Program.cs

Add this code after `builder.Services.AddControllers();`:

```csharp
// Add Windows Authentication
builder.Services.AddAuthentication(Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme);
```

### Step 2: Update launchSettings.json

Add to the profile:

```json
{
  "iisSettings": {
    "windowsAuthentication": true,
    "anonymousAuthentication": false
  }
}
```

### Step 3: Uncomment [Authorize] attribute

In `InvoiceController.cs`, uncomment:

```csharp
[Authorize] // Requires Windows authentication
```

---

## Option 3: Enable JWT Authentication

**Best for:** Modern web applications with token-based auth

### Step 1: Install NuGet Package

```powershell
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

### Step 2: Update appsettings.json

Add JWT configuration:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "EPay.Api",
    "Audience": "EPay.Client",
    "ExpiryInMinutes": 60
  }
}
```

### Step 3: Update Program.cs

Add this code after `builder.Services.AddControllers();`:

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
```

Add this before `app.MapControllers();`:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

### Step 4: Create Login Endpoint

Create a new controller `AuthController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EPay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // TODO: Validate credentials against database
            // For now, accept any credentials for testing

            var claims = new[]
            {
                new Claim("CustomerNumber", request.CustomerNumber),
                new Claim("ShipToNumber", request.ShipToNumber ?? ""),
                new Claim("AllShipTos", request.AllShipTos.ToString()),
                new Claim("SecurityMHS", request.SecurityMHS ?? ""),
                new Claim(ClaimTypes.Role, "EPAYANLYST")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    int.Parse(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
    }

    public class LoginRequest
    {
        public string CustomerNumber { get; set; }
        public string ShipToNumber { get; set; }
        public bool AllShipTos { get; set; }
        public string SecurityMHS { get; set; }
    }
}
```

### Step 5: Uncomment [Authorize] attribute

In `InvoiceController.cs`, uncomment:

```csharp
[Authorize] // Requires JWT authentication
```

---

## Testing with Authentication Enabled

### With JWT:

1. Call `POST /api/auth/login` to get a token
2. Copy the token
3. In Swagger UI, click **Authorize** button
4. Enter: `Bearer YOUR_TOKEN_HERE`
5. Now you can call protected endpoints

---

## Recommendation

**For Development:** Keep authentication disabled (current setup)  
**For Production:** Use Windows Authentication or JWT with proper user validation

---

**Last Updated:** 2026-01-25

