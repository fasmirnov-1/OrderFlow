using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

[Index("CategoryId", Name = "IX_BlogPosts_CategoryId")]
[Index("IsPublished", Name = "IX_BlogPosts_IsPublished")]
[Index("PublishedAt", Name = "IX_BlogPosts_PublishedAt")]
[Index("Slug", Name = "IX_BlogPosts_Slug")]
[Index("Id", Name = "UIX_BlogPosts_FullText", IsUnique = true)]
[Index("Slug", Name = "UQ__BlogPost__BC7B5FB64FDBCA4B", IsUnique = true)]
public partial class BlogPost
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    [StringLength(200)]
    public string Slug { get; set; } = null!;

    [StringLength(500)]
    public string Excerpt { get; set; } = null!;

    public string Content { get; set; } = null!;

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public int? CategoryId { get; set; }

    public DateTime? PublishedAt { get; set; }

    public bool IsPublished { get; set; }

    public int ViewsCount { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("BlogPosts")]
    public virtual BlogCategory? Category { get; set; }

    [ForeignKey("BlogPostId")]
    [InverseProperty("BlogPosts")]
    public virtual ICollection<Tag> Tags { get; } = new List<Tag>();
}
