// Controllers/LoginController.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Services;
using OrderFlow.Domain;
using OrderFlow.Domain.DTO;
using OrderFlow.Infrastructure.Data;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace OrderFlow.Controllers
{
    public class LoginController : Controller
    {
        private readonly ITokenLifecycleService _tokenService;
        private readonly AppDbContext _context;

        public LoginController(ITokenLifecycleService tokenService, AppDbContext context)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Index()
        {
            // Передаем во View модель с сгенерированным sessionHash для защиты от CSRF/Replay-атак
            var model = new Login
            {
                sessionHash = Guid.NewGuid().ToString()
            };
            return View(model);
        }

        /// <summary>
        /// Точка входа для авторизации. Принимает форму с Base64 строкой из крипто-туннеля.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] string encryptedPayload)
        {
            if (string.IsNullOrEmpty(encryptedPayload))
            {
                ModelState.AddModelError("", "Криптографический пакет пуст или поврежден.");
                return View("Index", new Login { sessionHash = Guid.NewGuid().ToString() });
            }

            DecryptedLoginPayload? payload;
            try
            {
                // 1. ДЕКОДИРОВАНИЕ КРИПТО-ТУННЕЛЯ: 
                // Восстанавливаем байты из Base64 (аналог btoa на клиенте)
                byte[] base64Bytes = Convert.FromBase64String(encryptedPayload);
                string decodedUriString = Encoding.UTF8.GetString(base64Bytes);

                // Декодируем URI-компоненты (аналог encodeURIComponent на клиенте)
                string jsonString = Uri.UnescapeDataString(decodedUriString);

                // Десериализуем в типизированный объект
                payload = JsonSerializer.Deserialize<DecryptedLoginPayload>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (payload == null || string.IsNullOrWhiteSpace(payload.Login) || string.IsNullOrWhiteSpace(payload.PasswordHash))
                {
                    ModelState.AddModelError("", "Некорректная структура крипто-пакета.");
                    return View("Index", new Login { sessionHash = Guid.NewGuid().ToString() });
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Ошибка дешифрации безопасного туннеля.");
                return View("Index", new Login { sessionHash = Guid.NewGuid().ToString() });
            }

            // 2. ПОИСК ПОЛЬЗОВАТЕЛЯ: Ищем в базе данных по полученному Login
            var user = await _context.AspNetUsers
                .FirstOrDefaultAsync(u => u.UserName == payload.Login);

            // 3. БЕЗОПАСНАЯ ВЕРИФИКАЦИЯ ХЭШЕЙ:
            // Сверяем хэш, вычисленный на клиенте, с хэшем из базы данных.
            // Используется CryptoService.VerifyHashes с алгоритмом FixedTimeEquals для защиты от атак по времени.
            if (user == null || string.IsNullOrEmpty(user.PasswordHash) ||
                !CryptoService.VerifyHashes(user.PasswordHash, payload.PasswordHash))
            {
                ModelState.AddModelError("", "Неверный логин или пароль.");
                return View("Index", new Login { sessionHash = Guid.NewGuid().ToString() });
            }

            // 4. УПРАВЛЕНИЕ ВРЕМЕНЕМ ЖИЗНИ ТОКЕНА (СЕССИИ):
            // Генерируем уникальный GUID-токен сессии пользователя
            string sessionToken = Guid.NewGuid().ToString();

            // Сохраняем/обновляем токен в системной таблице dbo.AspNetUserTokens через репозиторий
            await _tokenService.CreateSessionTokenAsync(user.Id, sessionToken);

            // 5. ФОРМИРОВАНИЕ СИНХРОННОЙ АВТОРИЗАЦИОННОЙ КУКИ:
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim("SessionToken", sessionToken) // Помещаем GUID сессии в куку для сверки в Events
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2) // Синхронизировано с жестким временем жизни
            };

            // Вызываем SignInAsync с явным указанием схемы (ошибка "No sign-in handlers" устранена)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Перенаправляем пользователя в закрытую зону приложения
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // При явном логауте уничтожаем сессионный токен из базы данных
                await _tokenService.InvalidateTokenAsync(userId);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult SessionError()
        {
            // Сюда middleware перенаправляет пользователя, если кука или токен в БД истекли
            return View();
        }
    }
}