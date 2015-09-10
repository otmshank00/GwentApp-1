using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GwentApp.Models;
using Galactic.ActiveDirectory;
using ADUser = Galactic.ActiveDirectory.User;
using Galactic.Sql;
using Galactic.Sql.MSSql;

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
            // Create the empty deck object.
            List<Card> deck = new List<Card>();

            // Wrap SQL calls in a try / catch so that any exceptions are caught and handled.
            try
            {
                // Create a utility to handle the SQL calls for this action.
                MSSqlUtility sqlUtil = new MSSqlUtility();

                // Execute a query to get the card values from the database.
                List<SqlRow> rows = sqlUtil.ExecuteQuery("select * from dbo.Cards", Global.connectionString, null);

                // Build the deck.

                if (rows != null)
                {
                    foreach (SqlRow row in rows)
                    {
                        Card card = new Card();
                        card.Name = (string)row["cardName"];
                        card.Power = (int)row["cardPower"];
                        deck.Add(card);
                    }
                }

                // This is our original manual way of creating the deck.
                /*List<Card> deck = new List<Card>() {
                    new Card() { Name = "Archer", Power = 1 },
                    new Card() { Name = "Catapult", Power = 10 },
                    new Card() { Name = "Bozo the Clown", Power = 3 },
                    new Card() { Name = "Will", Power = 99 }
                };*/
            }
            catch
            {
                // We're not going to do anything on an exception, we'll just return the empty deck initialized above.
            }

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