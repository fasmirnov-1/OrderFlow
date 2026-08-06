namespace OrderFlow.Infrastructure.Entities;

public partial class UserSession
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string SessionHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    // Навигационное свойство для связи с пользователем
    public virtual AspNetUser User { get; set; } = null!;
}