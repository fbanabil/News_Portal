using News_Portal.Core.Domain.Entities;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Helpers
{
    public class NewsHelper_01
    {
        public async Task<List<News>> SortNews(List<News> news,string sortBy, SortTypes sortTypes)
        {
            news = (sortBy, sortTypes)
                switch
            {
                (nameof(News.NewsStatus), SortTypes.Ascending)
                    => news.OrderBy(n => n.NewsStatus).ToList(),
                (nameof(NewsStatus), SortTypes.Descending)
                    => news.OrderByDescending(n => n.NewsStatus).ToList(),
                (nameof(News.TotalViews), SortTypes.Ascending)
                    => news.OrderBy(n => n.TotalViews).ToList(),
                (nameof(News.TotalViews), SortTypes.Descending)
                    => news.OrderByDescending(n => n.TotalViews).ToList(),
                (nameof(News.NewsType), SortTypes.Ascending)
                    => news.OrderBy(n => n.NewsType).ToList(),
                (nameof(News.NewsType), SortTypes.Descending)
                    => news.OrderByDescending(n => n.NewsType).ToList(),
                (nameof(News.NewsPriority), SortTypes.Ascending)
                    => news.OrderBy(n=>n.NewsPriority).ToList(),
                (nameof(News.NewsPriority),SortTypes.Descending)
                    => news.OrderByDescending(n => n.NewsPriority).ToList(),
                (nameof(News.PublishedDate), SortTypes.Ascending)
                    => news.OrderBy(n => n.PublishedDate).ToList(),
                (nameof(News.PublishedDate),SortTypes.Descending)
                    => news.OrderByDescending(n => n.PublishedDate).ToList(),

                _ => news

            };
            return await Task.FromResult(news);
        }

        public async Task<List<News>> FilterNews(List<News> news, NewsFilterParametersDTO authorNewsFilterParametersDTO)
        {
            if(authorNewsFilterParametersDTO == null)
            {
                return news;
            }
            news = news.Where(n =>
                (string.IsNullOrEmpty(authorNewsFilterParametersDTO.NewsTitle) || n.NewsTitle.Contains(authorNewsFilterParametersDTO.NewsTitle, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(authorNewsFilterParametersDTO.NewsContent) || n.NewsContent.Contains(authorNewsFilterParametersDTO.NewsContent, StringComparison.OrdinalIgnoreCase)) &&
                (!authorNewsFilterParametersDTO.NewsType.HasValue || n.NewsType == authorNewsFilterParametersDTO.NewsType) &&
                (!authorNewsFilterParametersDTO.PublishedDate.HasValue || n.PublishedDate.Date == authorNewsFilterParametersDTO.PublishedDate.Value.Date) &&
                (!authorNewsFilterParametersDTO.NewsStatus.HasValue || n.NewsStatus == authorNewsFilterParametersDTO.NewsStatus) &&
                (!authorNewsFilterParametersDTO.NewsPriority.HasValue || n.NewsPriority == authorNewsFilterParametersDTO.NewsPriority)
            ).ToList();
            return await Task.FromResult(news);
        }
    }
}
