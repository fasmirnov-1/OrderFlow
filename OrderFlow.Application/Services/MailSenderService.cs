using OrderFlow.Infrastructure.Mail;
using OrderFlow.Infrastructure.Repositories.Interfaces;

namespace OrderFlow.Application.Services
{
    public class EmailConfirmationService
    {
        private readonly IEmailTemplateRepo _templateRepo;
        private readonly IAspNetUserRepo _userRepo;
        private readonly EmailService _emailService;

        public EmailConfirmationService(
            IEmailTemplateRepo templateRepo,
            IAspNetUserRepo userRepo,
            EmailService emailService)
        {
            _templateRepo = templateRepo ?? throw new ArgumentNullException(nameof(templateRepo));
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        /// <summary>
        /// Формирует и отправляет письмо подтверждения почты для текущего пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор текущего пользователя</param>
        /// <param name="confirmationLink">Готовая ссылка с токеном подтверждения (генерируется вовне)</param>
        /// <param name="tokenExpirationHours">Время жизни токена в часах для отображения в шаблоне</param>
        public async Task SendConfirmationEmailAsync(string userId, string confirmationLink, int tokenExpirationHours = 24)
        {
            // 1. Находим пользователя в базе данных (учитывая, что его профиль открыт/создается)
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"Пользователь с ID '{userId}' не найден в системе.");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new InvalidOperationException("У текущего пользователя не заполнен адрес электронной почты.");
            }

            // 2. Ищем активный шаблон подтверждения почты в БД
            var template = await _templateRepo.GetActiveTemplateByNameAsync("EmailConfirmation");
            if (template == null)
            {
                // Если активных шаблонов нет — отдаем ошибку в интерфейс
                throw new InvalidOperationException("Активный шаблон для подтверждения электронной почты не найден. Пожалуйста, обратитесь к администратору.");
            }

            // 3. Формируем ФИО пользователя для подстановки
            string fullName = $"{user.FirstName} {user.LastName}".Trim();
            if (string.IsNullOrEmpty(fullName))
            {
                fullName = user.UserName ?? "Пользователь";
            }

            // 4. Заполняем словарь переменных шаблона
            var variables = new Dictionary<string, string>
            {
                { "UserFullName", fullName },
                { "UserEmail", user.Email },
                { "ConfirmationLink", confirmationLink },
                { "CompanyName", "OrderFlow" },
                { "TokenExpirationHours", tokenExpirationHours.ToString() },
                { "CurrentYear", DateTime.UtcNow.Year.ToString() }
            };

            // 5. Парсим тему и HTML-тело шаблона
            string subject = RenderTemplate(template.Subject, variables);
            string bodyHtml = RenderTemplate(template.BodyHtml, variables);

            // 6. Отправляем письмо с помощью низкоуровневого EmailService (с учетом конфигураций)
            await _emailService.SendEmailAsync(user.Email, subject, bodyHtml);
        }

        /// <summary>
        /// Вспомогательный метод заменяющий теги {{Key}} на соответствующие значения.
        /// </summary>
        private string RenderTemplate(string content, IDictionary<string, string> variables)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;

            foreach (var variable in variables)
            {
                string placeholder = $"{{{{{variable.Key}}}}}";
                content = content.Replace(placeholder, variable.Value ?? string.Empty);
            }

            return content;
        }
    }
}