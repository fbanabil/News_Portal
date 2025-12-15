using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.IdentityEntities;
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
    public class AuthorsNewsToShowDTO
    {
        public Guid NewsId { get; set; }

        public string? NewsTitle { get; set; }

        public string? NewsContent { get; set; }

        public NewsType NewsType { get; set; }

        public DateTime PublishedDate { get; set; }

        public DateTime LastUpdate { get; set; }
        public int TotalViews { get; set; } = 0;

        public string? VideoUrl { get; set; }

        public Guid AuthorId { get; set; }

        public NewsStatus NewsStatus { get; set; } = NewsStatus.Pending;

        public NewsPriority NewsPriority { get; set; } = NewsPriority.Medium;

        public ApplicationUser? Author { get; set; }

        public virtual ICollection<Images>? Images { get; set; }
        public virtual ICollection<Comments>? Comments { get; set; }
    }
}
public static class AuthorsNewsToShowDTOExtensions
{
    public static AuthorsNewsToShowDTO ToAuthorsNewsToShowDTO(this News news)
    {
        return new AuthorsNewsToShowDTO
        {
            NewsId = news.NewsId,
            NewsTitle = news.NewsTitle,
            NewsContent = news.NewsContent,
            NewsType = news.NewsType,
            PublishedDate = news.PublishedDate,
            LastUpdate = news.LastUpdate,
            TotalViews = news.TotalViews,
            AuthorId = news.AuthorId,
            NewsStatus = news.NewsStatus,
            NewsPriority = news.NewsPriority,
            Author = news.Author,
            Images = news.Images,
            Comments = news.Comments,
            VideoUrl = news?.VideoUrl
        };
    }
}
