using News_Portal.Core.DTO.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.ServiceContracts
{
    public interface ICommentService
    {
        Task<CommentToShowDTO> AddCommentAsync(CommentToAddDTO commentToAddDTO, Guid id);
    }
}
