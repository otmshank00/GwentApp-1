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
    }
}
