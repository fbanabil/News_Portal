using Microsoft.EntityFrameworkCore;
using News_Portal.Infrastructure.DbContext;
using News_Portal.UI.Middlewares;
using News_Portal.UI.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder);




var app = builder.Build();

using var serrviceScope = app.Services.CreateScope();
using var dbContext = serrviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

dbContext?.Database.Migrate();


if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    //app.UseMiddleware<ExceptionHandlingMiddleware>();
}



app.MapGet("/", () => Results.Redirect("/Home/Index"));

app.UseHsts();
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
