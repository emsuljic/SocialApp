using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using API.Data;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        //next -> what's gona happen next when action executed
        //us next to execute the action and do smth 

        //we want acces the context after it is executed
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //get context after the action is executed
            var resultContext = await next();

            //check to see if users are authenticated
            //if the user sent up a token and we authenticate user then it's true, otherwise it's false
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            //if they are authenticated then update lastActive property
            var userId = resultContext.HttpContext.User.GetUserId();
            //get acces to repository, use service located
            var uow = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            //get hold our user object
            var user = await uow.UserRepository.GetUserByIdAsync(userId);
            //set lastActive property
            user.LastActive = DateTime.Now;
            await uow.Complete();
        }
    }
}