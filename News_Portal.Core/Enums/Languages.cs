using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Enums
{
    public enum Languages
    {
        English,

        [Display(Name = "বাংলা")]
        Bangla
    }
}
