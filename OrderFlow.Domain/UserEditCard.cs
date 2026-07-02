using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Domain
{
    public class UserEditCard
    {
        [Required(ErrorMessage = "Введите имя")]
        [StringLength(100, ErrorMessage = "Имя не может превышать 100 символов")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Введите фамилию")]
        [StringLength(100, ErrorMessage = "Фамилия не может превышать 100 символов")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Укажите логин")]
        [StringLength(256, ErrorMessage = "Логин слишком длинный")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Укажите Email")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        [StringLength(256, ErrorMessage = "Email слишком длинный")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Повторите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
