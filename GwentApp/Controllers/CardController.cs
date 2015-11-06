using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GwentApp.Models;

namespace GwentApp.Controllers
{
    public class CardController : ApiController
    {
        public IHttpActionResult GetAllCards ()
        {
            return Ok(GwentApp.Global.gAllCards);
        }

        public IHttpActionResult GetAllFactions()
        {
            return Ok(GwentApp.Global.gAllFactions);
        }

        public IHttpActionResult GetAllLeaders ()
        {
            return Ok(GwentApp.Global.gAllLeaders);
        }

        public IHttpActionResult GetByPower(int power)
        {
            IEnumerable<Card> cards = GwentApp.Global.gAllCards.Where(card => card.Power == power);
            return Ok(cards);
        }

        public IHttpActionResult GetLeadersByFactionAbbreviation(string factionAbbreviation)
        {
            IEnumerable<LeaderInfo> leaders = GwentApp.Global.gAllLeaders.Where(leader => leader.LeaderFactionAbbr == factionAbbreviation);
            return Ok(leaders);
        }

        public IHttpActionResult GetLeaderByName(string leaderName)
        {
            IEnumerable<LeaderInfo> leader = GwentApp.Global.gAllLeaders.Where(l => l.LeaderName == leaderName);
            return Ok(leader);
        }

        [HttpGet]
        public IHttpActionResult BuildDeckByFaction(string factionAbbreviation)
        {
            List<Card> deck = SetupController.BuildDeck(factionAbbreviation);
            IEnumerable<Card> ieDeck = deck.AsEnumerable<Card>();
            return Ok(ieDeck);
        }
    }
}
