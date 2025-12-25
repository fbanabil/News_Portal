using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.News
{
    public class AdminNewsAllFiltersDTO
    {
        public NewsFilterParametersDTO? parametersDTO;

        [DataType(DataType.EmailAddress, ErrorMessage = "Must be an email address")]
        [Remote(action: "IsAuthor", controller: "Account", areaName: "Identity", ErrorMessage = "Not an Author")]
        public string? AuthorEmail { get; set; }

        [Display(Name = "ক্রম : ")]
        public string? sortBy;
        public SortTypes sortType;
        public int pageNo;
        public int pageSize;

    }
}

