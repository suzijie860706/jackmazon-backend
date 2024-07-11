using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Jacmazon_ECommerce.Models.LoginContext;

/// <summary>
/// 權杖
/// </summary>
public partial class Token
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    /// <summary>長期Token</summary>
    public string RefreshToken { get; set; } = null!;

    [JsonIgnore]
    public DateTime ExpiredDate { get; set; }

    [JsonIgnore]
    public DateTime CreatedDate { get; set; }

    [JsonIgnore]
    public DateTime UpdatedDate { get; set; }
}
