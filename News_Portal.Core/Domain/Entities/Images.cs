using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Domain.Entities
{
    public class Images
    {
        [Key]
        [Required]
        public Guid ImageId { get; set; }

        [Required(ErrorMessage = "Image URL can't be empty")]
        public string? ImageUrl { get; set; }
        
        [Required]
        public Guid NewsId { get; set; }

        [ForeignKey("NewsId")]
        public virtual News? News { get; set; }
    }
}
