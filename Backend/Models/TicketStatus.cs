using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class TicketStatus
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StatusID { get; set; }

    [Required]
    [MaxLength(50)]
    public string StatusName { get; set; } = string.Empty;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
