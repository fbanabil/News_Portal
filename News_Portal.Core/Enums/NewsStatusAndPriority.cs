using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Enums
{
    public enum NewsStatus
    {
        Pending, Published, Rejected, Hidden, Blacklisted
    }
    public enum NewsPriority
    {
        Low, Medium, High, Urgent
    }

    public enum NewsType
    {
        Sports, Politics, Technology, Entertainment, Health, Business, Science, World
    }
}
