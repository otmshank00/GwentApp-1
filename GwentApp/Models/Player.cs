using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.ObjectModel;
using System.Text;

namespace GwentApp.Models
{
    public class factionInfo
    {
        public string factionName { get; set; }
        public string factionAbbr { get; set; }
        public string factionPerk { get; set; }
    }
    public class leaderInfo
    {
        public string leaderName { get; set; }
        public string leaderAbility { get; set; }
        public string leaderFaction { get; set; }
        public string leaderFactionAbbr { get; set; }
    }
    //public class playerChoices
    //{
    //    public static List<factionInfo> factionList { get; set; }
    //    public static List<leaderInfo> leaderList { get; set; }
    //    public static List<SelectListItem> ddFactionList { get; set; }
    //    public static List<SelectListItem> ddLeaderList { get; set; }
    //    public factionInfo selectedFaction { get; set; }
    //    public leaderInfo selectedLeader { get; set; }
    //}
    //
    public class Player
    {
        /// <summary>
        /// The player's complete uber model.
        /// </summary>

        public factionInfo faction
        {
            get; set;
        }

        public leaderInfo leader
        {
            get; set;
        }
        public List<Card> deck
        {
            get; set;
        }
        public static List<factionInfo> factionList { get; set; }
        public static List<leaderInfo> leaderList { get; set; }
        public static List<SelectListItem> ddFactionList { get; set; }
        public static List<SelectListItem> ddLeaderList { get; set; }
        public factionInfo selectedFaction { get; set; }
        public leaderInfo selectedLeader { get; set; }
    }
}