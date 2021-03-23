using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Stratego.Api.Models;

namespace Stratego.Api.Controllers
{
    [Route("")]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        [Route("")]
        [OpenApiIgnore]
        public IActionResult Index()
        {
            return RedirectPermanent("~/swagger");
        }

        /// <summary>
        /// Shows that the api is up and running.
        /// </summary>
        [HttpGet("ping")]
        [ProducesResponseType(typeof(PingResultModel), StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            var model = new PingResultModel
            {
                IsAlive = true,
                Greeting = $"Hello on this fine {DateTime.Now.DayOfWeek}"
            };
            return Ok(model);
        }
    }
}