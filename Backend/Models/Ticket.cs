using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

public class Ticket
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TicketID { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [Required]
    public int CreatorUserID { get; set; }

    [ForeignKey(nameof(CreatorUserID))]
    public virtual User? Creator { get; set; }

    [Required]
    public int StatusID { get; set; }

    [ForeignKey(nameof(StatusID))]
    public virtual TicketStatus? Status { get; set; }
}
