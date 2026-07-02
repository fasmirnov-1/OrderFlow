using OrderFlow.Domain;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Application.Extensions
{
    public static class UserEditCardExtension
    {
        public static AspNetUser ToAspNetUser(this UserEditCard card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card), "Данные карточки редактирования не могут быть null.");
            }

            var now = DateTime.UtcNow;

            return new AspNetUser
            {
                Id = Guid.NewGuid().ToString(),

                UserName = card.UserName,
                NormalizedUserName = card.UserName?.ToUpperInvariant(),

                Email = card.Email,
                NormalizedEmail = card.Email?.ToUpperInvariant(),

                EmailConfirmed = false,

                // ИСПРАВЛЕНО: Убрано повторное хэширование. 
                // Так как card.Password уже содержит вычисленный на фронтенде SHA-256 хэш, 
                // мы просто сохраняем его в базу данных без изменений.
                PasswordHash = card.Password,

                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),

                PhoneNumber = null,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,

                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = 0,

                FirstName = card.FirstName,
                LastName = card.LastName,

                CreatedAt = now,
                LastLoginAt = null
            };
        }
    }
}