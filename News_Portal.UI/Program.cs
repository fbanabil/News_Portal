using News_Portal.UI.StartupExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder);

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/", () => Results.Redirect("/Home/Index"));

app.UseHsts();
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();
