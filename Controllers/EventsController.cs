using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapstoneApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using CapstoneApi.Services;
using CapstoneApi.Dtos;

namespace CapstoneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly CapstoneContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ImageService imageService;

        public class EventRequestBody
        {
            public string name { get; set; }
            public string description { get; set; }
            public DateTime start { get; set; }
            public DateTime end { get; set; }
            public int capacity { get; set; }
        }

        public EventsController(CapstoneContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            imageService = new ImageService();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetAll()
        {
            List<EventDto> allEventsDto = new List<EventDto>();
            List<Event> allEvents = await _context.Events
                    .Include(e => e.Registrations)
                    .ToListAsync();

            allEvents.ForEach(e =>
            {
                allEventsDto.Add(new EventDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Start = e.Start,
                    End = e.End,
                    Capacity = e.Capacity,
                    ImageId = e.ImageId,
                    ImageExtension = e.ImageExtension,
                    IsMyEvent = false,
                    Registrations = null,
                    RegistrationCount = e.Registrations.Where(r => !r.IsWaitList).Count(),
                    WaitListCount = e.Registrations.Where(r => r.IsWaitList).Count()
                });
            });

            return Ok(allEventsDto);
        }

        [HttpGet("foruser")]
        public async Task<ActionResult> GetAllForUser()
        {
            Guid userId;
            Guid.TryParse(User.Identity.Name, out userId);
            User user = _context.Users.Find(userId);

            List<EventDto> allEventsDto = new List<EventDto>();
            List<Event> allEvents;

            if (user.IsAdmin)
            {
                allEvents = await _context.Events
                    .Include(e => e.Registrations)
                    .ToListAsync();
            }
            else
            {
                allEvents = await _context.Events
                    .Where(e => e.CreatedBy == user)
                    .Include(e => e.Registrations)
                    .ToListAsync();
            }

            allEvents.ForEach(e =>
            {
                allEventsDto.Add(new EventDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Start = e.Start,
                    End = e.End,
                    Capacity = e.Capacity,
                    ImageId = e.ImageId,
                    ImageExtension = e.ImageExtension,
                    IsMyEvent = false,
                    Registrations = null,
                    RegistrationCount = e.Registrations.Where(r => !r.IsWaitList).Count(),
                    WaitListCount = e.Registrations.Where(r => r.IsWaitList).Count()
                });
            });

            return Ok(allEventsDto);
        }

        [HttpGet("{id}", Name = "GetEvent")]
        [AllowAnonymous]
        public async Task<ActionResult> GetById(Guid id)
        {
            Event item = await _context.Events.FindAsync(id);
            if (item == null) return NotFound();

            List<Registration> registrations = await _context.Registrations
                .Where(r => r.Event.Id == id)
                .Include(r => r.PrimaryContact)
                .Include(r => r.Registrant)
                .ToListAsync();

            Guid userId;
            Guid.TryParse(this.User.Identity.Name, out userId);
            User user = await _context.Users.FindAsync(userId);

            bool myEvent = false;

            if (item.CreatedBy != null)
            {
                myEvent = userId.Equals(item.CreatedBy.Id);
            }

            EventDto eventDto = new EventDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Start = item.Start,
                End = item.End,
                Capacity = item.Capacity,
                ImageId = item.ImageId,
                ImageExtension = item.ImageExtension,
                RegistrationCount = registrations.Where(r => !r.IsWaitList).Count(),
                WaitListCount = registrations.Where(r => r.IsWaitList).Count()
            };

            if (user != null)
            {
                eventDto.IsMyEvent = myEvent || user.IsAdmin;
                eventDto.Registrations = myEvent || user.IsAdmin ? registrations : null;
            }
            else
            {
                eventDto.IsMyEvent = myEvent;
            }

            return Ok(eventDto);
        }

        [HttpPost]
        public IActionResult Create()
        {
            if (Request.HasFormContentType)
            {
                Guid imageId = Guid.NewGuid();
                IFormCollection form = Request.Form;
                string name = form["name"];
                string description = form["description"];
                DateTime start = Convert.ToDateTime(form["start"]).ToUniversalTime();
                DateTime end = Convert.ToDateTime(form["end"]).ToUniversalTime();
                int capacity = int.Parse(form["capacity"]);
                IFormFile image = form.Files.FirstOrDefault();
                string fileExtension = String.Empty;
                User createdBy = _context.Users.Find(Guid.Parse(this.User.Identity.Name));

                // Handle upload
                if (image != null)
                {
                    // Create directory for uploads
                    string uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                    string imageDir = Path.Combine(uploads, imageId.ToString());
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    Directory.CreateDirectory(imageDir);

                    // Get file extension
                    fileExtension = Path.GetExtension(image.FileName);

                    Image<Rgba32> resizedImage = imageService.CreateThumbnail(image);
                    Image<Rgba32> heroImage = imageService.CreateHero(image);

                    // Set paths for saving files
                    string uniqueFileName = String.Concat("full", fileExtension);
                    string thumbnailFileName = String.Concat("thumbnail", fileExtension);
                    string heroFileName = String.Concat("hero", fileExtension);
                    string filePath = Path.Combine(imageDir, uniqueFileName);
                    string thumbnailPath = Path.Combine(imageDir, thumbnailFileName);
                    string heroPath = Path.Combine(imageDir, heroFileName);

                    // Save full-size file
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(fileStream);
                        fileStream.Flush();
                    }

                    // Save thumbnail and hero
                    resizedImage.Save(thumbnailPath);
                    heroImage.Save(heroPath);
                }

                Event item = new Event
                {
                    Name = name,
                    Description = description,
                    Start = start,
                    End = end,
                    Capacity = capacity,
                    ImageId = imageId,
                    ImageExtension = fileExtension,
                    CreatedBy = createdBy
                };
                _context.Events.Add(item);
                _context.SaveChanges();

                return CreatedAtRoute("GetEvent", new Event { Id = item.Id }, item);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id)
        {
            User editedBy = _context.Users.Find(Guid.Parse(this.User.Identity.Name));
            Event existingEvent = _context.Events
                .Where(e => e.Id == id)
                .Include(e => e.CreatedBy)
                .First();
            if (existingEvent == null) return NotFound();
            if (existingEvent.CreatedBy.Id != editedBy.Id && !editedBy.IsAdmin)
            {
                return BadRequest();
            }

            if (Request.HasFormContentType)
            {
                IFormCollection form = Request.Form;

                // Update Fields
                existingEvent.Name = form["name"];
                existingEvent.Description = form["description"];
                existingEvent.Start = Convert.ToDateTime(form["start"]).ToUniversalTime();
                existingEvent.End = Convert.ToDateTime(form["end"]).ToUniversalTime();
                existingEvent.Capacity = int.Parse(form["capacity"]);

                // Update Image
                IFormFile image = form.Files.FirstOrDefault();
                if (image != null)
                {
                    Guid imageId = Guid.NewGuid();

                    string uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                    string imageDir = Path.Combine(uploads, imageId.ToString());
                    string oldDir = Path.Combine(uploads, existingEvent.ImageId.ToString());
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                    // Delete existing images
                    if (Directory.Exists(oldDir)) Directory.Delete(oldDir, true);

                    //Create new image directory
                    Directory.CreateDirectory(imageDir);

                    // Get file extension
                    string fileExtension = Path.GetExtension(image.FileName);

                    Image<Rgba32> resizedImage = imageService.CreateThumbnail(image);
                    Image<Rgba32> heroImage = imageService.CreateHero(image);

                    // Set paths for saving files
                    string uniqueFileName = String.Concat("full", fileExtension);
                    string thumbnailFileName = String.Concat("thumbnail", fileExtension);
                    string heroFileName = String.Concat("hero", fileExtension);
                    string filePath = Path.Combine(imageDir, uniqueFileName);
                    string thumbnailPath = Path.Combine(imageDir, thumbnailFileName);
                    string heroPath = Path.Combine(imageDir, heroFileName);

                    // Save full-size file
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(fileStream);
                        fileStream.Flush();
                    }

                    // Save thumbnail and hero
                    resizedImage.Save(thumbnailPath);
                    heroImage.Save(heroPath);

                    existingEvent.ImageId = imageId;
                    existingEvent.ImageExtension = fileExtension;
                }

                _context.Events.Update(existingEvent);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            Event existingEvent = await _context.Events.FindAsync(id);
            if (existingEvent == null) return NotFound();

            string imagePath = Path.Combine(
                _hostingEnvironment.WebRootPath,
                "uploads",
                existingEvent.ImageId.ToString()
            );

            _context.Events.Remove(existingEvent);
            await _context.SaveChangesAsync();

            System.IO.Directory.Delete(imagePath, true);

            return NoContent();
        }
    }
}