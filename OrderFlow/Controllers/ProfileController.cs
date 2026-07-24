// Controllers/ProfileController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Extensions;
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

        public ProfileController(IAspNetUserRepo userRepo)
        {
            _userRepo = userRepo;
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
    }
}