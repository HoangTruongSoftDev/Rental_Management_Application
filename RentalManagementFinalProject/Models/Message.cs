using System;
using System.Collections.Generic;

namespace RentalManagementFinalProject.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int RecipientId { get; set; }

    public int SenderId { get; set; }

    public string MessageContent { get; set; } = null!;

    public DateTime TimeCreated { get; set; }

    public virtual User Recipient { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
