using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using News_Portal.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public ImageService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }   

        public async Task<string> UploadToCloudinary(IFormFile profileImage)
        {
            await using var stream = profileImage.OpenReadStream();

            var uploadParams = new CloudinaryDotNet.Actions.ImageUploadParams()
            {
                File = new CloudinaryDotNet.FileDescription(profileImage.FileName, stream),
                Folder = "NewsPortal/ProfileImages"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString(); 
            }

            return "https://res.cloudinary.com/dwkr48bj7/image/upload/User_fiy61j.jpg";
        }
    }
}
