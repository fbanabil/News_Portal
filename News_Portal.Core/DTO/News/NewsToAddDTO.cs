using Microsoft.AspNetCore.Http;
using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.News
{
    public class NewsToAddDTO
    {

        [Required(ErrorMessage = "News title cant't be empty")]
        [MaxLength(100, ErrorMessage = "News title can't exceed 100 characters")]

        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "News content cant't be empty")]
        public string? NewsContent { get; set; }

        [Required]
        public NewsType NewsType { get; set; }

        //[Required]
        //public NewsStatus NewsStatus { get; set; }

        [Required]
        public NewsPriority NewsPriority { get; set; }
        
        public string? VideoUrl { get; set; }

        public List<IFormFile>? Images { get; set; }

        public Domain.Entities.News ToNews()
        {
            Domain.Entities.News dto = new Domain.Entities.News();
            dto.NewsTitle = NewsTitle;
            dto.NewsContent = NewsContent;
            dto.NewsPriority = NewsPriority;
            dto.NewsType = NewsType;
            dto.VideoUrl = VideoUrl;
            dto.NewsId = Guid.NewGuid();
            //dto.NewsStatus = NewsStatus;
            dto.PublishedDate = DateTime.UtcNow;
            dto.LastUpdate = DateTime.UtcNow;
            dto.TotalViews = 0;
            return dto;
        }

    }
}

