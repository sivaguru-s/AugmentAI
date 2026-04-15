# Azure OpenAI Setup Guide

This guide explains how to create an Azure OpenAI resource, deploy a GPT model, and generate API keys for the Invoice Chatbot application.

---

## Prerequisites

- Azure subscription with access to Azure OpenAI Service
- Appropriate permissions to create resources in Azure
- Azure CLI installed (optional, for CLI-based setup)

---

## Step 1: Create Azure OpenAI Resource

### Using Azure Portal

1. **Navigate to Azure Portal**
   - Go to [https://portal.azure.com](https://portal.azure.com)
   - Sign in with your credentials

2. **Create Resource**
   - Click "+ Create a resource"
   - Search for "Azure OpenAI"
   - Click "Create"

3. **Configure Resource**
   - **Subscription**: Select your subscription
   - **Resource Group**: Create new or select existing (e.g., `rg-chatbot-onbase`)
   - **Region**: Choose a supported region (e.g., `East US`, `West Europe`)
   - **Name**: Enter a unique name (e.g., `ashley-invoice-chatbot-openai`)
   - **Pricing Tier**: Select Standard S0

4. **Review and Create**
   - Review your settings
   - Click "Create"
   - Wait for deployment to complete (usually 1-2 minutes)

### Using Azure CLI

```bash
# Login to Azure
az login

# Create resource group
az group create --name rg-chatbot-onbase --location eastus

# Create Azure OpenAI resource
az cognitiveservices account create \
  --name ashley-invoice-chatbot-openai \
  --resource-group rg-chatbot-onbase \
  --kind OpenAI \
  --sku S0 \
  --location eastus \
  --yes
```

---

## Step 2: Deploy a GPT Model

### Using Azure Portal

1. **Navigate to Azure OpenAI Studio**
   - Go to your Azure OpenAI resource
   - Click "Go to Azure OpenAI Studio"
   - Or visit [https://oai.azure.com](https://oai.azure.com)

2. **Create Deployment**
   - Click "Deployments" in the left menu
   - Click "+ Create new deployment"

3. **Configure Deployment**
   - **Model**: Select `gpt-4` or `gpt-35-turbo`
     - **gpt-4**: More accurate, higher cost, better for complex queries
     - **gpt-35-turbo**: Faster, lower cost, good for most queries
   - **Deployment Name**: Enter name (e.g., `gpt-4` or `invoice-chatbot`)
   - **Model Version**: Latest (auto-update enabled)
   - **Deployment Type**: Standard
   - **Tokens per Minute Rate Limit**: 10K (adjust based on usage)

4. **Deploy**
   - Click "Create"
   - Wait for deployment (usually instant)
   - Note down the **Deployment Name** (you'll need this for configuration)

### Using Azure CLI

```bash
# Deploy GPT-4 model
az cognitiveservices account deployment create \
  --name ashley-invoice-chatbot-openai \
  --resource-group rg-chatbot-onbase \
  --deployment-name gpt-4 \
  --model-name gpt-4 \
  --model-version "0613" \
  --model-format OpenAI \
  --sku-name "Standard" \
  --sku-capacity 10
```

---

## Step 3: Retrieve API Key and Endpoint

### Using Azure Portal

1. **Navigate to Resource**
   - Go to your Azure OpenAI resource in Azure Portal
   - Or from Azure OpenAI Studio, click "Settings" → "Resource"

2. **Get Endpoint**
   - Copy the **Endpoint** URL
   - Example: `https://ashley-invoice-chatbot-openai.openai.azure.com/`

3. **Get API Keys**
   - Click "Keys and Endpoint" in the left menu
   - Copy **KEY 1** or **KEY 2**
   - **Important**: Keep these keys secure!

### Using Azure CLI

```bash
# Get endpoint
az cognitiveservices account show \
  --name ashley-invoice-chatbot-openai \
  --resource-group rg-chatbot-onbase \
  --query "properties.endpoint" -o tsv

# Get API keys
az cognitiveservices account keys list \
  --name ashley-invoice-chatbot-openai \
  --resource-group rg-chatbot-onbase
```

---

## Step 4: Configure the Application

### Option 1: appsettings.json (Development Only)

**⚠️ WARNING**: Do NOT commit API keys to source control!

Edit `appsettings.json`:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://ashley-invoice-chatbot-openai.openai.azure.com/",
    "ApiKey": "YOUR_API_KEY_HERE",
    "DeploymentName": "gpt-4",
    "MaxTokens": 1500,
    "Temperature": 0.7
  }
}
```

### Option 2: User Secrets (Recommended for Development)

```bash
# Initialize user secrets
dotnet user-secrets init

# Set Azure OpenAI configuration
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://ashley-invoice-chatbot-openai.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "YOUR_API_KEY_HERE"
dotnet user-secrets set "AzureOpenAI:DeploymentName" "gpt-4"
dotnet user-secrets set "AzureOpenAI:MaxTokens" "1500"
dotnet user-secrets set "AzureOpenAI:Temperature" "0.7"
```

### Option 3: Azure Key Vault (Recommended for Production)

```bash
# Create Key Vault
az keyvault create \
  --name ashley-chatbot-kv \
  --resource-group rg-chatbot-onbase \
  --location eastus

# Store secrets
az keyvault secret set \
  --vault-name ashley-chatbot-kv \
  --name "AzureOpenAI--Endpoint" \
  --value "https://ashley-invoice-chatbot-openai.openai.azure.com/"

az keyvault secret set \
  --vault-name ashley-chatbot-kv \
  --name "AzureOpenAI--ApiKey" \
  --value "YOUR_API_KEY_HERE"

az keyvault secret set \
  --vault-name ashley-chatbot-kv \
  --name "AzureOpenAI--DeploymentName" \
  --value "gpt-4"
```

Then update `Program.cs` to use Key Vault:

```csharp
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault
var keyVaultName = builder.Configuration["KeyVaultName"];
if (!string.IsNullOrEmpty(keyVaultName))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential());
}
```

### Option 4: Environment Variables

```bash
# Windows PowerShell
$env:AzureOpenAI__Endpoint = "https://ashley-invoice-chatbot-openai.openai.azure.com/"
$env:AzureOpenAI__ApiKey = "YOUR_API_KEY_HERE"
$env:AzureOpenAI__DeploymentName = "gpt-4"

# Linux/Mac
export AzureOpenAI__Endpoint="https://ashley-invoice-chatbot-openai.openai.azure.com/"
export AzureOpenAI__ApiKey="YOUR_API_KEY_HERE"
export AzureOpenAI__DeploymentName="gpt-4"
```

---

## Step 5: Test the Configuration

### 1. Build and Run

```bash
cd C:\Chatbot-Onbase
dotnet build
dotnet run
```

### 2. Test with Sample Query

Using PowerShell:

```powershell
$body = @{
    prompt = "fetch invoices for vendor SUPREME GRAPHICS from last month"
} | ConvertTo-Json

Invoke-RestMethod `
  -Uri "http://localhost:5000/api/chatbot/query" `
  -Method POST `
  -ContentType "application/json" `
  -Body $body
```

Using curl:

```bash
curl -X POST http://localhost:5000/api/chatbot/query \
  -H "Content-Type: application/json" \
  -d '{"prompt":"fetch invoices for vendor GOOGLE to see 2025/2026 invoices"}'
```

### 3. Expected Response

```json
{
  "answer": "I found 15 invoice(s) matching your query...",
  "invoices": [
    {
      "invoiceId": 123456,
      "vendorName": "SUPREME GRAPHICS",
      "amount": 12500.00,
      ...
    }
  ],
  "query": "fetch invoices for vendor SUPREME GRAPHICS from last month",
  "resultCount": 15
}
```

---

## Step 6: Monitor Usage and Costs

### Azure Portal Monitoring

1. **Navigate to Azure OpenAI Resource**
2. **View Metrics**
   - Total Calls
   - Generated Tokens
   - Processed Prompts
3. **Cost Management**
   - Click "Cost Management + Billing"
   - View resource costs
   - Set up budget alerts

### Pricing Estimates (as of 2026)

| Model | Input (per 1K tokens) | Output (per 1K tokens) | Typical Cost per Query |
|-------|----------------------|------------------------|----------------------|
| GPT-4 | $0.03 | $0.06 | $0.01 - $0.05 |
| GPT-3.5-Turbo | $0.0015 | $0.002 | $0.001 - $0.005 |

**Note**: Actual costs vary based on query complexity and response length.

---

## Troubleshooting

### Error: "Azure OpenAI Endpoint and ApiKey must be configured"

**Solution**: Verify that the configuration is properly set in one of the methods above.

```bash
# Check if secrets are set
dotnet user-secrets list
```

### Error: "The API deployment for this resource does not exist"

**Solution**:
- Verify the deployment name matches exactly
- Ensure the deployment is in "Succeeded" status
- Check region availability

### Error: "429 Too Many Requests"

**Solution**:
- Check your tokens-per-minute limit
- Increase the limit in deployment settings
- Implement retry logic with exponential backoff

### Error: "401 Unauthorized"

**Solution**:
- Verify API key is correct
- Regenerate API key if compromised
- Check that the key hasn't expired

---

## Security Best Practices

1. **Never Commit API Keys**
   - Add `appsettings.json` to `.gitignore` if it contains keys
   - Use User Secrets for development
   - Use Key Vault for production

2. **Rotate Keys Regularly**
   - Regenerate keys every 90 days
   - Use Key 1, rotate Key 2, then switch

3. **Restrict Network Access**
   - Configure firewall rules
   - Use Private Endpoints if possible
   - Implement IP allowlisting

4. **Monitor Access**
   - Enable diagnostic logging
   - Set up alerts for unusual activity
   - Review access logs regularly

---

## Comparison: Azure OpenAI vs Pattern-Based NLP

| Feature | Pattern-Based NLP | Azure OpenAI |
|---------|------------------|--------------|
| **Accuracy** | 70-80% | 95-99% |
| **Setup Complexity** | Low | Medium |
| **Ongoing Cost** | $0 | ~$50-200/month |
| **Flexibility** | Limited patterns | Understands context |
| **Maintenance** | High (regex updates) | Low (model learns) |
| **Offline Capability** | Yes | No (requires API) |
| **Complex Queries** | Poor | Excellent |
| **Speed** | <50ms | 200-1000ms |

---

## Migration Benefits

### Before (Pattern-Based):
```
User: "show me invoices from GOOGLE last month over 10000"
Intent: PARTIAL MATCH - May miss "over 10000"
```

### After (Azure OpenAI):
```
User: "show me invoices from GOOGLE last month over 10000"
Intent: COMPLETE UNDERSTANDING
- Vendor: GOOGLE
- Date: Last 30 days
- Amount: > $10,000
```

---

## Next Steps

1. ✅ Create Azure OpenAI resource
2. ✅ Deploy GPT model
3. ✅ Configure API keys
4. ✅ Test the application
5. 📊 Monitor usage and optimize
6. 🔒 Implement security best practices
7. 📈 Scale based on user feedback

---

**Copyright © 2025 Ashley Furniture Industries, Inc. All rights reserved.**
This software is proprietary and confidential. Unauthorized copying, distribution, or use of this software, via any medium, is strictly prohibited.
