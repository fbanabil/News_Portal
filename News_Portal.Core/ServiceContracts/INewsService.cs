using Microsoft.AspNetCore.Http;
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
        Task AddNewsByAuthor(NewsToAddDTO newsToAddDTO, Guid authoId);
        Task<bool> ChangeNewsPriorityAsync(Guid newsId, NewsPriority newPriority);
        Task<bool> ChangeNewsStatusAsync(Guid newsId, NewsStatus newStatus);
        Task<bool> ChangeNewsTypeAsync(Guid newsId, NewsType newType);
        Task DeleteNewsAsync(Guid newsId, Guid id);
        Task<bool> DeleteNewsByNewsIdAsync(Guid newsId);
        Task<int> GetAdminPageNewsCountAsync(Guid? id, NewsFilterParametersDTO? parametersDTO);
        Task<AdminsNewsDetailesToShowDTO> GetAdminsNewsDetailsAsync(Guid newsId);
        Task<List<AuthorsNewsToShowDTO>> GetAllAuthorsNewsAsync(Guid userId, NewsFilterParametersDTO authorNewsFilterParametersDTO, string sortBy, SortTypes sortOptions, int pageNo = 1, int pageSize = 10);
        Task<List<AuthorsNewsToShowDTO>> GetAllNewsAsync(NewsFilterParametersDTO authorNewsFilterParametersDTO, string sortBy, SortTypes sortOptions, int pageNo, int pageSize);
        Task<int> GetAuthorsNewsCountAsync(Guid id, NewsFilterParametersDTO parametersDTO);
        Task<AuthorsNewsDetailesToShowDTO> GetAuthorsNewsDetailsAsync(Guid newsId);
        Task<(int, int, int, int)> GetAuthorsNewsSummaryAsync(Guid id);
        Task<List<HomePageNewsToShowDTO>> GetAuthorsTopNewsAsync(int size, Guid id);

        //Task<BestOfAuthorsNewsDTO> GetBestOfAuthorsNewsAsync(int size);
        Task<DetailedNewsToShowDTO> GetDetailedNewsToShowDTOsByNewsId(Guid newsId);
        Task<List<HomePageNewsToShowDTO>> GetNewsByTypeAsync(NewsType newsType, int pageNo, int pageSize);
        Task<NewsToEditDTO> GetNewsForEditAsync(Guid newsId);
        Task<List<HomePageNewsToShowDTO>> GetNewsForHomePageCarouselAsync();
        Task<List<HomePageNewsToShowDTO>> GetOtherNewsByTypeAsync(NewsType newsType);
        Task<List<HomePageNewsToShowDTO>> GetSuggestionNewsByTypeAsync(NewsType newsType);
        Task<List<HomePageNewsToShowDTO>> GetTopNewsAsync(TopOfXType type, int cnt);
        Task<int> GetTotalNewsCountByTypeAsync(NewsType newsType);
        Task<bool> IsNewsPinnedAsync(Guid newsId);
        Task<bool> PinNewsAsync(Guid newsId);
        Task<bool> UnpinNewsAsync(Guid newsId);
        Task UpdateNewsAsync(NewsToEditDTO newsToEditDTO, Guid id);
    }
}
