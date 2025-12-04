using News_Portal.Core.DTO.Image;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.Image
{
    public class ImageToShowDTO
    {
        public Guid ImageId { get; set; }

        public string? ImageUrl { get; set; }

    }
}

public static class ImageExtension
{
    public static ImageToShowDTO ToImageToShowDTO(this News_Portal.Core.Domain.Entities.Images image)
    {
        return new ImageToShowDTO
        {
            ImageId = image.ImageId,
            ImageUrl = image.ImageUrl
        };
    }
}