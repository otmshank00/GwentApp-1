using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GwentApp.Models;
using Galactic.ActiveDirectory;
using ADUser = Galactic.ActiveDirectory.User;

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

        [HttpPost]
        public ActionResult ProcessIndexForm(Player player)
        {
            return View(player);
        }

        public ActionResult CreateDeck()
        {
            List<Card> deck = new List<Card>() {
                new Card() { Name = "Archer", Power = 1 },
                new Card() { Name = "Catapult", Power = 10 },
                new Card() { Name = "Bozo the Clown", Power = 3 },
                new Card() { Name = "Will", Power = 99 }
            };

            return View(deck);
        }

        public ActionResult PlayerInfo()
        {
            ActiveDirectory ad = GwentApp.Global.ad;

            ADUser user = new ADUser(ad, ad.GetGUIDBySAMAccountName(User.Identity.Name));

            return View(user);
        }
    }
}