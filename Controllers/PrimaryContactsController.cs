using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapstoneApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            return _context.PrimaryContacts
                .Include(pc => pc.Registrations)
                .ToList();
        }
    }
}