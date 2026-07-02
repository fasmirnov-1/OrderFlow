using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Services;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Repositories;
using OrderFlow.Infrastructure.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// 1. РЕГИСТРАЦИЯ СЕРВИСОВ И БИБЛИОТЕК (Строго до builder.Build)
// =========================================================================

builder.Services.AddControllersWithViews();

// Добавляем сервисы определения устройств (исправляет ошибку со скриншота 2)
builder.Services.AddDetection();

// Регистрация прикладных сервисов
builder.Services.AddScoped<OrderFlow.Application.Services.TokenManagementService>();

// Настройка безопасности Cookie
// Настройка безопасности Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict; // Защита от CSRF

    // НАСТРОЙКА ВРЕМЕНИ ЖИЗНИ СЕССИИ (Исправлено)
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Автовыход при инактиве через 20 минут
    options.SlidingExpiration = true; // Сброс таймера активности при действиях пользователя
});

// Добавляем регистрацию контекста базы данных SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Инфраструктурные репозитории (Регистрация сопоставления интерфейс -> класс)
// Перенесено выше builder.Build(), что исправляет ошибку со скриншота 1
builder.Services.AddScoped<IAspNetUserTokenRepo, AspNetUserTokenRepo>();
builder.Services.AddScoped<IAspNetUserRepo, AspNetUserRepo>();

// Зарегистрируйте здесь остальные ваши репозитории, которые использует HomeController:
builder.Services.AddScoped<IServiceRepo, ServiceRepo>();
builder.Services.AddScoped<IPortfolioItemRepo, PortfolioItemRepo>();
builder.Services.AddScoped<ITestimonialRepo, TestimonialRepo>();
builder.Services.AddScoped<ISiteSettingRepo, SiteSettingRepo>();
builder.Services.AddSingleton<SessionManagerService>();

// 1. Настройка обработчиков аутентификации
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Куда перенаправлять пользователя, если он не авторизован
        options.LoginPath = "/Login/Index";

        // Время жизни куки (должно коррелировать с вашими требованиями)
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

        // Продлевать куки при активности пользователя
        options.SlidingExpiration = true;

        options.Cookie.Name = "OrderFlow.AuthCookie";
    });


// =========================================================================
// 2. СБОРКА ПРИЛОЖЕНИЯ (Контейнер становится Read-Only)
// =========================================================================

var app = builder.Build();


// =========================================================================
// 3. НАСТРОЙКА КОНВЕЙЕРА MIDDLEWARE (Маршрутизация и безопасность)
// =========================================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Включаем middleware для работы библиотеки определения устройств Wangkanai
app.UseDetection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Настройка дефолтного маршрута для MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();