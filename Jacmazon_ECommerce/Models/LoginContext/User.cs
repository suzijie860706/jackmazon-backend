using System;
using System.Collections.Generic;

namespace Jacmazon_ECommerce.Models.LoginContext;

public partial class User
{
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
    /// 密碼鹽
    /// </summary>
    public byte[] Salt { get; set; } = null!;

    /// <summary>
    /// 名稱
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 權限
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// 啟用
    /// </summary>
    public bool Approved { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 更新日期
    /// </summary>
    public DateTime UpdateDate { get; set; }

    /// <summary>
    /// 新增日期
    /// </summary>
    public DateTime CreateDate { get; set; }
}
