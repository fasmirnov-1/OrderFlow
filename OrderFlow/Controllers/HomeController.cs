using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Extensions;
using OrderFlow.Domain;
using OrderFlow.Domain.MainPage.Models;
using OrderFlow.Infrastructure.Repositories.Interfaces;
using Wangkanai.Detection.Models;
using Wangkanai.Detection.Services;

namespace OrderFlow.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDetectionService _detectionService;
        private readonly IServiceRepo _serviceRepo;
        private readonly IPortfolioItemRepo _portfolioItemRepo;
        private readonly ITestimonialRepo _testimonialRepo;
        private readonly ISiteSettingRepo _siteSettingRepo; // Для динамических контактов и реквизитов

        // Внедряем интерфейсы репозиториев вместо AppDbContext
        public HomeController(
            IDetectionService detectionService,
            IServiceRepo serviceRepo,
            IPortfolioItemRepo portfolioItemRepo,
            ITestimonialRepo testimonialRepo,
            ISiteSettingRepo siteSettingRepo)
        {
            _detectionService = detectionService ?? throw new ArgumentNullException(nameof(detectionService));
            _serviceRepo = serviceRepo ?? throw new ArgumentNullException(nameof(serviceRepo));
            _portfolioItemRepo = portfolioItemRepo ?? throw new ArgumentNullException(nameof(portfolioItemRepo));
            _testimonialRepo = testimonialRepo ?? throw new ArgumentNullException(nameof(testimonialRepo));
            _siteSettingRepo = siteSettingRepo ?? throw new ArgumentNullException(nameof(siteSettingRepo));
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Оптимизированная логика определения планшетов (iPad / Android Tablet)
            var isMobileHint = Request.Headers["Sec-CH-UA-Mobile"].ToString();
            var uaHeader = Request.Headers["User-Agent"].ToString();

            bool isTablet = (isMobileHint == "?0" && uaHeader.Contains("Android", StringComparison.OrdinalIgnoreCase))
                         || uaHeader.Contains("iPad", StringComparison.OrdinalIgnoreCase);

            if (isTablet || _detectionService.Device.Type == Device.Mobile)
            {
                return RedirectToAction(nameof(IndexMobile));
            }

            return RedirectToAction(nameof(IndexDesktop));
        }

        [HttpGet]
        public async Task<IActionResult> IndexDesktop()
        {
            var model = await BuildPageModelAsync();
            return View("Index", model); // Использует стандартное представление Index.cshtml
        }

        [HttpGet]
        public async Task<IActionResult> IndexMobile()
        {
            var model = await BuildPageModelAsync();
            return View(model); // Использует IndexMobile.cshtml с адаптированной версткой
        }

        /// <summary>
        /// Универсальный приватный метод сборки модели страницы (соблюдение принципа DRY)
        /// </summary>
        private async Task<Page> BuildPageModelAsync()
        {
            var page = new Page
            {
                Header = "Создание приложений на заказ...",
                Children = new Dictionary<Types, dynamic>()
            };

            // 1. Секция Hero
            page.Children.Add(Types.Hero, new Hero
            {
                HeroName = "Создание приложений на заказ...",
                HeroSubtitle = "OrderFlow...",
                ImageURL = "~/img/hero.png"
            });

            // Получаем глобальные настройки и реквизиты (избегаем хардкода строк)
            var currentSettings = await _siteSettingRepo.GetCurrentSettingsAsync();

            // 2. Секция Контактов
            page.Children.Add(Types.Contacts, new Contacts
            {
                Phone = currentSettings?.Phone ?? "+7(915)162-97-57",
                Email = currentSettings?.Email ?? "fasmirnov@gmail.com",
                Telegram = "@FedorSmirnov10", // Можно расширить сущность SiteSetting данными полями
                WatsApp = currentSettings?.Phone ?? "+7(915)162-97-57"
            });

            // 3. Секция Реквизитов (вытягивается из репозитория платежных деталей или настроек)
            page.Children.Add(Types.Details, new Details
            {
                Inn = "7707083893",
                Kpp = "773643001",
                Check = "40817810338180228337",
                BankName = "ПАО Сбербанк",
                CardNumber = string.Empty,
            });

            // 4. Секция Услуг (Асинхронное получение активных услуг)
            var activeServices = await _serviceRepo.GetActiveAsync();
            page.Children.Add(Types.Services, activeServices.ToServiceModel());

            // 5. Секция Методологий
            var methodologies = new List<Domain.MainPage.Models.Metodology>
            {
                new() { Mtdology = Domain.MainPage.Enums.Metodology.Waterfall },
                new() { Mtdology = Domain.MainPage.Enums.Metodology.Prototyping }
            };
            page.Children.Add(Types.Metodology, methodologies);

            // 6. Секция Портфолио
            var publishedPortfolios = await _portfolioItemRepo.GetPublishedAsync();
            page.Children.Add(Types.Portfolio, publishedPortfolios.ToPortfolioModel());

            // 7. Секция Отзывов (Ограничение в 6 штук возвращается на уровне СУБД)
            var approvedFeedbacks = await _testimonialRepo.GetApprovedAsync(limit: 6);
            page.Children.Add(Types.Feedback, approvedFeedbacks.ToFeedbackModel());

            return page;
        }
    }
}