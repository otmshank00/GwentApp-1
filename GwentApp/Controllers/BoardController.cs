using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using GwentApp.Models;
using GwentApp.App_Code;
using GwentApp.Controllers;
using Newtonsoft.Json;
using System.Web.SessionState;

//-card play
//-round total
//-game total
//-pass action
//-leader power
//-faction perk

//methods using web service that throws data around and not the player model.
//The method updates the model

namespace GwentApp.Controllers
{
    //[WebAPIIsLocal]
    [SessionState(SessionStateBehavior.Required)]
    public class BoardController : Controller
    {
        // GET: Board
        public ActionResult Index()
        {
            Player player = new Player();
            player.PlayerGuid = new Guid();
            Player player2 = new Player();
            player2.PlayerGuid = new Guid();
            //player.PlayerBoardState = new BoardState();
            //player.PlayerBoardState.CloseCombat = new List<Card>();
            //player.PlayerBoardState.RangedCombat = new List<Card>();
            //player.PlayerBoardState.SiegeCombat = new List<Card>();
            //player.PlayerBoardState.CloseCombatTotal = 0;
            //player.PlayerBoardState.RangedCombatTotal = 0;
            //player.PlayerBoardState.SiegeCombatTotal = 0;
            //player = BuildDeckAndHand(player, "NR");
            //Session["Player"] = JsonConvert.SerializeObject(player);
            Session[player2.PlayerGuid.ToString()] = player2;
            return View(player);
         
            //return View();
        }

        [HttpPost]
        public ActionResult BuildBoard(Player player, string factionAbbr)
        {
            try
            {
                if (player.Hand.Count == 0)
                {

                }
            }
            catch
            {
                SelectListItem item = new SelectListItem();
                player.PlayerBoardState = new BoardState();
                player.PlayerBoardState.CloseCombat = new List<Card>();
                player.PlayerBoardState.RangedCombat = new List<Card>();
                player.PlayerBoardState.SiegeCombat = new List<Card>();
                player.Discard = new List<Card>();
                player.PlayerBoardState.CloseCombatTotal = 0;
                player.PlayerBoardState.RangedCombatTotal = 0;
                player.PlayerBoardState.SiegeCombatTotal = 0;
                player.RivalScore = 0;
                player.PlayerBoardState.RoundNumber = 1;
                player.WinCount = 0;
                player.PlayerBoardState.Passed = false;
                player.PlayerBoardState.HornInCloseRow = false;
                player.PlayerBoardState.HornInRangedRow = false;
                player.PlayerBoardState.HornInSiegeRow = false;
                player.SelectedFaction = SelectFaction(factionAbbr);
                player = BuildDeckAndHand(player, player.SelectedFaction.FactionAbbr);
                player.SelectedFaction = Global.gAllFactions.Find(f => f.FactionAbbr == player.SelectedFaction.FactionAbbr);
                player.SelectedLeader = (Global.gAllLeaders.FindAll(l => l.LeaderFactionAbbr == player.SelectedFaction.FactionAbbr))[0];
                //foreach (Card c in player.Hand)
                //{
                //    item = new SelectListItem();
                //    item.Text = c.CardId.ToString();
                //    item.Value = c.CardId.ToString();
                //}
            }
            //Session["Player"] = JsonConvert.SerializeObject(player);
            //return Redirect("~/Board/BoardView");
            //string redirectUriString = Url.RouteUrl("", new { action = "BoardView", controller = "Board", id = player }, Request.Url.Scheme);
            //string v = RedirectToAction("BoardView", "Board", new { player = player }).RouteName;
            //var x = RedirectToAction("BoardView", "Board", new { player = player }).RouteValues;
            Session[player.PlayerGuid.ToString()] = player;
            return RedirectToAction("BoardView", "Board"); //, new RouteValueDictionary(player));
        }

        
        public ActionResult BoardView(Player player)
        {
            //player = TempData["tempPlayer"] as Player;
            //ViewBag.player = player;
            //List<SelectListItem> playCard = new List<SelectListItem>();
            //player = JsonConvert.DeserializeObject<Player>(Session["Player"].ToString());

            //string tempFactionAbbr = player.SelectedFaction.FactionAbbr;
            //player.PlayerBoardState = new BoardState();
            //player.PlayerBoardState.CloseCombat = new List<Card>();
            //player.PlayerBoardState.RangedCombat = new List<Card>();
            //player.PlayerBoardState.SiegeCombat = new List<Card>();
            return View(player);
        }

        public void SelectLeader(int selectedLeader, Player player)
        {
            
        }

        public FactionInfo SelectFaction(string factionAbbr)
        {
            FactionInfo sf = new FactionInfo();
            List<FactionInfo> factionListReturn = new List<FactionInfo>();
            //Create web client to do call
            System.Net.WebClient client = new System.Net.WebClient();
            //Create the url to pull from
            string factionUriString = Url.RouteUrl("", new { action = "getfaction", controller = "Card", id = factionAbbr }, Request.Url.Scheme);
            //Get the faction and deserialize
            string factionReturn = client.DownloadString(factionUriString);
            factionListReturn = JsonConvert.DeserializeObject<List<FactionInfo>>(factionReturn);
            sf = factionListReturn[0];
            return sf;
        }

        /// <summary>
        /// Adds the played card to the appropriate list
        /// </summary>
        /// <returns></returns>
        [HttpGet]       
        public ActionResult PlayCard(Player player, int id, string row)
        {
            int closeHornModifier = 0;
            int rangedHornModifier = 0;
            int siegeHornModifier = 0;
            player = Session[player.PlayerGuid.ToString()] as Player;
            int playedCardId = id;
            Card c = new Card();
            List<Card> cl = new List<Card>();
            //Create web client to do call
            System.Net.WebClient client = new System.Net.WebClient();
            //Create the url to pull from
            string cardUriString = Url.RouteUrl("", new { action = "byid", controller = "Card", id = playedCardId }, Request.Url.Scheme);
            //Get the card and deserialize
            string cardReturn = client.DownloadString(cardUriString);
            //c = JsonConvert.DeserializeObject(cardReturn);
            cl = JsonConvert.DeserializeObject<List<Card>>(cardReturn);
            c = cl[0];
            switch (c.Range) //very much need to add weather and special effects
            { 
                case "Ranged":
                    player.PlayerBoardState.RangedCombat.Add(c);
                    player.PlayerBoardState.RangedCombatTotal += c.Power;
                    player = ProcessCardAbility(c, player, "");
                    
                    break;
                case "Close":
                    
                    player.PlayerBoardState.CloseCombat.Add(c);
                    player.PlayerBoardState.CloseCombatTotal += c.Power;
                    player = ProcessCardAbility(c, player, "");
                    break;
                case "Siege":
                    player.PlayerBoardState.SiegeCombat.Add(c);
                    player.PlayerBoardState.SiegeCombatTotal += c.Power;
                    player = ProcessCardAbility(c, player, "");
                    break;
                case "Weather":
                    switch (c.Name)
                    {
                        case "Biting Frost":
                            player.PlayerBoardState.IsSnowing = true;
                            break;
                        case "Torrential Rain":
                            player.PlayerBoardState.IsRaining = true;
                            break;
                        case "Impenetrable Fog":
                            player.PlayerBoardState.IsFoggy = true;
                            break;
                        case "Clear Weather":
                            //player.PlayerBoardState.IsSnowing = false;
                            //player.PlayerBoardState.IsRaining = false;
                            //player.PlayerBoardState.IsFoggy = false;
                            break;
                        default:
                            break;
                    }
                    break;
                case "Decoy":
                    
                    break;

                case "Horn":
                    if (row == "Close")
                    {
                        player.PlayerBoardState.CloseCombat.Add(c);
                        player.PlayerBoardState.CloseCombatTotal += c.Power;
                        //DONT ALLOW DOUBLING HORNS!!!!!
                        player = ProcessCardAbility(c, player, row);
                    }
                    if (row == "Ranged")
                    {
                        player.PlayerBoardState.RangedCombat.Add(c);
                        player.PlayerBoardState.RangedCombatTotal += c.Power;
                        //DONT ALLOW DOUBLING HORNS!!!!!
                        player = ProcessCardAbility(c, player, row);
                    }
                    if (row == "Siege")
                    {
                        player.PlayerBoardState.SiegeCombat.Add(c);
                        player.PlayerBoardState.SiegeCombatTotal += c.Power;
                        //DONT ALLOW DOUBLING HORNS!!!!!
                        player = ProcessCardAbility(c, player, row);
                    }
                    break;
                case "Scorch":

                    break;
               

                default:
                    //default will be replaced with other card types later
                    break;
            }

            //Now process weather effects on ranges based on bool
            //Biting Frost-Sets NON HERO close combat cards to 1 power
//TO DO            //Check the BOND
//TO DO adding commanders horn to weather affected row doubles original value not weather value
            //sdsdsds
            if (player.PlayerBoardState.IsSnowing)
            {
                if (player.PlayerBoardState.HornInCloseRow)
                {
                    closeHornModifier = 2;
                }
                else
                {
                    closeHornModifier = 1;
                }
                int closeWeatherTotal = 0;
                foreach (Card cc in player.PlayerBoardState.CloseCombat)
                {
                    //closeWeatherCardList.Add(cc);
                    if (!(cc.Hero))
                    {
                        closeWeatherTotal += 1 * closeHornModifier;
                    }
                    else
                    {
                        closeWeatherTotal += cc.Power;
                    }
                }
                player.PlayerBoardState.CloseCombatTotal = closeWeatherTotal;
            }
            //Impenetrable Fog-Sets NON HERO ranged combat cards to 1 power
            if (player.PlayerBoardState.IsFoggy)
            {
                if (player.PlayerBoardState.HornInRangedRow)
                {
                    rangedHornModifier = 2;
                }
                else
                {
                    rangedHornModifier = 1;
                }
                int rangedWeatherTotal = 0;
                foreach (Card cc in player.PlayerBoardState.RangedCombat)
                {
                    //closeWeatherCardList.Add(cc);
                    if (!(cc.Hero))
                    {
                        rangedWeatherTotal += 1 * rangedHornModifier;
                    }
                    else
                    {
                        rangedWeatherTotal += cc.Power;
                    }
                }
                player.PlayerBoardState.RangedCombatTotal = rangedWeatherTotal;
            }
            //Torrential Rain-Sets NON HERO siege combat cards to 1 power
            if (player.PlayerBoardState.IsRaining)
            {
                if (player.PlayerBoardState.HornInSiegeRow)
                {
                    siegeHornModifier = 2;
                }
                else
                {
                    siegeHornModifier = 1;
                }
                int siegeWeatherTotal = 0;
                foreach (Card cc in player.PlayerBoardState.SiegeCombat)
                {
                    if (!(cc.Hero))
                    {
                        siegeWeatherTotal += 1 * siegeHornModifier;
                    }
                    else
                    {
                        siegeWeatherTotal += cc.Power;
                    }
                }
                player.PlayerBoardState.SiegeCombatTotal = siegeWeatherTotal;
            }
            ///////
//TO DO            //BONDS ARE NOT RECALCULATED AFTER CLEAR WEATHER
            ///////
            //sdfsdfds
            if (player.PlayerBoardState.IsSnowing && c.Name == "Clear Weather")
            {
                //int closeClearWeatherTotal = 0;
                List<Card> tempCc = new List<Card>();
                tempCc.AddRange(player.PlayerBoardState.CloseCombat.GetRange(0, player.PlayerBoardState.CloseCombat.Count));
                player.PlayerBoardState.CloseCombatTotal = 0;
                player.PlayerBoardState.CloseCombat.Clear();
                foreach (Card cc in tempCc)
                {
                    player.PlayerBoardState.CloseCombat.Add(cc);
                    player.PlayerBoardState.CloseCombatTotal += cc.Power;
                    player = ProcessCardAbility(cc, player, "");
                }
                //player.PlayerBoardState.CloseCombatTotal = closeClearWeatherTotal;
                player.PlayerBoardState.IsSnowing = false;
            }

            if (player.PlayerBoardState.IsFoggy && c.Name == "Clear Weather")
            {
                List<Card> tempRc = new List<Card>();
                tempRc.AddRange(player.PlayerBoardState.RangedCombat.GetRange(0, player.PlayerBoardState.RangedCombat.Count));
                player.PlayerBoardState.RangedCombatTotal = 0;
                player.PlayerBoardState.RangedCombat.Clear();
                foreach (Card rc in tempRc)
                {
                    player.PlayerBoardState.RangedCombat.Add(rc);
                    player.PlayerBoardState.RangedCombatTotal += rc.Power;
                    player = ProcessCardAbility(rc, player, "");
                }
                //player.PlayerBoardState.RangedCombatTotal = rangedClearWeatherTotal;
                player.PlayerBoardState.IsFoggy = false;
            }

            if (player.PlayerBoardState.IsRaining && c.Name == "Clear Weather")
            {
                List<Card> tempSc = new List<Card>();
                tempSc.AddRange(player.PlayerBoardState.SiegeCombat.GetRange(0, player.PlayerBoardState.SiegeCombat.Count));
                player.PlayerBoardState.SiegeCombatTotal = 0;
                player.PlayerBoardState.SiegeCombat.Clear();
                foreach (Card sc in tempSc)
                {
                    player.PlayerBoardState.SiegeCombat.Add(sc);
                    player.PlayerBoardState.SiegeCombatTotal += sc.Power;
                    player = ProcessCardAbility(sc, player, "");
                }
                //player.PlayerBoardState.SiegeCombatTotal = siegeClearWeatherTotal;
                player.PlayerBoardState.IsRaining = false;
            }

            //Session["Player"] = JsonConvert.SerializeObject(player);
            //return View(player);
            //Remove the played card from hand
            Card cardToRemove = new Card();
            cardToRemove = player.Hand.Find(i => i.CardId == playedCardId);
            player.Hand.Remove(cardToRemove);
            //TempData["tempPlayer"] = player;
            Session[player.PlayerGuid.ToString()] = player;
            return Redirect("~/Board/BoardView/");
        }
        public Player ProcessCardAbility (Card c, Player player, string row)
        {
            switch (c.Ability)
            {
                case "Spy":
                    //add to opponent row
                    Session["SpyAddToOpponent"] = c;
                    //draw 2 cards
                    if (player.Deck.Count > 2)
                    {
                        player.Hand.Add(Draw(player.Deck));
                        player.Hand.Add(Draw(player.Deck));
                    }
                    break;
                case "Horn":
                    if (c.Range == "Horn")
                    {
                        c.Range = row;
                    }
                    switch (c.Range)
                    {
                        case ("Close"):
                            int hornCloseTotal = 0;
                            player.PlayerBoardState.HornInCloseRow = true;
                            foreach (Card cc in player.PlayerBoardState.CloseCombat)
                            {
                                if (!(cc.Hero))
                                {
                                    if (player.PlayerBoardState.IsSnowing)
                                    {
                                        hornCloseTotal += 1 * 2;
                                    }
                                    else
                                    {
                                        hornCloseTotal += cc.Power * 2;
                                    }
                                }
                                else
                                {
                                    hornCloseTotal += cc.Power;
                                }
                            }
                            player.PlayerBoardState.CloseCombatTotal = hornCloseTotal;
                            break;

                        case "Ranged":
                            int hornRangedTotal = 0;
                            player.PlayerBoardState.HornInRangedRow = true;
                            foreach (Card rc in player.PlayerBoardState.RangedCombat)
                            {
                                if (!(rc.Hero))
                                {
                                    if (player.PlayerBoardState.IsFoggy)
                                    {
                                        hornRangedTotal += 1 * 2;
                                    }
                                    else
                                    {
                                        hornRangedTotal += rc.Power * 2;
                                    }
                                }
                                else
                                {
                                    hornRangedTotal += rc.Power;
                                }
                            }
                            player.PlayerBoardState.RangedCombatTotal = hornRangedTotal;
                            break;

                        case "Siege":
                            int hornSiegeTotal = 0;
                            player.PlayerBoardState.HornInSiegeRow = true;
                            foreach(Card sc in player.PlayerBoardState.SiegeCombat)
                            {
                                if (!(sc.Hero))
                                {
                                    if (player.PlayerBoardState.IsRaining)
                                    {
                                        hornSiegeTotal += 1 * 2;
                                    }
                                    else
                                    {
                                        hornSiegeTotal += sc.Power * 2;
                                    }
                                }
                                else
                                {
                                    hornSiegeTotal += sc.Power;
                                }
                            }
                            player.PlayerBoardState.SiegeCombatTotal = hornSiegeTotal;
                            break;
                        default:
                            break;
                    }
                    break;
                case "Scorch":
                    //Need access to other players rows
                    break;
                case "Bond":
                    List<Card> potentialBondMates = new List<Card>();
                    
                    switch (c.Range)
                    {
                        case "Close":
                            potentialBondMates = player.PlayerBoardState.CloseCombat.FindAll(pbm => pbm.Name == c.Name);
                            if (potentialBondMates.Count > 1) //don't bond to yourself
                            {
                                //add half the bonded power. when the function returns, the default code will add the card.power
                                //by adding the base value twice you're adding the bonded double
                                player.PlayerBoardState.CloseCombatTotal += c.Power; //double yourself
                                //foreach (Card pbmc in potentialBondMates)
                                for (int x=0; x < (potentialBondMates.Count - 1); x++)
                                {
                                    Card pbmc = new Card();
                                    pbmc = potentialBondMates[x];
                                    if (c.Name == pbmc.Name)
                                    {
                                        //add the existing bonded card again
                                        player.PlayerBoardState.CloseCombatTotal += pbmc.Power;
                                    }
                                }
                            }
                            //potentialBondMates = player.PlayerBoardState.CloseCombat.FindAll(pbm => pbm.Name == c.Name);
                            //if (potentialBondMates.Count > 0)
                            //{
                            //    player.PlayerBoardState.CloseCombatTotal += (c.Power);
                            //}
                            break;
                    }
                    break;
                case "Muster":
                    break;

                case "Medic":
                    break;

                case "Morale":
                    break;
                default:
                    break;
            }
            return player;
        }
        public ActionResult RoundTotal()
        {
            Player player = new Player();
            //player = TempData["tempPlayer"] as Player;
            player = Session[player.PlayerGuid.ToString()] as Player;
            player.PlayerBoardState.RoundTotal = player.PlayerBoardState.CloseCombatTotal + player.PlayerBoardState.RangedCombatTotal + player.PlayerBoardState.SiegeCombatTotal;
            // ADD CARDS TO DISCARD
            player.Discard.AddRange(player.PlayerBoardState.CloseCombat.AsEnumerable<Card>());
            player.Discard.AddRange(player.PlayerBoardState.RangedCombat.AsEnumerable<Card>());
            player.Discard.AddRange(player.PlayerBoardState.SiegeCombat.AsEnumerable<Card>());
            //Clear the board
            player.PlayerBoardState.CloseCombatTotal = 0;
            player.PlayerBoardState.RangedCombatTotal = 0;
            player.PlayerBoardState.SiegeCombatTotal = 0;
            player.PlayerBoardState.CloseCombat.Clear();
            player.PlayerBoardState.RangedCombat.Clear();
            player.PlayerBoardState.SiegeCombat.Clear();
            player.PlayerBoardState.IsFoggy = false;
            player.PlayerBoardState.IsRaining = false;
            player.PlayerBoardState.IsSnowing = false;
            player.PlayerBoardState.RoundNumber++;
            player.PlayerBoardState.Passed = false;
            if (player.PlayerBoardState.RoundTotal > player.RivalScore)
            {
                if (player.SelectedFaction.FactionAbbr == "NR")
                {
                    if (player.Deck.Count > 0)
                    {
                        player.Hand.Add(Draw(player.Deck));
                    }
                }
                player.WinCount += 1;
            }
            
            //TempData["tempPlayer"] = player;
            Session[player.PlayerGuid.ToString()] = player;
            if (player.PlayerBoardState.RoundNumber > 3 || player.WinCount > 2)
            {
                return Redirect("~/Setup/Index/");
            }
            else
            {
                return Redirect("~/Board/BoardView/");
            }
            
        }

        public void GameTotal()
        {

        }

        public ActionResult PlayerPass()
        {
            Player player = new Player();
            //player = TempData["tempPlayer"] as Player;
            player = Session[player.PlayerGuid.ToString()] as Player;
            player.PlayerBoardState.Passed = true;
            Session[player.PlayerGuid.ToString()] = player;
            //TempData["tempPlayer"] = player;
            return Redirect("~/Board/BoardView/");
        }

        public bool ActivateLeaderPower ()
        {
            return true;
        }

        public bool ActivateFactionPerk ()
        {
            return true;
        }
        /// <summary>
        /// Adds a card from your deck to your hand
        /// </summary>
        /// <returns></returns>
        public ActionResult DrawFromDeck()
        {
            Player player = new Player();
            //player = TempData["tempPlayer"] as Player;
            player = Session[player.PlayerGuid.ToString()] as Player;
            Card drawnCard = new Card();
            if (player.Deck.Count > 0)
            {
                drawnCard = (player.Deck[0]);
                player.Hand.Add(drawnCard);
                player.Deck.Remove(drawnCard);
            }
            //TempData["tempPlayer"] = player;
            Session[player.PlayerGuid.ToString()] = player;
            return Redirect("~/Board/BoardView/");
        }
        /// <summary>
        /// Quick way to return a single card from a list
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        public Card Draw(List<Card> pool)
        {
            return pool[0];
        }

        /// <summary>
        /// Builds the player deck and hand using web service calls and adds to the player model
        /// </summary>
        /// <param name="player"></param>
        /// <param name="factionAbbreviation"></param>
        /// <returns></returns>
        public Player BuildDeckAndHand(Player player, string factionAbbreviation)
        {
            //Create web client to do call
            System.Net.WebClient client = new System.Net.WebClient();
            //Create the url to pull from
            string deckUriString = Url.RouteUrl("", new {action = "builddeck/byfaction", controller = "Card",id = factionAbbreviation},Request.Url.Scheme);
            string requestUri = deckUriString;
            //Get the deck and deserialize
            string deckReturn = client.DownloadString(requestUri);
            player.Deck = JsonConvert.DeserializeObject<List<Card>>(deckReturn);
            player.Hand = SetupController.DealHand(player.Deck, factionAbbreviation);
            //Same thing we always do, purge the dealt cards from the deck
            foreach (Card c in player.Hand)
            {
                player.Deck.Remove(c);
            }
            return player;
        }

    }
}