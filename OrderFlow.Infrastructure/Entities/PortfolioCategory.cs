using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

[Index("Slug", Name = "IX_PortfolioCategories_Slug")]
[Index("Slug", Name = "UQ__Portfoli__BC7B5FB6134266FA", IsUnique = true)]
public partial class PortfolioCategory
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string Slug { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Categories")]
    public virtual ICollection<PortfolioItem> PortfolioItems { get; } = new List<PortfolioItem>();
}
