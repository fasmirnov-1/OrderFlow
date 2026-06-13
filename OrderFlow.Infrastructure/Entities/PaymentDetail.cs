using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Infrastructure.Entities;

public partial class PaymentDetail
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string? Inn { get; set; }

    [StringLength(200)]
    public string? BankName { get; set; }

    [StringLength(50)]
    public string? AccountNumber { get; set; }

    [StringLength(50)]
    public string? CardNumber { get; set; }

    [StringLength(200)]
    public string? CryptoWallet { get; set; }

    [StringLength(500)]
    public string? YooMoneyLink { get; set; }
}
