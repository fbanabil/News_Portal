using News_Portal.Core.Domain.Entities;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;

namespace News_Portal.Core.Helpers
{
    public class NewsHelper_01
    {
        public async Task<List<News>> SortNews(List<News> news, string sortBy, SortTypes sortTypes)
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
                    => news.OrderBy(n => n.NewsPriority).ToList(),
                (nameof(News.NewsPriority), SortTypes.Descending)
                    => news.OrderByDescending(n => n.NewsPriority).ToList(),
                (nameof(News.PublishedDate), SortTypes.Ascending)
                    => news.OrderBy(n => n.PublishedDate).ToList(),
                (nameof(News.PublishedDate), SortTypes.Descending)
                    => news.OrderByDescending(n => n.PublishedDate).ToList(),

                _ => news

            };
            return await Task.FromResult(news);
        }

        public async Task<List<News>> FilterNews(List<News> news, NewsFilterParametersDTO authorNewsFilterParametersDTO)
        {
            if (authorNewsFilterParametersDTO == null)
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
public static class Convertor
{
    public static string StripHtmlTags(this string html)
    {
        if (string.IsNullOrEmpty(html))
            return string.Empty;

        // Remove HTML tags
        string stripped = System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);

        // Decode HTML entities (&amp; → &, &lt; → <, etc.)
        stripped = System.Net.WebUtility.HtmlDecode(stripped);

        // Remove extra whitespace
        stripped = System.Text.RegularExpressions.Regex.Replace(stripped, @"\s+", " ");

        return stripped.Trim();
    }
    public static string RemoveClampBreakingElements(this string html)
    {
        if (string.IsNullOrEmpty(html))
            return string.Empty;

        // Step 1: Remove ONLY block element tags (keep content & inline styles)
        html = Regex.Replace(
            html,
            @"<(p|div|h[1-6]|ul|ol|li|blockquote|section|article|header|footer|nav|aside|main|table|tr|td|th|br)[^>]*?>",
            " ", // Space prevents word concatenation
            RegexOptions.IgnoreCase
        );

        // Step 2: Remove closing tags
        html = Regex.Replace(
            html,
            @"</(p|div|h[1-6]|ul|ol|li|blockquote|section|article|header|footer|nav|aside|main|table|tr|td|th)>",
            " ",
            RegexOptions.IgnoreCase
        );

        // Step 3: Clean up spaces
        html = Regex.Replace(html, @"\s+", " ");
        html = System.Net.WebUtility.HtmlDecode(html);

        return html.Trim();
    }
}
