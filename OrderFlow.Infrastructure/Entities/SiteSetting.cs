using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

public partial class SiteSetting
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string SiteName { get; set; } = null!;

    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(100)]
    public string? Telegram { get; set; }

    [StringLength(100)]
    public string? WhatsApp { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    public string? MapEmbedUrl { get; set; }

    [StringLength(100)]
    public string? ResponseHours { get; set; }

    [StringLength(500)]
    public string? DefaultOgImage { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
