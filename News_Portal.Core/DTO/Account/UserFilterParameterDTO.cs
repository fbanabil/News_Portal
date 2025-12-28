using News_Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.Account
{
    public class UserFilterParameterDTO
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public UserTypes? UserType { get; set; }
    }
}
