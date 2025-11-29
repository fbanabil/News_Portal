using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.News
{
    public class HomePageNewsToShowDTO
    {
        public Guid NewsId { get; set; }

        public string? NewsTitle { get; set; }

        public string? NewsContent { get; set; }

        public DateTime PublishedDate { get; set; }

        public DateTime LastUpdate { get; set; }

        public int TotalViews { get; set; }

        public ApplicationUser? Author { get; set; }

        public string? TitleImageUrl { get; set; }

    }
}
