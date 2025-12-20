using Microsoft.AspNetCore.Http;
using News_Portal.Core.Domain.Entities;
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
        Task<string> GetDefaultProfileImageUrl();
        Task<string> UploadNewsImageToCloudinary(IFormFile profileImage);
        Task AddImage(Images image);
        Task DeleteImageById(Guid imageId);
    }
}
