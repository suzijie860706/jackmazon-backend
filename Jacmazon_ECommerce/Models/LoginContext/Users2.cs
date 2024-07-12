﻿using System;
using System.Collections.Generic;

namespace Jacmazon_ECommerce.Models.LoginContext;

public partial class Users2
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Name { get; set; }

    public int Rank { get; set; }

    public bool Approved { get; set; }

    public string? Phone { get; set; }

    public DateTime UpdateDate { get; set; }

    public DateTime CreateDate { get; set; }
}
