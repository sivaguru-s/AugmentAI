using IncidentResolution.Core.Interfaces;
using IncidentResolution.Core.Models;
using System.Text.RegularExpressions;

namespace IncidentResolution.API.Services;

/// <summary>
/// Local keyword-based recommendation service - no external API required
/// Uses TF-IDF style similarity scoring to match incidents
/// </summary>
public class KeywordMatchingService : IAIRecommendationService
{
    private readonly ILogger<KeywordMatchingService> _logger;

    // Common stop words to ignore in matching
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a", "an", "the", "is", "are", "was", "were", "be", "been", "being",
        "have", "has", "had", "do", "does", "did", "will", "would", "could",
        "should", "may", "might", "must", "shall", "can", "need", "dare",
        "to", "of", "in", "for", "on", "with", "at", "by", "from", "as",
        "into", "through", "during", "before", "after", "above", "below",
        "between", "under", "again", "further", "then", "once", "here",
        "there", "when", "where", "why", "how", "all", "each", "few", "more",
        "most", "other", "some", "such", "no", "nor", "not", "only", "own",
        "same", "so", "than", "too", "very", "just", "and", "but", "if", "or",
        "because", "until", "while", "this", "that", "these", "those", "it",
        "its", "i", "me", "my", "we", "our", "you", "your", "he", "him", "his",
        "she", "her", "they", "them", "their", "what", "which", "who", "whom"
    };

    public KeywordMatchingService(ILogger<KeywordMatchingService> logger)
    {
        _logger = logger;
    }

    public Task<List<Resolution>> GetRecommendationsAsync(
        string incidentDescription,
        List<Incident> historicalIncidents,
        int topN = 3)
    {
        _logger.LogInformation("Analyzing {Count} historical incidents for matches", historicalIncidents.Count);

        // Extract keywords from the input description
        var queryKeywords = ExtractKeywords(incidentDescription);
        _logger.LogDebug("Extracted keywords: {Keywords}", string.Join(", ", queryKeywords));

        // Score each historical incident
        var scoredIncidents = historicalIncidents
            .Where(i => !string.IsNullOrWhiteSpace(i.ResolutionNotes))
            .Select(incident => new
            {
                Incident = incident,
                Score = CalculateRelevanceScore(queryKeywords, incident)
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(topN)
            .ToList();

        // Convert to resolutions
        var resolutions = scoredIncidents.Select((item, index) => new Resolution
        {
            Rank = index + 1,
            Title = GenerateTitle(item.Incident),
            Description = item.Incident.ShortDescription,
            Steps = ParseResolutionSteps(item.Incident.ResolutionNotes),
            ConfidenceScore = Math.Min(item.Score, 1.0),
            SourceIncidentNumber = item.Incident.Number,
            Category = item.Incident.Category
        }).ToList();

        _logger.LogInformation("Found {Count} matching resolutions", resolutions.Count);

        return Task.FromResult(resolutions);
    }

    /// <summary>
    /// Extract meaningful keywords from text
    /// </summary>
    private HashSet<string> ExtractKeywords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new HashSet<string>();

        // Normalize and tokenize
        var normalized = text.ToLowerInvariant();
        var words = Regex.Split(normalized, @"[\s\p{P}]+")
            .Where(w => w.Length > 2)
            .Where(w => !StopWords.Contains(w))
            .Where(w => !Regex.IsMatch(w, @"^\d+$")); // Exclude pure numbers

        return new HashSet<string>(words, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Calculate relevance score using keyword matching with weighting
    /// </summary>
    private double CalculateRelevanceScore(HashSet<string> queryKeywords, Incident incident)
    {
        if (!queryKeywords.Any())
            return 0;

        // Combine incident text fields with weights
        var shortDescKeywords = ExtractKeywords(incident.ShortDescription);
        var descKeywords = ExtractKeywords(incident.Description);
        var resolutionKeywords = ExtractKeywords(incident.ResolutionNotes);
        var categoryKeywords = ExtractKeywords(incident.Category);

        double score = 0;

        // Weighted matching: short description is most important
        var shortDescMatches = queryKeywords.Intersect(shortDescKeywords).Count();
        var descMatches = queryKeywords.Intersect(descKeywords).Count();
        var resolutionMatches = queryKeywords.Intersect(resolutionKeywords).Count();
        var categoryMatches = queryKeywords.Intersect(categoryKeywords).Count();

        // Weight: short desc (3x), description (2x), resolution (1.5x), category (2x)
        score += shortDescMatches * 3.0;
        score += descMatches * 2.0;
        score += resolutionMatches * 1.5;
        score += categoryMatches * 2.0;

        // Normalize by query size
        var normalizedScore = score / (queryKeywords.Count * 3.0);

        // Boost if resolution notes are substantial
        if (incident.ResolutionNotes?.Length > 50)
            normalizedScore *= 1.2;

        return Math.Round(normalizedScore, 2);
    }

    /// <summary>
    /// Generate a descriptive title for the resolution
    /// </summary>
    private string GenerateTitle(Incident incident)
    {
        var title = incident.ShortDescription;

        // Truncate if too long
        if (title.Length > 80)
            title = title.Substring(0, 77) + "...";

        // Add category prefix if available
        if (!string.IsNullOrEmpty(incident.Category))
            return $"[{incident.Category}] {title}";

        return title;
    }

    /// <summary>
    /// Parse resolution notes into steps
    /// </summary>
    private List<string> ParseResolutionSteps(string resolutionNotes)
    {
        if (string.IsNullOrWhiteSpace(resolutionNotes))
            return new List<string> { "No resolution steps documented." };

        var steps = new List<string>();

        // Try to split by common step patterns
        var lines = resolutionNotes
            .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        if (lines.Count > 1)
        {
            // Multiple lines - treat each as a step
            foreach (var line in lines)
            {
                // Remove numbering if present
                var step = Regex.Replace(line, @"^[\d]+[\.\)]\s*", "");
                if (!string.IsNullOrWhiteSpace(step))
                    steps.Add(step);
            }
        }
        else
        {
            // Single block - try to split by sentences or numbered items
            var text = resolutionNotes;

            // Check for numbered steps (1. 2. 3. or 1) 2) 3))
            var numberedSteps = Regex.Split(text, @"(?=\d+[\.\)]\s)");
            if (numberedSteps.Length > 1)
            {
                foreach (var step in numberedSteps)
                {
                    var cleaned = Regex.Replace(step.Trim(), @"^[\d]+[\.\)]\s*", "");
                    if (!string.IsNullOrWhiteSpace(cleaned))
                        steps.Add(cleaned);
                }
            }
            else
            {
                // Just use the whole text as one step
                steps.Add(resolutionNotes.Trim());
            }
        }

        return steps.Any() ? steps : new List<string> { resolutionNotes.Trim() };
    }
}
