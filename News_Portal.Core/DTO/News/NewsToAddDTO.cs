using Microsoft.AspNetCore.Http;
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
    public class NewsToAddDTO
    {

        [Required(ErrorMessage = "News title cant't be empty")]
        [MaxLength(100, ErrorMessage = "News title can't exceed 100 characters")]

        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "News content cant't be empty")]
        public string? NewsContent { get; set; }


        [Required]
        public NewsStatus NewsStatus { get; set; } = NewsStatus.Pending;

        [Required]
        public NewsPriority NewsPriority { get; set; } = NewsPriority.Medium;

        public List<IFormFile>? Images { get; set; }

    }
}
