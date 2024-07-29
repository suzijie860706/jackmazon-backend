using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Jacmazon_ECommerce.Extensions;

namespace Jacmazon_ECommerce.Models.LoginContext;

[ModelMetadataType(typeof(UserMetaData))]
public partial class User
{
}

public class UserMetaData
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    /// <summary>
    /// 電子信箱
    /// </summary>
    [Required(ErrorMessage = "請輸入Email地址")]
    [Length(minimumLength: 6, maximumLength: 30, ErrorMessage = "很抱歉，使用者名稱長度必須介於 6 到 30 個半形字元之間")]
    [EmailValidate(ErrorMessage = "Email格式錯誤")]
    public string Email { get; set; } = null!;

    /// <summary>
    /// 密碼
    /// </summary>
    [Required(ErrorMessage = "此欄位必填")]
    [MinLength(8, ErrorMessage = "密碼長度至少須為 8 個字元")]
    [MaxLength(100, ErrorMessage = "密碼長度最多為 100 個字元")]
    public string Password { get; set; } = null!;

    /// <summary>
    /// 名稱
    /// </summary>
    [JsonIgnore]
    public string? Name { get; set; }

    /// <summary>
    /// 權限
    /// </summary>
    [JsonIgnore]
    public int Rank { get; set; }

    /// <summary>
    /// 啟用
    /// </summary>
    [JsonIgnore]
    public bool Approved { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    [JsonIgnore]
    public string? Phone { get; set; }

    /// <summary>
    /// 更新日期
    /// </summary>
    [JsonIgnore]
    public DateTime UpdateDate { get; set; }

    /// <summary>
    /// 新增日期
    /// </summary>
    [JsonIgnore]
    public DateTime CreateDate { get; set; }
}