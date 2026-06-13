using OrderFlow.Domain.MainPage;
using OrderFlow.Domain.MainPage.Models;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Application.Extensions
{
    public static class PortfolioExtension
    {
        public static List<Portfolio> ToPortfolioModel(this List<PortfolioItem> portfolioItems)
        {
            List<Portfolio> portfolios = new List<Portfolio>();

            portfolioItems.ForEach(p =>
            {
                portfolios.Add(new Portfolio()
                {
                    ProjectName = new Link() { Title = p.Title, Url = p.ProjectUrl },
                    TechnologyName = p.Technologies,
                    Metodology = new OrderFlow.Domain.MainPage.Models.Metodology() { Mtdology = (OrderFlow.Domain.MainPage.Enums.Metodology)p.Methodology }
                });
            });

            return portfolios;
        }
        public static string ToRussian(this OrderFlow.Domain.MainPage.Enums.Metodology metodology)
        {
            switch (metodology)
            {
                case OrderFlow.Domain.MainPage.Enums.Metodology.Prototyping:
                    {
                        return "Прототипирование";
                    }
                case OrderFlow.Domain.MainPage.Enums.Metodology.Waterfall:
                    {
                        return "Водопад";
                    }
            }

            return null;
        }
    }
}
