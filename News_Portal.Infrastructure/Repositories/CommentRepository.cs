using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.RepositoryContracts;
using News_Portal.Core.DTO.Comment;
using News_Portal.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {

        private readonly ApplicationDbContext _dbContext;

        public CommentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddComment(Comments comment)
        {
            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddCommentAsync(Comments commentToAddDTO)
        {
            await _dbContext.Comments.AddAsync(commentToAddDTO);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> CommentExistsById(Guid commentId)
        {
            Comments? comment = await _dbContext.Comments.FindAsync(commentId);
            return comment != null;
        }
    }
}
