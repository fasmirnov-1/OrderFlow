using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

[Index("CreatedAt", Name = "IX_SystemNotifications_CreatedAt")]
[Index("IsRead", Name = "IX_SystemNotifications_IsRead")]
public partial class SystemNotification
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public int Type { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    [Required]
    public bool? ForAdminOnly { get; set; }
}
