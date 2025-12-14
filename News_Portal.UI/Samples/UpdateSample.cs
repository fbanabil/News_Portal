using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.Domain.RepositoryContracts;
using News_Portal.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace News_Portal.UI.Samples
{
    public class UpdateSample : IUpdateSample
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<UpdateSample> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly INewsRepository _newsRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ICommentRepository _commentRepository;

        public UpdateSample(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<UpdateSample> logger,
            IWebHostEnvironment env,
            INewsRepository newsRepository,
            ICommentRepository commentRepository,
            IImageRepository imageRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _env = env;
            _newsRepository = newsRepository;
            _imageRepository = imageRepository;
            _commentRepository = commentRepository;
        }

        public async Task UpdateAsync()
        {
            // Path: Samples/application_users.json (ensure Copy to Output Directory if needed)
            var usersPath = Path.Combine(_env.ContentRootPath, "Samples", "application_users.json");
            var rolesPath = Path.Combine(_env.ContentRootPath, "Samples", "user_roles.json");

            string rawUser = System.IO.File.ReadAllText(usersPath);
            string rawRoles = System.IO.File.ReadAllText(rolesPath);

            List<ApplicationUserDemo>? users = System.Text.Json.JsonSerializer.Deserialize<List<ApplicationUserDemo>>(rawUser);
            List<DemoRoles>? roles = System.Text.Json.JsonSerializer.Deserialize<List<DemoRoles>>(rawRoles);

            _logger.LogInformation(users?.Count().ToString());
            _logger.LogInformation(roles?.Count().ToString());

            if (_roleManager.FindByNameAsync(UserTypes.Admin.ToString()).Result == null)
            {
                await _roleManager.CreateAsync(new ApplicationRole() { Name = UserTypes.Admin.ToString(), Id=Guid.Parse("ebf9614e-bb17-4fc4-9293-a538ecd4395d") });
            }
            if (_roleManager.FindByNameAsync(UserTypes.Author.ToString()).Result == null)
            {
                await _roleManager.CreateAsync(new ApplicationRole() { Name = UserTypes.Author.ToString(), Id=Guid.Parse("fe21c474-cb69-4358-b570-85093239b166") });
            }
            if (_roleManager.FindByNameAsync(UserTypes.User.ToString()).Result == null)
            {
                await _roleManager.CreateAsync(new ApplicationRole() { Name = UserTypes.User.ToString(), Id=Guid.Parse("f4c3b0e9-b34d-42ab-a517-d24c192071da") });
            }



            foreach (ApplicationUserDemo us in users)
            {
                ApplicationUser? appUs = us.ToApplicationUser();
                List<DemoRoles>? thisUser = roles?.Where(t => t.UserId == appUs.Id).ToList();

                ApplicationUser? applicationUser = await _userManager.FindByNameAsync(appUs.UserName);
                if (applicationUser == null)
                {
                    await _userManager.CreateAsync(appUs,"11111111");
                    foreach(DemoRoles r in thisUser)
                    {
                        if(r.RoleId == Guid.Parse("ebf9614e-bb17-4fc4-9293-a538ecd4395d"))
                        {
                            await _userManager.AddToRoleAsync(appUs, UserTypes.Admin.ToString());
                        }
                        else if(r.RoleId == Guid.Parse("fe21c474-cb69-4358-b570-85093239b166"))
                        {
                            await _userManager.AddToRoleAsync(appUs, UserTypes.Author.ToString());
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(appUs, UserTypes.User.ToString());

                        }

                    }
                }
            }



            var newsPath = Path.Combine(_env.ContentRootPath, "Samples", "news.json");

            var rawNews = System.IO.File.ReadAllText(newsPath);

            List<DemoNews>? newsList = System.Text.Json.JsonSerializer.Deserialize<List<DemoNews>>(rawNews);

            foreach (DemoNews news in newsList)
            {
                bool exists = await _newsRepository.NewsExistsBuId(news.NewsId);

                if (exists)
                {
                    News newsx = news.ToNews();
                    await _newsRepository.UpdateNews(newsx);
                    continue;
                }

                ApplicationUser? author = await _userManager.FindByIdAsync(news.AuthorId.ToString());

                if(author != null)
                {
                    News newsx = news.ToNews();
                    await _newsRepository.AddNews(newsx);
                }
            }



            var imagePath = Path.Combine(_env.ContentRootPath, "Samples", "images.json");

            var rawImages = System.IO.File.ReadAllText(imagePath);

            List<DemoImage>? imageList = System.Text.Json.JsonSerializer.Deserialize<List<DemoImage>>(rawImages);

            foreach (DemoImage img in imageList)
            {
                Images imggg = img.ToImage();
                bool exists = await _imageRepository.ImageExistsById(imggg.ImageId);

                if (exists)  
                {
                    continue;
                }

                bool nws = await _newsRepository.NewsExistsBuId(imggg.NewsId);

                if (nws)
                {
                    await _imageRepository.AddImage(imggg);
                }
            }




            var commentsPath = Path.Combine(_env.ContentRootPath, "Samples", "comments.json");

            var rawComments = System.IO.File.ReadAllText(commentsPath);

            List<DemoComment>? commentsList = System.Text.Json.JsonSerializer.Deserialize<List<DemoComment>>(rawComments);

            foreach (DemoComment cmt in commentsList)
            {
                Comments cmtss = cmt.ToComment();
                bool exists = await _commentRepository.CommentExistsById(cmtss.CommentId);

                if (exists)
                {
                    continue;
                }

                ApplicationUser? ex = await _userManager.FindByIdAsync(cmtss.UserId.ToString());
                if (ex == null)
                {
                    continue;
                }

                bool nws = await _newsRepository.NewsExistsBuId(cmtss.NewsId);

                if (nws)
                {
                    await _commentRepository.AddComment(cmtss);
                }
            }

            await _userManager.Users.ExecuteUpdateAsync(s => s.SetProperty(u => u.EmailConfirmed, u => true) );



        }
    }
   
}


