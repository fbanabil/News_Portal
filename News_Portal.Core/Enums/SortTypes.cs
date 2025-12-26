using News_Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Enums
{
    public enum SortTypes
    {
        [Display(Name = "সাধারন")] Default,
        [Display(Name = "ছোট থেকে বড়")] Ascending,
        [Display(Name = "বড় থেকে ছোট")] Descending
    }

    public enum SortBy
    {
        [Display(Name = "শিরোনাম")]
        NewsTitle,

        [Display(Name = "মোট ভিউ")]
        TotalViews,

        [Display(Name = "স্ট্যাটাস")]
        NewsStatus,

        [Display(Name = "অগ্রাধিকার")]
        NewsPriority,

        [Display(Name = "বিভাগ")]
        NewsType,

        [Display(Name = "প্রকাশের তারিখ")]
        PublishedDate
    }
}

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        if (value == null) return string.Empty;

        var member = value.GetType()
                          .GetMember(value.ToString())
                          .FirstOrDefault();

        var display = member?.GetCustomAttribute<DisplayAttribute>();
        return display?.Name ?? value.ToString();
    }

    // Nullable enum overload (avoids checks in Razor)
    public static string? GetDisplayName<T>(this T? enumValue) where T : struct, Enum
    {
        if (!enumValue.HasValue) return null;
        return enumValue.Value.GetDisplayName();
    }

    public static string GetSortBy(this string s)
    {
        if(s==nameof(SortBy.NewsTitle))
            return SortBy.NewsTitle.GetDisplayName();
        else if (s == nameof(SortBy.TotalViews))
            return SortBy.TotalViews.GetDisplayName();
        else if (s == nameof(SortBy.NewsStatus))
            return SortBy.NewsStatus.GetDisplayName();
        else if (s == nameof(SortBy.NewsPriority))
            return SortBy.NewsPriority.GetDisplayName();
        else if (s == nameof(SortBy.NewsType))
            return SortBy.NewsType.GetDisplayName();
        else if (s == nameof(SortBy.PublishedDate))
            return SortBy.PublishedDate.GetDisplayName();
        else
            return string.Empty;
    }

    public static string GetSortByInEng(this string s)
    {
        if (s == nameof(SortBy.NewsTitle))
            return "News Title";
        else if (s == nameof(SortBy.TotalViews))
            return "Total Views";
        else if (s == nameof(SortBy.NewsStatus))
            return "News Status";
        else if (s == nameof(SortBy.NewsPriority))
            return "News Priority";
        else if (s == nameof(SortBy.NewsType))
            return "News Type";
        else if (s == nameof(SortBy.PublishedDate))
            return "Published Date";
        else
            return string.Empty;
    }
}
