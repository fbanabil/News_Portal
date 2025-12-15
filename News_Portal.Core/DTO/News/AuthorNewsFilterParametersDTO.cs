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
    public class AuthorNewsFilterParametersDTO
    {
        public string? NewsTitle { get; set; }
        public string? NewsContent { get; set; }
        public NewsType? NewsType { get; set; }
        public DateTime? PublishedDate { get; set; }
        public NewsStatus? NewsStatus { get; set; } 
        public NewsPriority? NewsPriority { get; set; }
    }
}
