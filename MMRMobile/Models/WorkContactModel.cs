using System;
using System.ComponentModel.DataAnnotations;

namespace MMRMobile.Models;

public class WorkContactModel
{
    public int WorkId { get; set; }
    public virtual WorkModel Work { get; set; }
    public int ContactId { get; set; }
    public virtual ContactModel Contact { get; set; }

    [Required] public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    [Required] public DateTime DateModified { get; set; } = DateTime.UtcNow;
}