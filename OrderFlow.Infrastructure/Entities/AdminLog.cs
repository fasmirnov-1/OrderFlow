using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

[Index("ActionType", Name = "IX_AdminLogs_ActionType")]
[Index("AdminUserId", Name = "IX_AdminLogs_AdminUserId")]
[Index("CreatedAt", Name = "IX_AdminLogs_CreatedAt")]
[Index("EntityName", Name = "IX_AdminLogs_EntityName")]
public partial class AdminLog
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string AdminUserId { get; set; } = null!;

    public int ActionType { get; set; }

    [StringLength(200)]
    public string EntityName { get; set; } = null!;

    public int? EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    [StringLength(50)]
    public string? IpAddress { get; set; }

    public DateTime CreatedAt { get; set; }
}
