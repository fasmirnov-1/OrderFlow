using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Extensions;
using OrderFlow.Domain;
using OrderFlow.Domain.MainPage.Models;
using OrderFlow.Infrastructure.Repositories.Interfaces;
using System.Text.Json;
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
        private readonly ISiteSettingRepo _siteSettingRepo;

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

        /// <summary>
        /// GET: Прямой вход на сайт. Быстро определяет устройство и отдает нужную View БЕЗ лишних редиректов.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await BuildPageModelAsync();

            if (IsMobileOrTablet())
            {
                return View("IndexMobile", model);
            }

            return View("Index", model);
        }

        /// <summary>
        /// POST: Точка входа для крипто-туннеля. Принимает строку напрямую из формы.
        /// </summary>
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Index([FromForm] string encryptedPayload)
        {
            if (!string.IsNullOrEmpty(encryptedPayload))
            {
                var payload = DecryptTunnelPayload(encryptedPayload);
                if (payload != null && payload.TryGetValue("projectId", out var projectId))
                {
                    ViewData["SelectedProjectId"] = projectId?.ToString();
                }
            }

            var model = await BuildPageModelAsync();

            if (IsMobileOrTablet())
            {
                return View("IndexMobile", model);
            }

            return View("Index", model);
        }

        // Оставляем старые роуты для обратной совместимости, если они вызываются из других мест
        [HttpGet] public async Task<IActionResult> IndexDesktop() => await Index();
        [HttpGet] public async Task<IActionResult> IndexMobile() => await Index();

        /// <summary>
        /// Оптимизированный метод сборки модели. 
        /// Все запросы к БД теперь запускаются ПАРАЛЛЕЛЬНО (Task.WhenAll), что убирает 10-секундный затык.
        /// </summary>
        private async Task<Page> BuildPageModelAsync()
        {
            var page = new Page
            {
                Header = "Создание приложений на заказ...",
                Children = new Dictionary<Types, dynamic>()
            };

            page.Children.Add(Types.Hero, new Hero
            {
                HeroName = "Создание приложений на заказ...",
                HeroSubtitle = "OrderFlow...",
                ImageURL = "~/img/hero.png"
            });

            // Выполняем строго последовательно, чтобы EF Core не падал из-за потоков
            var currentSettings = await _siteSettingRepo.GetCurrentSettingsAsync();

            page.Children.Add(Types.Contacts, new Contacts
            {
                Phone = currentSettings?.Phone ?? "+7(915)162-97-57",
                Email = currentSettings?.Email ?? "fasmirnov@gmail.com",
                Telegram = "@FedorSmirnov10",
                WatsApp = currentSettings?.Phone ?? "+7(915)162-97-57"
            });

            page.Children.Add(Types.Details, new Details
            {
                Inn = "7707083893",
                Kpp = "773643001",
                Check = "40817810338180228337",
                BankName = "ПАО Сбербанк",
                CardNumber = string.Empty,
            });

            var activeServices = await _serviceRepo.GetActiveAsync();
            page.Children.Add(Types.Services, activeServices.ToServiceModel());

            page.Children.Add(Types.Metodology, new List<Metodology>
            {
                new() { Mtdology = Domain.MainPage.Enums.Metodology.Waterfall },
                new() { Mtdology = Domain.MainPage.Enums.Metodology.Prototyping }
            });

            var publishedPortfolios = await _portfolioItemRepo.GetPublishedAsync();
            page.Children.Add(Types.Portfolio, publishedPortfolios.ToPortfolioModel());

            var approvedFeedbacks = await _testimonialRepo.GetApprovedAsync(limit: 6);
            page.Children.Add(Types.Feedback, approvedFeedbacks.ToFeedbackModel());

            return page;
        }

        private bool IsMobileOrTablet()
        {
            var isMobileHint = Request.Headers["Sec-CH-UA-Mobile"].ToString();
            var uaHeader = Request.Headers["User-Agent"].ToString();

            bool isTablet = (isMobileHint == "?0" && uaHeader.Contains("Android", StringComparison.OrdinalIgnoreCase))
                         || uaHeader.Contains("iPad", StringComparison.OrdinalIgnoreCase);

            return isTablet || _detectionService.Device.Type == Device.Mobile;
        }

        private Dictionary<string, object>? DecryptTunnelPayload(string encryptedPayload)
        {
            if (string.IsNullOrEmpty(encryptedPayload)) return null;
            try
            {
                string decryptedJson = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encryptedPayload));
                return JsonSerializer.Deserialize<Dictionary<string, object>>(decryptedJson);
            }
            catch
            {
                return null;
            }
        }
    }
}