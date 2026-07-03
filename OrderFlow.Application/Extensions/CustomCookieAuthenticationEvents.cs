// Application/Extensions/CustomCookieAuthenticationEvents.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Services;
using System.Security.Claims;

namespace OrderFlow.Application.Extensions
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly IServiceProvider _serviceProvider;

        public CustomCookieAuthenticationEvents(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;

            // Извлекаем уникальный ID пользователя и значение токена из Claims куки
            var userId = userPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tokenValue = userPrincipal?.FindFirst("SessionToken")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tokenValue))
            {
                await RejectAndSignOutAsync(context);
                return;
            }

            // Разрешаем Scoped-сервис внутри жизненного цикла куки
            using var scope = _serviceProvider.CreateScope();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenLifecycleService>();

            // 1. Проверяем, не истекло ли время самой куки на клиенте
            if (context.Properties.ExpiresUtc <= DateTimeOffset.UtcNow)
            {
                await tokenService.InvalidateTokenAsync(userId);
                await RejectAndSignOutAsync(context);
                return;
            }

            // 2. Проверяем синхронизацию: существует ли токен с таким значением в БД
            bool isTokenValid = await tokenService.IsTokenValidAsync(userId, tokenValue);
            if (!isTokenValid)
            {
                await RejectAndSignOutAsync(context);
                return;
            }
        }

        private async Task RejectAndSignOutAsync(CookieValidatePrincipalContext context)
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Перенаправление на страницу ошибки сессии
            context.Response.Redirect("/Login/SessionError");
        }
    }
}