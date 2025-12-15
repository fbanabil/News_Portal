using News_Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.DTO.News
{
    public class AuthorNewsAllFiltersDTO
    {
        public AuthorNewsFilterParametersDTO? parametersDTO;
        public string? sortBy;
        public SortTypes sortType;
        public int pageNo;
        public int pageSize;
    }
}
