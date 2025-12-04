using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.Enums;

namespace News_Portal.UI.Samples
{
    public class DemoClasses
    {

    }

    public class DemoNews
    {
        public Guid NewsId { get; set; }

        public string? NewsTitle { get; set; }

        public string? NewsContent { get; set; }

        public string? NewsType { get; set; }

        public DateTime PublishedDate { get; set; }

        public DateTime LastUpdate { get; set; }
        public int TotalViews { get; set; } = 0;

        public Guid AuthorId { get; set; }

        public string? NewsStatus { get; set; }

        public string? NewsPriority { get; set; }


        public News ToNews()
        {

            return new News()
            {
                NewsId = NewsId,
                NewsTitle = NewsTitle,
                NewsContent = NewsContent,
                NewsType = (NewsType)Enum.Parse(typeof(NewsType), NewsType ?? "General"),
                PublishedDate = PublishedDate,
                LastUpdate = LastUpdate,
                TotalViews = TotalViews,
                AuthorId = AuthorId,
                NewsStatus = (NewsStatus)Enum.Parse(typeof(NewsStatus), NewsStatus ?? "Pending"),
                NewsPriority = (NewsPriority)Enum.Parse(typeof(NewsPriority), NewsPriority ?? "Medium")
            };
        }

    }

    public class DemoRoles
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }


    }






    public class ApplicationUserDemo
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ConcurrencyStamp { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; } // Identity uses DateTimeOffset?
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string? PersonName { get; set; }
        public string? PersonImageUrl { get; set; }

        public ApplicationUser ToApplicationUser()
        {
            return new ApplicationUser()
            {
                Id = Id,
                UserName = UserName,
                Email = Email,
                PersonName = PersonName,
                PersonImageUrl = PersonImageUrl
            };
        }
    }

    public class DemoImage
    {
        public Guid ImageId { get; set; }
        public string? ImageUrl { get; set; }
        public Guid NewsId { get; set; }


        public Images ToImage()
        {
            return new Images()
            {
                ImageId = ImageId,
                ImageUrl = ImageUrl,
                NewsId =  NewsId
            };
        }
    }

    public class DemoComment
    {
        public Guid CommentId { get; set; }
        public string? CommentText { get; set; }
        public DateTime CommentDate { get; set; }
        public Guid NewsId { get; set; }
        public Guid UserId { get; set; }
        public Comments ToComment()
        {
            return new Comments()
            {
                CommentId = CommentId,
                CommentText = CommentText,
                CommentDate = CommentDate,
                NewsId = NewsId,
                UserId = UserId
            };
        }

    }
}


  //  {
  //  "CommentId": "35740d33-0029-422a-a346-0c6c6627863a",
  //  "CommentText": "Could you add sources for the second paragraph?",
  //  "CommentDate": "2024-05-20T03:07:16+00:00",
  //  "NewsId": "1497a2d2-169c-481a-8abd-ea4900f7119c",
  //  "UserId": "5dd18fe1-0951-4b1f-bbcb-731643d748cc"
  //}
//{
//    "ImageId": "5223b731-4abb-4dd6-8b89-3e0b7d68edac",
//    "ImageUrl": "https://picsum.photos/seed/5223b731-4abb-4dd6-8b89-3e0b7d68edac/1280/720",
//    "NewsId": "a7c92d3a-ed2b-4fe7-a15a-f64ffc4f9886"
//  },

//"Id": "67c08e11-49c5-46f2-98bd-f2dc0db8bb5d",
//    "UserName": "hasan.khan",
//    "NormalizedUserName": "HASAN.KHAN",
//    "Email": "hasan.khan@example.com",
//    "NormalizedEmail": "HASAN.KHAN@EXAMPLE.COM",
//    "EmailConfirmed": false,
//    "PasswordHash": null,
//    "SecurityStamp": "c4dedd3e-2b9b-4df4-9962-b7043f9c515d",
//    "ConcurrencyStamp": "ec398e19-4abf-4c67-847d-9d08b46c5879",
//    "PhoneNumber": null,
//    "PhoneNumberConfirmed": false,
//    "TwoFactorEnabled": false,
//    "LockoutEnd": null,
//    "LockoutEnabled": true,
//    "AccessFailedCount": 0,
//    "PersonName": "Hasan Khan",
//    "PersonImageUrl": "https://i.pravatar.cc/300?u=67c08e11-49c5-46f2-98bd-f2dc0db8bb5d"