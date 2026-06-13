using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

public partial class Advantage
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? IconClass { get; set; }

    public int DisplayOrder { get; set; }
}
