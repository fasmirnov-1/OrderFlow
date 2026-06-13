using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

[Index("Name", Name = "UQ__EmailTem__737584F63173D43D", IsUnique = true)]
public partial class EmailTemplate
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string Subject { get; set; } = null!;

    public string BodyHtml { get; set; } = null!;

    public string? Variables { get; set; }

    [Required]
    public bool? IsActive { get; set; }
}
