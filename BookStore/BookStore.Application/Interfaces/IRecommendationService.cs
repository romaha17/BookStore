using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces;

public interface IRecommendationService
{
    Task<string> GetRecommendationAsync(string userQuery, IEnumerable<BookDto> availableBooks, IEnumerable<string> conversationHistory, CancellationToken ct = default);
}
