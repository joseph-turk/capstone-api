using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using CapstoneApi.Models;
using CapstoneApi.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CapstoneApi.Controllers
{
    [Authorize]
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

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(Guid id)
        {
            Registration registration = await _context.Registrations
                .Where(r => r.Id == id)
                .Include(r => r.Registrant)
                .FirstOrDefaultAsync();

            if (registration == null) return NotFound();

            return Ok(registration);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Create(RegistrationDto registrationDto)
        {
            PrimaryContact primaryContact = registrationDto.PrimaryContact;
            Event vmEvent = _context.Events
                .Where(e => e.Id == registrationDto.Event.Id)
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
                    .Where(pc => pc.Name.Equals(registrationDto.PrimaryContact.Name))
                    .FirstOrDefault();
            }

            // Iterate over registrants
            registrationDto.Registrants.ForEach(reg =>
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

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, Registration registration)
        {
            Registration existingRegistration = await _context.Registrations.FindAsync(id);
            if (existingRegistration == null) return NotFound();

            existingRegistration.HasPhotoRelease = registration.HasPhotoRelease;
            existingRegistration.IsWaitList = registration.IsWaitList;

            _context.Registrations.Update(existingRegistration);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            Registration registration = await _context.Registrations
                .Where(r => r.Id == id)
                .Include(r => r.Registrant)
                .Include(r => r.PrimaryContact)
                .FirstOrDefaultAsync();
            if (registration == null) return NotFound();

            Registrant registrant = await _context.Registrants
                .Where(r => r.Id == registration.Registrant.Id)
                .Include(r => r.Registrations)
                .FirstOrDefaultAsync();

            PrimaryContact primaryContact = await _context.PrimaryContacts
                .Where(p => p.Id == registration.PrimaryContact.Id)
                .Include(p => p.Registrations)
                .FirstOrDefaultAsync();

            _context.Registrations.Remove(registration);

            // If no other registrations for registrant, remove as well
            if (registrant.Registrations.Count == 1)
            {
                _context.Registrants.Remove(registrant);
            }

            // If no other registrations for primary contact, remove as well
            if (primaryContact.Registrations.Count == 1)
            {
                _context.PrimaryContacts.Remove(primaryContact);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}