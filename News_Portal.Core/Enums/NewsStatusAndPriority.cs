using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Enums
{
    public enum NewsStatus
    {
        [Display(Name = "মুলতুবি")]
        Pending,
        [Display(Name = "প্রকাশিত")]
        Published,
        [Display(Name = "প্রত্যাখ্যাত")]
        Rejected,
        [Display(Name = "লুকানো")]
        Hidden,
        [Display(Name = "কালো তালিকাভুক্ত")]
        Blacklisted
    }

    public enum NewsPriority
    {
        [Display(Name = "কম")]
        Low,
        [Display(Name = "মাঝারি")]
        Medium,
        [Display(Name = "উচ্চ")]
        High,
        [Display(Name = "জরুরি")]
        Urgent
    }

    public enum NewsType
    {
        [Display(Name = "খেলা")]
        Sports,
        [Display(Name = "রাজনীতি")]
        Politics,
        [Display(Name = "প্রযুক্তি")]
        Technology,
        [Display(Name = "বিনোদন")]
        Entertainment,
        [Display(Name = "স্বাস্থ্য")]
        Health,
        [Display(Name = "ব্যবসা-বাণিজ্য")]
        Business,
        [Display(Name = "বিজ্ঞান")]
        Science,
        [Display(Name = "বিশ্ব")]
        World
    }
}