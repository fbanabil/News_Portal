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
        #region Cloudinary Methods
        //Task<string> UploadToCloudinary(IFormFile profileImage);
        //Task<bool> DeleteFromCloudinary(string imageUrl);
        //Task<bool> DeleteFromCloudinaryByPublicId(string publicId);
        //Task<string> UploadNewsImageToCloudinary(IFormFile profileImage);
        #endregion



        Task<string> GetDefaultProfileImageUrl();
        Task<string> GetDefaultNewsImageUrl();

        Task AddImage(Images image);
        Task DeleteImageById(Guid imageId);

        Task<string> SaveProfileImage(IFormFile image);
        Task<string> SaveNewsImage(IFormFile image);
        Task<bool> RemoveImage(string relativePath);

    }
}
