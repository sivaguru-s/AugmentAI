using Azure;
using Azure.AI.OpenAI;
using Chatbot_Onbase.Models;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.Json;

namespace Chatbot_Onbase.Services;

/// <summary>
/// Service for interacting with Azure OpenAI to process natural language queries
/// Uses function calling to extract structured parameters from user prompts
/// </summary>
public interface IAzureOpenAIService
{
    Task<QueryIntent> ParseQueryAsync(string userPrompt);
}

public class AzureOpenAIService : IAzureOpenAIService
{
    private readonly AzureOpenAIClient _client;
    private readonly string _deploymentName;
    private readonly int _maxTokens;
    private readonly float _temperature;
    private readonly ILogger<AzureOpenAIService> _logger;

    public AzureOpenAIService(
        IOptions<AzureOpenAISettings> settings,
        ILogger<AzureOpenAIService> logger)
    {
        _logger = logger;
        var config = settings.Value;

        // Validate configuration
        if (string.IsNullOrEmpty(config.Endpoint) || string.IsNullOrEmpty(config.ApiKey))
        {
            throw new ArgumentException("Azure OpenAI Endpoint and ApiKey must be configured in appsettings.json");
        }

        // Initialize Azure OpenAI client
        _client = new AzureOpenAIClient(
            new Uri(config.Endpoint),
            new ApiKeyCredential(config.ApiKey));

        _deploymentName = config.DeploymentName;
        _maxTokens = config.MaxTokens;
        _temperature = (float)config.Temperature;

        _logger.LogInformation("Azure OpenAI Service initialized with deployment: {Deployment}", _deploymentName);
    }

    public async Task<QueryIntent> ParseQueryAsync(string userPrompt)
    {
        try
        {
            _logger.LogInformation("Processing query with Azure OpenAI: {Query}", userPrompt);

            // Get chat client for the deployment
            var chatClient = _client.GetChatClient(_deploymentName);

            // Define the function for extracting invoice search parameters
            var extractParametersFunction = ChatTool.CreateFunctionTool(
                functionName: "extract_invoice_search_parameters",
                functionDescription: "Extracts structured parameters from natural language invoice search queries",
                functionParameters: BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {
                        "intent_type": {
                            "type": "string",
                            "enum": ["SearchByVendor", "AnalyzeByVendor", "SearchByInvoiceNumber", "SearchByPO", "SearchByStatus", "SearchByAmount", "SearchByDate", "GeneralSearch"],
                            "description": "The type of search intent detected"
                        },
                        "vendor_name": {
                            "type": "string",
                            "description": "Vendor or supplier name if mentioned"
                        },
                        "invoice_number": {
                            "type": "string",
                            "description": "Invoice number if specified"
                        },
                        "po_number": {
                            "type": "string",
                            "description": "Purchase order number if specified"
                        },
                        "status": {
                            "type": "string",
                            "enum": ["pending", "approved", "paid", "rejected", "processing", "completed", "cancelled"],
                            "description": "Invoice status if specified"
                        },
                        "min_amount": {
                            "type": "number",
                            "description": "Minimum amount for range search"
                        },
                        "max_amount": {
                            "type": "number",
                            "description": "Maximum amount for range search"
                        },
                        "start_date": {
                            "type": "string",
                            "format": "date",
                            "description": "Start date for date range search (YYYY-MM-DD format)"
                        },
                        "end_date": {
                            "type": "string",
                            "format": "date",
                            "description": "End date for date range search (YYYY-MM-DD format)"
                        },
                        "is_analytics": {
                            "type": "boolean",
                            "description": "True if this is an analytics/summary query, false for simple search"
                        },
                        "document_type": {
                            "type": "string",
                            "enum": ["vendor", "customer", "all"],
                            "description": "Type of invoice document (vendor AP or customer AR)"
                        },
                        "search_term": {
                            "type": "string",
                            "description": "General search term for full-text search"
                        }
                    },
                    "required": ["intent_type"]
                }
                """)
            );

            // Create chat completion options with function calling
            var chatOptions = new ChatCompletionOptions
            {
                MaxOutputTokenCount = _maxTokens,
                Temperature = _temperature
            };
            chatOptions.Tools.Add(extractParametersFunction);

            // Build the conversation messages
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(@"You are an AI assistant that helps extract structured parameters from natural language queries about invoices.

Your task is to analyze the user's query and extract relevant parameters for searching an invoice database.

Guidelines:
- Detect the primary intent (vendor search, date search, amount search, etc.)
- Extract vendor names carefully, preserving the exact spelling and capitalization
- Parse date expressions including relative dates (last month, this year, etc.) and convert to YYYY-MM-DD format
- Identify if the query is for analytics/summary or simple search
- Default to 'vendor' document type unless customer invoices are explicitly mentioned
- For date ranges like '2025/2026', interpret as January 1, 2025 to December 31, 2026
- For 'last month', calculate as 30 days before today
- For 'this month', use the current month start to today"),
                new UserChatMessage(userPrompt)
            };

            // Call Azure OpenAI with function calling
            var response = await chatClient.CompleteChatAsync(messages, chatOptions);

            // Check if the model wants to call the function
            if (response.Value.FinishReason == ChatFinishReason.ToolCalls)
            {
                var toolCall = response.Value.ToolCalls.FirstOrDefault();
                if (toolCall?.FunctionName == "extract_invoice_search_parameters")
                {
                    var functionArguments = toolCall.FunctionArguments;
                    _logger.LogInformation("Function called with arguments: {Arguments}", functionArguments);

                    // Parse the JSON response
                    return ParseFunctionArguments(functionArguments);
                }
            }

            // Fallback if no function call
            _logger.LogWarning("No function call returned, using fallback");
            return CreateFallbackIntent(userPrompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Azure OpenAI");
            return CreateFallbackIntent(userPrompt);
        }
    }

    private QueryIntent ParseFunctionArguments(BinaryData functionArguments)
    {
        var intent = new QueryIntent();

        try
        {
            using var jsonDoc = JsonDocument.Parse(functionArguments);
            var root = jsonDoc.RootElement;

            // Extract intent type
            if (root.TryGetProperty("intent_type", out var intentType))
            {
                intent.IntentType = intentType.GetString() ?? "GeneralSearch";
            }

            // Extract parameters
            var parameters = new Dictionary<string, object>();

            if (root.TryGetProperty("vendor_name", out var vendorName) && vendorName.ValueKind == JsonValueKind.String)
            {
                var vendor = vendorName.GetString();
                if (!string.IsNullOrWhiteSpace(vendor))
                {
                    parameters["vendorName"] = vendor;
                }
            }

            if (root.TryGetProperty("invoice_number", out var invoiceNumber) && invoiceNumber.ValueKind == JsonValueKind.String)
            {
                var invNum = invoiceNumber.GetString();
                if (!string.IsNullOrWhiteSpace(invNum))
                {
                    parameters["invoiceNumber"] = invNum;
                }
            }

            if (root.TryGetProperty("po_number", out var poNumber) && poNumber.ValueKind == JsonValueKind.String)
            {
                var po = poNumber.GetString();
                if (!string.IsNullOrWhiteSpace(po))
                {
                    parameters["poNumber"] = po;
                }
            }

            if (root.TryGetProperty("status", out var status) && status.ValueKind == JsonValueKind.String)
            {
                var stat = status.GetString();
                if (!string.IsNullOrWhiteSpace(stat))
                {
                    parameters["status"] = stat;
                }
            }

            if (root.TryGetProperty("min_amount", out var minAmount) && minAmount.ValueKind == JsonValueKind.Number)
            {
                parameters["minAmount"] = minAmount.GetDecimal();
            }

            if (root.TryGetProperty("max_amount", out var maxAmount) && maxAmount.ValueKind == JsonValueKind.Number)
            {
                parameters["maxAmount"] = maxAmount.GetDecimal();
            }

            if (root.TryGetProperty("start_date", out var startDate) && startDate.ValueKind == JsonValueKind.String)
            {
                var startStr = startDate.GetString();
                if (DateTime.TryParse(startStr, out var start))
                {
                    parameters["startDate"] = start;
                }
            }

            if (root.TryGetProperty("end_date", out var endDate) && endDate.ValueKind == JsonValueKind.String)
            {
                var endStr = endDate.GetString();
                if (DateTime.TryParse(endStr, out var end))
                {
                    parameters["endDate"] = end;
                }
            }

            if (root.TryGetProperty("is_analytics", out var isAnalytics))
            {
                parameters["isAnalytics"] = isAnalytics.ValueKind == JsonValueKind.True;
            }

            if (root.TryGetProperty("document_type", out var docType) && docType.ValueKind == JsonValueKind.String)
            {
                var type = docType.GetString();
                if (!string.IsNullOrWhiteSpace(type))
                {
                    parameters["documentType"] = type;
                }
            }
            else
            {
                // Default to vendor invoices
                parameters["documentType"] = "vendor";
            }

            if (root.TryGetProperty("search_term", out var searchTerm) && searchTerm.ValueKind == JsonValueKind.String)
            {
                var term = searchTerm.GetString();
                if (!string.IsNullOrWhiteSpace(term))
                {
                    parameters["searchTerm"] = term;
                }
            }

            intent.Parameters = parameters;

            _logger.LogInformation("Parsed intent: {Intent} with {Count} parameters",
                intent.IntentType, parameters.Count);

            return intent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing function arguments");
            return CreateFallbackIntent("");
        }
    }

    private QueryIntent CreateFallbackIntent(string userPrompt)
    {
        return new QueryIntent
        {
            IntentType = "GeneralSearch",
            Parameters = new Dictionary<string, object>
            {
                ["searchTerm"] = userPrompt,
                ["documentType"] = "vendor",
                ["isAnalytics"] = false
            }
        };
    }
}
