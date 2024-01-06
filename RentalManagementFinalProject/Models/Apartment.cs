using System;
using System.Collections.Generic;

namespace RentalManagementFinalProject.Models;

public partial class Apartment
{
    public int ApartmentNumber { get; set; }

    public int BuildingId { get; set; }

    public int StatusId { get; set; }

    public string Description { get; set; } = null!;

    public string Type { get; set; } = null!;

    public double Price { get; set; }

    public int NumberOfRoom { get; set; }

    public int NumberOfBathroom { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Building Building { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
