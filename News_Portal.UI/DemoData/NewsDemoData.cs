using AutoFixture;
using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Enums;

namespace News_Portal.UI.DemoData
{
    public class NewsDemoData
    {
        public static News[] CreateNewsArray()
        {
            var fixture = new Fixture();
            var random = new Random();

            // Sample Cloudinary image URLs with different public_ids and extensions
            var cloudinaryUrl = "https://res.cloudinary.com/dwkr48bj7/image/upload/DemoImage_mwi4jm.jpg";


            // Configure AutoFixture to generate valid data for News entities
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
                .Without(n => n.Author) // Skip navigation property
                .Without(n => n.Images) // Will be manually assigned
                .Without(n => n.Comments)); // Skip navigation property

            // Create array of 10 News objects
            var newsArray = fixture.CreateMany<News>(100).ToArray();

            // Assign 1-3 images to each news article
            foreach (var news in newsArray)
            {
                var imageCount = random.Next(1, 4); // 1 to 3 images per news
                var images = new List<Images>();

                for (int i = 0; i < imageCount; i++)
                {
                    var image = fixture.Create<Images>();
                    image.ImageId = Guid.NewGuid();
                    image.ImageUrl = cloudinaryUrl;
                    image.NewsId = news.NewsId;
                    images.Add(image);
                }

                news.Images = images;
            }

            return newsArray;
        }
    }
}
