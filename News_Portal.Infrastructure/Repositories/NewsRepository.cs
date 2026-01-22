using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.RepositoryContracts;
using News_Portal.Core.DTO.News;
using News_Portal.Core.Enums;
using News_Portal.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Infrastructure.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<NewsRepository> _logger;
        public NewsRepository(ApplicationDbContext dbContext, ILogger<NewsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task AddNews(News news)
        {
            await _dbContext.News.AddAsync(news);
            await _dbContext.SaveChangesAsync();
        }






        public async Task AddPinnedNewsAsync(PinnedNews pinnedNews)
        {
            await _dbContext.PinnedNews.AddAsync(pinnedNews);
            await _dbContext.SaveChangesAsync();
        }





        public async Task DeleteNewsAsync(Guid newsId)
        {
            News? news = await _dbContext.News.FirstOrDefaultAsync(n => n.NewsId == newsId);
            if(news != null)
            {
                _dbContext.News.Remove(news);
                await _dbContext.SaveChangesAsync();
            }
        }





        public async Task DeleteNewsByAuthorId(Guid id)
        {
            try
            {
                var newsIds = await _dbContext.News
                    .AsNoTracking()
                    .Where(n => n.AuthorId == id)
                    .Select(n => n.NewsId)
                    .ToListAsync();

                if (!newsIds.Any())
                    return;

                await _dbContext.Images.Where(i => newsIds.Contains(i.NewsId)).ExecuteDeleteAsync();
                await _dbContext.Comments.Where(c => newsIds.Contains(c.NewsId)).ExecuteDeleteAsync();
                await _dbContext.PinnedNews.Where(p => newsIds.Contains(p.NewsId)).ExecuteDeleteAsync();

                await _dbContext.News.Where(n => newsIds.Contains(n.NewsId)).ExecuteDeleteAsync();
            }
            catch (DbUpdateConcurrencyException dbex)
            {
                foreach (var entry in dbex.Entries)
                {
                    var keyValues = entry.Properties
                        .Where(p => p.Metadata.IsPrimaryKey())
                        .Select(p => p.CurrentValue)
                        .ToArray();
                    _logger.LogError("Concurrency failure deleting entity {EntityType} with keys [{Keys}]", entry.Entity.GetType().Name, string.Join(",", keyValues));
                }
                _logger.LogError(dbex, $"Concurrency error deleting news by author id {id}: {dbex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting news by author id {id}: {ex.Message}");
                throw;
            }
        }




        public async Task DeletePinnedNewsAsync(Guid id)
        {
            News? news = await _dbContext.News.FirstOrDefaultAsync(n => n.NewsId == id);
            if (news != null)
            {
                PinnedNews? pinnedNews = await _dbContext.PinnedNews.FirstOrDefaultAsync(n => n.NewsId == news.NewsId);
                if (pinnedNews != null)
                {
                    _dbContext.PinnedNews.Remove(pinnedNews);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }





        public async Task DeletePinnedNewsByNewsIdAsync(Guid newsId)
        {
            await _dbContext.PinnedNews.Where(n => n.NewsId == newsId).ExecuteDeleteAsync();
        }




        public async Task<List<News>> GetAllAuthorsNews(Guid authorId)
        {
            return await _dbContext.News.Include(i=>i.Images).Include(i=>i.Comments).Include(i=>i.Author).Where(n => n.AuthorId == authorId).ToListAsync();
        }




        public async Task<List<News>> GetAllNewsAsync()
        {
            return await _dbContext.News.Include(i=>i.Images).Include(i=>i.Comments).Include(i=>i.Author).ToListAsync();
        }





        public async Task<List<PinnedNews>> GetAllPinnedNewsAsync()
        {
            return  await _dbContext.PinnedNews.ToListAsync();
        }





        public async Task<News> GetNewsById(Guid newsId)
        {
            return await _dbContext.News.Include(i=>i.Author).Include(i=>i.Images).Include(i=>i.Comments).ThenInclude(i=>i.User).FirstOrDefaultAsync(n=>n.NewsId == newsId);
        }




        public async Task<List<News>> GetNewsByTypeAsync(NewsType newsType, int pageNo, int pageSize)
        {
            return await _dbContext.News.Include(i=>i.Images).Include(a=>a.Author).Where(n => n.NewsType == newsType && n.NewsStatus == NewsStatus.Published).
                OrderByDescending(p=>p.PublishedDate).Skip((pageNo-1) * pageSize).Take(pageSize).ToListAsync();
        }




        public async Task<List<HomePageNewsToShowDTO>> GetNewsForHomePageCarouselAsync()
        {
            return await _dbContext.News.Include(i => i.Images).OrderByDescending(n => n.TotalViews)
                .Where(t=> DateTime.UtcNow.Month-t.PublishedDate.Month <= 12 && t.NewsStatus==NewsStatus.Published)
                .Take(9)
                .Select(n => n.ToHomePageNewsToShowDTO())
                .ToListAsync();
        }




        public async Task<List<HomePageNewsToShowDTO>> GetOtherNewsByTypeAsync(NewsType newsType)
        {
            int take = await _dbContext.News.AsNoTracking().CountAsync(n => n.NewsType == newsType);
            return await _dbContext.News.Include(i => i.Images).Where(n => n.NewsType == newsType && n.NewsStatus == NewsStatus.Published)
                .OrderByDescending(n => n.PublishedDate)
                .Take(take<6?take:6)
                .Select(n => n.ToHomePageNewsToShowDTO())
                .ToListAsync();
        }




        public Task<List<HomePageNewsToShowDTO>> GetTopNewsAsync(TopOfXType type, int cnt)
        {
            if(type == TopOfXType.Pinned)
            {
                var pinnedNewsIds = _dbContext.PinnedNews.Select(p => p.NewsId).ToList();
                return _dbContext.News.Include(i => i.Images)
                    .Where(n => pinnedNewsIds.Contains(n.NewsId) && n.NewsStatus==NewsStatus.Published)
                    .OrderByDescending(n => n.TotalViews)
                    .Take(1)
                    .Select(n => n.ToHomePageNewsToShowDTO())
                    .ToListAsync();
            }
            else if(type == TopOfXType.Week)
            {
                return _dbContext.News.Include(i => i.Images)
                    .Where(n => n.PublishedDate >= DateTime.Now.AddDays(-30) && n.NewsStatus == NewsStatus.Published)
                    .OrderByDescending(n => n.TotalViews)
                    .Take(cnt)
                    .Select(n => n.ToHomePageNewsToShowDTO())
                    .ToListAsync();
            }
            else if(type == TopOfXType.Month)
            {
                return _dbContext.News.Include(i => i.Images)
                    .Where(n => n.PublishedDate >= DateTime.Now.AddMonths(-1) && n.NewsStatus == NewsStatus.Published)
                    .OrderByDescending(n => n.TotalViews)
                    .Take(cnt)
                    .Select(n => n.ToHomePageNewsToShowDTO())
                    .ToListAsync();
            }
            else 
            {
                return _dbContext.News.Include(i => i.Images)
                    .Where(n => n.NewsStatus == NewsStatus.Published)
                    .OrderByDescending(n => n.TotalViews)
                    .Take(cnt)
                    .Select(n => n.ToHomePageNewsToShowDTO())
                    .ToListAsync();
            }
        }




        public async Task<List<News>> GetTopOfWeekNewsAsync()
        {
            return await _dbContext.News.Include(i => i.Images)
                .Where(n => n.PublishedDate >= DateTime.Now.AddDays(-100) && n.NewsStatus == NewsStatus.Published)
                .OrderByDescending(n => n.TotalViews)
                .Take(5)
                .ToListAsync();
        }




        public async Task<int> GetTotalNewsCountByTypeAsync(NewsType newsType)
        {
            return await _dbContext.News.AsNoTracking().CountAsync(n => n.NewsType == newsType);
        }




        public async Task IncrementNewsViewsCount(Guid newsId)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"UPDATE News_Portal.News SET TotalViews = TotalViews + 1 WHERE NewsId = {newsId}");
        }




        public async Task<bool> NewsExistsBuId(Guid newsId)
        {
            return await _dbContext.News.AsNoTracking().AnyAsync(n => n.NewsId == newsId);
        }



        public async Task UpdateNews(News news)
        {
            _dbContext.News.Update(news);
            await _dbContext.SaveChangesAsync();
        }
    }
}
