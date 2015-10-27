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
            //string varname = Request.QueryString["myVar"];

            //session variables
            //passed in cookies
            //string varsession = Session["mySessionVar"];

            //Initialize objects
            Player player = new Player();
            Player.FactionList = new List<FactionInfo>();
            Player.LeaderList = new List<LeaderInfo>();
            Player.DdFactionList = new List<SelectListItem>();
            Player.DdLeaderList = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();

            //ADD LEADERS AND FACTIONS TO PLAYER INFO
            //Add the drop down list item. Note that the TEXT property is not passed, only the VALUE. This causes pain.

            foreach (FactionInfo faction in Global.gAllFactions)
            {
                item = new SelectListItem();
                item.Text = faction.FactionName;
                item.Value = faction.FactionAbbr;
                Player.DdFactionList.Add(item);
                Player.FactionList.Add(faction);
            }
            
            //Add the drop down list item. Note that the TEXT property is not passed, only the VALUE. This causes pain.
            foreach (LeaderInfo leader in Global.gAllLeaders)
            {
                item = new SelectListItem();
                item.Text = leader.LeaderName;
                item.Value = leader.LeaderFactionAbbr;
                Player.DdLeaderList.Add(item);
                Player.LeaderList.Add(leader);
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
            try
            {
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //Right here we will build the MAX player deck based off the motherlode list (not the start deck)
                //We will need to select out the following

                //Weather
                List<Card> allWeatherCards = new List<Card>();
                allWeatherCards.AddRange((Global.gAllCards.FindAll(c => c.Range.ToString().Trim() == Global.gAppOptions.WeatherIdentifier)).ToList<Card>());
                while (allWeatherCards.Count > Global.gAppOptions.MaxWeatherCards)
                {
                    Random rnd = new Random();
                    int rand = rnd.Next(0, allWeatherCards.Count);
                    allWeatherCards.RemoveAt(rand);
                }

                //Neutral Units
                List<Card> allNeutralUnits = new List<Card>();
                allNeutralUnits.AddRange((Global.gAllCards.FindAll(c => (c.Faction.ToString().Trim() == Global.gAppOptions.NeutralIdentifier && c.Hero == false && c.Range.ToString().Trim() != Global.gAppOptions.WeatherIdentifier))).ToList<Card>());
                while (allNeutralUnits.Count > Global.gAppOptions.MaxNeutralUnits)
                {
                    Random rnd = new Random();
                    int rand = rnd.Next(0, allNeutralUnits.Count);
                    allNeutralUnits.RemoveAt(rand);
                }

                //Neutral Heroes
                List<Card> allNeutralHeroes = new List<Card>();
                allNeutralHeroes.AddRange((Global.gAllCards.FindAll(c => (c.Faction.ToString().Trim() == Global.gAppOptions.NeutralIdentifier && c.Hero == true))).ToList<Card>());
                while (allNeutralHeroes.Count > Global.gAppOptions.MaxNeutralHeroes)
                {
                    Random rnd = new Random();
                    int rand = rnd.Next(0, allNeutralHeroes.Count);
                    allNeutralHeroes.RemoveAt(rand);
                }

                //Neutral Specials
                List<Card> allNeutralSpecials = new List<Card>();
                allNeutralSpecials.AddRange((Global.gAllCards.FindAll(c => ((c.Range.ToString().Trim() != Global.gAppOptions.CloseIdentifier) && (c.Range.ToString().Trim() != Global.gAppOptions.RangedIdentifier) && (c.Range.ToString().Trim() != Global.gAppOptions.SiegeIdentifier) && (c.Range.ToString().Trim() != Global.gAppOptions.WeatherIdentifier)))).ToList<Card>());
                while (allNeutralSpecials.Count > Global.gAppOptions.MaxNeutralSpecials)
                {
                    Random rnd = new Random();
                    int rand = rnd.Next(0, allNeutralSpecials.Count);
                    allNeutralSpecials.RemoveAt(rand);
                }

                //Faction Units
                List<Card> allFactionUnits = new List<Card>();
                allFactionUnits.AddRange((Global.gAllCards.FindAll(c => c.Faction.ToString().Trim() == player.Faction.FactionAbbr && c.Hero == false)).ToList<Card>());
                while (allFactionUnits.Count > Global.gAppOptions.MaxFactionUnits)
                {
                    Random rnd = new Random();
                    int rand = rnd.Next(0, allFactionUnits.Count);
                    allFactionUnits.RemoveAt(rand);
                }

                //Faction Heroes
                List<Card> allFactionHeroes = new List<Card>();
                allFactionHeroes.AddRange((Global.gAllCards.FindAll(c => c.Faction.ToString().Trim() == player.Faction.FactionAbbr && c.Hero == true)).ToList<Card>());
                while (allFactionHeroes.Count > Global.gAppOptions.MaxFactionHeroes)
                {
                    Random rnd = new Random();
                    int rand = rnd.Next(0, allFactionHeroes.Count);
                    allFactionHeroes.RemoveAt(rand);
                }

                //Add the pared down cards to the start deck
                player.Deck = new List<Card>();
                player.Deck.AddRange(allFactionHeroes);
                player.Deck.AddRange(allFactionUnits);
                player.Deck.AddRange(allNeutralHeroes);
                player.Deck.AddRange(allNeutralUnits);
                player.Deck.AddRange(allNeutralSpecials);
                player.Deck.AddRange(allWeatherCards);

                //Build the start deck
                //This is the initial draw the player starts with
                player.StartDeck = new List<Card>();
                //Stack the deck with Min Faction Units
                for (int f = 0; f < Global.gAppOptions.MinFactionUnits; f++)
                {
                    //search and find a faction unit
                    Card factionCard = player.Deck.Find(c => c.Faction.ToString().Trim() == player.Faction.FactionAbbr);
                    player.StartDeck.Add(factionCard);
                    player.Deck.Remove(factionCard);
                }
                //for (int index = 0; index < (Global.gAppOptions.StartingDeckSize-Global.gAppOptions.MinFactionUnits); index++)
                while (player.StartDeck.Count < Global.gAppOptions.StartingDeckSize)
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
            catch (Exception err)
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