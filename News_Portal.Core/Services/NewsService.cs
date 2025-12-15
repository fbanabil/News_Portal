using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using News_Portal.Core.DemoData;
using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.Domain.RepositoryContracts;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using News_Portal.Core.Helpers;
using News_Portal.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Services
{
    public class NewsService : INewsService
    {
        private readonly NewsDemoData _newsDemoData;
        private readonly INewsRepository _newsRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NewsHelper_01 _newsHelper_01;
        public NewsService(INewsRepository newsRepository,UserManager<ApplicationUser> userManager)
        {
            _newsDemoData = new NewsDemoData();
            _newsRepository = newsRepository;
            _userManager = userManager;
            _newsHelper_01 = new NewsHelper_01();
        }

        public async Task AddNews(News news)
        {
            await _newsRepository.AddNews(news);
        }

        public async Task<List<AuthorsNewsToShowDTO>> GetAllAuthorsNewsAsync(Guid userId, AuthorNewsFilterParametersDTO authorNewsFilterParametersDTO, string sortBy, SortTypes sortOptions, int pageNo = 1, int pageSize = 10)
        {
            List<News> news = await _newsRepository.GetAllAuthorsNews(userId);

            news = await _newsHelper_01.SortNews(news, sortBy, sortOptions);


            news = await _newsHelper_01.FilterNews(news, authorNewsFilterParametersDTO);

            List<AuthorsNewsToShowDTO> authorsNewsToShowDTOs = news.Skip(pageSize*(pageNo-1)).Take(pageSize).Select(n => n.ToAuthorsNewsToShowDTO()).ToList();

            return authorsNewsToShowDTOs;
        }

        public async Task<int> GetAuthorsNewsCountAsync(Guid id, AuthorNewsFilterParametersDTO parametersDTO)
        {
            List<News> news = await _newsRepository.GetAllAuthorsNews(id);

            news = await _newsHelper_01.FilterNews(news, parametersDTO);

            return news.Count;
        }

        public async Task<DetailedNewsToShowDTO> GetDetailedNewsToShowDTOsByNewsId(Guid newsId)
        {
            await _newsRepository.IncrementNewsViewsCount(newsId);
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
