using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Application.Services
{
    public class SessionCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SessionCleanupService> _logger;

        // Интервал проверки (например, запускать каждые 12 часов)
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(12);

        public SessionCleanupService(IServiceProvider serviceProvider, ILogger<SessionCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Фоновый сервис очистки сессий запущен.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanExpiredSessionsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Произошла ошибка при фоновой очистке устаревших сессий.");
                }

                // Ожидание перед следующей проверкой
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Фоновый сервис очистки сессий остановлен.");
        }

        private async Task CleanExpiredSessionsAsync()
        {
            // Создаем область видимости (Scope), так как DbContext зарегистрирован как Scoped
            using var scope = _serviceProvider.CreateScope();
            var sessionRepo = scope.ServiceProvider.GetRequiredService<IUserSessionsRepo>();

            // Метод поиска устаревших сессий и их удаления
            // (предполагается, что в IUserSessionsRepo добавлены соответствующие методы)
            var expiredSessions = await sessionRepo.GetExpiredSessionsAsync(DateTime.UtcNow);

            if (expiredSessions.Any())
            {
                foreach (var session in expiredSessions)
                {
                    sessionRepo.Remove(session);
                }

                await sessionRepo.SaveChangesAsync();
                _logger.LogInformation("Успешно удалено устаревших сессий: {Count}", expiredSessions.Count());
            }
        }
    }
}
