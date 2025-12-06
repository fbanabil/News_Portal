using News_Portal.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.Comment
{
    public class CommentToAddDTO
    {
        [Required]
        [MaxLength(200,ErrorMessage ="Can't be more than 200 characters")]
        public string? CommentText { get; set; }
        [Required]
        public Guid? NewsId { get; set; }

        public Comments ToComment(Guid UserId)
        {
            return new Comments()
            {
                CommentId = Guid.NewGuid(),
                CommentText = this.CommentText,
                NewsId = this.NewsId.Value,
                CommentDate = DateTime.UtcNow,
                UserId = UserId
            };
        }
    }
}
