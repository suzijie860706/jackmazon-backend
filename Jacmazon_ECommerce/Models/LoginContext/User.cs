using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Jacmazon_ECommerce.Models.LoginContext;

public partial class User
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    /// <summary>帳號</summary>
    public string Account { get; set; } = null!;
    
    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Rank { get; set; }

    public bool Approved { get; set; }

    [Required]
    public DateAndTime createDate { get; set; }
}
