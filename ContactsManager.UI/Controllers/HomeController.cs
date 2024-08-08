﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CRUDExample.Controllers;

[Route("[controller]")]
public class HomeController : Controller
{
    [AllowAnonymous]
    [Route("[action]")]
    public IActionResult Error()
    {
        IExceptionHandlerPathFeature? exceptionHandlerPathFeature =
            HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature != null &&
            exceptionHandlerPathFeature.Error != null)
            ViewBag.ErrorMessage = exceptionHandlerPathFeature.Error.Message;

        return View();
    }
}
