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
using System.Diagnostics;

namespace GwentApp.Controllers
{
    public class SetupController : Controller
    {
        // GET: Setup/Index
        public ActionResult Index()
        {
            playerChoices choices = new playerChoices();
            playerChoices.factionList = new List<factionInfo>();
            playerChoices.leaderList = new List<leaderInfo>();
            playerChoices.ddFactionList = new List<SelectListItem>();
            playerChoices.ddLeaderList = new List<SelectListItem>();
            Global.player.faction = new factionInfo();
            Global.player.leader = new leaderInfo();
            try
            {
                MSSqlUtility sqlUtil = new MSSqlUtility();
                List<SqlRow> rows = sqlUtil.ExecuteQuery("select * from dbo.Factions", Global.connectionString, null);
                if (rows != null)
                {
                    foreach (SqlRow row in rows)
                    {
                        factionInfo faction = new factionInfo();
                        SelectListItem item = new SelectListItem();
                        faction.factionName = ((string)row["factionName"]).Trim();
                        faction.factionAbbr = ((string)row["factionAbbreviation"]).Trim();
                        faction.factionPerk = ((string)row["factionPerk"]).Trim();
                        playerChoices.factionList.Add(faction);
                        item.Text = faction.factionName;
                        item.Value = faction.factionAbbr;
                        playerChoices.ddFactionList.Add(item);
                    }
                }
                rows = new List<SqlRow>();
                rows = sqlUtil.ExecuteQuery("select * from dbo.Leaders", Global.connectionString, null);
                if (rows != null)
                {
                    foreach (SqlRow row in rows)
                    {
                        leaderInfo leader = new leaderInfo();
                        SelectListItem item = new SelectListItem();
                        leader.leaderName = ((string)row["leaderName"]).Trim();
                        leader.leaderAbility = ((string)row["leaderAbility"]).Trim();
                        leader.leaderFaction = ((string)row["leaderFaction"]).Trim();
                        leader.leaderFactionAbbr = ((string)row["leaderFactionAbbreviation"]);
                        playerChoices.leaderList.Add(leader);
                        item.Text = leader.leaderName;
                        item.Value = leader.leaderFactionAbbr;
                        playerChoices.ddLeaderList.Add(item);
                    }
                }
            }
            catch
            {

            }
            return View(choices);
        }

        
        public ActionResult ProcessIndexForm(playerChoices choices)
        {
            
            return View(Global.player);
        }

        [HttpPost]
        public ActionResult CreateDeck(playerChoices choices)
        {
            // Create the empty deck object.
            //List<Card> deck = new List<Card>();
            Global.player.deck = new List<Card>();
            //define deck constants
            const int STARTING_DECK_SIZE = 10;
            const int MAX_DECK_SIZE = 28;
            const int MAX_WEATHER_CARDS = 4;
            const int MAX_N_UNITS = 3;
            const int MAX_N_HEROES = 3;
            const int MAX_N_SPECIALS = 3;
            const int MAX_F_HEROES = 3;
            const int MAX_F_UNITS = 12;
            Global.player.leader.leaderName = choices.selectedLeader.leaderName;
            //sort out faction
            //string factionAbbreviation = choices.selectedFaction.factionAbbr;
            //Global.player.leader.leaderFactionAbbr = choices.selectedLeader.leaderFactionAbbr;
            //Global.player.faction.factionAbbr = choices.selectedFaction.factionName;
            //choices.selectedFaction.factionAbbr = choices.selectedFaction.factionName;


            ///
            //new block
            ///
            Global.player.leader.leaderFactionAbbr = playerChoices.ddLeaderList.Find(i => i.Text == choices.selectedLeader.leaderName).Value;
            Global.player.faction.factionAbbr = Global.player.leader.leaderFactionAbbr;
            Global.player.faction.factionName = playerChoices.ddFactionList.Find(i => i.Value == Global.player.faction.factionAbbr).Text;
            string factionAbbreviation = Global.player.faction.factionAbbr;



            // Wrap SQL calls in a try / catch so that any exceptions are caught and handled.
            try
            {
                // Create a utility to handle the SQL calls for this action.
                MSSqlUtility sqlUtil = new MSSqlUtility();
                
                // Execute a query to get the card values from the database.
                List<SqlRow> rows = new List<SqlRow>();
                List<SqlRow> weatherRows = sqlUtil.ExecuteQuery("select * from AllWeather", Global.connectionString, null);
                List<SqlRow> neutralUnitsRows = sqlUtil.ExecuteQuery("select * from AllNeutralUnits", Global.connectionString, null);
                List<SqlRow> neutralHeroUnitsRows = sqlUtil.ExecuteQuery("select * from AllNeutralHeros", Global.connectionString, null);
                List<SqlRow> neutralSpecialUnitsRows = sqlUtil.ExecuteQuery("select * from AllNeutralSpecial", Global.connectionString, null);
                List<SqlRow> allFactionHeroesRows = sqlUtil.ExecuteQuery("select * from All" + factionAbbreviation + "Heros", Global.connectionString, null);
                List<SqlRow> allFactionUnitsRows = sqlUtil.ExecuteQuery("select * from All" + factionAbbreviation + "Units", Global.connectionString, null);
                //shuffle the deck and remove cards
                if (weatherRows != null)
                {
                    while (weatherRows.Count > MAX_WEATHER_CARDS)
                    {
                        Random rnd = new Random();
                        int rand = rnd.Next(0, weatherRows.Count);
                        weatherRows.RemoveAt(rand);
                    }
                }
                if (neutralUnitsRows != null)
                {
                    while (neutralUnitsRows.Count > MAX_N_UNITS)
                    {
                        Random rnd = new Random();
                        int rand = rnd.Next(0, neutralUnitsRows.Count);
                        neutralUnitsRows.RemoveAt(rand);
                    }
                }
                if (neutralHeroUnitsRows != null)
                {
                    while (neutralHeroUnitsRows.Count > MAX_N_HEROES)
                    {
                        Random rnd = new Random();
                        int rand = rnd.Next(0, neutralHeroUnitsRows.Count);
                        neutralHeroUnitsRows.RemoveAt(rand);
                    }
                }
                if (neutralSpecialUnitsRows != null)
                {
                    while (neutralSpecialUnitsRows.Count > MAX_N_SPECIALS)
                    {
                        Random rnd = new Random();
                        int rand = rnd.Next(0, neutralSpecialUnitsRows.Count);
                        neutralSpecialUnitsRows.RemoveAt(rand);
                    }
                }
                if (allFactionHeroesRows != null)
                {
                    while (allFactionHeroesRows.Count > MAX_F_HEROES)
                    {
                        Random rnd = new Random();
                        int rand = rnd.Next(0, allFactionHeroesRows.Count);
                        allFactionHeroesRows.RemoveAt(rand);
                    }
                }
                if (allFactionUnitsRows != null)
                {
                    while (allFactionUnitsRows.Count > MAX_F_UNITS)
                    {
                        Random rnd = new Random();
                        int rand = rnd.Next(0, allFactionUnitsRows.Count);
                        allFactionUnitsRows.RemoveAt(rand);
                    }
                }
                rows.AddRange(weatherRows);
                rows.AddRange(neutralUnitsRows);
                rows.AddRange(neutralHeroUnitsRows);
                rows.AddRange(neutralSpecialUnitsRows);
                rows.AddRange(allFactionHeroesRows);
                rows.AddRange(allFactionUnitsRows);
                // Build the deck.
                if (rows != null)
                {
                    foreach (SqlRow row in rows)
                    {
                        Card card = new Card();
                        try
                        {
                            card.Name = (string)row["cardName"];
                        }
                        catch
                        { }
                        try
                        {
                            card.Power = (int)row["cardPower"];
                        }
                        catch { }
                        try
                        {
                            card.Faction = (string)row["cardFaction"];
                        }
                        catch { }
                        try
                        {
                            card.Quote = (string)row["cardQuote"];
                        }
                        catch { }
                        try
                        {
                            card.Hero = (bool)(row["cardHero"]);
                        }
                        catch { }
                        try
                        {
                            card.Range = (string)row["cardRange"];
                        }
                        catch { }
                        try
                        {
                            card.Ability = (string)row["cardAbilities"];
                        }
                        catch { }

                        Global.player.deck.Add(card);
                    }
                }
                Global.player.deck.Sort((x, y) => string.Compare(x.Range, y.Range));
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

            return View(Global.player);
        }

        public ActionResult PlayerInfo()
        {
            ActiveDirectory ad = GwentApp.Global.ad;

            ADUser user = new ADUser(ad, ad.GetGUIDBySAMAccountName(User.Identity.Name));

            return View(user);
        }
    }
}