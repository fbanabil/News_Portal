using Microsoft.AspNetCore.Mvc.Filters;

namespace News_Portal.UI.Filters
{
    public class ModelStateValidationFilter : IAsyncActionFilter
    {
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(!context.ModelState.IsValid)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(context.ModelState);
                return Task.CompletedTask;
            }
            return next();
        }
    }
}
