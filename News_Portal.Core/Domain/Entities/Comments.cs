using News_Portal.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Domain.Entities
{
    public class Comments
    {
        [Key]
        [Required]
        public Guid CommentId { get; set; }
        [Required(ErrorMessage = "Comment text can't be empty")]
        public string? CommentText { get; set; }

        [Required]
        public DateTime CommentDate { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid NewsId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("NewsId")]
        public virtual News? News { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}
