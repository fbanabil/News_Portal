using Microsoft.AspNetCore.Http;
using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.Comment;
using News_Portal.Core.DTO.Image;
using News_Portal.Core.DTO.News;
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
    public class NewsToEditDTO
    {
        public Guid NewsId { get; set; }

        [Required(ErrorMessage = "News title cant't be empty")]
        [MaxLength(100, ErrorMessage = "News title can't exceed 100 characters")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "News content cant't be empty")]
        public string? NewsContent { get; set; }

        [Required]
        public NewsType NewsType { get; set; }

        public string? VideoUrl { get; set; }

        //[Required]
        //public NewsStatus NewsStatus { get; set; }

        [Required]
        public NewsPriority NewsPriority { get; set; }

        public virtual ICollection<ImageToShowDTO>? ExistingImages { get; set; }

        public List<IFormFile>? NewImages { get; set; }


        public Domain.Entities.News ToUpdatedNews()
        {
            Domain.Entities.News news = new Domain.Entities.News();
            //news.NewsId = this.NewsId;
            news.NewsTitle = this.NewsTitle;
            news.NewsContent = this.NewsContent;
            news.NewsType = this.NewsType;
            news.VideoUrl = this.VideoUrl;
            //news.NewsStatus = this.NewsStatus;
            news.NewsPriority = this.NewsPriority;
            return news;
        }
    }
}

public static class NewsToEditDTOExtensions
{
    public static NewsToEditDTO ToNewsToEditDTO(this News news)
    {
        return new NewsToEditDTO
        {
            NewsId = news.NewsId,
            NewsTitle = news.NewsTitle,
            NewsContent = news.NewsContent,
            NewsType = news.NewsType,
            VideoUrl = news.VideoUrl,
            //NewsStatus = news.NewsStatus,
            NewsPriority = news.NewsPriority,
            ExistingImages = news?.Images?.Select(img => img.ToImageToShowDTO()).ToList(),
        };
    }
}