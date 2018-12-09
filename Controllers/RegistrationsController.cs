using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using CapstoneApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationsController : ControllerBase
    {
        private readonly CapstoneContext _context;

        public RegistrationsController(CapstoneContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetAll")]
        public ActionResult<List<Registration>> GetAll()
        {
            return _context.Registrations.ToList();
        }

        [HttpPost]
        public IActionResult Create(RegistrationViewModel viewModel)
        {
            PrimaryContact primaryContact = viewModel.PrimaryContact;
            Event vmEvent = _context.Events
                .Where(e => e.Id == viewModel.Event.Id)
                .Include(e => e.Registrations)
                .ThenInclude(r => r.Registrant)
                .First();

            // Add primary contact if necessary
            if (!_context.PrimaryContacts.Any(pc => pc.Name.Equals(primaryContact.Name)
                && pc.EmailAddress.Equals(primaryContact.EmailAddress)
                && pc.PhoneNumber.Equals(primaryContact.PhoneNumber)))
            {
                _context.PrimaryContacts.Add(primaryContact);
                _context.SaveChanges();
            }
            else
            {
                primaryContact = _context.PrimaryContacts
                    .Where(pc => pc.Name.Equals(viewModel.PrimaryContact.Name))
                    .FirstOrDefault();
            }

            // Iterate over registrants
            viewModel.Registrants.ForEach(reg =>
            {
                Registrant registrant = new Registrant
                {
                    Name = reg.Name
                };
                bool isWaitList = false;

                // Add registrant if necessary
                if (!_context.Registrants.Any(r => r.Name.Equals(registrant.Name)))
                {
                    _context.Registrants.Add(registrant);
                    _context.SaveChanges();
                }
                else
                {
                    registrant = _context.Registrants
                        .Where(r => r.Name.Equals(reg.Name))
                        .FirstOrDefault();
                }

                if (vmEvent.Registrations.Where(r => !r.IsWaitList).Count()
                    >= vmEvent.Capacity)
                {
                    isWaitList = true;
                }

                // Create registration
                Registration registration = new Registration
                {
                    Event = vmEvent,
                    PrimaryContact = primaryContact,
                    Registrant = registrant,
                    HasPhotoRelease = reg.PhotoRelease,
                    IsWaitList = isWaitList
                };

                // Add registration if it doesn't already exist
                if (vmEvent.Registrations == null
                    || vmEvent.Registrations.Count == 0
                    || !vmEvent.Registrations.Any(r => r.Registrant.Name.Equals(registration.Registrant.Name)))
                {
                    _context.Registrations.Add(registration);
                    _context.SaveChanges();
                }
            });

            return CreatedAtRoute("GetAll", null);
        }
    }
}