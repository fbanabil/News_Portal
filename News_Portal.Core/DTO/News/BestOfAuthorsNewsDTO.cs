using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.IdentityEntities;
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
    public class BestOfAuthorsNewsDTO
    {
        public Guid NewsId { get; set; }

        public string? NewsTitle { get; set; }

        public string? NewsContent { get; set; }

        public NewsType NewsType { get; set; }

        public DateTime PublishedDate { get; set; }

        public int TotalViews { get; set; }

        public NewsStatus NewsStatus { get; set; } 

        public string? Image { get; set; }
    }
}

public static class BestOfAuthorsNewsDTOExtensions
{
    public static BestOfAuthorsNewsDTO ToBestOfAuthorsNewsDTO(this News news)
    {
        return new BestOfAuthorsNewsDTO
        {
            NewsId = news.NewsId,
            NewsTitle = news.NewsTitle,
            NewsContent = news.NewsContent,
            NewsType = news.NewsType,
            PublishedDate = news.PublishedDate,
            TotalViews = news.TotalViews,
            NewsStatus = news.NewsStatus
        };
    }
}
