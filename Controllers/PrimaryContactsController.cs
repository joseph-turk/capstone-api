using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapstoneApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CapstoneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrimaryContactsController : ControllerBase
    {
        private readonly CapstoneContext _context;

        public PrimaryContactsController(CapstoneContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<PrimaryContact>> GetAll()
        {
            Guid userId;
            Guid.TryParse(User.Identity.Name, out userId);

            User user = _context.Users.Find(userId);
            if (user == null) return BadRequest();

            if (user.IsAdmin)
            {
                return _context.PrimaryContacts
                    .Include(pc => pc.Registrations)
                    .ToList();
            }
            else
            {
                List<Event> events = _context.Events
                    .Where(e => e.CreatedBy.Id == user.Id)
                    .Include(e => e.Registrations)
                    .ThenInclude(r => r.PrimaryContact)
                    .ToList();

                List<PrimaryContact> primaryContacts = new List<PrimaryContact>();

                events.ForEach(e =>
                {
                    e.Registrations.ForEach(r =>
                    {
                        if (!primaryContacts.Any(pc => pc.Id == r.PrimaryContact.Id))
                        {
                            primaryContacts.Add(r.PrimaryContact);
                        }
                    });
                });

                return primaryContacts;
            }
        }
    }
}