using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace BookStore.Infrastructure.Services;

public class OpenAiRecommendationService : IRecommendationService
{
    private readonly OpenAISettings _settings;
    private readonly ILogger<OpenAiRecommendationService> _logger;

    public OpenAiRecommendationService(IOptions<OpenAISettings> options, ILogger<OpenAiRecommendationService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<string> GetRecommendationAsync(string userQuery, IEnumerable<BookDto> availableBooks, IEnumerable<string> conversationHistory, CancellationToken ct = default)
    {
        var client = new ChatClient(_settings.Model, _settings.OpenAIKey);

        var bookSummary = string.Join("\n", availableBooks.Select(b =>
            $"{b.BookName} by {string.Join(", ", b.Authors.Select(a => $"{a.FirstName} {a.LastName}"))} - {b.Genre}, {b.AvailableLanguage}, ${b.Price}"));

        var messages = new List<ChatMessage>
        {
            new UserChatMessage("You are a helpful assistant that recommends books based on user preferences."),
            new UserChatMessage($"Available books in the store:\n{bookSummary}")
        };

        foreach (var historyMessage in conversationHistory)
            messages.Add(new UserChatMessage(historyMessage));

        messages.Add(new UserChatMessage(userQuery));

        var result = await client.CompleteChatAsync(messages, cancellationToken: ct);

        if (result?.Value.Content is { Count: > 0 })
            return result.Value.Content[0].Text ?? "No response";

        return "No response received from assistant.";
    }
}
