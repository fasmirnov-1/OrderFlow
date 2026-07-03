// Application/Services/TokenLifecycleService.cs
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Application.Services
{
    public class TokenLifecycleService : ITokenLifecycleService
    {
        private readonly IAspNetUserTokenRepo _tokenRepo;
        private readonly AppDbContext _context; // Используется для массовой очистки при старте приложения

        // Строгие константы для изоляции сессионных токенов RAUTH
        public const string ProviderName = "Site Session";
        public const string TokenName = "UserSessionToken";

        public TokenLifecycleService(IAspNetUserTokenRepo tokenRepo, AppDbContext context)
        {
            _tokenRepo = tokenRepo ?? throw new ArgumentNullException(nameof(tokenRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateSessionTokenAsync(string userId, string tokenValue)
        {
            // Проверяем, существует ли уже токен сессии для данного пользователя
            var existingToken = await _tokenRepo.ReadAsync(userId, ProviderName, TokenName);

            if (existingToken != null)
            {
                // Если существует, обновляем значение сессии (перезапись токена)
                existingToken.Value = tokenValue;
                await _tokenRepo.UpdateAsync(existingToken);
            }
            else
            {
                // Если токена нет, создаем новую запись через репозиторий
                var newToken = new AspNetUserToken
                {
                    UserId = userId,
                    LoginProvider = ProviderName,
                    Name = TokenName,
                    Value = tokenValue
                };
                await _tokenRepo.CreateAsync(newToken);
            }
        }

        public async Task<bool> IsTokenValidAsync(string userId, string tokenValue)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(tokenValue))
                return false;

            var token = await _tokenRepo.ReadAsync(userId, ProviderName, TokenName);

            // Токен валиден только если он физически существует в БД и совпадает с переданным из куки
            return token != null && token.Value == tokenValue;
        }

        public async Task InvalidateTokenAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;

            // Удаляем токен по составному ключу через репозиторий
            await _tokenRepo.DeleteAsync(userId, ProviderName, TokenName);
        }

        public async Task ClearAllSessionTokensAsync()
        {
            // Массовое удаление всех сессионных токенов при старте приложения 
            // (защита от "повисших" токенов, когда сервер лежал или перезапускался)
            var sessionTokens = _context.AspNetUserTokens
                .Where(t => t.LoginProvider == ProviderName && t.Name == TokenName);

            _context.AspNetUserTokens.RemoveRange(sessionTokens);
            await _context.SaveChangesAsync();
        }
    }
}