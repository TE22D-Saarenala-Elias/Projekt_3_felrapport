using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Backend.Data;
using Backend.Models;
using Backend.Models.DTOs;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TicketsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetTickets()
    {
        var tickets = await _context.Tickets
            .Include(t => t.Creator)
            .Include(t => t.Status)
            .OrderByDescending(t => t.DateCreated)
            .Select(t => new TicketDto
            {
                TicketID = t.TicketID,
                Title = t.Title,
                Description = t.Description,
                DateCreated = t.DateCreated,
                CreatorUsername = t.Creator!.Username,
                StatusName = t.Status!.StatusName,
                StatusID = t.StatusID
            })
            .ToListAsync();

        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDto>> GetTicket(int id)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Creator)
            .Include(t => t.Status)
            .Where(t => t.TicketID == id)
            .Select(t => new TicketDto
            {
                TicketID = t.TicketID,
                Title = t.Title,
                Description = t.Description,
                DateCreated = t.DateCreated,
                CreatorUsername = t.Creator!.Username,
                StatusName = t.Status!.StatusName,
                StatusID = t.StatusID
            })
            .FirstOrDefaultAsync();

        if (ticket == null)
        {
            return NotFound(new { message = "Rapporten hittades inte" });
        }

        return Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<TicketDto>> CreateTicket([FromBody] CreateTicketDto createTicketDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized(new { message = "Användare inte autentiserad" });
        }

        var userId = int.Parse(userIdClaim.Value);

        // Get default "Ny" status
        var nyStatus = await _context.TicketStatuses.FirstOrDefaultAsync(s => s.StatusName == "Ny");
        if (nyStatus == null)
        {
            return BadRequest(new { message = "Standard status hittades inte" });
        }

        var ticket = new Ticket
        {
            Title = createTicketDto.Title,
            Description = createTicketDto.Description,
            DateCreated = DateTime.UtcNow,
            CreatorUserID = userId,
            StatusID = nyStatus.StatusID
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        // Reload with related data
        var createdTicket = await _context.Tickets
            .Include(t => t.Creator)
            .Include(t => t.Status)
            .Where(t => t.TicketID == ticket.TicketID)
            .Select(t => new TicketDto
            {
                TicketID = t.TicketID,
                Title = t.Title,
                Description = t.Description,
                DateCreated = t.DateCreated,
                CreatorUsername = t.Creator!.Username,
                StatusName = t.Status!.StatusName,
                StatusID = t.StatusID
            })
            .FirstOrDefaultAsync();

        return CreatedAtAction(nameof(GetTicket), new { id = ticket.TicketID }, createdTicket);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTicketStatus(int id, [FromBody] UpdateTicketStatusDto updateDto)
    {
        var ticket = await _context.Tickets.FindAsync(id);

        if (ticket == null)
        {
            return NotFound(new { message = "Rapporten hittades inte" });
        }

        var status = await _context.TicketStatuses.FindAsync(updateDto.StatusID);
        if (status == null)
        {
            return BadRequest(new { message = "Ogiltig status" });
        }

        ticket.StatusID = updateDto.StatusID;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Status uppdaterad framgångsrikt" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);

        if (ticket == null)
        {
            return NotFound(new { message = "Rapporten hittades inte" });
        }

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Rapport borttagen framgångsrikt" });
    }

    [HttpGet("statuses")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<TicketStatus>>> GetStatuses()
    {
        var statuses = await _context.TicketStatuses.ToListAsync();
        return Ok(statuses);
    }
}
