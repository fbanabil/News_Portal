using System.ComponentModel.DataAnnotations;

namespace News_Portal.Core.Enums
{
    public enum UserTypes
    {
        [Display(Name = "ব্যবহারকারী")]
        User,

        [Display(Name = "লেখক")]
        Author,

        [Display(Name = "প্রশাসক")]
        Admin
    }
}