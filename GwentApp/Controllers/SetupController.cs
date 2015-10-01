using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using GwentApp.Models;
using Galactic.ActiveDirectory;
using ADUser = Galactic.ActiveDirectory.User;
using Galactic.Sql;
using Galactic.Sql.MSSql;
using System.Diagnostics;
using Newtonsoft.Json;


namespace GwentApp.Controllers
{
    public class SetupController : Controller
    {
        // GET: Setup/Index
        public ActionResult Index()
        {
            //Configuration Data
            //This is a second sanity check to make sure we can read the configuration.
            //This will run if the Global.asax checks did not succeed.
            //This will force a creation of the config file so that the Global check should always work.
            if (System.IO.File.Exists(Global.gwentAppConfig.FilePath))
            {
                //Read the data here
                string gwentAppOptions = Global.gwentAppConfig.Value;
                if (gwentAppOptions.Length < 1)
                {
                    //Something went wrong opening file. Lets go to definition page
                    return Redirect("~/AdminPage/");
                }
                else
                {
                    
                }

            }
            else
            {
                //File doesn't exist. To the definition page with ye!
                return Redirect("~/AdminPage/");

            }

            //Initialize objects
            Player player = new Player();
            Player.FactionList = new List<FactionInfo>();
            Player.LeaderList = new List<LeaderInfo>();
            Player.DdFactionList = new List<SelectListItem>();
            Player.DdLeaderList = new List<SelectListItem>();

            //Read SQL query commands from options that (should) be loaded
            string SELECT_ALL_FACTIONS;
            string SELECT_ALL_LEADERS;
            Global.gAppOptions.adminOptions.TryGetValue("SELECT_ALL_FACTIONS", out SELECT_ALL_FACTIONS);
            Global.gAppOptions.adminOptions.TryGetValue("SELECT_ALL_LEADERS", out SELECT_ALL_LEADERS);

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
            string S_STARTING_DECK_SIZE;
            int STARTING_DECK_SIZE;
            Global.gAppOptions.adminOptions.TryGetValue("STARTING_DECK_SIZE", out S_STARTING_DECK_SIZE);
            int.TryParse(S_STARTING_DECK_SIZE, out STARTING_DECK_SIZE);
            //Max size of initialized deck
            string S_MAX_DECK_SIZE;
            int MAX_DECK_SIZE;
            Global.gAppOptions.adminOptions.TryGetValue("MAX_DECK_SIZE", out S_MAX_DECK_SIZE);
            int.TryParse(S_MAX_DECK_SIZE, out MAX_DECK_SIZE);
            //Max number of weather cards to be dealt
            string S_MAX_WEATHER_CARDS;
            int MAX_WEATHER_CARDS;
            Global.gAppOptions.adminOptions.TryGetValue("MAX_WEATHER_CARDS", out S_MAX_WEATHER_CARDS);
            int.TryParse(S_MAX_WEATHER_CARDS, out MAX_WEATHER_CARDS);
            //Max number of neutral unit cards to be dealt
            string S_MAX_NEUTRAL_UNITS;
            int MAX_NEUTRAL_UNITS;
            Global.gAppOptions.adminOptions.TryGetValue("MAX_NEUTRAL_UNITS", out S_MAX_NEUTRAL_UNITS);
            int.TryParse(S_MAX_NEUTRAL_UNITS, out MAX_NEUTRAL_UNITS);
            //Max number of neutral hero cards to be dealt
            string S_MAX_NEUTRAL_HEROES;
            int MAX_NEUTRAL_HEROES;
            Global.gAppOptions.adminOptions.TryGetValue("MAX_NEUTRAL_HEROES", out S_MAX_NEUTRAL_HEROES);
            int.TryParse(S_MAX_NEUTRAL_HEROES, out MAX_NEUTRAL_HEROES);
            //Max number of neutral special cards to be dealt
            string S_MAX_NEUTRAL_SPECIALS;
            int MAX_NEUTRAL_SPECIALS;
            Global.gAppOptions.adminOptions.TryGetValue("MAX_NEUTRAL_SPECIALS", out S_MAX_NEUTRAL_SPECIALS);
            int.TryParse(S_MAX_NEUTRAL_SPECIALS, out MAX_NEUTRAL_SPECIALS);
            //Max number of faction hero cards to be dealt
            string S_MAX_FACTION_HEROES;
            int MAX_FACTION_HEROES;
            Global.gAppOptions.adminOptions.TryGetValue("MAX_FACTION_HEROES", out S_MAX_FACTION_HEROES);
            int.TryParse(S_MAX_FACTION_HEROES, out MAX_FACTION_HEROES);
            //Max number of faction unit cards to be dealt
            string S_MAX_FACTIONS_UNITS;
            int MAX_FACTION_UNITS;
            Global.gAppOptions.adminOptions.TryGetValue("MAX_FACTION_UNITS", out S_MAX_FACTIONS_UNITS);
            int.TryParse(S_MAX_FACTIONS_UNITS, out MAX_FACTION_UNITS);

            //Define SQL query constants. These work off of existing views in the database.
            //Get all weather cards
            string SELECT_ALL_WEATHER_CARDS;
            Global.gAppOptions.adminOptions.TryGetValue("SELECT_ALL_WEATHER_CARDS", out SELECT_ALL_WEATHER_CARDS);
            //Get all neutral unit cards
            string SELECT_ALL_NEUTRAL_UNITS;
            Global.gAppOptions.adminOptions.TryGetValue("SELECT_ALL_NEUTRAL_UNITS", out SELECT_ALL_NEUTRAL_UNITS);
            //Get all neutral hero cards
            string SELECT_ALL_NEUTRAL_HEROES;
            Global.gAppOptions.adminOptions.TryGetValue("SELECT_ALL_NEUTRAL_HEROES", out SELECT_ALL_NEUTRAL_HEROES);
            //Get all neutral special cards
            string SELECT_ALL_NEUTRAL_SPECIALS;
            Global.gAppOptions.adminOptions.TryGetValue("SELECT_ALL_NEUTRAL_SPECIALS", out SELECT_ALL_NEUTRAL_SPECIALS);

            //Get the SQL to select all faction specific heroes
            string SELECT_ALL_NR_HEROES;
            Global.gAppOptions.adminOptions.TryGetValue("SELECT_ALL_NR_HEROES", out SELECT_ALL_NR_HEROES);
            string SELECT_ALL_NE_HEROES;
            Global.gAppOptions.adminOptions.TryGetValue("SELECT_ALL_NE_HEROES", out SELECT_ALL_NE_HEROES);
            string SELECT_ALL_MS_HEROES;
            Global.gAppOptions.adminOptions.TryGetValue("SELECT_ALL_MS_HEROES", out SELECT_ALL_MS_HEROES);
            string SELECT_ALL_ST_HEROES;
            Global.gAppOptions.adminOptions.TryGetValue("SELECT_ALL_ST_HEROES", out SELECT_ALL_ST_HEROES);

            //Get the SQL to select all faction specific units
            string SELECT_ALL_NR_UNITS;
            Global.gAppOptions.adminOptions.TryGetValue("SELECT_ALL_NR_UNITS", out SELECT_ALL_NR_UNITS);
            string SELECT_ALL_NE_UNITS;
            Global.gAppOptions.adminOptions.TryGetValue("SELECT_ALL_NE_UNITS", out SELECT_ALL_NE_UNITS);
            string SELECT_ALL_MS_UNITS;
            Global.gAppOptions.adminOptions.TryGetValue("SELECT_ALL_MS_UNITS", out SELECT_ALL_MS_UNITS);
            string SELECT_ALL_ST_UNITS;
            Global.gAppOptions.adminOptions.TryGetValue("SELECT_ALL_ST_UNITS", out SELECT_ALL_ST_UNITS);

            //Sort out what faction and heroes we're pulling units from
            string selectAllFUnits = "";
            string selectAllFHeroes = "";
            switch (factionAbbreviation)
            {
                case "NR":
                    selectAllFUnits = SELECT_ALL_NR_UNITS;
                    selectAllFHeroes = SELECT_ALL_NR_HEROES;
                    break;
                case "NE":
                    selectAllFUnits = SELECT_ALL_NE_UNITS;
                    selectAllFHeroes = SELECT_ALL_NE_HEROES;
                    break;
                case "MS":
                    selectAllFUnits = SELECT_ALL_MS_UNITS;
                    selectAllFHeroes = SELECT_ALL_MS_HEROES;
                    break;
                case "ST":
                    selectAllFUnits = SELECT_ALL_ST_UNITS;
                    selectAllFHeroes = SELECT_ALL_ST_HEROES;
                    break;
                default: //Good gracious, please never be here. For Temeria!
                    selectAllFUnits = SELECT_ALL_NR_UNITS;
                    selectAllFHeroes = SELECT_ALL_NR_HEROES;
                    break;
            }

            // Wrap SQL calls in a try / catch so that any exceptions are caught and handled.
            try
            {
                // Create a utility to handle the SQL calls for this action.
                MSSqlUtility sqlUtil = new MSSqlUtility();

                //Create an empty row list to be used as the deck holder
                List<SqlRow> rows = new List<SqlRow>();
                
                // Execute queries to get the card values from the database.
                List<SqlRow> weatherRows = sqlUtil.ExecuteQuery(SELECT_ALL_WEATHER_CARDS, Global.connectionString, null);
                List<SqlRow> neutralUnitsRows = sqlUtil.ExecuteQuery(SELECT_ALL_NEUTRAL_UNITS, Global.connectionString, null);
                List<SqlRow> neutralHeroUnitsRows = sqlUtil.ExecuteQuery(SELECT_ALL_NEUTRAL_HEROES, Global.connectionString, null);
                List<SqlRow> neutralSpecialUnitsRows = sqlUtil.ExecuteQuery(SELECT_ALL_NEUTRAL_SPECIALS, Global.connectionString, null);
                List<SqlRow> allFactionHeroesRows = sqlUtil.ExecuteQuery(selectAllFHeroes, Global.connectionString, null);
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
                    while (neutralUnitsRows.Count > MAX_NEUTRAL_UNITS)
                    {
                        Random rnd = new Random();
                        int rand = rnd.Next(0, neutralUnitsRows.Count);
                        neutralUnitsRows.RemoveAt(rand);
                    }
                }
                if (neutralHeroUnitsRows != null)
                {
                    while (neutralHeroUnitsRows.Count > MAX_NEUTRAL_HEROES)
                    {
                        Random rnd = new Random();
                        int rand = rnd.Next(0, neutralHeroUnitsRows.Count);
                        neutralHeroUnitsRows.RemoveAt(rand);
                    }
                }
                if (neutralSpecialUnitsRows != null)
                {
                    while (neutralSpecialUnitsRows.Count > MAX_NEUTRAL_SPECIALS)
                    {
                        Random rnd = new Random();
                        int rand = rnd.Next(0, neutralSpecialUnitsRows.Count);
                        neutralSpecialUnitsRows.RemoveAt(rand);
                    }
                }
                if (allFactionHeroesRows != null)
                {
                    while (allFactionHeroesRows.Count > MAX_FACTION_HEROES)
                    {
                        Random rnd = new Random();
                        int rand = rnd.Next(0, allFactionHeroesRows.Count);
                        allFactionHeroesRows.RemoveAt(rand);
                    }
                }
                if (allFactionUnitsRows != null)
                {
                    while (allFactionUnitsRows.Count > MAX_FACTION_UNITS)
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
                        //card.picturePath = Global.pictureMapPath+(card.Name.ToString().Trim().Replace(" ", "%20"))+".png";
                        card.picturePath = "~/Content/Images/" + (card.Name.ToString().Trim().Replace(" ", "%20")) + ".png";
                        player.Deck.Add(card);
                    }
                }

                //Build the start deck
                //This is the initial draw the player starts with
                player.StartDeck = new List<Card>();
                for (int index = 0; index < STARTING_DECK_SIZE; index++)
                {
                    Random rnd = new Random();
                    int rand = rnd.Next(0, player.Deck.Count);
                    player.StartDeck.Add(player.Deck[rand]);
                    player.Deck.RemoveAt(rand);
                }

                //Sort the deck based on card range (close, range, siege). This is because im neurotic.
                player.Deck.Sort((x, y) => string.Compare(x.Range, y.Range));
                player.StartDeck.Sort((x, y) => string.Compare(x.Range, y.Range));

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