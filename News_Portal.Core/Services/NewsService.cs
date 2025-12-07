using News_Portal.Core.DemoData;
using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.RepositoryContracts;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using News_Portal.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Services
{
    public class NewsService : INewsService
    {
        private readonly NewsDemoData _newsDemoData;
        private readonly INewsRepository _newsRepository;
        public NewsService(INewsRepository newsRepository)
        {
            _newsDemoData = new NewsDemoData();
            _newsRepository = newsRepository;
        }

        public async Task AddNews(News news)
        {
            await _newsRepository.AddNews(news);
        }

        public async Task<DetailedNewsToShowDTO> GetDetailedNewsToShowDTOsByNewsId(Guid newsId)
        {
            News detailedNews = await _newsRepository.GetNewsById(newsId);
            return detailedNews.ToDetailedNewsToShowDTO();
        }

        public async Task<List<HomePageNewsToShowDTO>> GetNewsByTypeAsync(NewsType newsType, int pageNo, int pageSize)
        {
            List<News> news = await _newsRepository.GetNewsByTypeAsync(newsType, pageNo, pageSize);
            List<HomePageNewsToShowDTO> homePageNewsToShowDTOs = news.Select(n=> n.ToHomePageNewsToShowDTO()).ToList();
            return homePageNewsToShowDTOs;
        }        

        public async Task<List<HomePageNewsToShowDTO>> GetNewsForHomePageCarouselAsync()
        {
            List<HomePageNewsToShowDTO> homePageNewsToShowDTOs = await _newsRepository.GetNewsForHomePageCarouselAsync();
            return homePageNewsToShowDTOs;
        }

        public async Task<List<HomePageNewsToShowDTO>> GetOtherNewsByTypeAsync(NewsType newsType)
        {
            List<HomePageNewsToShowDTO> homePageNewsToShowDTOs = await _newsRepository.GetOtherNewsByTypeAsync(newsType);
            return homePageNewsToShowDTOs;
        }

        public async Task<List<HomePageNewsToShowDTO>> GetSuggestionNewsByTypeAsync(NewsType newsType)
        {
            List<HomePageNewsToShowDTO> homePageNewsToShowDTOs = await _newsRepository.GetOtherNewsByTypeAsync(newsType);
            return homePageNewsToShowDTOs;
        }

        public async Task<List<HomePageNewsToShowDTO>> GetTopNewsAsync(TopOfXType type, int cnt)
        {
                return await _newsRepository.GetTopNewsAsync(type, cnt);
        }

        public async Task<List<HomePageNewsToShowDTO>> GetTopOfWeekNewsAsync()
        {
            List<News> news = await _newsRepository.GetTopOfWeekNewsAsync();
            List<HomePageNewsToShowDTO> homePageNewsToShowDTOs = news.Select(n => n.ToHomePageNewsToShowDTO()).ToList();
            return homePageNewsToShowDTOs;
        }

        public async Task<int> GetTotalNewsCountByTypeAsync(NewsType newsType)
        {
            return await _newsRepository.GetTotalNewsCountByTypeAsync(newsType);
        }
    }
}
