using System.ComponentModel.DataAnnotations;

namespace News_Portal.Core.Enums
{
    public enum TopOfXType
    {
        [Display(Name = "পিন করা")]
        Pinned,

        [Display(Name = "সাপ্তাহিক")]
        Week,

        [Display(Name = "মাসিক")]
        Month,

        [Display(Name = "সর্বকালের")]
        AllTime
    }
}