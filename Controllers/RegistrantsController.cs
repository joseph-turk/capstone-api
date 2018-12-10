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
    public class RegistrantsController : ControllerBase
    {
        private readonly CapstoneContext _context;

        public RegistrantsController(CapstoneContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Registrant>> GetAll()
        {
            Guid userId;
            Guid.TryParse(User.Identity.Name, out userId);

            User user = _context.Users.Find(userId);
            if (user == null) return BadRequest();

            if (user.IsAdmin)
            {
                return _context.Registrants
                    .Include(r => r.Registrations)
                    .ToList();
            }
            else
            {
                List<Event> events = _context.Events
                    .Where(e => e.CreatedBy.Id == user.Id)
                    .Include(e => e.Registrations)
                    .ThenInclude(r => r.Registrant)
                    .ToList();

                List<Registrant> registrants = new List<Registrant>();

                events.ForEach(e =>
                {
                    e.Registrations.ForEach(r =>
                    {
                        if (!registrants.Any(reg => reg.Id == r.Registrant.Id))
                        {
                            registrants.Add(r.Registrant);
                        }
                    });
                });

                return registrants;
            }

        }
    }
}