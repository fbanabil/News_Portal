using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Helpers
{
    public class ImageHelper
    {
        public async Task<byte[]> ResizeImage(IFormFile file, int width, int height)
        {
            await using var stream = file.OpenReadStream();
            using var image = await Image.LoadAsync(stream);

            image.Mutate(x =>
            {
                x.AutoOrient();

                if (image.Width > width || image.Height > height)
                {
                    x.Resize(new ResizeOptions
                    {
                        Size = new Size(width, height),
                        Mode = ResizeMode.Max
                    });
                }
            });

            await using var ms = new MemoryStream();
            await image.SaveAsJpegAsync(ms, new JpegEncoder
            {
                Quality = 85
            });

            return ms.ToArray();
        }

    }
}
