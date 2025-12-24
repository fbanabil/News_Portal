using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Domain.Entities
{
    public class PinnedNews
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid NewsId { get; set; }
    }
}
