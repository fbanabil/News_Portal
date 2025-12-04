using AutoFixture;
using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.Enums;

namespace News_Portal.Core.DemoData
{
    public class NewsDemoData
    {
        public static News[] CreateNewsArray()
        {
            var fixture = new Fixture();
            var random = new Random();

            var cloudinaryUrl = "https://res.cloudinary.com/dwkr48bj7/image/upload/DemoImage_mwi4jm.jpg";


            fixture.Customize<News>(composer => composer
                .With(n => n.NewsId, () => Guid.NewGuid())
                .With(n => n.NewsTitle, () => fixture.Create<string>())
                .With(n => n.NewsContent, () => fixture.Create<string>())
                .With(n => n.PublishedDate, () => DateTime.Now.AddDays(-fixture.Create<int>() % 365))
                .With(n => n.LastUpdate, () => DateTime.Now.AddHours(-fixture.Create<int>() % 24))
                .With(n => n.TotalViews, () => fixture.Create<int>() % 10000)
                .With(n => n.AuthorId, () => Guid.NewGuid())
                .With(n => n.NewsStatus, () => fixture.Create<NewsStatus>())
                .With(n => n.NewsPriority, () => fixture.Create<NewsPriority>())
                .With(n => n.NewsType, () => fixture.Create<NewsType>())
                .Without(n => n.Author) 
                .Without(n => n.Images) 
                .Without(n => n.Comments));


            var newsArray = fixture.CreateMany<News>(1000).ToArray();

            foreach (var news in newsArray)
            {
                news.NewsTitle = "YoYO";
                ApplicationUser author = fixture.Create<ApplicationUser>();
                news.Author = author;

                var imageCount = random.Next(1, 4); 
                var images = new List<Images>();
                var commentsList = new List<Comments>();

                for (int i = 0; i < imageCount; i++)
                {
                    var image = fixture.Create<Images>();
                    image.ImageId = Guid.NewGuid();
                    image.ImageUrl = cloudinaryUrl;
                    image.NewsId = news.NewsId;

                    Comments comments = fixture.Create<Comments>();
                    commentsList.Add(comments);
                    images.Add(image);
                }
                news.Comments = commentsList;
                news.Images = images;
            }

            return newsArray;
        }
    }
}
