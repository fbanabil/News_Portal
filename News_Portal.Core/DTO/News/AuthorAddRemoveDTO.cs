using Microsoft.AspNetCore.Mvc;
using News_Portal.Core.DTO.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.News
{
    public class AuthorAddRemoveDTO
    {
        [DataType(DataType.EmailAddress,ErrorMessage = "Must be an email address")]
        [Remote(action: "ValidAuthorEmailToAdd", controller: "Account", areaName: "Identity", ErrorMessage = "Not a valid User To Add")]
        public string? AddEmail { get; set; }




        [DataType(DataType.EmailAddress, ErrorMessage = "Must be an email address")]
        [Remote(action: "IsAnAuthor", controller: "Account", areaName: "Identity", ErrorMessage = "Not a valid User To Add")]
        public string? RemoveEmail { get; set; }
    }
}
