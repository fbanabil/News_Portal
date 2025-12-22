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
    public class AdminsNewsDetailesToShowDTO
    {
        public Guid NewsId { get; set; }

        public string? NewsTitle { get; set; }

        public string? NewsContent { get; set; }

        public NewsType NewsType { get; set; }

        public DateTime PublishedDate { get; set; }

        public DateTime LastUpdate { get; set; }
        public int TotalViews { get; set; } = 0;

        public string? VideoUrl { get; set; }

        public NewsStatus NewsStatus { get; set; }

        public NewsPriority NewsPriority { get; set; }


        [ForeignKey("AuthorId")]
        public ApplicationUser? Author { get; set; }

        public virtual ICollection<ImageToShowDTO>? Images { get; set; }
        public virtual ICollection<CommentToShowDTO>? Comments { get; set; }
    }
}

public static class AdminsNewsDetailesToShowDTOExtensions
{
    public static AdminsNewsDetailesToShowDTO ToAdminsNewsDetailesToShowDTO(this News news)
    {
        return new AdminsNewsDetailesToShowDTO
        {
            NewsId = news.NewsId,
            NewsTitle = news.NewsTitle,
            NewsContent = news.NewsContent,
            NewsType = news.NewsType,
            PublishedDate = news.PublishedDate,
            LastUpdate = news.LastUpdate,
            TotalViews = news.TotalViews,
            Author = news.Author,
            Images = news?.Images?.Select(img => img.ToImageToShowDTO()).ToList(),
            Comments = news?.Comments?.Select(cmt => cmt.ToCommentToShowDTO()).ToList(),
            VideoUrl = news?.VideoUrl,
            NewsStatus = news.NewsStatus,
            NewsPriority = news.NewsPriority
        };
    }
}
