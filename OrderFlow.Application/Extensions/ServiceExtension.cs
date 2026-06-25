using OrderFlow.Domain.MainPage.Models;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Application.Extensions
{
    public static class ServiceExtension
    {
        /// <summary>
        /// Преобразует коллекцию инфраструктурных сущностей Service в доменные модели Services.
        /// </summary>
        public static List<Services> ToServiceModel(this IEnumerable<Service> services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // Оптимизация памяти: если размер коллекции известен заранее, 
            // выделяем под список точный объем памяти без лишних аллокаций.
            var result = services is ICollection<Service> collection
                ? new List<Services>(collection.Count)
                : new List<Services>();

            // Классический цикл foreach работает быстрее вызова делегата List.ForEach
            foreach (var service in services)
            {
                result.Add(new Services
                {
                    Name = service.Title,
                    Description = service.Description
                });
            }

            return result;
        }
    }
}