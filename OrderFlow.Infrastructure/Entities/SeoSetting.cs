using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

[Index("PageRoute", Name = "IX_SeoSettings_PageRoute")]
[Index("PageRoute", Name = "UQ__SeoSetti__B16F0842319386AC", IsUnique = true)]
public partial class SeoSetting
{
    [Key]
    public int Id { get; set; }

    [StringLength(500)]
    public string PageRoute { get; set; } = null!;

    [StringLength(500)]
    public string? MetaTitle { get; set; }

    [StringLength(1000)]
    public string? MetaDescription { get; set; }

    [StringLength(500)]
    public string? MetaKeywords { get; set; }

    [StringLength(1000)]
    public string? CanonicalUrl { get; set; }

    [StringLength(100)]
    public string? Robots { get; set; }

    [StringLength(500)]
    public string? OgTitle { get; set; }

    [StringLength(1000)]
    public string? OgDescription { get; set; }

    [StringLength(1000)]
    public string? OgImage { get; set; }

    [StringLength(500)]
    public string? OgImageAlt { get; set; }

    [StringLength(50)]
    public string? OgType { get; set; }

    public string? JsonLdSchema { get; set; }
}
