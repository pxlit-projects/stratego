using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Stratego.Api.Models;
using Stratego.Common;

namespace Stratego.Api.Filters
{
    public class StrategoExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public StrategoExceptionFilterAttribute(ILogger logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception,
                $"An unhandled exception occurred in the application. Request: {GetRequestUrl(context)}");

            if (context.Exception is DataNotFoundException)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Result = new NotFoundResult();
            }
            else if (context.Exception is ApplicationException)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Result = new JsonResult(new ErrorModel(context.Exception));
            }
            else
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Result = new JsonResult(new ErrorModel(context.Exception));
            }

           
        }

        private string GetRequestUrl(ExceptionContext context)
        {
            if (context.HttpContext?.Request == null) return string.Empty;
            return $"{context.HttpContext.Request.Method} - {context.HttpContext.Request.GetDisplayUrl()}";
        }
    }
}