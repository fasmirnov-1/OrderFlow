using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

public partial class Service
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    [StringLength(100)]
    public string? IconClass { get; set; }

    [Required]
    public bool? IsActive { get; set; }

    public int DisplayOrder { get; set; }
}
