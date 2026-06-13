using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

[Index("Slug", Name = "IX_Technologies_Slug")]
[Index("Name", Name = "UQ__Technolo__737584F616873B18", IsUnique = true)]
[Index("Slug", Name = "UQ__Technolo__BC7B5FB6545BE9C0", IsUnique = true)]
public partial class Technology
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string Slug { get; set; } = null!;

    [StringLength(50)]
    public string? Category { get; set; }

    [StringLength(100)]
    public string? IconClass { get; set; }

    [ForeignKey("TechnologyId")]
    [InverseProperty("TechnologiesNavigation")]
    public virtual ICollection<PortfolioItem> PortfolioItems { get; } = new List<PortfolioItem>();
}
