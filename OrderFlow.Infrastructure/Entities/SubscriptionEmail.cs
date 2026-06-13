using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

[Index("Email", Name = "IX_SubscriptionEmails_Email")]
[Index("IsActive", Name = "IX_SubscriptionEmails_IsActive")]
[Index("UnsubscribeToken", Name = "IX_SubscriptionEmails_UnsubscribeToken")]
[Index("Email", Name = "UQ__Subscrip__A9D1053428EE7911", IsUnique = true)]
[Index("UnsubscribeToken", Name = "UQ__Subscrip__EC33483CB62E3DEF", IsUnique = true)]
public partial class SubscriptionEmail
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Email { get; set; } = null!;

    [StringLength(200)]
    public string? Name { get; set; }

    public DateTime SubscribedAt { get; set; }

    [Required]
    public bool? IsActive { get; set; }

    [StringLength(100)]
    public string UnsubscribeToken { get; set; } = null!;
}
