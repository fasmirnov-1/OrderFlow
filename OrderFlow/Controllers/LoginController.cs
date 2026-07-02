using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Services;
using OrderFlow.Domain;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace OrderFlow.Controllers
{
    public class LoginController : Controller
    {
        private readonly SessionManagerService _sessionManagerService;
        private readonly IAspNetUserRepo _userRepo;
        private readonly TokenManagementService _tokenManagementService;

        public LoginController(
            SessionManagerService sessionManagerService,
            IAspNetUserRepo userRepo,
            TokenManagementService tokenManagementService)
        {
            _sessionManagerService = sessionManagerService ?? throw new ArgumentNullException(nameof(sessionManagerService));
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _tokenManagementService = tokenManagementService ?? throw new ArgumentNullException(nameof(tokenManagementService));
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Получаем уже готовый хэшированный строковый ключ
            string sessionHash = _sessionManagerService.CreateSession();

            // Передаем модель во View без повторного хэширования!
            var login = new Login
            {
                sessionHash = sessionHash
            };

            return View(login);
        }

        /// <summary>
        /// POST: Принимает зашифрованный payload из крипто-туннеля фронтенда
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("application/x-www-form-urlencoded")] // Ждем данные из формы
        public async Task<IActionResult> Login([FromForm] string encryptedPayload)
        {
            // 1. Проверяем, что зашифрованный контейнер вообще дошел
            if (string.IsNullOrWhiteSpace(encryptedPayload))
            {
                return RedirectToAction(nameof(UserNotFoundError));
            }

            // 2. Расшифровываем payload туннеля в словарь
            var payload = DecryptTunnelPayload(encryptedPayload);
            if (payload == null)
            {
                return RedirectToAction(nameof(SessionError));
            }

            // 3. Безопасно извлекаем данные, которые упаковал login.js
            payload.TryGetValue("login", out var loginObj);
            payload.TryGetValue("passwordHash", out var passwordHashObj);
            payload.TryGetValue("sessionHash", out var sessionHashObj);

            string login = loginObj?.ToString() ?? string.Empty;
            string passwordHash = passwordHashObj?.ToString() ?? string.Empty;
            string sessionHash = sessionHashObj?.ToString() ?? string.Empty;

            // 4. Проверяем верхнеуровневую валидность полей
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(passwordHash) || string.IsNullOrWhiteSpace(sessionHash))
            {
                return RedirectToAction(nameof(UserNotFoundError));
            }

            // 5. Валидация временной сессии крипто-туннеля
            Guid? session = _sessionManagerService.GetSession(sessionHash);
            if (session == null)
            {
                return RedirectToAction(nameof(SessionError));
            }

            // 6. Извлечение пользователя из БД
            AspNetUser? user = await _userRepo.GetUserAsync(login);
            if (user == null)
            {
                return RedirectToAction(nameof(UserNotFoundError));
            }

            // 7. Проверка соответствия хэшей паролей
            if (string.IsNullOrEmpty(user.PasswordHash) || !CryptoService.VerifyHashes(user.PasswordHash, passwordHash))
            {
                return RedirectToAction(nameof(PasswordNotCorrectError));
            }

            // 8. Инициализация Cookie-авторизации (на базе вашего Program.cs)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            // 9. Удаление сессии туннеля после успешного входа
            _sessionManagerService.RemoveSession(sessionHash);

            // 10. Выписка инфраструктурного токена
            var token = new AspNetUserToken()
            {
                UserId = user.Id,
                LoginProvider = "Local",
                Name = "RefreshToken",
                Value = Guid.NewGuid().ToString()
            };
            try
            {
                await _tokenManagementService.WriteTokenAsync(token);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Токен для пользователя") == true && ex.Message.Contains("уже существует") == true)
                {
                    RedirectToAction("Index", "Home");
                }
            }

            // Фиксация времени активности
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepo.UpdateAsync(user);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet] public IActionResult SessionError() => View();
        [HttpGet] public IActionResult UserNotFoundError() => View();
        [HttpGet] public IActionResult PasswordNotCorrectError() => View();

        /// <summary>
        /// Вспомогательный метод дешифрации Base64-туннеля (идентичен логике в HomeController)
        /// </summary>
        private Dictionary<string, string>? DecryptTunnelPayload(string encryptedPayload)
        {
            if (string.IsNullOrEmpty(encryptedPayload)) return null;
            try
            {
                // 1. Декодируем Base64 в исходный JSON-текст
                string decryptedJson = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedPayload));

                // 2. ИСПРАВЛЕНО: Принудительно декодируем HTML-сущности (превратит &#x2B; обратно в +)
                decryptedJson = WebUtility.HtmlDecode(decryptedJson);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedJson, options);
            }
            catch
            {
                return null;
            }
        }
    }
}