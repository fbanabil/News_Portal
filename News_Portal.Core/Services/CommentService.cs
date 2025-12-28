using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.RepositoryContracts;
using News_Portal.Core.DTO.Comment;
using News_Portal.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public async Task<CommentToShowDTO> AddCommentAsync(CommentToAddDTO commentToAddDTO, Guid id)
        {
            Comments commentToAdd = commentToAddDTO.ToComment(id);
            await _commentRepository.AddCommentAsync(commentToAdd);
            return commentToAdd.ToCommentToShowDTO();
        }




        public async Task DeleteCommentById(Guid commentId)
        {
            await _commentRepository.DeleteCommentById(commentId);
        }





        public async Task DeleteCommentsByUserId(Guid id)
        {
            await _commentRepository.DeleteCommentsByUserId(id);
        }
    }
}
