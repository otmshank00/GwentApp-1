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
            //Initialize objects
            Player player = new Player();
            Player.FactionList = new List<FactionInfo>();
            Player.LeaderList = new List<LeaderInfo>();
            Player.DdFactionList = new List<SelectListItem>();
            Player.DdLeaderList = new List<SelectListItem>();

            //Define SQL query constants
            const string SELECT_ALL_FACTIONS = "select * from dbo.Factions";
            const string SELECT_ALL_LEADERS = "select * from dbo.Leaders";

            try
            {
                // Create a utility to handle the SQL calls for this action.
                MSSqlUtility sqlUtil = new MSSqlUtility();

                List<SqlRow> rows = sqlUtil.ExecuteQuery(SELECT_ALL_FACTIONS, Global.connectionString, null);
                if (rows != null)
                {
                    foreach (SqlRow row in rows)
                    {
                        //Assign the returned database values into the object
                        FactionInfo faction = new FactionInfo();
                        SelectListItem item = new SelectListItem();
                        faction.FactionName = ((string)row["factionName"]).Trim();
                        faction.FactionAbbr = ((string)row["factionAbbreviation"]).Trim();
                        faction.FactionPerk = ((string)row["factionPerk"]).Trim();
                        Player.FactionList.Add(faction);

                        //Add the drop down list item. Note that the TEXT property is not passed, only the VALUE. This causes pain.
                        item.Text = faction.FactionName;
                        item.Value = faction.FactionAbbr;
                        Player.DdFactionList.Add(item);
                    }
                }
                //Reinitialize the rows list.
                rows = new List<SqlRow>();
                rows = sqlUtil.ExecuteQuery(SELECT_ALL_LEADERS, Global.connectionString, null);

                if (rows != null)
                {
                    foreach (SqlRow row in rows)
                    {
                        //Assign the returned database values into the object
                        LeaderInfo leader = new LeaderInfo();
                        SelectListItem item = new SelectListItem();
                        leader.LeaderName = ((string)row["leaderName"]).Trim();
                        leader.LeaderAbility = ((string)row["leaderAbility"]).Trim();
                        leader.LeaderFaction = ((string)row["leaderFaction"]).Trim();
                        leader.LeaderFactionAbbr = ((string)row["leaderFactionAbbreviation"]);
                        Player.LeaderList.Add(leader);

                        //Add the drop down list item. Note that the TEXT property is not passed, only the VALUE. This causes pain.
                        item.Text = leader.LeaderName;
                        item.Value = leader.LeaderFactionAbbr;
                        Player.DdLeaderList.Add(item);
                    }
                }
            }
            catch
            {
                //Do we need to perform an action on catch?
            }
            return View(player);
        }

        [HttpPost]
        public ActionResult ProcessIndexForm(Player player)
        {

            
            return View(player);
        }

        [HttpPost]
        public ActionResult CreateDeck(Player player)
        {
            // Create the empty deck object, faction object and leader object.
            player.Deck = new List<Card>();
            player.Faction = new FactionInfo();
            player.Leader = new LeaderInfo();

            //Sort out the selected leader and faction from the model. This was harder than it should have been.
            //Assign the selected leader name to the player.Leader
            player.Leader.LeaderName = player.SelectedLeader.LeaderName;
            //Since the drop down list only sends the value (and not the text) I have to query the list to determine what faction abbreviation the selected leader belongs to
            player.Leader.LeaderFactionAbbr = Player.DdLeaderList.Find(i => i.Text == player.SelectedLeader.LeaderName).Value;
            //Assign the selected faction abbreviation to the player.Leader
            player.Faction.FactionAbbr = player.Leader.LeaderFactionAbbr;
            //Since the drop down list only sends the value (and not the text) I have to query the list to determine what faction name the selected leader belongs to
            player.Faction.FactionName = Player.DdFactionList.Find(i => i.Value == player.Faction.FactionAbbr).Text;
            //This is a place holder of the faction abbreviation used in the SQL queries.
            string factionAbbreviation = player.Faction.FactionAbbr;

            //Define deck constants
            //Final count of cards dealt to player from initialized deck
            const int STARTING_DECK_SIZE = 10;
            //Max size of initialized deck
            const int MAX_DECK_SIZE = 28;
            //Max number of weather cards to be dealt
            const int MAX_WEATHER_CARDS = 4;
            //Max number of neutral unit cards to be dealt
            const int MAX_N_UNITS = 3;
            //Max number of neutral hero cards to be dealt
            const int MAX_N_HEROES = 3;
            //Max number of neutral special cards to be dealt
            const int MAX_N_SPECIALS = 3;
            //Max number of faction hero cards to be dealt
            const int MAX_F_HEROES = 3;
            //Max number of faction unit cards to be dealt
            const int MAX_F_UNITS = 12;

            //Define SQL query constants. These work off of existing views in the database.
            //Get all weather cards
            const string SELECT_ALL_WEATHER_CARDS = "select * from AllWeather";
            //Get all neutral unit cards
            const string SELECT_ALL_N_UNITS = "select * from AllNeutralUnits";
            //Get all neutral hero cards
            const string SELECT_ALL_N_HEROS = "select * from AllNeutralHeros";
            //Get all neutral special cards
            const string SELECT_ALL_N_SPECIALS = "select * from AllNeutralSpecial";


            //Get all selected faction hero cards. Can't be a const because of the abbreviation trick
            string selectAllFHeros = "select * from All" + factionAbbreviation + "Heros";
            //Get all selected faction unit cards. Can't be a const because of the abbreviation trick
            string selectAllFUnits = "select * from All" + factionAbbreviation + "Units";



            // Wrap SQL calls in a try / catch so that any exceptions are caught and handled.
            try
            {
                // Create a utility to handle the SQL calls for this action.
                MSSqlUtility sqlUtil = new MSSqlUtility();

                //Create an empty row list to be used as the deck holder
                List<SqlRow> rows = new List<SqlRow>();
                
                // Execute queries to get the card values from the database.
                List<SqlRow> weatherRows = sqlUtil.ExecuteQuery(SELECT_ALL_WEATHER_CARDS, Global.connectionString, null);
                List<SqlRow> neutralUnitsRows = sqlUtil.ExecuteQuery(SELECT_ALL_N_UNITS, Global.connectionString, null);
                List<SqlRow> neutralHeroUnitsRows = sqlUtil.ExecuteQuery(SELECT_ALL_N_HEROS, Global.connectionString, null);
                List<SqlRow> neutralSpecialUnitsRows = sqlUtil.ExecuteQuery(SELECT_ALL_N_SPECIALS, Global.connectionString, null);
                List<SqlRow> allFactionHeroesRows = sqlUtil.ExecuteQuery(selectAllFHeros, Global.connectionString, null);
                List<SqlRow> allFactionUnitsRows = sqlUtil.ExecuteQuery(selectAllFUnits, Global.connectionString, null);
                
                //Shuffle the deck and remove a random card until our row count meets the defined max constant
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

                //Add the pared down cards to our deck holder, rows
                rows.AddRange(weatherRows);
                rows.AddRange(neutralUnitsRows);
                rows.AddRange(neutralHeroUnitsRows);
                rows.AddRange(neutralSpecialUnitsRows);
                rows.AddRange(allFactionHeroesRows);
                rows.AddRange(allFactionUnitsRows);

                //Build the deck by assigning the attributes from the database into the object attributes.
                //The TRY/CATCH is there to handle potential null values because C# null is not the same as DBNULL 
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

                        player.Deck.Add(card);
                    }
                }

                //Sort the deck based on card range (close, range, siege). This is because im neurotic.
                player.Deck.Sort((x, y) => string.Compare(x.Range, y.Range));
                
            }
            catch
            {
                // We're not going to do anything on an exception, we'll just return the empty deck initialized above.
            }

            return View(player);
        }

        public ActionResult PlayerInfo()
        {
            ActiveDirectory ad = GwentApp.Global.ad;

            ADUser user = new ADUser(ad, ad.GetGUIDBySAMAccountName(User.Identity.Name));

            return View(user);
        }
    }
}