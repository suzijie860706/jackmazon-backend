using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

/// <summary>
/// partial Token
/// </summary>
public partial class Token
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    public string RefreshToken { get; set; } = null!;

    [JsonIgnore]
    public DateTime ExpiredDate { get; set; }

    [JsonIgnore]
    public DateTime CreatedDate { get; set; }

    [JsonIgnore]
    public DateTime UpdatedDate { get; set; }
}
