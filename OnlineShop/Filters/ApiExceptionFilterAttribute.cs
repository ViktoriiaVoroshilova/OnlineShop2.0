using System.Net;
using DataAccess.EF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is DbUpdateConcurrencyException exception)
        {
            var entity = exception.Entries.SingleOrDefault();
            var model = entity?.Entity as BaseModel;
            if (entity?.Context.Find(entity.Entity.GetType(), model!.Id) == null)
            {
                context.Result = new NotFoundResult();
                return;
            }
            context.Result = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}