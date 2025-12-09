using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Domain.Entities
{
    public class News
    {
        [Key]
        [Required]
        public Guid NewsId { get; set; }

        [Required(ErrorMessage = "News title cant't be empty")]
        [MaxLength(100, ErrorMessage = "News title can't exceed 100 characters")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "News content cant't be empty")]
        public string? NewsContent { get; set; }

        [Required]
        public NewsType NewsType { get; set; }

        [Required]
        public DateTime PublishedDate { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }
        public int TotalViews { get; set; } = 0;

        public string? VideoUrl { get; set; }

        [Required]
        public Guid AuthorId { get; set; }

        [Required]
        public NewsStatus NewsStatus { get; set; } = NewsStatus.Pending;

        [Required]
        public NewsPriority NewsPriority { get; set; } = NewsPriority.Medium;


        [ForeignKey("AuthorId")]
        public ApplicationUser? Author { get; set; }

        public virtual ICollection<Images>? Images { get; set; }
        public virtual ICollection<Comments>? Comments { get; set; }
    }
}
