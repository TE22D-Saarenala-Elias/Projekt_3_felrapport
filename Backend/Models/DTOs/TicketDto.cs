namespace Backend.Models.DTOs;

public class TicketDto
{
    public int TicketID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string CreatorUsername { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public int StatusID { get; set; }
}
