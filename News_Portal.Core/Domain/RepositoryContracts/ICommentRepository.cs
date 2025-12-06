using News_Portal.Core.Domain.Entities;
using News_Portal.Core.DTO.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Domain.RepositoryContracts
{
    public interface ICommentRepository
    {
        Task AddComment(Comments comment);
        Task AddCommentAsync(Comments commentToAddDTO);
        Task<bool> CommentExistsById(Guid commentId);
    }
}
