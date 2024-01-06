using System;
using System.Collections.Generic;

namespace RentalManagementFinalProject.Models;

public partial class UserType
{
    public int UserTypeId { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
