using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Extensions;
using OrderFlow.Application.Middlewares;
using OrderFlow.Application.Services;
using OrderFlow.Infrastructure.Data;
using OrderFlow.Infrastructure.Mail;
using OrderFlow.Infrastructure.Repositories;
using OrderFlow.Infrastructure.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. ПОДКЛЮЧЕНИЕ БАЗЫ ДАННЫХ
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. РЕГИСТРАЦИЯ СЕРВИСОВ И ДОПОЛНИТЕЛЬНЫХ МОДУЛЕЙ
builder.Services.AddDetection(); // Модуль детекции устройств/браузеров (Wangkanai)
builder.Services.AddControllersWithViews();

// 3. РЕГИСТРАЦИЯ ИНФРАСТРУКТУРНЫХ РЕПОЗИТОРИЕВ (DI)
builder.Services.AddScoped<IServiceRepo, ServiceRepo>();
builder.Services.AddScoped<IPortfolioItemRepo, PortfolioItemRepo>();
builder.Services.AddScoped<ITestimonialRepo, TestimonialRepo>();
builder.Services.AddScoped<ISiteSettingRepo, SiteSettingRepo>();
builder.Services.AddScoped<IAspNetUserTokenRepo, AspNetUserTokenRepo>();
builder.Services.AddScoped<IAspNetUserRepo, AspNetUserRepo>();
builder.Services.AddSingleton<SessionManagerService>();

// 4. СЕРВИСЫ УПРАВЛЕНИЯ СЕССИЯМИ, КРИПТО-ТУННЕЛЯМИ И ПОЧТОЙ
builder.Services.AddScoped<ITokenLifecycleService, TokenLifecycleService>();
builder.Services.AddScoped<CustomCookieAuthenticationEvents>();

// Привязываем секцию "EmailSettings" из appsettings.json к классу EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
// Регистрируем EmailService для внедрения
builder.Services.AddScoped<EmailService>();

// 5. НАСТРОЙКА АУТЕНТИФИКАЦИИ (COOKIE BINDING)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "Orderflow.AuthCookie";
        options.LoginPath = "/Login/Index";
        options.LogoutPath = "/Login/Logout";

        // Жесткое время жизни авторизационной сессии (2 часа)
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = false;

        // Кастомный валидатор сессий в dbo.AspNetUserTokens
        options.EventsType = typeof(CustomCookieAuthenticationEvents);
    });

var app = builder.Build();

// 6. ИНИЦИАЛИЗАЦИЯ ДАННЫХ ПРИ СТАРТЕ (ОЧИСТКА СТАРЫХ СЕССИЙ)
using (var scope = app.Services.CreateScope())
{
    var tokenService = scope.ServiceProvider.GetRequiredService<ITokenLifecycleService>();
    await tokenService.ClearAllSessionTokensAsync();
}

// 7. НАСТРОЙКА HTTP-КОНВЕЙЕРА (MIDDLEWARE)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Подключение middleware детекции устройств (исправляет runtime сбои контроллеров)
app.UseDetection();

app.UseHttpsRedirection();
app.UseStaticFiles();

// ВАЖНО: Подключаем крипто-туннель middleware ДО UseRouting() и UseAuthentication(),
// чтобы он перехватил заголовок X-Encrypted-Path, расшифровал его и подменил Path и Query.
app.UseMiddleware<UrlDecryptionMiddleware>();

app.UseRouting();

// Строгий порядок middleware безопасности ASP.NET Core
app.UseAuthentication();
app.UseAuthorization();

// 8. МАРШРУТИЗАЦИЯ
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();