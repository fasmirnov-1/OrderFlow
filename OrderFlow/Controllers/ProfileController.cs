// Controllers/ProfileController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Extensions;
using OrderFlow.Application.Services;
using OrderFlow.Domain;
using OrderFlow.Infrastructure.Entities;
using OrderFlow.Infrastructure.Repositories.Interfaces;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace OrderFlow.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IAspNetUserRepo _userRepo;
        private readonly EmailConfirmationService _emailConfirmationService;
        private readonly IUserSessionsRepo _userSessionsRepo;

        public ProfileController(IAspNetUserRepo userRepo, EmailConfirmationService emailConfirmationService, IUserSessionsRepo userSessionsRepo)
        {
            _userRepo = userRepo;
            _emailConfirmationService = emailConfirmationService;
            _userSessionsRepo = userSessionsRepo;
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Index([FromForm] string encryptedPayload)
        {
            if (!string.IsNullOrEmpty(encryptedPayload))
            {
                try
                {
                    byte[] base64Bytes = Convert.FromBase64String(encryptedPayload);
                    string decodedUriString = Encoding.UTF8.GetString(base64Bytes);
                    string jsonString = Uri.UnescapeDataString(decodedUriString);

                    JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Некорректная структура крипто-пакета.");
                    return View("Index", new UserProfile { sessionHash = Guid.NewGuid().ToString() });
                }
            }

            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Challenge();
            }

            AspNetUser? aspNetUser = await _userRepo.GetUserAsync(userName);
            if (aspNetUser == null)
            {
                return NotFound();
            }

            UserProfile userProfile = aspNetUser.ToUserProfile();
            userProfile.sessionHash ??= Guid.NewGuid().ToString();

            return View("Index", userProfile);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromForm] UserProfile updatedProfile)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", updatedProfile);
            }
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Challenge();
            }
            AspNetUser? aspNetUser = await _userRepo.GetUserAsync(userName);
            if (aspNetUser == null)
            {
                return NotFound();
            }
            // Update the user profile properties
            aspNetUser.FirstName = updatedProfile.FirstName;
            aspNetUser.LastName = updatedProfile.LastName;
            aspNetUser.Email = updatedProfile.Email;
            aspNetUser.EmailConfirmed = updatedProfile.EmailConfirmed;
            aspNetUser.PhoneNumber = updatedProfile.PhoneNumber;
            aspNetUser.UserName = userName;
            aspNetUser.PhoneNumberConfirmed = updatedProfile.PhoneNumberConfirmed;
            aspNetUser.ConcurrencyStamp = Guid.NewGuid().ToString();
            aspNetUser.LastLoginAt = DateTime.UtcNow;
            aspNetUser.CreatedAt = updatedProfile.CreatedAt;
            // Add other properties as needed
            await _userRepo.UpdateAsync(aspNetUser);
            TempData["SuccessMessage"] = "Профиль успешно обновлен.";
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailConfirm([FromForm] UserProfile profileModel)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                return Challenge();
            }

            var aspNetUser = await _userRepo.GetUserAsync(userName);
            if (aspNetUser == null)
            {
                return NotFound();
            }

            try
            {
                // 1. Получаем или генерируем SessionHash и определяем время жизни (например, 24 часа)
                string sessionHash = profileModel.sessionHash;
                if (string.IsNullOrEmpty(sessionHash))
                {
                    sessionHash = Guid.NewGuid().ToString();
                    profileModel.sessionHash = sessionHash;
                }

                var expiresAt = DateTime.UtcNow.AddHours(24);

                // 2. Сохраняем сессию в базу данных через IUserSessionsRepo
                // (предполагается, что репозиторий внедрен через конструктор контроллера: _userSessionsRepo)
                var userSession = new OrderFlow.Infrastructure.Entities.UserSession
                {
                    UserId = aspNetUser.Id,
                    SessionHash = sessionHash,
                    ExpiresAt = expiresAt,
                    CreatedAt = DateTime.UtcNow
                };

                await _userSessionsRepo.AddAsync(userSession);
                await _userSessionsRepo.SaveChangesAsync();

                // 3. Формируем словарь данных профиля для отправки через крипто-туннель
                var payloadData = new Dictionary<string, object?>
            {
                { "Id", aspNetUser.Id },
                { "UserName", profileModel.UserName ?? aspNetUser.UserName },
                { "Email", profileModel.Email ?? aspNetUser.Email },
                { "FirstName", profileModel.FirstName ?? aspNetUser.FirstName },
                { "LastName", profileModel.LastName ?? aspNetUser.LastName },
                { "PhoneNumber", profileModel.PhoneNumber ?? aspNetUser.PhoneNumber },
                { "SessionHash", sessionHash },
                { "Timestamp", DateTime.UtcNow }
            };

                // 4. Сериализуем и шифруем полезную нагрузку в Base64
                string jsonString = JsonSerializer.Serialize(payloadData);
                string escapedUriString = Uri.EscapeDataString(jsonString);
                byte[] bytesToEncode = Encoding.UTF8.GetBytes(escapedUriString);
                string encryptedPayload = Convert.ToBase64String(bytesToEncode);

                // 5. Формируем ссылку подтверждения, ведущую на метод ConfirmEmail
                string confirmationLink = Url.Action("ConfirmEmail", "Profile", new { encryptedPayload = encryptedPayload }, Request.Scheme)
    ?? $"{Request.Scheme}://{Request.Host}/Profile/ConfirmEmail?encryptedPayload={encryptedPayload}";

                // 6. Отправляем письмо через сервис EmailConfirmationService
                await _emailConfirmationService.SendConfirmationEmailAsync(aspNetUser.Id, confirmationLink);

                TempData["SuccessMessage"] = "Письмо с подтверждением успешно отправлено на ваш Email.";
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);

                profileModel.CreatedAt = aspNetUser.CreatedAt;
                profileModel.LastLoginAt = aspNetUser.LastLoginAt;
                profileModel.EmailConfirmed = aspNetUser.EmailConfirmed;
                profileModel.PhoneNumberConfirmed = aspNetUser.PhoneNumberConfirmed;
                profileModel.sessionHash ??= Guid.NewGuid().ToString();

                return View("Index", profileModel);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Произошла ошибка при формировании крипто-пакета или отправке письма.");

                profileModel.CreatedAt = aspNetUser.CreatedAt;
                profileModel.LastLoginAt = aspNetUser.LastLoginAt;
                profileModel.EmailConfirmed = aspNetUser.EmailConfirmed;
                profileModel.PhoneNumberConfirmed = aspNetUser.PhoneNumberConfirmed;
                profileModel.sessionHash ??= Guid.NewGuid().ToString();

                return View("Index", profileModel);
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        [AllowAnonymous] // Разрешаем доступ по ссылке из письма без предварительной авторизации
        public async Task<IActionResult> ConfirmEmail(string encryptedPayload)
        {
            if (string.IsNullOrEmpty(encryptedPayload))
            {
                return View("ConfirmationError", "Некорректная ссылка подтверждения.");
            }

            try
            {
                // Расшифровываем payload
                byte[] base64Bytes = Convert.FromBase64String(encryptedPayload);
                string decodedUriString = Encoding.UTF8.GetString(base64Bytes);
                string jsonString = Uri.UnescapeDataString(decodedUriString);

                var payloadData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (payloadData == null || !payloadData.TryGetValue("Id", out var idElement))
                {
                    return View("ConfirmationError", "Неверный формат данных в ключе подтверждения.");
                }

                string userId = idElement.GetString() ?? string.Empty;
                if (string.IsNullOrEmpty(userId))
                {
                    return View("ConfirmationError", "Пользователь не найден.");
                }

                // Извлекаем SessionHash из полезной нагрузки
                string sessionHash = string.Empty;
                if (payloadData.TryGetValue("SessionHash", out var hashElement))
                {
                    sessionHash = hashElement.GetString() ?? string.Empty;
                }

                // Если хэш отсутствует в payload
                if (string.IsNullOrEmpty(sessionHash))
                {
                    return View("ConfirmationError", "Отсутствует защитный токен сессии в данных подтверждения.");
                }

                // Ищем пользователя по ID
                var aspNetUser = await _userRepo.GetByIdAsync(userId);
                if (aspNetUser == null)
                {
                    return View("ConfirmationError", "Пользователь не найден в базе данных.");
                }

                // Ищем сессию по хэшу (если она удалена фоновым сервисом или уже использована — вернет null)
                var activeSession = await _userSessionsRepo.GetByHashAsync(sessionHash);
                if (activeSession == null)
                {
                    return View("ConfirmationError", "Срок действия ссылки подтверждения истек (сессия устарела) или она уже была использована.");
                }

                // Обновляем статус подтверждения
                aspNetUser.EmailConfirmed = true;
                aspNetUser.ConcurrencyStamp = Guid.NewGuid().ToString();

                await _userRepo.UpdateAsync(aspNetUser);

                // Удаляем использованную сессию из таблицы UserSessions
                _userSessionsRepo.Remove(activeSession);
                await _userSessionsRepo.SaveChangesAsync();

                // Возвращаем представление успешного подтверждения
                return View("EmailConfirmedSuccess");
            }
            catch (Exception)
            {
                return View("ConfirmationError", "Произошла ошибка при обработке защищенного токена подтверждения.");
            }
        }
    }
}