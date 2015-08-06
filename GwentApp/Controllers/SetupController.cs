using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GwentApp.Models;

namespace GwentApp.Controllers
{
    public class SetupController : Controller
    {
        // GET: Setup/Index
        public ActionResult Index()
        {
            Player player = new Player();
            return View(player);
        }
    }
}