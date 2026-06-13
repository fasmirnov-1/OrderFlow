using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

public partial class AspNetUser
{
    [Key]
    public string Id { get; set; } = null!;

    [StringLength(256)]
    public string? UserName { get; set; }

    [StringLength(256)]
    public string? NormalizedUserName { get; set; }

    [StringLength(256)]
    public string? Email { get; set; }

    [StringLength(256)]
    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    [Required]
    public bool? LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; } = new List<AspNetUserClaim>();

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; } = new List<AspNetUserLogin>();

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; } = new List<AspNetUserToken>();

    [ForeignKey("UserId")]
    [InverseProperty("Users")]
    public virtual ICollection<AspNetRole> Roles { get; } = new List<AspNetRole>();
}
