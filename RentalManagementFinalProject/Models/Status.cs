using System;
using System.Collections.Generic;

namespace RentalManagementFinalProject.Models;

public partial class Status
{
    public int StatusId { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
