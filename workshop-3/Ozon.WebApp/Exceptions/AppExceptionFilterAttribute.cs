using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ozon.WebApp.Exceptions;

internal sealed class AppExceptionFilterAttribute: ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ValidationException ex:
                HandleEx(context, ex.Message, HttpStatusCode.BadRequest);
                return;
            
            case { } ex:
                HandleEx(context, "Возникла ошибка. Обратитесь к разработчикам", HttpStatusCode.InternalServerError);
                return;

        }
        base.OnException(context);
    }

    private void HandleEx(ExceptionContext context, string message, HttpStatusCode code)
    {
        context.Result = new JsonResult(new ReturnObject(message))
        {
            StatusCode = (int)code
        };
    }
}

public record ReturnObject(string Error)
{
    public override string ToString()
    {
        return $"{{ Error = {Error} }}";
    }
}