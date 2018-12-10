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
            return _context.Registrants
                .Include(r => r.Registrations)
                .ToList();
        }
    }
}