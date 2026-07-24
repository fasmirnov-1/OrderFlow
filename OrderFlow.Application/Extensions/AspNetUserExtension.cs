using OrderFlow.Domain;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Application.Extensions
{
    public static class AspNetUserExtension
    {
        public static UserProfile ToUserProfile(this AspNetUser? user)
        {
            if (user == null)
            {
                return new UserProfile
                {
                    sessionHash = Guid.NewGuid().ToString()
                };
            }

            return new UserProfile
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                sessionHash = Guid.NewGuid().ToString(),
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }
    }
}