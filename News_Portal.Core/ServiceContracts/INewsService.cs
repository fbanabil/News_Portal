using News_Portal.Core.Domain.Entities;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.ServiceContracts
{
    public interface INewsService
    {
        Task AddNews(News news);
        Task<List<AuthorsNewsToShowDTO>> GetAllAuthorsNewsAsync(Guid userId, AuthorNewsFilterParametersDTO authorNewsFilterParametersDTO, string sortBy, SortTypes sortOptions, int pageNo = 1, int pageSize = 10);
        Task<int> GetAuthorsNewsCountAsync(Guid id, AuthorNewsFilterParametersDTO parametersDTO);
        Task<DetailedNewsToShowDTO> GetDetailedNewsToShowDTOsByNewsId(Guid newsId);
        Task<List<HomePageNewsToShowDTO>> GetNewsByTypeAsync(NewsType newsType, int pageNo, int pageSize);
        Task<List<HomePageNewsToShowDTO>> GetNewsForHomePageCarouselAsync();
        Task<List<HomePageNewsToShowDTO>> GetOtherNewsByTypeAsync(NewsType newsType);
        Task<List<HomePageNewsToShowDTO>> GetSuggestionNewsByTypeAsync(NewsType newsType);
        Task<List<HomePageNewsToShowDTO>> GetTopNewsAsync(TopOfXType type, int cnt);
        Task<int> GetTotalNewsCountByTypeAsync(NewsType newsType);
    }
}
