using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

[Index("Slug", Name = "IX_Tags_Slug")]
[Index("Name", Name = "UQ__Tags__737584F6434C9489", IsUnique = true)]
[Index("Slug", Name = "UQ__Tags__BC7B5FB61718C60F", IsUnique = true)]
public partial class Tag
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string Slug { get; set; } = null!;

    [ForeignKey("TagId")]
    [InverseProperty("Tags")]
    public virtual ICollection<BlogPost> BlogPosts { get; } = new List<BlogPost>();
}
