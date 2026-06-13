using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OrderFlow.Domain.MainPage.Models;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Application.Extensions
{
    public static class ServiceExtension
    {
        public static List<Services>ToServiceModel(this List<Service> service)
        {
            List<Services> services = new List<Services>();
            service.ForEach(s => services.Add(new Services
            {
                Name = s.Title,
                Description = s.Description
            }));
            return services;
        }
    }
}
