using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Domain
{
    public class UserProfile
    {
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Имя пользователя обязательно.")]
        [StringLength(256, ErrorMessage = "Имя пользователя не должно превышать 256 символов.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Укажите корректный адрес электронной почты.")]
        [StringLength(256)]
        public string? Email { get; set; }

        public bool EmailConfirmed { get; set; }

        // Добавляем регулярное выражение для проверки формата номера телефона
        [Phone(ErrorMessage = "Укажите корректный номер телефона.")]
        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        [StringLength(100, ErrorMessage = "Имя не должно превышать 100 символов.")]
        public string? FirstName { get; set; }

        [StringLength(100, ErrorMessage = "Фамилия не должна превышать 100 символов.")]
        public string? LastName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string sessionHash { get; set; }
    }
}