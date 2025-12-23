using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.News
{
    public class SideBarDTO
    {
        public AuthorAddRemoveDTO? authorAddRemoveDTO { get; set; }
        public AdminsNewsDetailesToShowDTO? adminsNewsDetailedDTO { get; set; }
    }
}
