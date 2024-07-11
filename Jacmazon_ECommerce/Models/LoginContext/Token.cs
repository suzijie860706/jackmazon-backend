using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Jacmazon_ECommerce.Models.LoginContext;

public partial class Token
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    /// <summary>帳號</summary>
    public string Account { get; set; } = null!;

    public DateTime ExpiredDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
