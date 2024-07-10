using System;
using System.Collections.Generic;

namespace Jacmazon_ECommerce.Models;

public partial class DbToken
{
    public int Id { get; set; }

    public string RefreshToken { get; set; } = null!;

    public DateTime ExpiredDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
