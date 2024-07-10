using System;
using System.Collections.Generic;

namespace Jacmazon_ECommerce.Models;

public partial class DbUser
{
    public int Id { get; set; }

    public string UserAccount { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public int UserRank { get; set; }

    public bool UserApproved { get; set; }
}
