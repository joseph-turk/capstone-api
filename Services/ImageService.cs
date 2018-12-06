using Microsoft.AspNetCore.Http;
using SixLabors.Primitives;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace CapstoneApi.Services
{
    public class ImageService
    {
        public Image<Rgba32> CreateThumbnail(IFormFile file)
        {
            return ResizeImage(file, 150);
        }

        public Image<Rgba32> CreateHero(IFormFile file)
        {
            return ResizeImage(file, 1920, 1080);
        }

        private Image<Rgba32> ResizeImage(IFormFile file, int size)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                Image<Rgba32> image = Image.Load(ms.ToArray());

                return image.Clone(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(size),
                    Mode = ResizeMode.Crop
                }));
            }
        }

        private Image<Rgba32> ResizeImage(IFormFile file, int width, int height)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                Image<Rgba32> image = Image.Load(ms.ToArray());

                return image.Clone(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Mode = ResizeMode.Crop
                }));
            }
        }
    }
}