using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Jacmazon_ECommerce.Models.LoginContext;

[ModelMetadataType(typeof(TokenMetaData))]
public partial class Token
{
}

public class TokenMetaData
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
