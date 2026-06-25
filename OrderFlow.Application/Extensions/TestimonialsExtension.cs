using OrderFlow.Domain.MainPage.Models;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Application.Extensions
{
    public static class TestimonialsExtension
    {
        /// <summary>
        /// Преобразует коллекцию инфраструктурных сущностей Testimonial в доменные модели Feedback.
        /// </summary>
        public static List<Feedback> ToFeedbackModel(this IEnumerable<Testimonial> testimonials)
        {
            if (testimonials == null) throw new ArgumentNullException(nameof(testimonials));

            // Оптимизация памяти: если размер коллекции известен заранее, 
            // выделяем под список точный объем памяти без лишних аллокаций.
            var feedbacks = testimonials is ICollection<Testimonial> collection
                ? new List<Feedback>(collection.Count)
                : new List<Feedback>();

            // Классический цикл foreach работает быстрее вызова делегата List.ForEach
            foreach (var testimonial in testimonials)
            {
                feedbacks.Add(new Feedback
                {
                    Name = testimonial.ClientName,
                    Company = testimonial.Company,
                    Text = testimonial.Text,

                    // Приведение типа рейтинга к доменному Enum. 
                    // Проверьте написание свойства (Rating/Raiting) в вашей модели Feedback.
                    Raiting = (Raiting)testimonial.Rating
                });
            }

            return feedbacks;
        }
    }
}