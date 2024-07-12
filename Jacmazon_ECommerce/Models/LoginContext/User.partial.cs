using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

/// <summary>
/// partial class User
/// </summary>
public partial class User
{
    [Key]
    [JsonIgnore]
    public int Id { get; set; }

    /// <summary>
    /// 電子信箱
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// 密碼
    /// </summary>
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