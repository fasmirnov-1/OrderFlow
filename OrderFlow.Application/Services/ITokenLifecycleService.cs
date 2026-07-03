// Application/Services/ITokenLifecycleService.cs
namespace OrderFlow.Application.Services
{
    public interface ITokenLifecycleService
    {
        Task CreateSessionTokenAsync(string userId, string tokenValue);
        Task<bool> IsTokenValidAsync(string userId, string tokenValue);
        Task InvalidateTokenAsync(string userId);
        Task ClearAllSessionTokensAsync(); // Для вызова при старте приложения
    }
}