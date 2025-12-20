using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.RepositoryContracts;
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
        private readonly string _defaultImageUrl;
        private readonly IConfiguration _configuration;
        private readonly IImageRepository _imageRepository;
        public ImageService(Cloudinary cloudinary, IConfiguration configuration, IImageRepository imageRepository)
        {
            _cloudinary = cloudinary;
            _configuration = configuration;
            _defaultImageUrl = _configuration["DefaultValues:DefaultProfileImageUrl"]
                ?? "https://res.cloudinary.com/dwkr48bj7/image/upload/User_fiy61j.jpg";
            _imageRepository = imageRepository;
        }



        public async Task<string> UploadToCloudinary(IFormFile profileImage)
        {
            await using var stream = profileImage.OpenReadStream();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(profileImage.FileName, stream),
                Folder = "NewsPortal/ProfileImages",
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString(); 
            }

            return _defaultImageUrl;
        }




        public async Task<string> UploadNewsImageToCloudinary(IFormFile profileImage)
        {
            await using var stream = profileImage.OpenReadStream();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(profileImage.FileName, stream),
                Folder = "NewsPortal/NewsImages",
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();
            }

            return _defaultImageUrl;
        }



        public async Task<bool> DeleteFromCloudinary(string imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl) || imageUrl == _defaultImageUrl)
                    return false;

                string publicId = ExtractPublicIdFromUrl(imageUrl);
                
                if (string.IsNullOrWhiteSpace(publicId))
                    return false;

                return await DeleteFromCloudinaryByPublicId(publicId);
            }
            catch (Exception)
            {
                return false;
            }
        }




        public async Task<bool> DeleteFromCloudinaryByPublicId(string publicId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(publicId))
                    return false;

                var deleteParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Image
                };

                var deletionResult = await _cloudinary.DestroyAsync(deleteParams);
                
                return deletionResult.StatusCode == System.Net.HttpStatusCode.OK && 
                       deletionResult.Result == "ok";
            }
            catch (Exception)
            {
                return false;
            }
        }



        private string ExtractPublicIdFromUrl(string imageUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl))
                    return string.Empty;

                
                Uri uri = new Uri(imageUrl);
                string path = uri.AbsolutePath;
                
                if (path.StartsWith("/"))
                    path = path.Substring(1);

                string[] pathParts = path.Split('/');
                

                int publicIdStartIndex = -1;
                for (int i = 0; i < pathParts.Length; i++)
                {
                    if (pathParts[i] == "upload")
                    {
                        publicIdStartIndex = i + 1;
                        if (publicIdStartIndex < pathParts.Length && 
                            pathParts[publicIdStartIndex].StartsWith("v") && 
                            pathParts[publicIdStartIndex].Length > 1 &&
                            pathParts[publicIdStartIndex].Substring(1).All(char.IsDigit))
                        {
                            publicIdStartIndex++; // Skip version
                        }
                        break;
                    }
                }
                
                if (publicIdStartIndex == -1 || publicIdStartIndex >= pathParts.Length)
                    return string.Empty;
                
                var publicIdParts = pathParts.Skip(publicIdStartIndex).ToArray();
                string publicIdWithExtension = string.Join("/", publicIdParts);
                
                int lastDotIndex = publicIdWithExtension.LastIndexOf('.');
                if (lastDotIndex > 0)
                {
                    return publicIdWithExtension.Substring(0, lastDotIndex);
                }
                
                return publicIdWithExtension;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }




        public async Task<string> GetDefaultProfileImageUrl()
        {
            return _defaultImageUrl;
        }




        public async Task AddImage(Images image)
        {
            await _imageRepository.AddImage(image);
        }





        public async Task DeleteImageById(Guid imageId)
        {
            Images images = await _imageRepository.GetImageById(imageId);
            await DeleteFromCloudinary(images.ImageUrl);
            await _imageRepository.DeleteImageFromImageTable(imageId);
        }
    }
}
