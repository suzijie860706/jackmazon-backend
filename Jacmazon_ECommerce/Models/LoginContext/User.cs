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

    /// <summary>密碼</summary>
    public string Password { get; set; } = null!;

    /// <summary>名稱</summary>
    public string Name { get; set; } = null!;

    [JsonIgnore]
    public int Rank { get; set; }

    [JsonIgnore]
    public bool Approved { get; set; }

    /// <summary>電話</summary>
    public string? Phone { get; set; }

    /// <summary>電子信箱</summary>
    public string? Email { get; set; }

    [JsonIgnore]
    public DateTime UpdateDate { get; set; }

    [JsonIgnore]
    public DateTime CreateDate { get; set; }
}
