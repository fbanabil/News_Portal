using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.Domain.RepositoryContracts;
using News_Portal.Core.DTO.Image;
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
        private readonly INewsRepository _newsRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NewsHelper_01 _newsHelper_01;
        private readonly IImageService _imageService;
        private readonly ICommentService _commentService;
        public NewsService(INewsRepository newsRepository,UserManager<ApplicationUser> userManager, IImageService imageService, ICommentService commentService)
        {
            _newsRepository = newsRepository;
            _userManager = userManager;
            _newsHelper_01 = new NewsHelper_01();
            _imageService = imageService;
            _commentService = commentService;
        }




        public async Task AddNews(News news)
        {
            await _newsRepository.AddNews(news);
        }




        public async Task AddNewsByAuthor(NewsToAddDTO newsToAddDTO, Guid authorId)
        {

            News news = newsToAddDTO.ToNews();       
            news.Comments = new List<Comments>();
            news.AuthorId = authorId;

            await _newsRepository.AddNews(news);

            if(newsToAddDTO.Images != null)
            {
                foreach (var img in newsToAddDTO.Images)
                {
                    string uploadPath = await _imageService.UploadNewsImageToCloudinary(img);
                    Images image = new Images();
                    image.ImageUrl = uploadPath;
                    image.ImageId = Guid.NewGuid();
                    image.NewsId = news.NewsId;

                    await _imageService.AddImage(image);
                }
            }
            
        }





        public async Task<bool> ChangeNewsPriorityAsync(Guid newsId, NewsPriority newPriority)
        {
            News news = await _newsRepository.GetNewsById(newsId);
            if (news != null)
            {
                news.NewsPriority = newPriority;
                news.LastUpdate = DateTime.UtcNow;
                await _newsRepository.UpdateNews(news);
                return true;
            }
            return false;
        }





        public async Task<bool> ChangeNewsStatusAsync(Guid newsId, NewsStatus newStatus)
        {
            News news = await _newsRepository.GetNewsById(newsId);
            if (news != null)
            {
                news.NewsStatus = newStatus;
                news.LastUpdate = DateTime.UtcNow;
                if (newStatus == NewsStatus.Published)
                {
                    news.PublishedDate = DateTime.UtcNow;
                }
                await _newsRepository.UpdateNews(news);
                return true;
            }
            return false;
        }





        public async Task<bool> ChangeNewsTypeAsync(Guid newsId, NewsType newType)
        {
            News news = await _newsRepository.GetNewsById(newsId);
            if (news != null)
            {
                news.NewsType = newType;
                news.LastUpdate = DateTime.UtcNow;
                await _newsRepository.UpdateNews(news);
                return true;
            }
            return false;
        }





        public async Task DeleteNewsAsync(Guid newsId, Guid id)
        {
            News news = await _newsRepository.GetNewsById(newsId);
            if(news.AuthorId != id)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this news.");
            }
            await _newsRepository.DeleteNewsAsync(newsId);
        }





        public async Task DeleteNewsByAuthorId(Guid id)
        {
            await _newsRepository.DeleteNewsByAuthorId(id);    
        }





        public async Task<bool> DeleteNewsByNewsIdAsync(Guid newsId)
        {
            try
            {
                News news =  await _newsRepository.GetNewsById(newsId);
                if(news.Images!=null)
                {
                    foreach (Images img in news.Images)
                    {
                        await _imageService.DeleteImageById(img.ImageId);
                    }
                }
                
                if(news.Comments!=null)
                {
                    foreach (Comments comment in news.Comments)
                    {
                        await _commentService.DeleteCommentById(comment.CommentId);
                    }
                }

                await _newsRepository.DeleteNewsAsync(newsId);

                return true;
            }
            catch
            {
                return false;
            }
        }





        public async Task<int> GetAdminPageNewsCountAsync(Guid? id, NewsFilterParametersDTO? parametersDTO)
        {
            List<News> news = new List<News>();
            if (id != null)
            {
                news = await _newsRepository.GetAllAuthorsNews(id.Value);
            }
            else
            {
                news = await _newsRepository.GetAllNewsAsync();
            }
            if (parametersDTO != null)
            {
                news = await _newsHelper_01.FilterNews(news, parametersDTO);
            }
            return news.Count;
        }





        public async Task<AdminsNewsDetailesToShowDTO> GetAdminsNewsDetailsAsync(Guid newsId)
        {
            News news = await _newsRepository.GetNewsById(newsId);
            return news.ToAdminsNewsDetailesToShowDTO();
        }




        public async Task<List<AuthorsNewsToShowDTO>> GetAllAuthorsNewsAsync(Guid userId, NewsFilterParametersDTO authorNewsFilterParametersDTO, string sortBy, SortTypes sortOptions, int pageNo = 1, int pageSize = 10)
        {
            List<News> news = await _newsRepository.GetAllAuthorsNews(userId);
            news = await _newsHelper_01.SortNews(news, sortBy, sortOptions);
            news = await _newsHelper_01.FilterNews(news, authorNewsFilterParametersDTO);

            List<AuthorsNewsToShowDTO> authorsNewsToShowDTOs = news.OrderByDescending(p=>p.PublishedDate).Skip(pageSize*(pageNo-1)).Take(pageSize).Select(n => n.ToAuthorsNewsToShowDTO()).ToList();

            return authorsNewsToShowDTOs;
        }





        public async Task<List<AuthorsNewsToShowDTO>> GetAllNewsAsync(NewsFilterParametersDTO newsFilterParametersDTO, string sortBy, SortTypes sortOptions, int pageNo, int pageSize)
        {
            List<News> news = await _newsRepository.GetAllNewsAsync();
            news = await _newsHelper_01.SortNews(news, sortBy, sortOptions);
            news = await _newsHelper_01.FilterNews(news, newsFilterParametersDTO);
            List<AuthorsNewsToShowDTO> authorsNewsToShowDTOs = news.OrderByDescending(p => p.PublishedDate).Skip(pageSize * (pageNo - 1)).Take(pageSize).Select(n => n.ToAuthorsNewsToShowDTO()).ToList();
            return authorsNewsToShowDTOs;
        }





        public async Task<int> GetAuthorsNewsCountAsync(Guid id, NewsFilterParametersDTO parametersDTO)
        {
            List<News> news = await _newsRepository.GetAllAuthorsNews(id);
            news = await _newsHelper_01.FilterNews(news, parametersDTO);
            return news.Count;
        }




        public async Task<AuthorsNewsDetailesToShowDTO> GetAuthorsNewsDetailsAsync(Guid newsId)
        {
            News detailedNews = await _newsRepository.GetNewsById(newsId);
            return detailedNews.ToAuthorsNewsDetailesToShowDTO();
        }





        public async Task<(int, int , int, int)> GetAuthorsNewsSummaryAsync(Guid id)
        {
            List<News> news = await _newsRepository.GetAllAuthorsNews(id);
            int totalArticles = news.Count;
            int totalPublishedArticles = news.Where(n => n.NewsStatus == NewsStatus.Published).Count();
            int totalViews = news.Sum(n => n.TotalViews);
            int thisMonth = news.Where(n => n?.PublishedDate != null && n.PublishedDate.Month == DateTime.UtcNow.Month && n.PublishedDate.Year == DateTime.UtcNow.Year).Count();
            return (totalArticles, totalPublishedArticles, totalViews, thisMonth);
        }





        public async Task<List<HomePageNewsToShowDTO>> GetAuthorsTopNewsAsync(int size, Guid id)
        {
            List<News> news = await _newsRepository.GetAllAuthorsNews(id);
            List<HomePageNewsToShowDTO> homePageNewsToShowDTOs = news.OrderByDescending(n => n.TotalViews).Take(size).Select(n => n.ToHomePageNewsToShowDTO()).ToList();
            return homePageNewsToShowDTOs;
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
            List<HomePageNewsToShowDTO> homePageNewsToShowDTOs = news.OrderByDescending(p => p.PublishedDate).Select(n=> n.ToHomePageNewsToShowDTO()).ToList();
            return homePageNewsToShowDTOs;
        }




        public async Task<NewsToEditDTO> GetNewsForEditAsync(Guid newsId)
        {
            News news = await _newsRepository.GetNewsById(newsId);
            return news.ToNewsToEditDTO();
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
            List<HomePageNewsToShowDTO> homePageNewsToShowDTOs = news.OrderByDescending(p => p.PublishedDate).Select(n => n.ToHomePageNewsToShowDTO()).ToList();
            return homePageNewsToShowDTOs;
        }




        public async Task<int> GetTotalNewsCountByTypeAsync(NewsType newsType)
        {
            return await _newsRepository.GetTotalNewsCountByTypeAsync(newsType);
        }





        public async Task<bool> IsNewsPinnedAsync(Guid newsId)
        {
            List<PinnedNews> pinnedNewsList = await _newsRepository.GetAllPinnedNewsAsync();
            foreach (PinnedNews pn in pinnedNewsList)
            {
                if (pn.NewsId == newsId)
                {
                    return true;
                }
            }
            return false;
        }





        public async Task<bool> PinNewsAsync(Guid newsId)
        {
            //remove existing pinned news
            List<PinnedNews> pinnedNewsList = await _newsRepository.GetAllPinnedNewsAsync();
            foreach (PinnedNews pn in pinnedNewsList)
            {
                await _newsRepository.DeletePinnedNewsAsync(pn.Id);
            }
            //add new pinned news
            PinnedNews pinnedNews = new PinnedNews();
            pinnedNews.Id = Guid.NewGuid();
            pinnedNews.NewsId = newsId;
            await _newsRepository.AddPinnedNewsAsync(pinnedNews);
            return true;
        }





        public async Task<bool> UnpinNewsAsync(Guid newsId)
        {
            try
            {
                await _newsRepository.DeletePinnedNewsByNewsIdAsync(newsId);
                return true;
            }
            catch
            {
                return false;
            }
        }





        public async Task UpdateNewsAsync(NewsToEditDTO newsToEditDTO, Guid id)
        {
            News news = await _newsRepository.GetNewsById(newsToEditDTO.NewsId);

            if(news.AuthorId != id)
            {
                throw new UnauthorizedAccessException("You are not authorized to edit this news.");
            }

            if (newsToEditDTO.ExistingImages == null)
            {
                foreach (Images img in news.Images)
                {
                    await _imageService.DeleteImageById(img.ImageId);   
                }
            }
            else if(newsToEditDTO.ExistingImages.Count != news.Images?.Count && newsToEditDTO.ExistingImages.Count != 0)
            {
                foreach(Images img in news.Images)
                {
                    if(!newsToEditDTO.ExistingImages.Any(ei => ei.ImageId == img.ImageId))
                    {
                        await _imageService.DeleteImageById(img.ImageId);
                    }
                }
            }

            news.NewsTitle = newsToEditDTO.NewsTitle;
            news.NewsContent = newsToEditDTO.NewsContent;
            news.NewsType = newsToEditDTO.NewsType;
            news.VideoUrl = newsToEditDTO.VideoUrl;
            //news.NewsStatus = newsToEditDTO.NewsStatus;
            news.NewsPriority = newsToEditDTO.NewsPriority;
            news.LastUpdate = DateTime.UtcNow;


            await _newsRepository.UpdateNews(news);

            if (newsToEditDTO.NewImages != null)
            {
                foreach (var img in newsToEditDTO.NewImages)
                {
                    string uploadPath = await _imageService.UploadNewsImageToCloudinary(img);
                    Images image = new Images();
                    image.ImageUrl = uploadPath;
                    image.ImageId = Guid.NewGuid();
                    image.NewsId = news.NewsId;
                    await _imageService.AddImage(image);
                }
            }

        }

    }
}
