using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.ServiceContracts
{
    public interface IImageService
    {
        Task<string> UploadToCloudinary(IFormFile profileImage);
        Task<bool> DeleteFromCloudinary(string imageUrl);
        Task<bool> DeleteFromCloudinaryByPublicId(string publicId);
    }
}
