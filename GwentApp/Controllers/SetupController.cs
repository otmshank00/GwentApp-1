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
        // GET: Setup/ApiExamples
        public ActionResult ApiExamples()
        {
            return View();
        }

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
            Global.gAppOptions = GwentApp.Controllers.AdminPageController.ReadAppOptions(Global.gwentAppConfig.Value);

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
            //Assign faction perk to player model
            player.Faction.FactionPerk = Global.gAllFactions.Find(i => i.FactionAbbr == player.Faction.FactionAbbr).FactionPerk;
            //Assign leader ability to player model
            player.Leader.LeaderAbility = Global.gAllLeaders.Find(i => i.LeaderName == player.Leader.LeaderName).LeaderAbility;
            //This is a place holder of the faction abbreviation used in the SQL queries.
            string factionAbbreviation = player.Faction.FactionAbbr;
            try
            {
                player.Deck = new List<Card>();
                player.Deck = BuildDeck(factionAbbreviation);
                //Build the start deck
                //This is the initial draw the player starts with
                player.Hand = new List<Card>();
                //Moved to function
                player.Hand = DealHand(player.Deck, player.Faction.FactionAbbr);

                //You need to remove the hand cards from the deck
                foreach (Card c in player.Hand)
                {
                    player.Deck.Remove(c);
                }

                //Sort the Hand based on card range (close, range, siege). This is because im neurotic.
                //player.Deck.Sort((x, y) => string.Compare(x.Range, y.Range));
                player.Hand.Sort((x, y) => string.Compare(x.Range, y.Range));

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

        public static List<Card> Shuffle (List<Card> DeckToShuffle)
        {
            Card tempCard = new Card();
            Random r = new Random();
            int randomNumber;
            for (int i = DeckToShuffle.Count - 1; i > 0; i--)
            {
                randomNumber = r.Next(0, i + 1);
                tempCard = DeckToShuffle[i];
                DeckToShuffle[i] = DeckToShuffle[randomNumber];
                DeckToShuffle[randomNumber] = tempCard;
            }
            return DeckToShuffle;
        }

        public static List<Card> DealHand(List<Card> startDeck, string factionAbbreviation)
        {
            List<Card> dealtHand = new List<Card>();
            List<Card> factionSearchResults = new List<Card>();
            //Stack the deck with Min Faction Units
            //Create a list to store the faction search results
            factionSearchResults = (startDeck.FindAll(c => c.Faction == factionAbbreviation)).ToList<Card>();
            factionSearchResults = Shuffle(factionSearchResults);
            //Add the MinFactionUnits to the start deck
            for (int f = 0; f < Global.gAppOptions.MinFactionUnits; f++)
            {
                Card c = new Card();
                c = factionSearchResults[f];
                dealtHand.Add(c);
                startDeck.Remove(c);

            }
            //Shuffle the player deck
            startDeck = Shuffle(startDeck);
            //don't pick randomly, just deal top StartingDeckSize-MinFactionUnits from shuffled deck above
            for (int deal = 0; deal < (Global.gAppOptions.StartingDeckSize - Global.gAppOptions.MinFactionUnits); deal++)
            {
                dealtHand.Add(startDeck[deal]);
                //player.Deck.RemoveAt(deal);
            }
            return dealtHand;
        }

        public static List<Card> BuildDeck(string factionAbbreviation)
        {
            List<Card> builtDeck = new List<Card>();
            //////////////
            //Start Copy
            //////////////

            //Right here we will build the MAX player deck based off the motherlode list (not the start deck)
            //We will need to select out the following

            //Weather
            List<Card> allWeatherCards = new List<Card>();
            allWeatherCards.AddRange((Global.gAllCards.FindAll(c => c.Range == Global.gAppOptions.WeatherIdentifier)).ToList<Card>());
            allWeatherCards = Shuffle(allWeatherCards);

            //Neutral Units
            List<Card> allNeutralUnits = new List<Card>();
            allNeutralUnits.AddRange((Global.gAllCards.FindAll(c => (c.Faction == Global.gAppOptions.NeutralIdentifier && c.Hero == false && c.Range != Global.gAppOptions.WeatherIdentifier))).ToList<Card>());
            allNeutralUnits = Shuffle(allNeutralUnits);

            //Neutral Heroes
            List<Card> allNeutralHeroes = new List<Card>();
            allNeutralHeroes.AddRange((Global.gAllCards.FindAll(c => (c.Faction == Global.gAppOptions.NeutralIdentifier && c.Hero == true))).ToList<Card>());
            allNeutralHeroes = Shuffle(allNeutralHeroes);

            //Neutral Specials
            List<Card> allNeutralSpecials = new List<Card>();
            allNeutralSpecials.AddRange((Global.gAllCards.FindAll(c => ((c.Range != Global.gAppOptions.CloseIdentifier) && (c.Range != Global.gAppOptions.RangedIdentifier) && (c.Range != Global.gAppOptions.SiegeIdentifier) && (c.Range != Global.gAppOptions.WeatherIdentifier)))).ToList<Card>());
            allNeutralSpecials = Shuffle(allNeutralSpecials);

            //Faction Units
            List<Card> allFactionUnits = new List<Card>();
            //allFactionUnits.AddRange((Global.gAllCards.FindAll(c => c.Faction == player.Faction.FactionAbbr && c.Hero == false)).ToList<Card>());
            allFactionUnits.AddRange((Global.gAllCards.FindAll(c => c.Faction == factionAbbreviation && c.Hero == false)).ToList<Card>());
            allFactionUnits = Shuffle(allFactionUnits);

            //Faction Heroes
            List<Card> allFactionHeroes = new List<Card>();
            //allFactionHeroes.AddRange((Global.gAllCards.FindAll(c => c.Faction == player.Faction.FactionAbbr && c.Hero == true)).ToList<Card>());
            allFactionHeroes.AddRange((Global.gAllCards.FindAll(c => c.Faction == factionAbbreviation && c.Hero == true)).ToList<Card>());
            allFactionHeroes = Shuffle(allFactionHeroes);

            //Add the shuffled cards to the start deck, selecting only the amount specified in the appoptions config file
            builtDeck.AddRange(allFactionHeroes.GetRange(0, Global.gAppOptions.MaxFactionHeroes));
            builtDeck.AddRange(allFactionUnits.GetRange(0, Global.gAppOptions.MaxFactionUnits));
            builtDeck.AddRange(allNeutralHeroes.GetRange(0, Global.gAppOptions.MaxNeutralHeroes));
            builtDeck.AddRange(allNeutralUnits.GetRange(0, Global.gAppOptions.MaxNeutralUnits));
            builtDeck.AddRange(allNeutralSpecials.GetRange(0, Global.gAppOptions.MaxNeutralSpecials));
            builtDeck.AddRange(allWeatherCards.GetRange(0, Global.gAppOptions.MaxNeutralSpecials));
            //So what if our max deck requirement is not met?
            if (builtDeck.Count < Global.gAppOptions.MaxDeckSize)
            {
                List<Card> spilloverUnits = new List<Card>();
                allFactionUnits.RemoveRange(0, Global.gAppOptions.MaxFactionUnits);
                allFactionHeroes.RemoveRange(0, Global.gAppOptions.MaxFactionHeroes);
                allNeutralSpecials.RemoveRange(0, Global.gAppOptions.MaxNeutralSpecials);
                allNeutralUnits.RemoveRange(0, Global.gAppOptions.MaxNeutralUnits);
                spilloverUnits.AddRange(allFactionUnits.GetRange(0, allFactionUnits.Count));
                spilloverUnits.AddRange(allFactionHeroes.GetRange(0, allFactionHeroes.Count));
                spilloverUnits.AddRange(allNeutralSpecials.GetRange(0, allNeutralSpecials.Count));
                spilloverUnits.AddRange(allNeutralUnits.GetRange(0, allNeutralUnits.Count));
                spilloverUnits = Shuffle(spilloverUnits);
                builtDeck.AddRange(spilloverUnits.GetRange(0, (Global.gAppOptions.MaxDeckSize - builtDeck.Count)));
            }
            

            //////////////
            //End Copy
            //////////////
            return builtDeck;
        }
    }
}