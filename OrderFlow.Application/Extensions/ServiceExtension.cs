namespace OrderFlow.Application.Extensions
{
    public static class ServiceExtension
    {
        /// <summary>
        /// Преобразует коллекцию инфраструктурных сущностей Service в доменные модели Services.
        /// </summary>
        public static List<Domain.MainPage.Models.Service> ToServiceModel(this IEnumerable<Infrastructure.Entities.Service> services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // .Select().ToList() в .NET Core автоматически проверяет, реализует ли коллекция интерфейс ICollection.
            // Если реализует, размер результирующего списка выставляется сразу, избегая лишних аллокаций памяти.
            return services.Select(service => new Domain.MainPage.Models.Service
            {
                Name = service.Title,
                Description = service.Description
            }).ToList();
        }
    }
}