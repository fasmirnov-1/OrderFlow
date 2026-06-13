using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

[Index("Slug", Name = "IX_BlogCategories_Slug")]
[Index("Slug", Name = "UQ__BlogCate__BC7B5FB656B2E32C", IsUnique = true)]
public partial class BlogCategory
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string Slug { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<BlogPost> BlogPosts { get; } = new List<BlogPost>();
}
