using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

[Index("CreatedAt", Name = "IX_ContactMessages_CreatedAt")]
[Index("IsRead", Name = "IX_ContactMessages_IsRead")]
public partial class ContactMessage
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    [StringLength(200)]
    public string? Email { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsRead { get; set; }

    public DateTime? RepliedAt { get; set; }
}
