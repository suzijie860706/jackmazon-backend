using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jacmazon_ECommerce.Models.LoginContext;

public partial class Token
{
    public int Id { get; set; }

    public string RefreshToken { get; set; } = null!;

    public DateTime ExpiredDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }
}
