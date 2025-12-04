using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.DTO.Comment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.Comment
{
    public class CommentToShowDTO
    {     
        public Guid CommentId { get; set; }
        public string? CommentText { get; set; }
        public DateTime CommentDate { get; set; }
        public string? UserName { get; set; }
    }
}

public static class CommentExtension
{
    public static CommentToShowDTO ToCommentToShowDTO(this Comments comment)
    {
        return new CommentToShowDTO
        {
            CommentId = comment.CommentId,
            CommentText = comment.CommentText,
            CommentDate = comment.CommentDate,
            UserName = comment.User?.UserName
        };
    }

}
