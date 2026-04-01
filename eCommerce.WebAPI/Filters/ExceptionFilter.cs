using eCommerce.Model.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace eCommerce.WebAPI.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An exception occurred.");

            if (context.Exception is ClinetException)
            {
                context.ModelState.AddModelError("clientError", context.Exception.Message);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else
            {
                //context.ModelState.AddModelError("serverError", context.Exception.Message);
                context.ModelState.AddModelError("serverError", "Server side error, please check logs.");
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            var list = context.ModelState.Where(c => c.Value.Errors.Count > 0)
                .ToDictionary(c => c.Key, c => c.Value.Errors.Select(z=> z.ErrorMessage));

                context.Result = new JsonResult(new { 
                    errors = list
                });


        }
    }
}
