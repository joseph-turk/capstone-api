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
        public ActionResult<List<Event>> GetAll()
        {
            return _context.Events.ToList();
        }

        [HttpGet("{id}", Name = "GetEvent")]
        public async Task<ActionResult> GetById(Guid id)
        {
            Event item = await _context.Events.FindAsync(id);

            Guid userId;
            Guid.TryParse(this.User.Identity.Name, out userId);
            User user = await _context.Users.FindAsync(userId);

            bool myEvent = false;

            if (item.CreatedBy != null)
            {
                myEvent = userId.Equals(item.CreatedBy.Id);
            }

            if (item == null) return NotFound();

            EventDto eventDto = new EventDto
            {
                Event = item
            };

            if (user != null)
            {
                eventDto.IsMyEvent = myEvent || user.IsAdmin;
            }
            else
            {
                eventDto.IsMyEvent = myEvent;
            }

            return Ok(eventDto);
        }

        [HttpPost]
        [Authorize]
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
        public IActionResult Update(Guid id, Event item)
        {
            Event existingEvent = _context.Events.Find(id);
            if (existingEvent == null) return NotFound();

            existingEvent.Name = item.Name;
            existingEvent.Description = item.Description;
            existingEvent.Start = item.Start;
            existingEvent.End = item.End;
            existingEvent.Capacity = item.Capacity;

            _context.Events.Update(existingEvent);
            _context.SaveChanges();

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