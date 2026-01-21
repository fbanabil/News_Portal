using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.RepositoryContracts;
using News_Portal.Core.Helpers;
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
        private readonly string _defaultProfileImageUrl;
        private readonly string _defaultNewsImageUrl;
        private readonly IConfiguration _configuration;
        private readonly IImageRepository _imageRepository;
        private readonly ImageHelper _imageHelper;
        public ImageService(Cloudinary cloudinary, IConfiguration configuration, IImageRepository imageRepository)
        {
            _cloudinary = cloudinary;
            _configuration = configuration;
            //_defaultImageUrl = _configuration["DefaultValues:DefaultProfileImageUrl"]
            //    ?? "https://res.cloudinary.com/dwkr48bj7/image/upload/User_fiy61j.jpg";
            _defaultProfileImageUrl = "/images/defaults/User.jpg";
            _defaultNewsImageUrl = "/images/defaults/News.jpg";
            _imageRepository = imageRepository;
            _imageHelper = new ImageHelper();
        }


        #region Cloudinary Methods

        //public async Task<string> UploadToCloudinary(IFormFile profileImage)
        //{
        //    await using var stream = profileImage.OpenReadStream();

        //    var uploadParams = new ImageUploadParams()
        //    {
        //        File = new FileDescription(profileImage.FileName, stream),
        //        Folder = "NewsPortal/ProfileImages",
        //        UseFilename = true,
        //        UniqueFilename = true,
        //        Overwrite = false
        //    };

        //    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        //    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        return uploadResult.SecureUrl.ToString(); 
        //    }

        //    return _defaultImageUrl;
        //}




        //public async Task<string> UploadNewsImageToCloudinary(IFormFile profileImage)
        //{
        //    await using var stream = profileImage.OpenReadStream();

        //    var uploadParams = new ImageUploadParams()
        //    {
        //        File = new FileDescription(profileImage.FileName, stream),
        //        Folder = "NewsPortal/NewsImages",
        //        UseFilename = true,
        //        UniqueFilename = true,
        //        Overwrite = false
        //    };

        //    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        //    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        return uploadResult.SecureUrl.ToString();
        //    }

        //    return _defaultImageUrl;
        //}



        //public async Task<bool> DeleteFromCloudinary(string imageUrl)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(imageUrl) || imageUrl == _defaultImageUrl)
        //            return false;

        //        string publicId = ExtractPublicIdFromUrl(imageUrl);

        //        if (string.IsNullOrWhiteSpace(publicId))
        //            return false;

        //        return await DeleteFromCloudinaryByPublicId(publicId);
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}




        //public async Task<bool> DeleteFromCloudinaryByPublicId(string publicId)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(publicId))
        //            return false;

        //        var deleteParams = new DeletionParams(publicId)
        //        {
        //            ResourceType = ResourceType.Image
        //        };

        //        var deletionResult = await _cloudinary.DestroyAsync(deleteParams);

        //        return deletionResult.StatusCode == System.Net.HttpStatusCode.OK && 
        //               deletionResult.Result == "ok";
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}



        //private string ExtractPublicIdFromUrl(string imageUrl)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(imageUrl))
        //            return string.Empty;


        //        Uri uri = new Uri(imageUrl);
        //        string path = uri.AbsolutePath;

        //        if (path.StartsWith("/"))
        //            path = path.Substring(1);

        //        string[] pathParts = path.Split('/');


        //        int publicIdStartIndex = -1;
        //        for (int i = 0; i < pathParts.Length; i++)
        //        {
        //            if (pathParts[i] == "upload")
        //            {
        //                publicIdStartIndex = i + 1;
        //                if (publicIdStartIndex < pathParts.Length && 
        //                    pathParts[publicIdStartIndex].StartsWith("v") && 
        //                    pathParts[publicIdStartIndex].Length > 1 &&
        //                    pathParts[publicIdStartIndex].Substring(1).All(char.IsDigit))
        //                {
        //                    publicIdStartIndex++; // Skip version
        //                }
        //                break;
        //            }
        //        }

        //        if (publicIdStartIndex == -1 || publicIdStartIndex >= pathParts.Length)
        //            return string.Empty;

        //        var publicIdParts = pathParts.Skip(publicIdStartIndex).ToArray();
        //        string publicIdWithExtension = string.Join("/", publicIdParts);

        //        int lastDotIndex = publicIdWithExtension.LastIndexOf('.');
        //        if (lastDotIndex > 0)
        //        {
        //            return publicIdWithExtension.Substring(0, lastDotIndex);
        //        }

        //        return publicIdWithExtension;
        //    }
        //    catch (Exception)
        //    {
        //        return string.Empty;
        //    }
        //}


        #endregion

        public async Task<string> GetDefaultProfileImageUrl()
        {
            return _defaultProfileImageUrl;
        }


        public async Task<string> GetDefaultNewsImageUrl()
        {
            return _defaultNewsImageUrl;
        }


        public async Task AddImage(Images image)
        {
            await _imageRepository.AddImage(image);
        }





        public async Task DeleteImageById(Guid imageId)
        {
            Images images = await _imageRepository.GetImageById(imageId);
            await RemoveImage(images.ImageUrl);
            await _imageRepository.DeleteImageFromImageTable(imageId);
        }

        public async Task<string> SaveProfileImage(IFormFile image)
        {
            byte[] imageBytes = await _imageHelper.ResizeImage(image, 300, 300);

            string uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                "profiles"
            );

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string relativePath = Path.Combine("images", "profiles", Guid.NewGuid().ToString() + Path.GetExtension(image.FileName));
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

            try
            {
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

            }
            catch
            {
                return _defaultProfileImageUrl;
            }
            return "/" + relativePath.Replace("\\", "/");
        }

        public async Task<string> SaveNewsImage(IFormFile image)
        {
            byte[] imageBytes = await _imageHelper.ResizeImage(image, 800, 600);

            string uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                "news"
            );

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string relativePath = Path.Combine("images", "news", Guid.NewGuid().ToString() + Path.GetExtension(image.FileName));
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

            try
            {
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

            }
            catch
            {
                return _defaultNewsImageUrl;
            }
            return "/" + relativePath.Replace("\\", "/");
        }


        public async Task<bool> RemoveImage(string relativePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relativePath) ||
                    relativePath == _defaultProfileImageUrl ||
                    relativePath == _defaultNewsImageUrl)
                {
                    return false;
                }

                string normalizedPath = relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", normalizedPath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

    }
}
