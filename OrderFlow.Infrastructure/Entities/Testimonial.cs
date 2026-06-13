using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

public partial class Testimonial
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string ClientName { get; set; } = null!;

    [StringLength(200)]
    public string? Company { get; set; }

    public string Text { get; set; } = null!;

    public int? Rating { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }
}
