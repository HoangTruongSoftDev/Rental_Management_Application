using System;
using System.Collections.Generic;

namespace RentalManagementFinalProject.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int UserTypeId { get; set; }

    public int StatusId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual ICollection<Appointment> AppointmentPropertyManagers { get; set; } = new List<Appointment>();

    public virtual ICollection<Appointment> AppointmentTenants { get; set; } = new List<Appointment>();

    public virtual ICollection<Message> MessageRecipients { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

    public virtual Status Status { get; set; } = null!;

    public virtual UserType UserType { get; set; } = null!;
}
