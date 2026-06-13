using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

[Index("CreatedAt", Name = "IX_Orders_CreatedAt")]
[Index("Email", Name = "IX_Orders_Email")]
[Index("Methodology", Name = "IX_Orders_Methodology")]
[Index("Status", Name = "IX_Orders_Status")]
public partial class Order
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string FullName { get; set; } = null!;

    [StringLength(200)]
    public string? Company { get; set; }

    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    public int Methodology { get; set; }

    public string Description { get; set; } = null!;

    public string? FilePathsJson { get; set; }

    public bool GdprConsent { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }
}
