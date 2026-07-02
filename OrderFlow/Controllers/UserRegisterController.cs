using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Extensions;
using OrderFlow.Application.Services;
using OrderFlow.Domain;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;
using System.Net;
using System.Text.Json;

namespace OrderFlow.Controllers
{
    public class UserRegisterController : Controller
    {
        private readonly IAspNetUserRepo _userRepo;
        private readonly SessionManagerService _sessionManagerService;

        public UserRegisterController(IAspNetUserRepo userRepo, SessionManagerService sessionManagerService)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _sessionManagerService = sessionManagerService ?? throw new ArgumentNullException(nameof(sessionManagerService));
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Генерируем уникальный токен временной сессии для этой формы
            string sessionHash = _sessionManagerService.CreateSession();

            // Передаем его во View для упаковки внутрь JS-туннеля
            ViewBag.SessionHash = sessionHash;

            return View();
        }

        /// <summary>
        /// POST: Принимает данные регистрации исключительно через крипто-туннель.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Register([FromForm] string encryptedPayload)
        {
            // 1. Проверяем физическое наличие контейнера
            if (string.IsNullOrWhiteSpace(encryptedPayload))
            {
                ModelState.AddModelError(string.Empty, "Данные формы не были переданы или пустые.");
                return View("Index");
            }

            // 2. Дешифруем Base64-пакет в JSON-словарь
            var payload = DecryptTunnelPayload(encryptedPayload);
            if (payload == null)
            {
                ModelState.AddModelError(string.Empty, "Не удалось расшифровать пакет данных (нарушена целостность туннеля).");
                return View("Index");
            }

            // 3. Извлекаем и проверяем защитный токен сессии
            payload.TryGetValue("sessionHash", out var sessionHashObj);
            string sessionHash = sessionHashObj?.ToString() ?? string.Empty;

            Guid? session = _sessionManagerService.GetSession(sessionHash);
            if (session == null)
            {
                // Если сессия истекла или её нет в синглтоне — редирект на стандартную ошибку сессии
                return RedirectToAction("SessionError", "Login");
            }

            // 4. Потокобезопасно вычитываем все переданные с фронтенда поля
            payload.TryGetValue("firstName", out var firstNameObj);
            payload.TryGetValue("lastName", out var lastNameObj);
            payload.TryGetValue("login", out var loginObj);
            payload.TryGetValue("email", out var emailObj);
            payload.TryGetValue("passwordHash", out var passwordHashObj);

            // 5. Собираем объект UserEditCard
            var card = new UserEditCard
            {
                FirstName = firstNameObj?.ToString() ?? string.Empty,
                LastName = lastNameObj?.ToString() ?? string.Empty,
                UserName = loginObj?.ToString() ?? string.Empty,
                Email = emailObj?.ToString() ?? string.Empty,
                // Сюда ложится SHA-256 хэш, посчитанный браузером нативного через Web Crypto API
                Password = passwordHashObj?.ToString() ?? string.Empty
            };

            // 6. Бизнес-валидация критических полей на стороне сервера (вместо ModelState.IsValid)
            if (string.IsNullOrWhiteSpace(card.UserName))
            {
                ModelState.AddModelError(nameof(card.UserName), "Логин (Имя пользователя) обязательно для заполнения.");
            }
            if (string.IsNullOrWhiteSpace(card.Password))
            {
                ModelState.AddModelError(nameof(card.Password), "Пароль обязателен для заполнения.");
            }
            if (string.IsNullOrWhiteSpace(card.Email))
            {
                ModelState.AddModelError(nameof(card.Email), "Email обязателен для заполнения.");
            }

            // Если нашли пустые поля — возвращаем форму с текущим хэшем сессии
            if (!ModelState.IsValid)
            {
                ViewBag.SessionHash = sessionHash;
                return View("Index", card);
            }

            try
            {
                // Конвертируем заполненную карту в сущность базы данных
                AspNetUser user = card.ToAspNetUser();

                // Сохраняем пользователя в СУБД (с await, контроллер заблокирует поток до конца транзакции)
                await _userRepo.CreateAsync(user);

                // Обязательно аннулируем использованный одноразовый токен сессии туннеля
                _sessionManagerService.RemoveSession(sessionHash);

                return RedirectToAction(nameof(Successful), "UserRegister");
            }
            catch (ArgumentException ex)
            {
                // Обработка нарушения уникальности (дубликат логина/email), вызванного из CheckUniqueConstraintsAsync в репозитории
                ModelState.AddModelError(string.Empty, ex.Message);

                // Перегенерируем токен, так как старый при неудаче мог устареть по таймауту, пока пользователь думает
                ViewBag.SessionHash = sessionHash;
                return View("Index", card);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Внутренняя ошибка при создании учетной записи: {ex.Message}");
                ViewBag.SessionHash = sessionHash;
                return View("Index", card);
            }
        }

        [HttpGet]
        public IActionResult Successful()
        {
            return View();
        }

        /// <summary>
        /// Безопасный метод парсинга крипто-пакета туннеля.
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