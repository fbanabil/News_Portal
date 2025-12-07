using Microsoft.EntityFrameworkCore;
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
        public NewsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddNews(News news)
        {
            await _dbContext.News.AddAsync(news);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<News> GetNewsById(Guid newsId)
        {
            return await _dbContext.News.Include(i=>i.Author).Include(i=>i.Images).Include(i=>i.Comments).ThenInclude(i=>i.User).FirstOrDefaultAsync(n=>n.NewsId == newsId);
        }

        public async Task<List<News>> GetNewsByTypeAsync(NewsType newsType, int pageNo, int pageSize)
        {
            return await _dbContext.News.Include(i=>i.Images).Where(n => n.NewsType == newsType).
                OrderByDescending(p=>p.PublishedDate).Skip((pageNo-1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<HomePageNewsToShowDTO>> GetNewsForHomePageCarouselAsync()
        {
            return await _dbContext.News.Include(i => i.Images).OrderByDescending(n => n.PublishedDate)
                .Take(9)
                .Select(n => n.ToHomePageNewsToShowDTO())
                .ToListAsync();
        }

        public async Task<List<HomePageNewsToShowDTO>> GetOtherNewsByTypeAsync(NewsType newsType)
        {
            return await _dbContext.News.Include(i => i.Images).Where(n => n.NewsType == newsType)
                .OrderByDescending(n => n.PublishedDate)
                .Take(5)
                .Select(n => n.ToHomePageNewsToShowDTO())
                .ToListAsync();
        }

        public async Task<List<News>> GetTopOfWeekNewsAsync()
        {
            return await _dbContext.News.Include(i => i.Images)
                .Where(n => n.PublishedDate >= DateTime.Now.AddDays(-100))
                .OrderByDescending(n => n.TotalViews)
                .Take(5)
                .ToListAsync();
        }

        public async Task<int> GetTotalNewsCountByTypeAsync(NewsType newsType)
        {
            return await _dbContext.News.AsNoTracking().CountAsync(n => n.NewsType == newsType);
        }

        public async Task<bool> NewsExistsBuId(Guid newsId)
        {
            return await _dbContext.News.AsNoTracking().AnyAsync(n => n.NewsId == newsId);
        }

        public async Task UpdateNews(News newsx)
        {
            _dbContext.News.Update(newsx);
            await _dbContext.SaveChangesAsync();
        }
    }
}
