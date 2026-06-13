using OrderFlow.Domain.MainPage.Models;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Application.Extensions
{
    public static class TestimonialsExtension
    {
        public static List<Feedback> ToFeedbackModel(this List<Testimonial> testimonials)
        {
            List<Feedback> feedbacks = new List<Feedback>();
            testimonials.ForEach(t =>
            {
                feedbacks.Add(new Feedback()
                {
                    Name = t.ClientName,
                    Company = t.Company,
                    Text = t.Text,
                    Raiting = (Raiting)t.Rating
                });
            });

            return feedbacks;
        }
    }
}
