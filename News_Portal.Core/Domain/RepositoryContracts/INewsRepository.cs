using News_Portal.Core.Domain.Entities;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Domain.RepositoryContracts
{
    public interface INewsRepository
    {
        Task AddNews(News news);
        Task DeleteNewsAsync(Guid newsId);
        Task<List<News>> GetAllAuthorsNews(Guid authorId);
        Task<List<News>> GetAllNewsAsync();
        Task<News> GetNewsById(Guid newsId);
        Task<List<News>> GetNewsByTypeAsync(NewsType newsType, int pageNo, int pageSize);
        Task<List<HomePageNewsToShowDTO>> GetNewsForHomePageCarouselAsync();
        Task<List<HomePageNewsToShowDTO>> GetOtherNewsByTypeAsync(NewsType newsType);
        Task<List<HomePageNewsToShowDTO>> GetTopNewsAsync(TopOfXType type, int cnt);
        Task<List<News>> GetTopOfWeekNewsAsync();
        Task<int> GetTotalNewsCountByTypeAsync(NewsType newsType);
        Task IncrementNewsViewsCount(Guid newsId);
        Task<bool> NewsExistsBuId(Guid newsId);
        Task UpdateNews(News newsx);
    }
}
