using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Repositories;
using OrderFlow.Infrastructure.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. НАСТРОЙКА СЕРВИСОВ (DI КОНТЕЙНЕР)

// Добавление контроллеров и представлений (MVC)
builder.Services.AddControllersWithViews();

// Добавление сервиса определения устройств (Wangkanai Detection)
builder.Services.AddDetection();

// Извлечение и проверка строки подключения
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' не настроена. Проверьте appsettings.json.");
}

// Регистрация контекста базы данных Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// РЕГИСТРАЦИЯ РЕПОЗИТОРИЕВ И ИХ ИНТЕРФЕЙСОВ (Scoped - на каждый HTTP-запрос)
builder.Services.AddScoped<IPaymentDetailRepo, PaymentDetailRepo>();
builder.Services.AddScoped<IPortfolioCategoryRepo, PortfolioCategoryRepo>();
builder.Services.AddScoped<IPortfolioItemRepo, PortfolioItemRepo>();
builder.Services.AddScoped<ISeoSettingRepo, SeoSettingRepo>();
builder.Services.AddScoped<IServiceRepo, ServiceRepo>();
builder.Services.AddScoped<ISiteSettingRepo, SiteSettingRepo>();
builder.Services.AddScoped<ISubscriptionEmailRepo, SubscriptionEmailRepo>();
builder.Services.AddScoped<ISystemNotificationRepo, SystemNotificationRepo>();
builder.Services.AddScoped<ITagRepo, TagRepo>();
builder.Services.AddScoped<ITechnologyRepo, TechnologyRepo>();
builder.Services.AddScoped<ITestimonialRepo, TestimonialRepo>();

var app = builder.Build();

// 2. НАСТРОЙКА КОНВЕЙЕРА ОБРАБОТКИ HTTP-ЗАПРОСОВ (MIDDLEWARE)

// Обработка ошибок для Production-окружения
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ВАЖНО: Подключение middleware для парсинга User-Agent и детекции устройств (Mobile/Tablet/Desktop)
//app.UseDetection();

//app.UseAuthorization();

// Настройка маршрутизации по умолчанию
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();