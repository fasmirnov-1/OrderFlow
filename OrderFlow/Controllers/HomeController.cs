using Microsoft.AspNetCore.Mvc;
using OrderFlow.Application.Extensions;
using OrderFlow.Domain;
using OrderFlow.Domain.MainPage.Models;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Repositories;
using Wangkanai.Detection.Models;
using Wangkanai.Detection.Services;

namespace OrderFlow.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext _context = null;
        private IDetectionService _detectionService;

        public HomeController(AppDbContext context, IDetectionService detectionService)
        {
            _context = context;
            _detectionService = detectionService;
        }
        public IActionResult Index()
        {
            var isMobileHint = Request.Headers["Sec-CH-UA-Mobile"].ToString();
            var uaHeader = Request.Headers["User-Agent"].ToString();

            bool isTablet = false;

            if (isMobileHint == "?0" && uaHeader.Contains("Android", StringComparison.OrdinalIgnoreCase))
            {
                isTablet = true;
            }

            if (uaHeader.Contains("iPad", StringComparison.OrdinalIgnoreCase))
            {
                isTablet = true;
            }

            if (isTablet)
            {
                return RedirectToAction("IndexMobile", "Home");
            }

            if (_detectionService.Device.Type == Device.Mobile)
            {
                return RedirectToAction("IndexMobile", "Home");
            }

            Page page = new Page();

            page.Header = "Создание приложений на заказ...";

            page.Children = new Dictionary<Types, dynamic>();

            page.Children.Add(Types.Hero, new Hero()
            {
                HeroName = "Создание приложений на заказ...",
                HeroSubtitle = "OrderFlow...",
                ImageURL = "~/img/hero.png"
            });
            page.Children.Add(Types.Contacts, new Contacts()
            {
                Phone = "+7(915)162-97-57",
                Email = "fasmirnov@gmail.com",
                Telegram = "@FedorSmirnov10",
                WatsApp = "+7(915)162-97-57"
            });
            page.Children.Add(Types.Details, new Details()
            {
                Inn = "7707083893",
                Kpp = "773643001",
                Check = "40817810338180228337",
                BankName = "ПАО Сбербанк",
                CardNumber = string.Empty,
            });

            ServiceRepo serviceRepo = new ServiceRepo(_context);
            var services = serviceRepo.GetAllAsync();
            page.Children.Add(Types.Services, services.Result.ToServiceModel());

            List<OrderFlow.Domain.MainPage.Models.Metodology> metodologies = new List<OrderFlow.Domain.MainPage.Models.Metodology>
                {
                    new OrderFlow.Domain.MainPage.Models.Metodology()
                    {
                        Mtdology = OrderFlow.Domain.MainPage.Enums.Metodology.Waterfall
                    },
                    new OrderFlow.Domain.MainPage.Models.Metodology()
                    {
                        Mtdology = OrderFlow.Domain.MainPage.Enums.Metodology.Prototyping
                    }
                };

            page.Children.Add(Types.Metodology, metodologies);

            PortfolioItemRepo portfolioItem = new PortfolioItemRepo(_context);
            var portfolios = portfolioItem.GetAllAsync();

            try
            {
                page.Children.Add(Types.Portfolio, portfolios.Result.ToList().ToPortfolioModel());
            }
            catch { }

            TestimonialRepo testimonialRepo = new TestimonialRepo(_context);
            try
            {
                page.Children.Add(Types.Feedback, testimonialRepo.GetLimited(6).Result.ToList().ToFeedbackModel());
            }
            catch { }

            return View(page);
        }
        public IActionResult IndexMobile()
        {
            Page page = new Page();

            page.Header = "Создание приложений на заказ...";

            page.Children = new Dictionary<Types, dynamic>();

            page.Children.Add(Types.Hero, new Hero()
            {
                HeroName = "Создание приложений на заказ...",
                HeroSubtitle = "OrderFlow...",
                ImageURL = "~/img/hero.png"
            });
            page.Children.Add(Types.Contacts, new Contacts()
            {
                Phone = "+7(915)162-97-57",
                Email = "fasmirnov@gmail.com",
                Telegram = "@FedorSmirnov10",
                WatsApp = "+7(915)162-97-57"
            });
            page.Children.Add(Types.Details, new Details()
            {
                Inn = "7707083893",
                Kpp = "773643001",
                Check = "40817810338180228337",
                BankName = "ПАО Сбербанк",
                CardNumber = string.Empty,
            });

            ServiceRepo serviceRepo = new ServiceRepo(_context);
            var services = serviceRepo.GetAllAsync();
            page.Children.Add(Types.Services, services.Result.ToServiceModel());

            List<OrderFlow.Domain.MainPage.Models.Metodology> metodologies = new List<OrderFlow.Domain.MainPage.Models.Metodology>
                {
                    new OrderFlow.Domain.MainPage.Models.Metodology()
                    {
                        Mtdology = OrderFlow.Domain.MainPage.Enums.Metodology.Waterfall
                    },
                    new OrderFlow.Domain.MainPage.Models.Metodology()
                    {
                        Mtdology = OrderFlow.Domain.MainPage.Enums.Metodology.Prototyping
                    }
                };

            page.Children.Add(Types.Metodology, metodologies);

            PortfolioItemRepo portfolioItem = new PortfolioItemRepo(_context);
            var portfolios = portfolioItem.GetAllAsync();

            try
            {
                page.Children.Add(Types.Portfolio, portfolios.Result.ToList().ToPortfolioModel());
            }
            catch { }

            TestimonialRepo testimonialRepo = new TestimonialRepo(_context);
            try
            {
                page.Children.Add(Types.Feedback, testimonialRepo.GetLimited(6).Result.ToList().ToFeedbackModel());
            }
            catch { }

            return View(page);
        }
    }
}
