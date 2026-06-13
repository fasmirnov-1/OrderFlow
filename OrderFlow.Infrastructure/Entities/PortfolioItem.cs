using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

[Index("CreatedAt", Name = "IX_PortfolioItems_CreatedAt")]
[Index("IsPublished", Name = "IX_PortfolioItems_IsPublished")]
[Index("Methodology", Name = "IX_PortfolioItems_Methodology")]
public partial class PortfolioItem
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    [StringLength(500)]
    public string? PreviewImageUrl { get; set; }

    public int Methodology { get; set; }

    public string? Technologies { get; set; }

    public int? DurationDays { get; set; }

    [StringLength(500)]
    public string? ProjectUrl { get; set; }

    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("PortfolioItemId")]
    [InverseProperty("PortfolioItems")]
    public virtual ICollection<PortfolioCategory> Categories { get; } = new List<PortfolioCategory>();

    [ForeignKey("PortfolioItemId")]
    [InverseProperty("PortfolioItems")]
    public virtual ICollection<Technology> TechnologiesNavigation { get; } = new List<Technology>();
}
