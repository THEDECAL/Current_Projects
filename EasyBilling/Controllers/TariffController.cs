﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EasyBilling.Controllers
{
    public class TariffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
