using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Zodo.Assets.Website.Controllers
{
    public class StockWorkController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}