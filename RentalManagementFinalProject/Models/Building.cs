using System;
using System.Collections.Generic;

namespace RentalManagementFinalProject.Models;

public partial class Building
{
    public int BuildingId { get; set; }

    public string BuildingName { get; set; } = null!;

    public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
}
