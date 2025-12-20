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
       [Display(Name = "সাধারন")] Default, [Display(Name = "ছোট থেকে বড়")] Ascending, [Display(Name = "বড় থেকে ছোট")] Descending
    }
}
public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType()
                          .GetMember(value.ToString())
                          .FirstOrDefault();

        var display = member?.GetCustomAttribute<DisplayAttribute>();
        return display?.Name ?? value.ToString();
    }
}
