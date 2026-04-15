namespace Chatbot_Onbase.Models;

/// <summary>
/// Configuration settings for Azure OpenAI service
/// </summary>
public class AzureOpenAISettings
{
    /// <summary>
    /// Azure OpenAI endpoint URL
    /// Example: https://your-resource-name.openai.azure.com/
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Azure OpenAI API Key for authentication
    /// Store securely in Azure Key Vault or User Secrets in production
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Deployment name for the GPT model
    /// Example: gpt-4, gpt-35-turbo
    /// </summary>
    public string DeploymentName { get; set; } = string.Empty;

    /// <summary>
    /// Maximum number of tokens to generate in the response
    /// Default: 1500
    /// </summary>
    public int MaxTokens { get; set; } = 1500;

    /// <summary>
    /// Temperature for response generation (0.0 - 1.0)
    /// Lower values = more focused and deterministic
    /// Higher values = more creative and varied
    /// Default: 0.7
    /// </summary>
    public double Temperature { get; set; } = 0.7;
}
