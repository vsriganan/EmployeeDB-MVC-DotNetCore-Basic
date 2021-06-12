using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        [Route("Error/{statuscode}")]
        public IActionResult HttpStatusCodeController(int statuscode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statuscode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the page could not be found.";
                    logger.LogWarning($"404 error occured. Path = {statusCodeResult.OriginalPath} and QueryString = {statusCodeResult.OriginalQueryString}");
                    break;
            }

            return View("NotFound");
        }

        [AllowAnonymous]
        [Route("Error")]
        public IActionResult HttpExceptionController()
        {
            var exceptionResult = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            logger.LogError($"The path {exceptionResult.Path} threw an exception {exceptionResult.Error}");

            return View("Error");
        }
    }
}
