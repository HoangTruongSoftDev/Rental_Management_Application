using System;
using System.Collections.Generic;

namespace RentalManagementFinalProject.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int PropertyManagerId { get; set; }

    public int TenantId { get; set; }

    public int ApartmentId { get; set; }

    public DateTime MeetingDateTime { get; set; }

    public string AppointmentReason { get; set; } = null!;

    public virtual Apartment Apartment { get; set; } = null!;

    public virtual User PropertyManager { get; set; } = null!;

    public virtual User Tenant { get; set; } = null!;
}
