using OrderFlow.Domain.MainPage;
using OrderFlow.Domain.MainPage.Models;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Application.Extensions
{
    public static class PortfolioExtension
    {
        /// <summary>
        /// Преобразует коллекцию сущностей PortfolioItem в доменные модели Portfolio.
        /// </summary>
        public static List<Portfolio> ToPortfolioModel(this IEnumerable<PortfolioItem> portfolioItems)
        {
            if (portfolioItems == null) throw new ArgumentNullException(nameof(portfolioItems));

            // Если передан список или коллекция с известным размером, инициализируем Capacity 
            // для предотвращения лишних аллокаций памяти при расширении списка.
            var portfolios = portfolioItems is ICollection<PortfolioItem> collection
                ? new List<Portfolio>(collection.Count)
                : new List<Portfolio>();

            // Классический foreach работает быстрее, чем List.ForEach на больших объемах данных
            foreach (var item in portfolioItems)
            {
                portfolios.Add(new Portfolio
                {
                    ProjectName = new Link { Title = item.Title, Url = item.ProjectUrl },
                    TechnologyName = item.Technologies,
                    Metodology = new Domain.MainPage.Models.Metodology
                    {
                        Mtdology = (Domain.MainPage.Enums.Metodology)item.Methodology
                    }
                });
            }

            return portfolios;
        }

        /// <summary>
        /// Возвращает русскоязычное наименование методологии разработки.
        /// </summary>
        public static string ToRussian(this Domain.MainPage.Enums.Metodology methodology)
        {
            // Используем современный и лаконичный switch-expression (C# 8.0+)
            return methodology switch
            {
                Domain.MainPage.Enums.Metodology.Prototyping => "Прототипирование",
                Domain.MainPage.Enums.Metodology.Waterfall => "Водопад",
                _ => string.Empty // Вместо null возвращаем пустую строку для безопасности UI-рендеринга
            };
        }
    }
}