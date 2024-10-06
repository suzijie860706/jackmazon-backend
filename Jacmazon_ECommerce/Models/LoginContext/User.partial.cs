using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Jacmazon_ECommerce.Attributes;

namespace Jacmazon_ECommerce.Models.LoginContext;

public class UserMetaData
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    /// <summary>
    /// 電子信箱
    /// </summary>
    [Required(ErrorMessage = "請輸入Email地址")]
    [EmailValidate(ErrorMessage = "Email格式錯誤")]
    [EmailLength(minimumLength: 6, maximumLength: 30)]
    public string Email { get; set; } = null!;

    /// <summary>
    /// 密碼
    /// </summary>
    [Required(ErrorMessage = "此欄位必填")]
    [MinLength(8, ErrorMessage = "密碼長度至少須為 8 個字元")]
    [MaxLength(100, ErrorMessage = "密碼長度最多為 100 個字元")]
    public string Password { get; set; } = null!;

    /// <summary>
    /// 密碼鹽
    /// </summary>
    [JsonIgnore]
    public byte[] Salt { get; set; } = null!;

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